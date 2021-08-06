﻿using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed class PBEReadOnlyMoveset : IPBEMoveset, IPBEMoveset<PBEReadOnlyMoveset.PBEReadOnlyMovesetSlot>
    {
        public sealed class PBEReadOnlyMovesetSlot : IPBEMovesetSlot
        {
            public PBEMove Move { get; }
            public byte PPUps { get; }

            internal PBEReadOnlyMovesetSlot(PBEMove move, byte ppUps)
            {
                Move = move;
                PPUps = ppUps;
            }
        }

        private readonly PBEReadOnlyMovesetSlot[] _list;
        public int Count => _list.Length;
        public PBEReadOnlyMovesetSlot this[int index]
        {
            get
            {
                if (index >= _list.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                return _list[index];
            }
        }
        IPBEMovesetSlot IReadOnlyList<IPBEMovesetSlot>.this[int index] => this[index];

        internal PBEReadOnlyMoveset(EndianBinaryReader r)
        {
            int count = r.ReadByte();
            _list = new PBEReadOnlyMovesetSlot[count];
            for (int i = 0; i < count; i++)
            {
                _list[i] = new PBEReadOnlyMovesetSlot(r.ReadEnum<PBEMove>(), r.ReadByte());
            }
        }
        internal PBEReadOnlyMoveset(JArray jArray)
        {
            int count = jArray.Count;
            _list = new PBEReadOnlyMovesetSlot[count];
            for (int i = 0; i < count; i++)
            {
                JToken jToken = jArray[i];
                if (!PBEDataProvider.Instance.GetMoveByName(jToken.GetSafeString(nameof(IPBEMovesetSlot.Move)), out PBEMove? move))
                {
                    throw new InvalidDataException("Invalid move");
                }
                byte ppUps = jToken.GetSafe(nameof(IPBEMovesetSlot.PPUps)).Value<byte>();
                _list[i] = new PBEReadOnlyMovesetSlot(move.Value, ppUps);
            }
        }
        public PBEReadOnlyMoveset(IPBEMoveset other)
        {
            int count = other.Count;
            _list = new PBEReadOnlyMovesetSlot[count];
            for (int i = 0; i < count; i++)
            {
                IPBEMovesetSlot oSlot = other[i];
                _list[i] = new PBEReadOnlyMovesetSlot(oSlot.Move, oSlot.PPUps);
            }
        }

        public IEnumerator<PBEReadOnlyMovesetSlot> GetEnumerator()
        {
            for (int i = 0; i < _list.Length; i++)
            {
                yield return _list[i];
            }
        }
        IEnumerator<IPBEMovesetSlot> IEnumerable<IPBEMovesetSlot>.GetEnumerator()
        {
            return GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public sealed class PBEReadOnlyPartyMoveset : IPBEPartyMoveset, IPBEPartyMoveset<PBEReadOnlyPartyMoveset.PBEReadOnlyPartyMovesetSlot>
    {
        public sealed class PBEReadOnlyPartyMovesetSlot : IPBEPartyMovesetSlot
        {
            public PBEMove Move { get; }
            public int PP { get; }
            public byte PPUps { get; }

            internal PBEReadOnlyPartyMovesetSlot(PBESettings settings, PBEMove move, byte ppUps)
            {
                Move = move;
                PP = PBEDataUtils.CalcMaxPP(move, ppUps, settings);
                PPUps = ppUps;
            }
            internal PBEReadOnlyPartyMovesetSlot(PBEMove move, int pp, byte ppUps)
            {
                Move = move;
                PP = pp;
                PPUps = ppUps;
            }
        }

        private readonly PBEReadOnlyPartyMovesetSlot[] _list;
        public int Count => _list.Length;
        public PBEReadOnlyPartyMovesetSlot this[int index]
        {
            get
            {
                if (index >= _list.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                return _list[index];
            }
        }
        IPBEPartyMovesetSlot IReadOnlyList<IPBEPartyMovesetSlot>.this[int index] => this[index];

        internal PBEReadOnlyPartyMoveset(EndianBinaryReader r)
        {
            int count = r.ReadByte();
            _list = new PBEReadOnlyPartyMovesetSlot[count];
            for (int i = 0; i < count; i++)
            {
                _list[i] = new PBEReadOnlyPartyMovesetSlot(r.ReadEnum<PBEMove>(), r.ReadInt32(), r.ReadByte());
            }
        }
        internal PBEReadOnlyPartyMoveset(JArray jArray)
        {
            int count = jArray.Count;
            _list = new PBEReadOnlyPartyMovesetSlot[count];
            for (int i = 0; i < count; i++)
            {
                JToken jToken = jArray[i];
                if (!PBEDataProvider.Instance.GetMoveByName(jToken.GetSafeString(nameof(IPBEMovesetSlot.Move)), out PBEMove? move))
                {
                    throw new InvalidDataException("Invalid move");
                }
                int pp = jToken.GetSafe(nameof(IPBEPartyMovesetSlot.PP)).Value<int>();
                byte ppUps = jToken.GetSafe(nameof(IPBEPartyMovesetSlot.PPUps)).Value<byte>();
                _list[i] = new PBEReadOnlyPartyMovesetSlot(move.Value, pp, ppUps);
            }
        }
        public PBEReadOnlyPartyMoveset(PBESettings settings, IPBEMoveset other)
        {
            settings.ShouldBeReadOnly(nameof(settings));
            int count = other.Count;
            _list = new PBEReadOnlyPartyMovesetSlot[count];
            for (int i = 0; i < count; i++)
            {
                IPBEMovesetSlot oSlot = other[i];
                _list[i] = new PBEReadOnlyPartyMovesetSlot(settings, oSlot.Move, oSlot.PPUps);
            }
        }
        public PBEReadOnlyPartyMoveset(IPBEPartyMoveset other)
        {
            int count = other.Count;
            _list = new PBEReadOnlyPartyMovesetSlot[count];
            for (int i = 0; i < count; i++)
            {
                IPBEPartyMovesetSlot oSlot = other[i];
                _list[i] = new PBEReadOnlyPartyMovesetSlot(oSlot.Move, oSlot.PP, oSlot.PPUps);
            }
        }

        public IEnumerator<PBEReadOnlyPartyMovesetSlot> GetEnumerator()
        {
            for (int i = 0; i < _list.Length; i++)
            {
                yield return _list[i];
            }
        }
        IEnumerator<IPBEPartyMovesetSlot> IEnumerable<IPBEPartyMovesetSlot>.GetEnumerator()
        {
            return GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
