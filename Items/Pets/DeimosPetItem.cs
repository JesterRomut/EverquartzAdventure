using EverquartzAdventure.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EverquartzAdventure.Items.Pets
{
	public class DeimosPetItem : ModItem
	{
		public override void SetStaticDefaults() {			
            DisplayName.SetDefault("Celestial Jar");
			DisplayName.AddTranslation(7, "缸中天国");
			DisplayName.AddTranslation(7, "Космическая Банка");
			Tooltip.SetDefault("Trust me guys, it's snow. Snow. Snow? \nSNOW.");
			Tooltip.AddTranslation(7, "相信我，它下的真正是雪...雪？\n雪（悲）");
			Tooltip.AddTranslation(7, "Ребят, доверьтесь мне, это снег. Снег. Снег？\nСНЕГ.");
		}
		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.ZephyrFish); 

			Item.shoot = ModContent.ProjectileType<DeimosPetProjectile>(); 
			Item.buffType = ModContent.BuffType<DeimosPetBuff>(); 
		}

		public override void UseStyle(Player player, Rectangle heldItemFrame) {
			if (player.whoAmI == Main.myPlayer && player.itemTime == 0) {
				player.AddBuff(Item.buffType, 3600);
			}
		}
        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Item.type);
            recipe.AddIngredient<DeimosFumo>(1);
            recipe.Register();
        }
	}
}