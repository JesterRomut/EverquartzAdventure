using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.SummonItems;
using CalamityMod.Items;
using CalamityMod.Items.Placeables.Furniture.CraftingStations;

namespace EverquartzAdventure
{
    internal static partial class CalamityWeakRef
    {
        internal static class ItemType
        {
            internal static int ProfanedCrucible => ModContent.ItemType<ProfanedCrucible>();
            internal static int DivineGeode => ModContent.ItemType<DivineGeode>();
            internal static int NightmareFuel => ModContent.ItemType<NightmareFuel>();
            internal static int EndothermicEnergy => ModContent.ItemType<EndothermicEnergy>();
            internal static int DarksunFragment => ModContent.ItemType<DarksunFragment>();
            internal static int RuneOfKos => ModContent.ItemType<RuneofKos>();
            internal static int ElysianAegis => ModContent.ItemType<ElysianAegis>();
            internal static int AsgardianAegis => ModContent.ItemType<AsgardianAegis>();

            internal static int ExoPrism => ModContent.ItemType<ExoPrism>();
        }
    }
}

namespace EverquartzAdventure.Items
{
    //public abstract class AltUsableItem: ModItem
    //{
    //    public abstract void LeftClickBehavior();
    //    public abstract void RightClickBehavior();

    //    public override bool? UseItem(Player player)
    //    {
    //        if (player.IsAltFunctionUse())
    //        {
    //            RightClickBehavior();
    //        }
    //        else
    //        {
    //            LeftClickBehavior();
    //        }
    //        return base.UseItem(player);
    //    }

    //    public override bool AltFunctionUse(Player player)
    //    {
    //        return true;
    //    }
    //}

}
