﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Data
{
    public static partial class PBEDataUtils
    {
        #region Static Collections
        public static PBEAlphabeticalList<PBEMove> AllMoves { get; } = new PBEAlphabeticalList<PBEMove>(Enum.GetValues(typeof(PBEMove)).Cast<PBEMove>().Except(new[] { PBEMove.None, PBEMove.MAX }));
        public static PBEAlphabeticalList<PBEMove> MetronomeMoves { get; } = new PBEAlphabeticalList<PBEMove>(GetMovesWithoutFlag(PBEMoveFlag.BlockedFromMetronome));
        public static PBEAlphabeticalList<PBEMove> SketchLegalMoves { get; } = new PBEAlphabeticalList<PBEMove>(GetMovesWithoutFlag(PBEMoveFlag.BlockedFromSketch, exception: PBEMove.Sketch));

        public static PBEAlphabeticalList<PBEStat> MoodyStats { get; } = new PBEAlphabeticalList<PBEStat>(new[] { PBEStat.Attack, PBEStat.Defense, PBEStat.SpAttack, PBEStat.SpDefense, PBEStat.Speed, PBEStat.Accuracy, PBEStat.Evasion });
        public static PBEAlphabeticalList<PBEStat> StarfBerryStats { get; } = new PBEAlphabeticalList<PBEStat>(new[] { PBEStat.Attack, PBEStat.Defense, PBEStat.SpAttack, PBEStat.SpDefense, PBEStat.Speed });
        #endregion

        private static IEnumerable<PBEMove> GetMovesWithoutFlag(PBEMoveFlag flag, PBEMove exception = PBEMove.None)
        {
            return AllMoves.Where(m =>
            {
                if (exception != PBEMove.None && m == exception)
                {
                    return true;
                }
                PBEMoveData mData = PBEMoveData.Data[m];
                return mData.IsMoveUsable() && !mData.Flags.HasFlag(flag);
            });
        }
    }
}