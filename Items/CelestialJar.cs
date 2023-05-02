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
	public class CelestialJar : ModItem
	{
		public override void SetStaticDefaults() {			
            DisplayName.SetDefault("Celestial Jar");
			DisplayName.AddTranslation(7, "缸中天国");
			Tooltip.SetDefault("Trust me guys, it's snow. Snow. Snow? \nSNOW.");
			Tooltip.AddTranslation(7, "相信我，它下的真正是雪...雪？\n雪（悲）");
		}
		public override void SetDefaults() {
            Item.SetFoodDefault();
            Item.width = 20; 
			Item.height = 26; 
			Item.maxStack = 9999; 
			Item.value = Item.buyPrice(gold: 1); 
            Item.rare = ItemRarityID.Expert;
            
        }
        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Item.type);
            recipe.AddIngredient<DeimosFumo>(1);
            recipe.Register();
        }
    }
}