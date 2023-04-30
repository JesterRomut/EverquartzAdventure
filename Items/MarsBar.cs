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
    public class MarsBar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mars Bar");
            DisplayName.AddTranslation(7, "火星锭");
            Tooltip.SetDefault("Delicious!");
            Tooltip.AddTranslation(7, "好吃！");
        }
        public override void SetDefaults()
        {
            Item.width = 360;
            Item.height = 280;
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ItemRarityID.Purple;
        }
        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(ItemID.MartianConduitPlating, 100);
            recipe.AddIngredient<MarsBar>();
            recipe.Register();
        }
    }
}