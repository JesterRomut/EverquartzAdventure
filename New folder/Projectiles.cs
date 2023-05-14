using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Ranged;
using Terraria.ModLoader;

namespace EverquartzAdventure
{
    internal static partial class CalamityWeakRef
    {
        internal static class ProjectileType
        {
            public static int TelluricGlareArrow => ModContent.ProjectileType<TelluricGlareArrow>();
            public static int DWArrow => ModContent.ProjectileType<DWArrow>();
            public static int DeathhailBeam => ModContent.ProjectileType<DeathhailBeam>();
            public static int HolyLaser => ModContent.ProjectileType<HolyLaser>();
        }
    }
}