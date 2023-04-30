using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using System.Linq;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent;

namespace EverquartzAdventure
{
	internal static partial class CalamityWeakRef
	{

	}
}

namespace EverquartzAdventure.Items
{
	public class DivineCore : ModItem
	{
        public static readonly int buffDuration = 126000;
        public static readonly List<int> buffs = new List<int>() {
            BuffID.WellFed3
        };
		public override void SetStaticDefaults() {			
            DisplayName.SetDefault("Divine Core");
			DisplayName.AddTranslation(7, "神圣核心");
			DisplayName.AddTranslation(6, "Божественное Ядро");
			Tooltip.SetDefault("You feel such a divine warmth seethe through your body...\nSomething tells you that you should not have this item in your hands.");
			Tooltip.AddTranslation(7, "你感到如此神圣的温度在你的体中沸腾...\n你最好不要把这个拿在手里。");
			Tooltip.AddTranslation(6, "Вы чувствуете, как божественная теплота бурлит в вашем теле...\nЧто то говорит тебе, что этот предмет не должен быть в твоих руках.");
		}
		public override void SetDefaults() {
			Item.width = 42; 
			Item.height = 102; 
			Item.maxStack = 1; 
			Item.value = Item.buyPrice(platinum: 5); 
            Item.rare = ItemRarityID.Lime;

            base.Item.consumable = true;
            base.Item.useAnimation = 17;
            base.Item.useTime = 17;
            base.Item.UseSound = SoundID.Item2;
            base.Item.useStyle = ItemUseStyleID.EatFood;
            base.Item.useTurn = true;
            base.Item.buffType = BuffID.WellFed3;
            base.Item.buffTime = 3600 * 60;
        }

        public override void HoldItem(Player player)
        {
            if (ModCompatibility.calamityEnabled)
            {
                player.AddBuff(CalamityWeakRef.HolyFlamesBuff, 120);
            }
			
        }

        public override bool ConsumeItem(Player player)
        {
            //buffs.ForEach(buff => player.AddBuff(buff, buffDuration));
            return false;
        }

    }
}