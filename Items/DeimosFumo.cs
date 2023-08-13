using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using System.Linq;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent;
using EverquartzAdventure.NPCs.Hypnos;
using EverquartzAdventure.Tiles;

namespace EverquartzAdventure.Items
{
	public class DeimosFumo : ModItem
	{
		public override void SetStaticDefaults() {
            
            // DisplayName.SetDefault("Deimos Plushie");
            //DisplayName.AddTranslation(7, "戴莫斯玩偶");
            //DisplayName.AddTranslation(6, "Плюшевая Игрушка Деймоса");
            //// Tooltip.SetDefault("I hope you don't make it into anything suspiscious...");
            //Tooltip.AddTranslation(7, "我希望你不要对它干奇怪的事情...");
            //Tooltip.AddTranslation(6, "Понадеемся, что ты не будешь делать ничего подозрительного из этой вещи...");
        }
		public override void SetDefaults() {
            //Item.SetFoodDefault();
            Item.width = 40; 
			Item.height = 64; 
			Item.maxStack = 24; 
			Item.value = Item.buyPrice(gold: 1); 
            Item.rare = ItemRarityID.Cyan;
            base.Item.consumable = true;
            base.Item.createTile = ModContent.TileType<DeimosFumoPlaced>();
            base.Item.useAnimation = 20;
            base.Item.useTime = 20;
            base.Item.noUseGraphic = true;
            base.Item.noMelee = true;
            base.Item.UseSound = SoundID.Item1;
            base.Item.useStyle = 1;
        }
    }
}