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
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Magic;

namespace EverquartzAdventure
{

    internal static partial class CalamityWeakRef
    {
        [JITWhenModsEnabled("Calamitymod")]
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

            internal static int AstralBlaster => ModContent.ItemType<AstralBlaster>();
            internal static int SeaPrism => ModContent.ItemType<SeaPrism>();
            internal static int ClockGatlignum => ModContent.ItemType<ClockGatlignum>();
            internal static int Oracle => ModContent.ItemType<Oracle>();
            internal static int TheMicrowave => ModContent.ItemType<TheMicrowave>();
            internal static int PoleWarper => ModContent.ItemType<PoleWarper>();
            internal static int RottingEyeball => ModContent.ItemType<RottingEyeball>();
            internal static int RottenMatter => ModContent.ItemType<RottenMatter>();
            internal static int BloodyVein => ModContent.ItemType<BloodyVein>();
            internal static int LeviathanAmbergris => ModContent.ItemType<LeviathanAmbergris>();
            internal static int Nucleogenesis => ModContent.ItemType<Nucleogenesis>();
            internal static int CryoStone => ModContent.ItemType<CryoStone>();
            internal static int EssenceofSunlight => ModContent.ItemType<EssenceofSunlight>();
            internal static int ShardofAntumbra => ModContent.ItemType<ShardofAntumbra>();
            internal static int LightGodsBrilliance => ModContent.ItemType<LightGodsBrilliance>();
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
