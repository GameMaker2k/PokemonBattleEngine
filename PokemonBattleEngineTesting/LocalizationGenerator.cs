﻿using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Kermalis.PokemonBattleEngineTesting
{
    class LocalizationGenerator
    {
        readonly NARC english, french, german, italian, japanese, korean, spanish;

        public LocalizationGenerator()
        {
            english = new NARC(@"../../../\Dumped Data\English.narc");
            french = new NARC(@"../../../\Dumped Data\French.narc");
            german = new NARC(@"../../../\Dumped Data\German.narc");
            italian = new NARC(@"../../../\Dumped Data\Italian.narc");
            japanese = new NARC(@"../../../\Dumped Data\Japanese.narc");
            korean = new NARC(@"../../../\Dumped Data\Korean.narc");
            spanish = new NARC(@"../../../\Dumped Data\Spanish.narc");
        }

        string[][] eng, fre, ger, ita, jap, kor, spa;
        void LoadTexts(int fileNum)
        {
            string[][] ReadTextFile(NARC narc)
            {
                using (var r = new BinaryReader(narc.Files[fileNum]))
                {
                    ushort numBlocks = r.ReadUInt16();
                    ushort numEntries = r.ReadUInt16();
                    r.ReadUInt32(); // fileSize
                    r.ReadUInt32(); // padding
                    var texts = new string[numBlocks][];
                    var blockOffsets = new uint[numBlocks];
                    for (int i = 0; i < numBlocks; i++)
                    {
                        texts[i] = new string[numEntries];
                        blockOffsets[i] = r.ReadUInt32();
                    }
                    for (int i = 0; i < numBlocks; i++)
                    {
                        r.BaseStream.Position = blockOffsets[i];
                        r.ReadUInt32(); // blockSize
                        var stringOffsets = new uint[numEntries];
                        var stringLengths = new ushort[numEntries];
                        for (int j = 0; j < numEntries; j++)
                        {
                            stringOffsets[j] = r.ReadUInt32();
                            stringLengths[j] = r.ReadUInt16();
                            r.ReadUInt16(); // textFlags[j]
                        }
                        for (int j = 0; j < numEntries; j++)
                        {
                            r.BaseStream.Position = blockOffsets[i] + stringOffsets[j];
                            var encoded = new ushort[stringLengths[j]];
                            for (int k = 0; k < stringLengths[j]; k++)
                            {
                                encoded[k] = r.ReadUInt16();
                            }
                            int key = encoded[stringLengths[j] - 1] ^ 0xFFFF;
                            var decoded = new int[stringLengths[j]];
                            for (int k = stringLengths[j] - 1; k >= 0; k--)
                            {
                                decoded[k] = encoded[k] ^ key;
                                key = ((key >> 3) | (key << 13)) & 0xFFFF;
                            }
                            for (int k = 0; k < stringLengths[j]; k++)
                            {
                                int c = decoded[k];
                                if (c == 0xFFFF)
                                {
                                    break;
                                }
                                else
                                {
                                    string car;
                                    switch (c)
                                    {
                                        case '"': car = "\\\""; break;
                                        case 0x246D: car = "♂"; break;
                                        case 0x246E: car = "♀"; break;
                                        case 0x2486: car = "[PK]"; break;
                                        case 0x2487: car = "[MN]"; break;
                                        case 0xFFFE: car = "\\n"; break;
                                        default: car = ((char)c).ToString(); break;
                                    }
                                    texts[i][j] += car;
                                }
                            }
                        }
                    }
                    return texts;
                }
            }

            eng = ReadTextFile(english);
            fre = ReadTextFile(french);
            ger = ReadTextFile(german);
            ita = ReadTextFile(italian);
            jap = ReadTextFile(japanese);
            kor = ReadTextFile(korean);
            spa = ReadTextFile(spanish);
        }

        public void GenerateAbilities()
        {
            IEnumerable<PBEAbility> allAbilities = new[] { PBEAbility.None }.Concat(Enum.GetValues(typeof(PBEAbility)).Cast<PBEAbility>().Except(new[] { PBEAbility.None, PBEAbility.MAX }).OrderBy(e => e.ToString()));
            PBEAbility lastAbility = allAbilities.Last();
            var sb = new StringBuilder();
            void WriteAll()
            {
                sb.AppendLine("        {");
                foreach (PBEAbility ability in allAbilities)
                {
                    byte i = (byte)ability;
                    sb.AppendLine($"            {{ PBEAbility.{ability}, new PBELocalizedString(\"{eng[0][i]}\", \"{fre[0][i]}\", \"{ger[0][i]}\", \"{ita[0][i]}\", \"{jap[0][i]}\", \"{jap[1][i]}\", \"{kor[0][i]}\", \"{spa[0][i]}\") }}{(ability == lastAbility ? string.Empty : ",")}");
                }
                sb.AppendLine("        });");
            }

            sb.AppendLine("using Kermalis.PokemonBattleEngine.Data;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Collections.ObjectModel;");
            sb.AppendLine();
            sb.AppendLine("namespace Kermalis.PokemonBattleEngine.Localization");
            sb.AppendLine("{");
            sb.AppendLine("    public static class PBEAbilityLocalization");
            sb.AppendLine("    {");
            sb.AppendLine("        public static ReadOnlyDictionary<PBEAbility, PBELocalizedString> Names { get; } = new ReadOnlyDictionary<PBEAbility, PBELocalizedString>(new Dictionary<PBEAbility, PBELocalizedString>()");
            LoadTexts(374);
            WriteAll();
            sb.AppendLine();
            sb.AppendLine("        public static ReadOnlyDictionary<PBEAbility, PBELocalizedString> Descriptions { get; } = new ReadOnlyDictionary<PBEAbility, PBELocalizedString>(new Dictionary<PBEAbility, PBELocalizedString>()");
            LoadTexts(375);
            WriteAll();
            sb.AppendLine("    }");
            sb.AppendLine("}");
            File.WriteAllText(@"../../../../\PokemonBattleEngine\Localization\AbilityLocalization.cs", sb.ToString());
        }
        public void GenerateItems()
        {
            IEnumerable<PBEItem> allItems = new[] { PBEItem.None }.Concat(Enum.GetValues(typeof(PBEItem)).Cast<PBEItem>().Except(new[] { PBEItem.None }).OrderBy(e => e.ToString()));
            PBEItem lastItem = allItems.Last();
            var sb = new StringBuilder();
            void WriteAll()
            {
                sb.AppendLine("        {");
                foreach (PBEItem item in allItems)
                {
                    ushort i = (ushort)item;
                    sb.AppendLine($"            {{ PBEItem.{item}, new PBELocalizedString(\"{eng[0][i]}\", \"{fre[0][i]}\", \"{ger[0][i]}\", \"{ita[0][i]}\", \"{jap[0][i]}\", \"{jap[1][i]}\", \"{kor[0][i]}\", \"{spa[0][i]}\") }}{(item == lastItem ? string.Empty : ",")}");
                }
                sb.AppendLine("        });");
            }

            sb.AppendLine("using Kermalis.PokemonBattleEngine.Data;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Collections.ObjectModel;");
            sb.AppendLine();
            sb.AppendLine("namespace Kermalis.PokemonBattleEngine.Localization");
            sb.AppendLine("{");
            sb.AppendLine("    public static class PBEItemLocalization");
            sb.AppendLine("    {");
            sb.AppendLine("        public static ReadOnlyDictionary<PBEItem, PBELocalizedString> Names { get; } = new ReadOnlyDictionary<PBEItem, PBELocalizedString>(new Dictionary<PBEItem, PBELocalizedString>()");
            LoadTexts(64);
            WriteAll();
            sb.AppendLine();
            sb.AppendLine("        public static ReadOnlyDictionary<PBEItem, PBELocalizedString> Descriptions { get; } = new ReadOnlyDictionary<PBEItem, PBELocalizedString>(new Dictionary<PBEItem, PBELocalizedString>()");
            LoadTexts(63);
            WriteAll();
            sb.AppendLine("    }");
            sb.AppendLine("}");
            File.WriteAllText(@"../../../../\PokemonBattleEngine\Localization\ItemLocalization.cs", sb.ToString());
        }
        public void GenerateMoves()
        {
            const ushort lastMove = (ushort)(PBEMove.MAX - 1);
            var sb = new StringBuilder();
            void WriteAll()
            {
                sb.AppendLine("        {");
                for (ushort i = 0; i <= lastMove; i++)
                {
                    sb.AppendLine($"            {(Enum.IsDefined(typeof(PBEMove), i) ? string.Empty : "// ")}{{ PBEMove.{(PBEMove)i}, new PBELocalizedString(\"{eng[0][i]}\", \"{fre[0][i]}\", \"{ger[0][i]}\", \"{ita[0][i]}\", \"{jap[0][i]}\", \"{jap[1][i]}\", \"{kor[0][i]}\", \"{spa[0][i]}\") }}{(i == lastMove ? string.Empty : ",")}");
                }
                sb.AppendLine("        });");
            }

            sb.AppendLine("using Kermalis.PokemonBattleEngine.Data;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Collections.ObjectModel;");
            sb.AppendLine();
            sb.AppendLine("namespace Kermalis.PokemonBattleEngine.Localization");
            sb.AppendLine("{");
            sb.AppendLine("    public static class PBEMoveLocalization");
            sb.AppendLine("    {");
            sb.AppendLine("        public static ReadOnlyDictionary<PBEMove, PBELocalizedString> Names { get; } = new ReadOnlyDictionary<PBEMove, PBELocalizedString>(new Dictionary<PBEMove, PBELocalizedString>()");
            LoadTexts(403);
            WriteAll();
            sb.AppendLine();
            sb.AppendLine("        public static ReadOnlyDictionary<PBEMove, PBELocalizedString> Descriptions { get; } = new ReadOnlyDictionary<PBEMove, PBELocalizedString>(new Dictionary<PBEMove, PBELocalizedString>()");
            LoadTexts(402);
            WriteAll();
            sb.AppendLine("    }");
            sb.AppendLine("}");
            File.WriteAllText(@"../../../../\PokemonBattleEngine\Localization\MoveLocalization.cs", sb.ToString());
        }
        public void GeneratePokemon()
        {
            const uint lastSpecies = 649;
            var sb = new StringBuilder();
            void WriteAll()
            {
                sb.AppendLine("        {");
                for (uint i = 1; i <= lastSpecies; i++)
                {
                    sb.AppendLine($"            {(Enum.IsDefined(typeof(PBESpecies), i) ? string.Empty : "// ")}{{ PBESpecies.{(PBESpecies)i}, new PBELocalizedString(\"{eng[0][i]}\", \"{fre[0][i]}\", \"{ger[0][i]}\", \"{ita[0][i]}\", \"{jap[0][i]}\", \"{jap[1][i]}\", \"{kor[0][i]}\", \"{spa[0][i]}\") }}{(i == lastSpecies ? string.Empty : ",")}");
                }
                sb.AppendLine("        });");
            }

            sb.AppendLine("using Kermalis.PokemonBattleEngine.Data;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Collections.ObjectModel;");
            sb.AppendLine();
            sb.AppendLine("namespace Kermalis.PokemonBattleEngine.Localization");
            sb.AppendLine("{");
            sb.AppendLine("    public static class PBEPokemonLocalization");
            sb.AppendLine("    {");
            sb.AppendLine("        public static ReadOnlyDictionary<PBESpecies, PBELocalizedString> Names { get; } = new ReadOnlyDictionary<PBESpecies, PBELocalizedString>(new Dictionary<PBESpecies, PBELocalizedString>()");
            LoadTexts(90);
            WriteAll();
            sb.AppendLine();
            sb.AppendLine("        public static ReadOnlyDictionary<PBESpecies, PBELocalizedString> Entries { get; } = new ReadOnlyDictionary<PBESpecies, PBELocalizedString>(new Dictionary<PBESpecies, PBELocalizedString>()");
            LoadTexts(442);
            WriteAll();
            sb.AppendLine();
            sb.AppendLine("        public static ReadOnlyDictionary<PBESpecies, PBELocalizedString> Categories { get; } = new ReadOnlyDictionary<PBESpecies, PBELocalizedString>(new Dictionary<PBESpecies, PBELocalizedString>()");
            LoadTexts(464);
            WriteAll();
            sb.AppendLine("    }");
            sb.AppendLine("}");
            File.WriteAllText(@"../../../../\PokemonBattleEngine\Localization\PokemonLocalization.cs", sb.ToString());
        }
    }
}
