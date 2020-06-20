﻿using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Network;
using Kermalis.PokemonBattleEngine.Packets;
using Kermalis.PokemonBattleEngine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

namespace Kermalis.PokemonBattleEngineServer
{
    internal sealed class BattleServer
    {
        private enum ServerState
        {
            Resetting,           // Server is currently resetting the game
            WaitingForPlayers,   // Server is waiting for 2 players to connect
            WaitingForParties,   // Server is waiting for both players to send their party
            WaitingForActions,   // Server is waiting for players to select actions
            WaitingForSwitchIns, // Server is waiting for players to switch in new Pokémon
            BattleProcessing,    // Battle is running and sending events
            BattleEnded          // Battle ended
        }
        public bool RequireLegalParties;
        private readonly PBEServer _server;
        private ServerState _state = ServerState.Resetting;
        private PBEBattle _battle;
        private Player[] _battlers;
        private readonly List<IPBEPacket> _spectatorPackets = new List<IPBEPacket>();
        private readonly Dictionary<PBEServerClient, Player> _readyPlayers = new Dictionary<PBEServerClient, Player>();
        private readonly ManualResetEvent _resetEvent = new ManualResetEvent(true);

        private static void PrintUsage()
        {
            Console.WriteLine("Usage:\tPokemonBattleEngineServer.dll {ip} {port} {requireLegalParties}");
            Console.WriteLine("Example:\tPokemonBattleEngineServer.dll 127.0.0.1 8888 true");
        }
        public static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                PrintUsage();
            }
            else
            {
                PBEUtils.InitEngine(string.Empty);
                new BattleServer(args);
            }
        }
        private BattleServer(string[] args)
        {
            if (!IPAddress.TryParse(args[0], out IPAddress ip)
                || !ushort.TryParse(args[1], out ushort port)
                || !bool.TryParse(args[2], out bool requireLegalParties))
            {
                PrintUsage();
            }
            else
            {
                using (_server = new PBEServer())
                {
                    _server.ClientConnected += OnClientConnected;
                    _server.ClientDisconnected += OnClientDisconnected;
                    _server.ClientRefused += OnClientRefused;
                    _server.Error += OnError; // Events unsubscribe in _server.Dispose()
                    _server.Start(new IPEndPoint(ip, port), 100);
                    RequireLegalParties = requireLegalParties;
                    Console.WriteLine("Server online.");
                    Reset();
                    Thread.Sleep(-1);
                }
            }
        }
        private void OnClientConnected(object sender, PBEServerClient client)
        {
            // Need to spawn a new thread so "WaitOne()" doesn't block the thread that receives client packets
            new Thread(() =>
            {
                lock (this)
                {
                    // Wait for the server to be in a state where no events will be sent
                    _resetEvent.WaitOne();

                    var player = new Player(this, client);

                    // Set new player's info
                    for (int i = 0; i < int.MaxValue; i++)
                    {
                        if (!_readyPlayers.Any(p => p.Value.BattleId == i))
                        {
                            player.BattleId = i;
                            break;
                        }
                    }
                    player.TrainerName = Utils.RandomElement(new string[] { "Sasha", "Nikki", "Lara", "Violet", "Naomi", "Rose", "Sabrina", "Nicole" });
                    _readyPlayers.Add(client, player);
                    Console.WriteLine($"Client connected ({client.IP} {player.BattleId} {player.TrainerName})");

                    foreach (Player rp in _readyPlayers.Values.ToArray()) // Copy so a disconnect doesn't cause an exception
                    {
                        // Alert new player of all other players that have already joined
                        if (rp != player)
                        {
                            player.Send(new PBEPlayerJoinedPacket(false, rp.BattleId, rp.TrainerName));
                            if (!player.WaitForResponse())
                            {
                                return;
                            }
                        }
                        // Alert all players that this new player joined (including him/herself)
                        bool isMe = rp == player;
                        rp.Send(new PBEPlayerJoinedPacket(isMe, player.BattleId, player.TrainerName));
                        if (!rp.WaitForResponse() && (isMe || rp.BattleId < 2))
                        {
                            return;
                        }
                    }

                    if (player.BattleId >= 2) // Catch up spectator
                    {
                        foreach (IPBEPacket packet in _spectatorPackets)
                        {
                            player.Send(packet);
                            if (!player.WaitForResponse())
                            {
                                return;
                            }
                        }
                    }
                    else if (_readyPlayers.Count == 2) // Try to start the battle
                    {
                        _state = ServerState.WaitingForParties;
                        Console.WriteLine("Two players connected! Waiting for parties...");
                        _battlers = _readyPlayers.Values.ToArray();
                        SendTo(_battlers, new PBEPartyRequestPacket(RequireLegalParties));
                    }
                }
            })
            {
                Name = "Client Connected Thread"
            }.Start();
        }
        private void OnClientDisconnected(object sender, PBEServerClient client)
        {
            // Need to spawn a new thread so "WaitOne()" doesn't block the thread that receives client packets
            new Thread(() =>
            {
                lock (this)
                {
                    // Wait for the server to be in a state where no events will be sent
                    _resetEvent.WaitOne();

                    if (_readyPlayers.ContainsKey(client))
                    {
                        Player player = _readyPlayers[client];
                        _readyPlayers.Remove(client);
                        player.Dispose();

                        Console.WriteLine($"Client disconnected ({player.BattleId} {player.TrainerName})");
                        if (player.BattleId < 2)
                        {
                            if (_state != ServerState.WaitingForPlayers)
                            {
                                CancelMatch();
                            }
                        }
                        else
                        {
                            // Temporarily ignore spectators
                        }
                    }
                }
            })
            {
                Name = "Client Disconnected Thread"
            }.Start();
        }
        private void OnClientRefused(object sender, IPEndPoint clientIP, bool refusedForBan)
        {
            Console.WriteLine($"Client refused ({clientIP} {(refusedForBan ? "banned" : "no more room")})");
        }
        private void OnError(object sender, Exception ex)
        {
            Console.WriteLine($"Server error:{Environment.NewLine}{ex}");
        }

        private void CancelMatch()
        {
            if (_state == ServerState.Resetting)
            {
                return;
            }
            lock (this)
            {
                if (_state == ServerState.Resetting)
                {
                    return;
                }
                _state = ServerState.Resetting;
                Console.WriteLine("Cancelling match...");
                SendToAll(new PBEMatchCancelledPacket());
                Reset();
            }
        }
        private void Reset()
        {
            Console.WriteLine("Resetting...");
            _resetEvent.Reset();
            _state = ServerState.Resetting;
            foreach (Player c in _readyPlayers.Values.ToArray())
            {
                DisconnectClient(c);
            }
            if (_battle != null)
            {
                _battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
                _battle.OnNewEvent -= BattleEventHandler;
                _battle.OnStateChanged -= BattleStateHandler;
            }
            _battle = new PBEBattle(PBEBattleTerrain.Plain, PBEBattleFormat.Double, PBESettings.DefaultSettings);
            _battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            _battle.OnNewEvent += BattleEventHandler;
            _battle.OnStateChanged += BattleStateHandler;
            _server.Battle = _battle;
            _battlers = null;
            _spectatorPackets.Clear();
            _state = ServerState.WaitingForPlayers;
            _resetEvent.Set();
        }
        public void PartySubmitted(Player player, IPBEPokemonCollection party)
        {
            if (_state != ServerState.WaitingForParties)
            {
                return;
            }
            lock (this)
            {
                if (_state != ServerState.WaitingForParties)
                {
                    return;
                }
                PBEBattle.CreateTeamParty(_battle.Teams[player.BattleId], new PBETeamInfo(party, player.TrainerName));
            }
        }
        public void ActionsSubmitted(Player player, IList<PBETurnAction> actions)
        {
            if (_state != ServerState.WaitingForActions)
            {
                return;
            }
            lock (this)
            {
                if (_state != ServerState.WaitingForActions)
                {
                    return;
                }
                PBETeam team = _battle.Teams[player.BattleId];
                Console.WriteLine($"Received actions ({player.BattleId} {player.TrainerName})");
                if (!PBEBattle.SelectActionsIfValid(team, actions))
                {
                    Console.WriteLine("Actions are invalid!");
                    CancelMatch();
                }
            }
        }
        public void SwitchesSubmitted(Player player, IList<PBESwitchIn> switches)
        {
            if (_state != ServerState.WaitingForSwitchIns)
            {
                return;
            }
            lock (this)
            {
                if (_state != ServerState.WaitingForSwitchIns)
                {
                    return;
                }
                PBETeam team = _battle.Teams[player.BattleId];
                Console.WriteLine($"Received switches ({player.BattleId} {player.TrainerName})");
                if (!PBEBattle.SelectSwitchesIfValid(team, switches))
                {
                    Console.WriteLine("Switches are invalid!");
                    CancelMatch();
                }
            }
        }

        private void BattleStateHandler(PBEBattle battle)
        {
            Console.WriteLine("Battle state changed: {0}", battle.BattleState);
            switch (battle.BattleState)
            {
                case PBEBattleState.Ended:
                {
                    _resetEvent.Set();
                    _state = ServerState.BattleEnded;
                    break;
                }
                case PBEBattleState.Processing:
                {
                    _resetEvent.Reset();
                    _state = ServerState.BattleProcessing;
                    break;
                }
                case PBEBattleState.ReadyToBegin:
                {
                    _resetEvent.Reset();
                    Console.WriteLine("Battle starting!");
                    new Thread(battle.Begin) { Name = "Battle Thread" }.Start();
                    break;
                }
                case PBEBattleState.ReadyToRunSwitches:
                {
                    new Thread(battle.RunSwitches) { Name = "Battle Thread" }.Start();
                    break;
                }
                case PBEBattleState.ReadyToRunTurn:
                {
                    new Thread(battle.RunTurn) { Name = "Battle Thread" }.Start();
                    break;
                }
            }
        }
        private void BattleEventHandler(PBEBattle battle, IPBEPacket packet)
        {
            void SendOriginalPacketToTeamOwnerAndEveryoneElseGetsAPacketWithHiddenInfo(IPBEPacket realPacket, IPBEPacket hiddenInfo, byte teamOwnerId)
            {
                _spectatorPackets.Add(hiddenInfo);
                Player teamOwner = _battlers[teamOwnerId];
                teamOwner.Send(realPacket);
                if (!teamOwner.WaitForResponse())
                {
                    return;
                }
                foreach (Player player in _readyPlayers.Values.Except(new[] { teamOwner }))
                {
                    player.Send(hiddenInfo);
                    if (!player.WaitForResponse() && player.BattleId < 2)
                    {
                        return;
                    }
                }
            }

            switch (packet)
            {
                case PBEMoveLockPacket mlp:
                {
                    Player teamOwner = _battlers[mlp.MoveUserTeam.Id];
                    teamOwner.Send(mlp);
                    if (!teamOwner.WaitForResponse())
                    {
                        return;
                    }
                    break;
                }
                case PBEPkmnFaintedPacket pfp:
                {
                    SendOriginalPacketToTeamOwnerAndEveryoneElseGetsAPacketWithHiddenInfo(pfp, new PBEPkmnFaintedPacket_Hidden(pfp), pfp.PokemonTeam.Id);
                    break;
                }
                case PBEPkmnFormChangedPacket pfcp:
                {
                    SendOriginalPacketToTeamOwnerAndEveryoneElseGetsAPacketWithHiddenInfo(pfcp, new PBEPkmnFormChangedPacket_Hidden(pfcp), pfcp.PokemonTeam.Id);
                    break;
                }
                case PBEPkmnHPChangedPacket phcp:
                {
                    SendOriginalPacketToTeamOwnerAndEveryoneElseGetsAPacketWithHiddenInfo(phcp, new PBEPkmnHPChangedPacket_Hidden(phcp), phcp.PokemonTeam.Id);
                    break;
                }
                case PBEPkmnSwitchInPacket psip:
                {
                    SendOriginalPacketToTeamOwnerAndEveryoneElseGetsAPacketWithHiddenInfo(psip, new PBEPkmnSwitchInPacket_Hidden(psip), psip.Team.Id);
                    break;
                }
                case PBEPkmnSwitchOutPacket psop:
                {
                    SendOriginalPacketToTeamOwnerAndEveryoneElseGetsAPacketWithHiddenInfo(psop, new PBEPkmnSwitchOutPacket_Hidden(psop), psop.PokemonTeam.Id);
                    break;
                }
                case PBEReflectTypePacket rtp:
                {
                    var hidden = new PBEReflectTypePacket_Hidden(rtp);
                    _spectatorPackets.Add(hidden);
                    _battlers[0].Send(rtp.UserTeam.Id == 0 || rtp.TargetTeam.Id == 0 ? (IPBEPacket)rtp : hidden);
                    if (!_battlers[0].WaitForResponse())
                    {
                        return;
                    }
                    _battlers[1].Send(rtp.UserTeam.Id == 1 || rtp.TargetTeam.Id == 1 ? (IPBEPacket)rtp : hidden);
                    if (!_battlers[1].WaitForResponse())
                    {
                        return;
                    }
                    foreach (Player player in _readyPlayers.Values.Except(_battlers))
                    {
                        player.Send(hidden);
                        player.WaitForResponse();
                    }
                    break;
                }
                case PBETransformPacket tp:
                {
                    if (tp.UserTeam.Id == 0 || tp.TargetTeam.Id == 0)
                    {
                        _battlers[0].Send(tp);
                        if (!_battlers[0].WaitForResponse())
                        {
                            return;
                        }
                    }
                    if (tp.UserTeam.Id == 1 || tp.TargetTeam.Id == 1)
                    {
                        _battlers[1].Send(tp);
                        if (!_battlers[1].WaitForResponse())
                        {
                            return;
                        }
                    }
                    break;
                }
                case PBEActionsRequestPacket _:
                {
                    _state = ServerState.WaitingForActions;
                    _spectatorPackets.Add(packet);
                    SendToAll(packet);
                    _resetEvent.Set();
                    break;
                }
                case PBEAutoCenterPacket acp:
                {
                    var team0 = new PBEAutoCenterPacket_Hidden0(acp);
                    var team1 = new PBEAutoCenterPacket_Hidden1(acp);
                    var spectators = new PBEAutoCenterPacket_Hidden01(acp);
                    _spectatorPackets.Add(spectators);
                    _battlers[0].Send(team0);
                    if (!_battlers[0].WaitForResponse())
                    {
                        return;
                    }
                    _battlers[1].Send(team1);
                    if (!_battlers[1].WaitForResponse())
                    {
                        return;
                    }
                    foreach (Player player in _readyPlayers.Values.Except(_battlers))
                    {
                        player.Send(spectators);
                        player.WaitForResponse();
                    }
                    break;
                }
                case PBETeamPacket tp:
                {
                    Player teamOwner = _battlers[tp.Team.Id];
                    teamOwner.Send(tp);
                    if (!teamOwner.WaitForResponse())
                    {
                        return;
                    }
                    break;
                }
                case PBESwitchInRequestPacket _:
                {
                    _state = ServerState.WaitingForSwitchIns;
                    _spectatorPackets.Add(packet);
                    SendToAll(packet);
                    _resetEvent.Set();
                    break;
                }
                default:
                {
                    _spectatorPackets.Add(packet);
                    foreach (Player player in _readyPlayers.Values.ToArray())
                    {
                        player.Send(packet);
                        if (!player.WaitForResponse() && player.BattleId < 2)
                        {
                            return;
                        }
                    }
                    break;
                }
            }
        }

        public void DisconnectClient(Player player)
        {
            Console.WriteLine($"Disconnecting client ({player.BattleId} {player.TrainerName})");
            _server.DisconnectClient(player.Client);
        }
        private void SendTo(IEnumerable<Player> players, IPBEPacket packet)
        {
            foreach (Player p in players)
            {
                p.Send(packet);
            }
        }
        private void SendToAll(IPBEPacket packet)
        {
            SendTo(_readyPlayers.Values.ToArray(), packet);
        }
    }
}
