using CalamityMod.Buffs;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using Terraria.ModLoader;

namespace EverquartzAdventure
{
    internal static partial class CalamityWeakRef
    {
        [JITWhenModsEnabled("Calamitymod")]
        internal static class BuffType
        {
            internal static int HolyFlames => ModContent.BuffType<HolyFlames>();
            internal static int GodSlayerInferno => ModContent.BuffType<GodSlayerInferno>();

            public static int MarkedForDeath => ModContent.BuffType<MarkedforDeath>();
            public static int ArmorCrunch => ModContent.BuffType<ArmorCrunch>();
            public static int KamiFlu => ModContent.BuffType<KamiFlu>();
        }
    }
}