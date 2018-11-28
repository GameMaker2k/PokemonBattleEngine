﻿using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Kermalis.PokemonBattleEngine.Battle
{
    [StructLayout(LayoutKind.Explicit)]
    public struct PAction
    {
        [FieldOffset(0)]
        public byte PokemonId;
        [FieldOffset(1)]
        public PDecision Decision;
        [FieldOffset(2)]
        public PMove FightMove;
        [FieldOffset(4)]
        public PTarget FightTargets;
        [FieldOffset(4)]
        public byte SwitchPokemonId;

        internal byte[] ToBytes()
        {
            var bytes = new List<byte>();
            bytes.Add(PokemonId);
            bytes.Add((byte)Decision);
            bytes.AddRange(BitConverter.GetBytes((ushort)FightMove));
            bytes.Add(SwitchPokemonId);
            return bytes.ToArray();
        }
        internal static PAction FromBytes(BinaryReader r)
        {
            return new PAction
            {
                PokemonId = r.ReadByte(),
                Decision = (PDecision)r.ReadByte(),
                FightMove = (PMove)r.ReadUInt16(),
                SwitchPokemonId = r.ReadByte()
            };
        }
    }

    public sealed partial class PBattle
    {
        public bool AreActionsValid(PAction[] actions)
        {
            var standBy = new List<PPokemon>();

            foreach (PAction action in actions)
            {
                PPokemon pkmn = GetPokemon(action.PokemonId);

                // Not on the field
                if (pkmn.FieldPosition == PFieldPosition.None)
                    return false;

                switch (action.Decision)
                {
                    case PDecision.Fight:
                        // Invalid move
                        if (!pkmn.Moves.Contains(action.FightMove) || action.FightMove == PMove.None)
                            return false;

                        // TODO: Struggle

                        // Out of PP
                        if (pkmn.PP[Array.IndexOf(pkmn.Moves, action.FightMove)] < 1)
                            return false;

                        // If the mon has a locked move, it must be used
                        if (pkmn.LockedAction.Decision == PDecision.Fight)
                        {
                            if ((pkmn.LockedAction.FightMove != PMove.None && pkmn.LockedAction.FightMove != action.FightMove)
                                || (pkmn.LockedAction.FightTargets != PTarget.None && pkmn.LockedAction.FightTargets != action.FightTargets))
                                return false;
                        }

                        // Verify targets
                        PMoveData mData = PMoveData.Data[action.FightMove];
                        switch (BattleStyle)
                        {
                            case PBattleStyle.Single:
                            case PBattleStyle.Rotation:
                                switch (mData.Targets)
                                {
                                    case PMoveTarget.All:
                                        if (action.FightTargets != (PTarget.AllyCenter | PTarget.FoeCenter))
                                            return false;
                                        break;
                                    case PMoveTarget.AllFoes:
                                    case PMoveTarget.AllFoesSurrounding:
                                    case PMoveTarget.AllSurrounding:
                                    case PMoveTarget.SingleFoeSurrounding:
                                    case PMoveTarget.SingleNotSelf:
                                    case PMoveTarget.SingleSurrounding:
                                        if (action.FightTargets != PTarget.FoeCenter)
                                            return false;
                                        break;
                                    case PMoveTarget.AllTeam:
                                    case PMoveTarget.Self:
                                    case PMoveTarget.SelfOrAllySurrounding:
                                        if (action.FightTargets != PTarget.AllyCenter)
                                            return false;
                                        break;
                                }
                                break;
                            case PBattleStyle.Double:
                                switch (mData.Targets)
                                {
                                    case PMoveTarget.All:
                                        if (action.FightTargets != (PTarget.AllyLeft | PTarget.AllyRight | PTarget.FoeLeft | PTarget.FoeRight))
                                            return false;
                                        break;
                                    case PMoveTarget.AllFoes:
                                    case PMoveTarget.AllFoesSurrounding:
                                        if (action.FightTargets != (PTarget.FoeLeft | PTarget.FoeRight))
                                            return false;
                                        break;
                                    case PMoveTarget.AllTeam:
                                        if (action.FightTargets != (PTarget.AllyLeft | PTarget.AllyRight))
                                            return false;
                                        break;
                                    case PMoveTarget.AllSurrounding:
                                        if (pkmn.FieldPosition == PFieldPosition.Left)
                                        {
                                            if (action.FightTargets != (PTarget.AllyRight | PTarget.FoeLeft | PTarget.FoeRight))
                                                return false;
                                        }
                                        else
                                        {
                                            if (action.FightTargets != (PTarget.AllyLeft | PTarget.FoeLeft | PTarget.FoeRight))
                                                return false;
                                        }
                                        break;
                                    case PMoveTarget.Self:
                                        if (pkmn.FieldPosition == PFieldPosition.Left)
                                        {
                                            if (action.FightTargets != PTarget.AllyLeft)
                                                return false;
                                        }
                                        else
                                        {
                                            if (action.FightTargets != PTarget.AllyRight)
                                                return false;
                                        }
                                        break;
                                    case PMoveTarget.SelfOrAllySurrounding:
                                        if (action.FightTargets != PTarget.AllyLeft && action.FightTargets != PTarget.AllyRight)
                                            return false;
                                        break;
                                    case PMoveTarget.SingleAllySurrounding:
                                        if (pkmn.FieldPosition == PFieldPosition.Left)
                                        {
                                            if (action.FightTargets != PTarget.AllyRight)
                                                return false;
                                        }
                                        else
                                        {
                                            if (action.FightTargets != PTarget.AllyLeft)
                                                return false;
                                        }
                                        break;
                                    case PMoveTarget.SingleFoeSurrounding:
                                        if (action.FightTargets != PTarget.FoeLeft && action.FightTargets != PTarget.FoeRight)
                                            return false;
                                        break;
                                    case PMoveTarget.SingleNotSelf:
                                    case PMoveTarget.SingleSurrounding:
                                        if (pkmn.FieldPosition == PFieldPosition.Left)
                                        {
                                            if (action.FightTargets != PTarget.AllyRight && action.FightTargets != PTarget.FoeLeft && action.FightTargets != PTarget.FoeRight)
                                                return false;
                                        }
                                        else
                                        {
                                            if (action.FightTargets != PTarget.AllyLeft && action.FightTargets != PTarget.FoeLeft && action.FightTargets != PTarget.FoeRight)
                                                return false;
                                        }
                                        break;
                                }
                                break;
                            case PBattleStyle.Triple:
                                switch (mData.Targets)
                                {
                                    case PMoveTarget.All:
                                        if (action.FightTargets != (PTarget.AllyLeft | PTarget.AllyCenter | PTarget.AllyRight | PTarget.FoeLeft | PTarget.FoeCenter | PTarget.FoeRight))
                                            return false;
                                        break;
                                    case PMoveTarget.AllFoes:
                                        if (action.FightTargets != (PTarget.FoeLeft | PTarget.FoeCenter | PTarget.FoeRight))
                                            return false;
                                        break;
                                    case PMoveTarget.AllFoesSurrounding:
                                        if (pkmn.FieldPosition == PFieldPosition.Left)
                                        {
                                            if (action.FightTargets != (PTarget.FoeCenter | PTarget.FoeRight))
                                                return false;
                                        }
                                        else if (pkmn.FieldPosition == PFieldPosition.Center)
                                        {
                                            if (action.FightTargets != (PTarget.FoeLeft | PTarget.FoeCenter | PTarget.FoeRight))
                                                return false;
                                        }
                                        else
                                        {
                                            if (action.FightTargets != (PTarget.FoeLeft | PTarget.FoeCenter))
                                                return false;
                                        }
                                        break;
                                    case PMoveTarget.AllSurrounding:
                                        if (pkmn.FieldPosition == PFieldPosition.Left)
                                        {
                                            if (action.FightTargets != (PTarget.AllyCenter | PTarget.FoeCenter | PTarget.FoeRight))
                                                return false;
                                        }
                                        else if (pkmn.FieldPosition == PFieldPosition.Center)
                                        {
                                            if (action.FightTargets != (PTarget.AllyLeft | PTarget.AllyRight | PTarget.FoeLeft | PTarget.FoeCenter | PTarget.FoeRight))
                                                return false;
                                        }
                                        else
                                        {
                                            if (action.FightTargets != (PTarget.AllyCenter | PTarget.FoeLeft | PTarget.FoeCenter))
                                                return false;
                                        }
                                        break;
                                    case PMoveTarget.AllTeam:
                                        if (action.FightTargets != (PTarget.AllyLeft | PTarget.AllyCenter | PTarget.AllyRight))
                                            return false;
                                        break;
                                    case PMoveTarget.Self:
                                        if (pkmn.FieldPosition == PFieldPosition.Left)
                                        {
                                            if (action.FightTargets != PTarget.AllyLeft)
                                                return false;
                                        }
                                        else if (pkmn.FieldPosition == PFieldPosition.Center)
                                        {
                                            if (action.FightTargets != PTarget.AllyCenter)
                                                return false;
                                        }
                                        else
                                        {
                                            if (action.FightTargets != PTarget.AllyRight)
                                                return false;
                                        }
                                        break;
                                    case PMoveTarget.SelfOrAllySurrounding:
                                        if (pkmn.FieldPosition == PFieldPosition.Left)
                                        {
                                            if (action.FightTargets != PTarget.AllyLeft && action.FightTargets != PTarget.AllyCenter)
                                                return false;
                                        }
                                        else if (pkmn.FieldPosition == PFieldPosition.Center)
                                        {
                                            if (action.FightTargets != PTarget.AllyLeft && action.FightTargets != PTarget.AllyCenter && action.FightTargets != PTarget.AllyRight)
                                                return false;
                                        }
                                        else
                                        {
                                            if (action.FightTargets != PTarget.AllyCenter && action.FightTargets != PTarget.AllyRight)
                                                return false;
                                        }
                                        break;
                                    case PMoveTarget.SingleAllySurrounding:
                                        if (pkmn.FieldPosition == PFieldPosition.Left)
                                        {
                                            if (action.FightTargets != PTarget.AllyCenter)
                                                return false;
                                        }
                                        else if (pkmn.FieldPosition == PFieldPosition.Center)
                                        {
                                            if (action.FightTargets != PTarget.AllyLeft && action.FightTargets != PTarget.AllyRight)
                                                return false;
                                        }
                                        else
                                        {
                                            if (action.FightTargets != PTarget.AllyCenter)
                                                return false;
                                        }
                                        break;
                                    case PMoveTarget.SingleFoeSurrounding:
                                        if (pkmn.FieldPosition == PFieldPosition.Left)
                                        {
                                            if (action.FightTargets != PTarget.FoeCenter && action.FightTargets != PTarget.FoeRight)
                                                return false;
                                        }
                                        else if (pkmn.FieldPosition == PFieldPosition.Center)
                                        {
                                            if (action.FightTargets != PTarget.FoeLeft && action.FightTargets != PTarget.FoeCenter && action.FightTargets != PTarget.FoeRight)
                                                return false;
                                        }
                                        else
                                        {
                                            if (action.FightTargets != PTarget.FoeLeft && action.FightTargets != PTarget.FoeCenter)
                                                return false;
                                        }
                                        break;
                                    case PMoveTarget.SingleNotSelf:
                                        if (pkmn.FieldPosition == PFieldPosition.Left)
                                        {
                                            if (action.FightTargets != PTarget.AllyCenter && action.FightTargets != PTarget.AllyRight && action.FightTargets != PTarget.FoeLeft && action.FightTargets != PTarget.FoeCenter && action.FightTargets != PTarget.FoeRight)
                                                return false;
                                        }
                                        else if (pkmn.FieldPosition == PFieldPosition.Center)
                                        {
                                            if (action.FightTargets != PTarget.AllyLeft && action.FightTargets != PTarget.AllyRight && action.FightTargets != PTarget.FoeLeft && action.FightTargets != PTarget.FoeCenter && action.FightTargets != PTarget.FoeRight)
                                                return false;
                                        }
                                        else
                                        {
                                            if (action.FightTargets != PTarget.AllyLeft && action.FightTargets != PTarget.AllyCenter && action.FightTargets != PTarget.FoeLeft && action.FightTargets != PTarget.FoeCenter && action.FightTargets != PTarget.FoeRight)
                                                return false;
                                        }
                                        break;
                                    case PMoveTarget.SingleSurrounding:
                                        if (pkmn.FieldPosition == PFieldPosition.Left)
                                        {
                                            if (action.FightTargets != PTarget.AllyCenter && action.FightTargets != PTarget.FoeCenter && action.FightTargets != PTarget.FoeRight)
                                                return false;
                                        }
                                        else if (pkmn.FieldPosition == PFieldPosition.Center)
                                        {
                                            if (action.FightTargets != PTarget.AllyLeft && action.FightTargets != PTarget.AllyRight && action.FightTargets != PTarget.FoeLeft && action.FightTargets != PTarget.FoeCenter && action.FightTargets != PTarget.FoeRight)
                                                return false;
                                        }
                                        else
                                        {
                                            if (action.FightTargets != PTarget.AllyCenter && action.FightTargets != PTarget.FoeLeft && action.FightTargets != PTarget.FoeCenter)
                                                return false;
                                        }
                                        break;
                                }
                                break;
                        }
                        break;
                    case PDecision.Switch:
                        PPokemon switchPkmn = GetPokemon(action.SwitchPokemonId);
                        // Validate the new battler's ID
                        if (switchPkmn == null || switchPkmn.Id == pkmn.Id)
                            return false;
                        // Cannot switch into a foe's Pokémon
                        if (switchPkmn.Local != pkmn.Local)
                            return false;
                        // Cannot switch while airborne, underground or underwater
                        if (pkmn.Status2.HasFlag(PStatus2.Airborne) || pkmn.Status2.HasFlag(PStatus2.Underground) || pkmn.Status2.HasFlag(PStatus2.Underwater))
                            return false;
                        // Cannot switch into a Pokémon already on the field
                        if (switchPkmn.FieldPosition != PFieldPosition.None)
                            return false;
                        // Cannot switch into a Pokémon already selected to switch in this turn
                        if (standBy.Contains(switchPkmn))
                            return false;
                        standBy.Add(switchPkmn);
                        break;
                }
            }
            return true;
        }
        // Returns true if the actions were valid (and selected)
        public bool SelectActionsIfValid(PAction[] actions)
        {
            if (AreActionsValid(actions))
            {
                foreach (PAction action in actions)
                    SelectAction(action);
                return true;
            }
            return false;
        }
        void SelectAction(PAction action)
        {
            PPokemon pkmn = GetPokemon(action.PokemonId);
            switch (action.Decision)
            {
                case PDecision.Fight:
                    switch (PMoveData.Data[action.FightMove].Targets)
                    {
                        case PMoveTarget.RandomFoeSurrounding:
                            switch (BattleStyle)
                            {
                                case PBattleStyle.Single:
                                case PBattleStyle.Rotation:
                                    action.FightTargets = PTarget.FoeCenter;
                                    break;
                                case PBattleStyle.Double:
                                    action.FightTargets = PUtils.RNG.Next(2) == 0 ? PTarget.FoeLeft : PTarget.FoeRight;
                                    break;
                                case PBattleStyle.Triple:
                                    if (pkmn.FieldPosition == PFieldPosition.Left)
                                    {
                                        // If one is fainted, BattleEffects.cs->UseMove() will change the target
                                        action.FightTargets = PUtils.RNG.Next(2) == 0 ? PTarget.FoeCenter : PTarget.FoeRight;
                                    }
                                    else if (pkmn.FieldPosition == PFieldPosition.Center)
                                    {
                                        PTeam opposingTeam = Teams[pkmn.Local ? 1 : 0]; // Other team
                                                                                        // Keep randomly picking until a non-fainted foe is selected
                                        int r;
                                        roll:
                                        r = PUtils.RNG.Next(3);
                                        // Prioritize left
                                        if (r == 0)
                                        {
                                            if (opposingTeam.PokemonAtPosition(PFieldPosition.Left) != null)
                                                action.FightTargets = PTarget.FoeLeft;
                                            else
                                                goto roll;
                                        }
                                        // Prioritize center
                                        else if (r == 1)
                                        {
                                            if (opposingTeam.PokemonAtPosition(PFieldPosition.Center) != null)
                                                action.FightTargets = PTarget.FoeCenter;
                                            else
                                                goto roll;
                                        }
                                        // Prioritize right
                                        else
                                        {
                                            if (opposingTeam.PokemonAtPosition(PFieldPosition.Right) != null)
                                                action.FightTargets = PTarget.FoeRight;
                                            else
                                                goto roll;
                                        }
                                    }
                                    else
                                    {
                                        // If one is fainted, BattleEffects.cs->UseMove() will change the target
                                        action.FightTargets = PUtils.RNG.Next(2) == 0 ? PTarget.FoeLeft : PTarget.FoeCenter;
                                    }
                                    break;
                            }
                            break;
                        case PMoveTarget.SingleAllySurrounding:
                            if (BattleStyle == PBattleStyle.Single || BattleStyle == PBattleStyle.Rotation)
                                action.FightTargets = PTarget.AllyCenter;
                            break;
                    }
                    break;
            }
            pkmn.SelectedAction = action;
        }

        public static PFieldPosition GetPositionAcross(PBattleStyle style, PFieldPosition pos)
        {
            switch (style)
            {
                case PBattleStyle.Single:
                case PBattleStyle.Rotation:
                    if (pos == PFieldPosition.Center)
                        return PFieldPosition.Center;
                    break;
                case PBattleStyle.Double:
                    if (pos == PFieldPosition.Left)
                        return PFieldPosition.Right;
                    else if (pos == PFieldPosition.Right)
                        return PFieldPosition.Left;
                    break;
                case PBattleStyle.Triple:
                    if (pos == PFieldPosition.Left)
                        return PFieldPosition.Right;
                    else if (pos == PFieldPosition.Center)
                        return PFieldPosition.Center;
                    else if (pos == PFieldPosition.Right)
                        return PFieldPosition.Left;
                    break;
            }
            return PFieldPosition.None;
        }
        // Gets Pokémon that can be hit at the moment
        // For example, if "FoeCenter" is passed in, logic will be used to fall-back to another opponent that can be hit if FoeCenter fainted
        // For each flag passed in, zero or one opponent will be returned for it
        PPokemon[] GetRuntimeTargets(PPokemon user, PTarget requestedTargets, bool canHitFarCorners)
        {
            PTeam attackerTeam = Teams[user.Local ? 0 : 1]; // Attacker's team
            PTeam opposingTeam = Teams[user.Local ? 1 : 0]; // Other team

            var targets = new List<PPokemon>();
            if (requestedTargets.HasFlag(PTarget.AllyLeft))
            {
                PPokemon b = attackerTeam.PokemonAtPosition(PFieldPosition.Left);
                targets.Add(b);
            }
            if (requestedTargets.HasFlag(PTarget.AllyCenter))
            {
                PPokemon b = attackerTeam.PokemonAtPosition(PFieldPosition.Center);
                targets.Add(b);
            }
            if (requestedTargets.HasFlag(PTarget.AllyRight))
            {
                PPokemon b = attackerTeam.PokemonAtPosition(PFieldPosition.Right);
                targets.Add(b);
            }
            if (requestedTargets.HasFlag(PTarget.FoeLeft))
            {
                PPokemon b = opposingTeam.PokemonAtPosition(PFieldPosition.Left);
                // Target fainted, fallback to its teammate
                if (b == null)
                {
                    if (BattleStyle == PBattleStyle.Double)
                    {
                        b = opposingTeam.PokemonAtPosition(PFieldPosition.Right);
                    }
                    else if (BattleStyle == PBattleStyle.Triple)
                    {
                        b = opposingTeam.PokemonAtPosition(PFieldPosition.Center);
                        // Center fainted as well and user can reach far right
                        if (b == null && (user.FieldPosition != PFieldPosition.Right || canHitFarCorners))
                        {
                            b = opposingTeam.PokemonAtPosition(PFieldPosition.Right);
                        }
                    }
                }
                targets.Add(b);
            }
            if (requestedTargets.HasFlag(PTarget.FoeCenter))
            {
                PPokemon b = opposingTeam.PokemonAtPosition(PFieldPosition.Center);
                // Target fainted, fallback to its teammate
                if (b == null)
                {
                    if (BattleStyle == PBattleStyle.Triple)
                    {
                        if (user.FieldPosition == PFieldPosition.Left)
                        {
                            b = opposingTeam.PokemonAtPosition(PFieldPosition.Right);
                            // Right fainted as well and user can reach far left
                            if (b == null && (user.FieldPosition != PFieldPosition.Left || canHitFarCorners))
                            {
                                b = opposingTeam.PokemonAtPosition(PFieldPosition.Left);
                            }
                        }
                        else if (user.FieldPosition == PFieldPosition.Right)
                        {
                            b = opposingTeam.PokemonAtPosition(PFieldPosition.Left);
                            // Left fainted as well and user can reach far right
                            if (b == null && (user.FieldPosition != PFieldPosition.Right || canHitFarCorners))
                            {
                                b = opposingTeam.PokemonAtPosition(PFieldPosition.Right);
                            }
                        }
                        else // Center
                        {
                            PPokemon oppLeft = opposingTeam.PokemonAtPosition(PFieldPosition.Left),
                                oppRight = opposingTeam.PokemonAtPosition(PFieldPosition.Right);
                            // Left is dead but not right
                            if (oppLeft == null && oppRight != null)
                            {
                                b = oppRight;
                            }
                            // Right is dead but not left
                            else if (oppLeft != null && oppRight == null)
                            {
                                b = oppLeft;
                            }
                            // Randomly select left or right
                            else
                            {
                                b = PUtils.RNG.NextBoolean() ? oppLeft : oppRight;
                            }
                        }
                    }
                }
                targets.Add(b);
            }
            if (requestedTargets.HasFlag(PTarget.FoeRight))
            {
                PPokemon b = opposingTeam.PokemonAtPosition(PFieldPosition.Right);
                // Target fainted, fallback to its teammate
                if (b == null)
                {
                    if (BattleStyle == PBattleStyle.Double)
                    {
                        b = opposingTeam.PokemonAtPosition(PFieldPosition.Left);
                    }
                    else if (BattleStyle == PBattleStyle.Triple)
                    {
                        b = opposingTeam.PokemonAtPosition(PFieldPosition.Center);
                        // Center fainted as well and user can reach far left
                        if (b == null && (user.FieldPosition != PFieldPosition.Left || canHitFarCorners))
                        {
                            b = opposingTeam.PokemonAtPosition(PFieldPosition.Left);
                        }
                    }
                }
                targets.Add(b);
            }
            return targets.Where(p => p != null).Distinct().ToArray(); // Remove duplicate targets
        }
    }
}
