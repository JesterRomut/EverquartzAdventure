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
	public class DeimosFumo : ModItem
	{
		public override void SetStaticDefaults() {			
            DisplayName.SetDefault("Deimos Plushie");
			DisplayName.AddTranslation(7, "戴莫斯玩偶");
			Tooltip.SetDefault("I hope you don't make it into anything suspiscious...");
			Tooltip.AddTranslation(7, "我希望你不要对它干奇怪的事情...");
		}
		public override void SetDefaults() {
			Item.width = 40; 
			Item.height = 64; 
			Item.maxStack = 24; 
			Item.value = Item.buyPrice(gold: 1); 
            Item.rare = ItemRarityID.Cyan;
		}
    }
}