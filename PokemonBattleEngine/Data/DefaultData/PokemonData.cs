﻿using Kermalis.PokemonBattleEngine.Data.Legality;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Data.DefaultData
{
    public sealed class PBEPokemonData : IPBEPokemonDataExtended
    {
        public PBESpecies Species { get; }
        public PBEForm Form { get; }
        public PBEReadOnlyStatCollection BaseStats { get; }
        IPBEReadOnlyStatCollection IPBEPokemonData.BaseStats => BaseStats;
        public PBEType Type1 { get; }
        public PBEType Type2 { get; }
        public PBEGenderRatio GenderRatio { get; }
        public PBEGrowthRate GrowthRate { get; }
        public ushort BaseEXPYield { get; }
        public byte CatchRate { get; }
        public byte FleeRate { get; }
        /// <summary>Weight in Kilograms</summary>
        public float Weight { get; }
        public ReadOnlyCollection<PBEAbility> Abilities { get; }
        IReadOnlyList<PBEAbility> IPBEPokemonData.Abilities => Abilities;
        public ReadOnlyCollection<(PBESpecies Species, PBEForm Form)> PreEvolutions { get; }
        IReadOnlyList<(PBESpecies Species, PBEForm Form)> IPBEPokemonDataExtended.PreEvolutions => PreEvolutions;
        public ReadOnlyCollection<(PBESpecies Species, PBEForm Form)> Evolutions { get; }
        IReadOnlyList<(PBESpecies Species, PBEForm Form)> IPBEPokemonDataExtended.Evolutions => Evolutions;
        public ReadOnlyCollection<(PBEMove Move, byte Level, PBEMoveObtainMethod ObtainMethod)> LevelUpMoves { get; }
        IReadOnlyList<(PBEMove Move, byte Level, PBEMoveObtainMethod ObtainMethod)> IPBEPokemonDataExtended.LevelUpMoves => LevelUpMoves;
        public ReadOnlyCollection<(PBEMove Move, PBEMoveObtainMethod ObtainMethod)> OtherMoves { get; }
        IReadOnlyList<(PBEMove Move, PBEMoveObtainMethod ObtainMethod)> IPBEPokemonDataExtended.OtherMoves => OtherMoves;

        private PBEPokemonData(SearchResult result)
        {
            BaseStats = new PBEReadOnlyStatCollection(result);
            Type1 = (PBEType)result.Type1;
            Type2 = (PBEType)result.Type2;
            GenderRatio = (PBEGenderRatio)result.GenderRatio;
            GrowthRate = (PBEGrowthRate)result.GrowthRate;
            BaseEXPYield = result.BaseEXPYield;
            CatchRate = result.CatchRate;
            FleeRate = result.FleeRate;
            Weight = result.Weight;

            const char Split1Chars = '|';
            const char Split2Chars = ',';

            string[] split1 = result.PreEvolutions.Split(Split1Chars, StringSplitOptions.RemoveEmptyEntries);
            var preEvolutions = new (PBESpecies, PBEForm)[split1.Length];
            for (int i = 0; i < preEvolutions.Length; i++)
            {
                string[] split2 = split1[i].Split(Split2Chars);
                preEvolutions[i] = ((PBESpecies)ushort.Parse(split2[0]), (PBEForm)byte.Parse(split2[1]));
            }
            PreEvolutions = new ReadOnlyCollection<(PBESpecies, PBEForm)>(preEvolutions);

            split1 = result.Evolutions.Split(Split1Chars, StringSplitOptions.RemoveEmptyEntries);
            var evolutions = new (PBESpecies, PBEForm)[split1.Length];
            for (int i = 0; i < evolutions.Length; i++)
            {
                string[] split2 = split1[i].Split(Split2Chars);
                evolutions[i] = ((PBESpecies)ushort.Parse(split2[0]), (PBEForm)byte.Parse(split2[1]));
            }
            Evolutions = new ReadOnlyCollection<(PBESpecies, PBEForm)>(evolutions);

            split1 = result.Abilities.Split(Split1Chars, StringSplitOptions.RemoveEmptyEntries);
            var abilities = new PBEAbility[split1.Length];
            for (int i = 0; i < abilities.Length; i++)
            {
                abilities[i] = (PBEAbility)byte.Parse(split1[i]);
            }
            Abilities = new ReadOnlyCollection<PBEAbility>(abilities);

            split1 = result.LevelUpMoves.Split(Split1Chars, StringSplitOptions.RemoveEmptyEntries);
            var levelUpMoves = new (PBEMove, byte, PBEMoveObtainMethod)[split1.Length];
            for (int i = 0; i < levelUpMoves.Length; i++)
            {
                string[] split2 = split1[i].Split(Split2Chars);
                levelUpMoves[i] = ((PBEMove)ushort.Parse(split2[0]), byte.Parse(split2[1]), (PBEMoveObtainMethod)ulong.Parse(split2[2]));
            }
            LevelUpMoves = new ReadOnlyCollection<(PBEMove, byte, PBEMoveObtainMethod)>(levelUpMoves);

            split1 = result.OtherMoves.Split(Split1Chars, StringSplitOptions.RemoveEmptyEntries);
            var otherMoves = new (PBEMove, PBEMoveObtainMethod)[split1.Length];
            for (int i = 0; i < otherMoves.Length; i++)
            {
                string[] split2 = split1[i].Split(Split2Chars);
                otherMoves[i] = ((PBEMove)ushort.Parse(split2[0]), (PBEMoveObtainMethod)ulong.Parse(split2[1]));
            }
            OtherMoves = new ReadOnlyCollection<(PBEMove, PBEMoveObtainMethod)>(otherMoves);
        }

        #region Database Querying

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private class SearchResult : IPBEStatCollection
        {
            public ushort Species { get; set; }
            public byte Form { get; set; }
            public byte HP { get; set; }
            public byte Attack { get; set; }
            public byte Defense { get; set; }
            public byte SpAttack { get; set; }
            public byte SpDefense { get; set; }
            public byte Speed { get; set; }
            public byte Type1 { get; set; }
            public byte Type2 { get; set; }
            public byte GenderRatio { get; set; }
            public byte GrowthRate { get; set; }
            public ushort BaseEXPYield { get; set; }
            public byte CatchRate { get; set; }
            public byte FleeRate { get; set; }
            public float Weight { get; set; }
            public string PreEvolutions { get; set; }
            public string Evolutions { get; set; }
            public string Abilities { get; set; }
            public string LevelUpMoves { get; set; }
            public string OtherMoves { get; set; }
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

#pragma warning disable IDE0060 // Remove unused parameter
        public static PBEPokemonData GetData(PBESpecies species, PBEForm form, bool cache = true)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            PBELegalityChecker.ValidateSpecies(species, form, false);
            List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>($"SELECT * FROM PokemonData WHERE Species={(ushort)species} AND Form={(byte)form}");
            if (results.Count == 1)
            {
                return new PBEPokemonData(results[0]);
            }
            throw new InvalidDataException();
        }

        #endregion
    }
}
