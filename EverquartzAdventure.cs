using System.Collections.Generic;
using Terraria.ModLoader;
using System.Linq;
using Terraria;

namespace EverquartzAdventure
{
	public class EverquartzAdventure : Mod
	{
        public override void PostSetupContent()
        {
			ModCompatibility.calamityEnabled = ModLoader.HasMod("CalamityMod");
        }
    }

	public static class ModCompatibility
	{
		public static bool calamityEnabled = false;
	}

	[JITWhenModsEnabled("CalamityMod")]
	internal static partial class CalamityWeakRef
	{

	}

    internal static class EverquartzExtensions
    {
        internal static T Random<T>(this IEnumerable<T> li)
        {
            return li.ElementAt(Main.rand.Next(li.Count()));
        }
    }
    }