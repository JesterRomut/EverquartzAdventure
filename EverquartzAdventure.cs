using Terraria.ModLoader;

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
}