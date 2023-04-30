using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using System.Linq;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent;

namespace EverquartzAdventure.Items
{
	public class DivineCore : ModItem
	{
		public override void SetStaticDefaults() {			
            DisplayName.SetDefault("Divine Core");
			DisplayName.AddTranslation(7, "神圣核心");
			Tooltip.SetDefault("You feel such a divine warmth seethe through your body...\nSomething tells you that you should not have this item in your hands.");
			Tooltip.AddTranslation(7, "你感到如此神圣的温度在你的体中沸腾...\n你最好不要把这个拿在手里。");
		}
		public override void SetDefaults() {
			Item.width = 42; 
			Item.height = 102; 
			Item.maxStack = 10; 
			Item.value = Item.buyPrice(platinum: 5); 
            Item.rare = ItemRarityID.Lime;
		}
    }
}