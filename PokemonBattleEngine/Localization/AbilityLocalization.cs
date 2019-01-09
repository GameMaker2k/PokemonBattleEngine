using Kermalis.PokemonBattleEngine.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Kermalis.PokemonBattleEngine.Localization
{
    public static class PBEAbilityLocalization
    {
        public static ReadOnlyDictionary<PBEAbility, PBELocalizedString> Names { get; } = new ReadOnlyDictionary<PBEAbility, PBELocalizedString>(new Dictionary<PBEAbility, PBELocalizedString>()
        {
            { PBEAbility.None, new PBELocalizedString("--", "--") },
            { PBEAbility.Adaptability, new PBELocalizedString("Adaptability", "てきおうりょく") },
            { PBEAbility.Aftermath, new PBELocalizedString("Aftermath", "ゆうばく") },
            { PBEAbility.AirLock, new PBELocalizedString("Air Lock", "エアロック") },
            { PBEAbility.Analytic, new PBELocalizedString("Analytic", "アナライズ") },
            { PBEAbility.AngerPoint, new PBELocalizedString("Anger Point", "いかりのつぼ") },
            { PBEAbility.Anticipation, new PBELocalizedString("Anticipation", "きけんよち") },
            { PBEAbility.ArenaTrap, new PBELocalizedString("Arena Trap", "ありじごく") },
            { PBEAbility.BadDreams, new PBELocalizedString("Bad Dreams", "ナイトメア") },
            { PBEAbility.BattleArmor, new PBELocalizedString("Battle Armor", "カブトアーマー") },
            { PBEAbility.BigPecks, new PBELocalizedString("Big Pecks", "はとむね") },
            { PBEAbility.Blaze, new PBELocalizedString("Blaze", "もうか") },
            { PBEAbility.Chlorophyll, new PBELocalizedString("Chlorophyll", "ようりょくそ") },
            { PBEAbility.ClearBody, new PBELocalizedString("Clear Body", "クリアボディ") },
            { PBEAbility.CloudNine, new PBELocalizedString("Cloud Nine", "ノーてんき") },
            { PBEAbility.ColorChange, new PBELocalizedString("Color Change", "へんしょく") },
            { PBEAbility.Compoundeyes, new PBELocalizedString("Compoundeyes", "ふくがん") },
            { PBEAbility.Contrary, new PBELocalizedString("Contrary", "あまのじゃく") },
            { PBEAbility.CursedBody, new PBELocalizedString("Cursed Body", "のろわれボディ") },
            { PBEAbility.CuteCharm, new PBELocalizedString("Cute Charm", "メロメロボディ") },
            { PBEAbility.Damp, new PBELocalizedString("Damp", "しめりけ") },
            { PBEAbility.Defeatist, new PBELocalizedString("Defeatist", "よわき") },
            { PBEAbility.Defiant, new PBELocalizedString("Defiant", "まけんき") },
            { PBEAbility.Download, new PBELocalizedString("Download", "ダウンロード") },
            { PBEAbility.Drizzle, new PBELocalizedString("Drizzle", "あめふらし") },
            { PBEAbility.Drought, new PBELocalizedString("Drought", "ひでり") },
            { PBEAbility.DrySkin, new PBELocalizedString("Dry Skin", "かんそうはだ") },
            { PBEAbility.EarlyBird, new PBELocalizedString("Early Bird", "はやおき") },
            { PBEAbility.EffectSpore, new PBELocalizedString("Effect Spore", "ほうし") },
            { PBEAbility.Filter, new PBELocalizedString("Filter", "フィルター") },
            { PBEAbility.FlameBody, new PBELocalizedString("Flame Body", "ほのおのからだ") },
            { PBEAbility.FlareBoost, new PBELocalizedString("Flare Boost", "ねつぼうそう") },
            { PBEAbility.FlashFire, new PBELocalizedString("Flash Fire", "もらいび") },
            { PBEAbility.FlowerGift, new PBELocalizedString("Flower Gift", "フラワーギフト") },
            { PBEAbility.Forecast, new PBELocalizedString("Forecast", "てんきや") },
            { PBEAbility.Forewarn, new PBELocalizedString("Forewarn", "よちむ") },
            { PBEAbility.FriendGuard, new PBELocalizedString("Friend Guard", "フレンドガード") },
            { PBEAbility.Frisk, new PBELocalizedString("Frisk", "おみとおし") },
            { PBEAbility.Gluttony, new PBELocalizedString("Gluttony", "くいしんぼう") },
            { PBEAbility.Guts, new PBELocalizedString("Guts", "こんじょう") },
            { PBEAbility.Harvest, new PBELocalizedString("Harvest", "しゅうかく") },
            { PBEAbility.Healer, new PBELocalizedString("Healer", "いやしのこころ") },
            { PBEAbility.Heatproof, new PBELocalizedString("Heatproof", "たいねつ") },
            { PBEAbility.HeavyMetal, new PBELocalizedString("Heavy Metal", "ヘヴィメタル") },
            { PBEAbility.HoneyGather, new PBELocalizedString("Honey Gather", "みつあつめ") },
            { PBEAbility.HugePower, new PBELocalizedString("Huge Power", "ちからもち") },
            { PBEAbility.Hustle, new PBELocalizedString("Hustle", "はりきり") },
            { PBEAbility.Hydration, new PBELocalizedString("Hydration", "うるおいボディ") },
            { PBEAbility.HyperCutter, new PBELocalizedString("Hyper Cutter", "かいりきバサミ") },
            { PBEAbility.IceBody, new PBELocalizedString("Ice Body", "アイスボディ") },
            { PBEAbility.Illuminate, new PBELocalizedString("Illuminate", "はっこう") },
            { PBEAbility.Illusion, new PBELocalizedString("Illusion", "イリュージョン") },
            { PBEAbility.Immunity, new PBELocalizedString("Immunity", "めんえき") },
            { PBEAbility.Imposter, new PBELocalizedString("Imposter", "かわりもの") },
            { PBEAbility.Infiltrator, new PBELocalizedString("Infiltrator", "すりぬけ") },
            { PBEAbility.InnerFocus, new PBELocalizedString("Inner Focus", "せいしんりょく") },
            { PBEAbility.Insomnia, new PBELocalizedString("Insomnia", "ふみん") },
            { PBEAbility.Intimidate, new PBELocalizedString("Intimidate", "いかく") },
            { PBEAbility.IronBarbs, new PBELocalizedString("Iron Barbs", "てつのトゲ") },
            { PBEAbility.IronFist, new PBELocalizedString("Iron Fist", "てつのこぶし") },
            { PBEAbility.Justified, new PBELocalizedString("Justified", "せいぎのこころ") },
            { PBEAbility.KeenEye, new PBELocalizedString("Keen Eye", "するどいめ") },
            { PBEAbility.Klutz, new PBELocalizedString("Klutz", "ぶきよう") },
            { PBEAbility.LeafGuard, new PBELocalizedString("Leaf Guard", "リーフガード") },
            { PBEAbility.Levitate, new PBELocalizedString("Levitate", "ふゆう") },
            { PBEAbility.LightMetal, new PBELocalizedString("Light Metal", "ライトメタル") },
            { PBEAbility.Lightningrod, new PBELocalizedString("Lightningrod", "ひらいしん") },
            { PBEAbility.Limber, new PBELocalizedString("Limber", "じゅうなん") },
            { PBEAbility.LiquidOoze, new PBELocalizedString("Liquid Ooze", "ヘドロえき") },
            { PBEAbility.MagicBounce, new PBELocalizedString("Magic Bounce", "マジックミラー") },
            { PBEAbility.MagicGuard, new PBELocalizedString("Magic Guard", "マジックガード") },
            { PBEAbility.MagmaArmor, new PBELocalizedString("Magma Armor", "マグマのよろい") },
            { PBEAbility.MagnetPull, new PBELocalizedString("Magnet Pull", "じりょく") },
            { PBEAbility.MarvelScale, new PBELocalizedString("Marvel Scale", "ふしぎなうろこ") },
            { PBEAbility.Minus, new PBELocalizedString("Minus", "マイナス") },
            { PBEAbility.MoldBreaker, new PBELocalizedString("Mold Breaker", "かたやぶり") },
            { PBEAbility.Moody, new PBELocalizedString("Moody", "ムラっけ") },
            { PBEAbility.MotorDrive, new PBELocalizedString("Motor Drive", "でんきエンジン") },
            { PBEAbility.Moxie, new PBELocalizedString("Moxie", "じしんかじょう") },
            { PBEAbility.Multiscale, new PBELocalizedString("Multiscale", "マルチスケイル") },
            { PBEAbility.Multitype, new PBELocalizedString("Multitype", "マルチタイプ") },
            { PBEAbility.Mummy, new PBELocalizedString("Mummy", "ミイラ") },
            { PBEAbility.NaturalCure, new PBELocalizedString("Natural Cure", "しぜんかいふく") },
            { PBEAbility.NoGuard, new PBELocalizedString("No Guard", "ノーガード") },
            { PBEAbility.Normalize, new PBELocalizedString("Normalize", "ノーマルスキン") },
            { PBEAbility.Oblivious, new PBELocalizedString("Oblivious", "どんかん") },
            { PBEAbility.Overcoat, new PBELocalizedString("Overcoat", "ぼうじん") },
            { PBEAbility.Overgrow, new PBELocalizedString("Overgrow", "しんりょく") },
            { PBEAbility.OwnTempo, new PBELocalizedString("Own Tempo", "マイペース") },
            { PBEAbility.Pickpocket, new PBELocalizedString("Pickpocket", "わるいてぐせ") },
            { PBEAbility.Pickup, new PBELocalizedString("Pickup", "ものひろい") },
            { PBEAbility.Plus, new PBELocalizedString("Plus", "プラス") },
            { PBEAbility.PoisonHeal, new PBELocalizedString("Poison Heal", "ポイズンヒール") },
            { PBEAbility.PoisonPoint, new PBELocalizedString("Poison Point", "どくのトゲ") },
            { PBEAbility.PoisonTouch, new PBELocalizedString("Poison Touch", "どくしゅ") },
            { PBEAbility.Prankster, new PBELocalizedString("Prankster", "いたずらごころ") },
            { PBEAbility.Pressure, new PBELocalizedString("Pressure", "プレッシャー") },
            { PBEAbility.PurePower, new PBELocalizedString("Pure Power", "ヨガパワー") },
            { PBEAbility.QuickFeet, new PBELocalizedString("Quick Feet", "はやあし") },
            { PBEAbility.RainDish, new PBELocalizedString("Rain Dish", "あめうけざら") },
            { PBEAbility.Rattled, new PBELocalizedString("Rattled", "びびり") },
            { PBEAbility.Reckless, new PBELocalizedString("Reckless", "すてみ") },
            { PBEAbility.Regenerator, new PBELocalizedString("Regenerator", "さいせいりょく") },
            { PBEAbility.Rivalry, new PBELocalizedString("Rivalry", "とうそうしん") },
            { PBEAbility.RockHead, new PBELocalizedString("Rock Head", "いしあたま") },
            { PBEAbility.RoughSkin, new PBELocalizedString("Rough Skin", "さめはだ") },
            { PBEAbility.RunAway, new PBELocalizedString("Run Away", "にげあし") },
            { PBEAbility.SandForce, new PBELocalizedString("Sand Force", "すなのちから") },
            { PBEAbility.SandRush, new PBELocalizedString("Sand Rush", "すなかき") },
            { PBEAbility.SandStream, new PBELocalizedString("Sand Stream", "すなおこし") },
            { PBEAbility.SandVeil, new PBELocalizedString("Sand Veil", "すながくれ") },
            { PBEAbility.SapSipper, new PBELocalizedString("Sap Sipper", "そうしょく") },
            { PBEAbility.Scrappy, new PBELocalizedString("Scrappy", "きもったま") },
            { PBEAbility.SereneGrace, new PBELocalizedString("Serene Grace", "てんのめぐみ") },
            { PBEAbility.ShadowTag, new PBELocalizedString("Shadow Tag", "かげふみ") },
            { PBEAbility.ShedSkin, new PBELocalizedString("Shed Skin", "だっぴ") },
            { PBEAbility.SheerForce, new PBELocalizedString("Sheer Force", "ちからずく") },
            { PBEAbility.ShellArmor, new PBELocalizedString("Shell Armor", "シェルアーマー") },
            { PBEAbility.ShieldDust, new PBELocalizedString("Shield Dust", "りんぷん") },
            { PBEAbility.Simple, new PBELocalizedString("Simple", "たんじゅん") },
            { PBEAbility.SkillLink, new PBELocalizedString("Skill Link", "スキルリンク") },
            { PBEAbility.SlowStart, new PBELocalizedString("Slow Start", "スロースタート") },
            { PBEAbility.Sniper, new PBELocalizedString("Sniper", "スナイパー") },
            { PBEAbility.SnowCloak, new PBELocalizedString("Snow Cloak", "ゆきがくれ") },
            { PBEAbility.SnowWarning, new PBELocalizedString("Snow Warning", "ゆきふらし") },
            { PBEAbility.SolarPower, new PBELocalizedString("Solar Power", "サンパワー") },
            { PBEAbility.SolidRock, new PBELocalizedString("Solid Rock", "ハードロック") },
            { PBEAbility.Soundproof, new PBELocalizedString("Soundproof", "ぼうおん") },
            { PBEAbility.SpeedBoost, new PBELocalizedString("Speed Boost", "かそく") },
            { PBEAbility.Stall, new PBELocalizedString("Stall", "あとだし") },
            { PBEAbility.Static, new PBELocalizedString("Static", "せいでんき") },
            { PBEAbility.Steadfast, new PBELocalizedString("Steadfast", "ふくつのこころ") },
            { PBEAbility.Stench, new PBELocalizedString("Stench", "あくしゅう") },
            { PBEAbility.StickyHold, new PBELocalizedString("Sticky Hold", "ねんちゃく") },
            { PBEAbility.StormDrain, new PBELocalizedString("Storm Drain", "よびみず") },
            { PBEAbility.Sturdy, new PBELocalizedString("Sturdy", "がんじょう") },
            { PBEAbility.SuctionCups, new PBELocalizedString("Suction Cups", "きゅうばん") },
            { PBEAbility.SuperLuck, new PBELocalizedString("Super Luck", "きょううん") },
            { PBEAbility.Swarm, new PBELocalizedString("Swarm", "むしのしらせ") },
            { PBEAbility.SwiftSwim, new PBELocalizedString("Swift Swim", "すいすい") },
            { PBEAbility.Synchronize, new PBELocalizedString("Synchronize", "シンクロ") },
            { PBEAbility.TangledFeet, new PBELocalizedString("Tangled Feet", "ちどりあし") },
            { PBEAbility.Technician, new PBELocalizedString("Technician", "テクニシャン") },
            { PBEAbility.Telepathy, new PBELocalizedString("Telepathy", "テレパシー") },
            { PBEAbility.Teravolt, new PBELocalizedString("Teravolt", "テラボルテージ") },
            { PBEAbility.ThickFat, new PBELocalizedString("Thick Fat", "あついしぼう") },
            { PBEAbility.TintedLens, new PBELocalizedString("Tinted Lens", "いろめがね") },
            { PBEAbility.Torrent, new PBELocalizedString("Torrent", "げきりゅう") },
            { PBEAbility.ToxicBoost, new PBELocalizedString("Toxic Boost", "どくぼうそう") },
            { PBEAbility.Trace, new PBELocalizedString("Trace", "トレース") },
            { PBEAbility.Truant, new PBELocalizedString("Truant", "なまけ") },
            { PBEAbility.Turboblaze, new PBELocalizedString("Turboblaze", "ターボブレイズ") },
            { PBEAbility.Unaware, new PBELocalizedString("Unaware", "てんねん") },
            { PBEAbility.Unburden, new PBELocalizedString("Unburden", "かるわざ") },
            { PBEAbility.Unnerve, new PBELocalizedString("Unnerve", "きんちょうかん") },
            { PBEAbility.VictoryStar, new PBELocalizedString("Victory Star", "しょうりのほし") },
            { PBEAbility.VitalSpirit, new PBELocalizedString("Vital Spirit", "やるき") },
            { PBEAbility.VoltAbsorb, new PBELocalizedString("Volt Absorb", "ちくでん") },
            { PBEAbility.WaterAbsorb, new PBELocalizedString("Water Absorb", "ちょすい") },
            { PBEAbility.WaterVeil, new PBELocalizedString("Water Veil", "みずのベール") },
            { PBEAbility.WeakArmor, new PBELocalizedString("Weak Armor", "くだけるよろい") },
            { PBEAbility.WhiteSmoke, new PBELocalizedString("White Smoke", "しろいけむり") },
            { PBEAbility.WonderGuard, new PBELocalizedString("Wonder Guard", "ふしぎなまもり") },
            { PBEAbility.WonderSkin, new PBELocalizedString("Wonder Skin", "ミラクルスキン") },
            { PBEAbility.ZenMode, new PBELocalizedString("Zen Mode", "ダルマモード") }
        });
    }
}