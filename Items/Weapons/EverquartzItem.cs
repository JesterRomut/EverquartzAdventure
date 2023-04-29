using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using System;
using System.Linq;
using System.Collections.Generic;

namespace EverquartzAdventure.Items.Weapons
{

	public class EverquartzItem : ModItem
	{
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Ever Quartz");
			Tooltip.SetDefault("Not a reference to any person, at all, ever.");
		}
		public override void SetDefaults() {
            
			Item.width = 52;
			Item.height = 84;

			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.autoReuse = true;

			Item.DamageType = DamageClass.Magic;
			Item.damage = 50000;
			Item.knockBack = 1;
			Item.crit = 10;
			
			Item.value = 0;
			Item.rare = ItemRarityID.Red;
			Item.shoot = ProjectileID.FairyQueenMagicItemShot; 
			Item.shootSpeed = 50f; 

			
		}
        // here we see the magic that is ExampleMod. i don't code, i steal
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			if(player.altFunctionUse == 2)
			{
                //List<Item> li = player.inventory.ToList();
                //player.inventory = li.Except(li.Where(item => item.type == ModContent.ItemType<EverquartzItem>())).ToArray();
				
                return false;
			}
			
			Vector2 target = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY);
			float ceilingLimit = target.Y;
			if (ceilingLimit > player.Center.Y - 200f) {
				ceilingLimit = player.Center.Y - 200f;
			}
			
			for (int i = 0; i < 10; i++) {
				position = player.Center - new Vector2(Main.rand.NextFloat(401) * player.direction, 600f);
				position.Y -= 100 * i;
				Vector2 heading = target - position;

				if (heading.Y < 0f) {
					heading.Y *= -1f;
				}

				if (heading.Y < 20f) {
					heading.Y = 20f;
				}

				heading.Normalize();
				heading *= velocity.Length();
				heading.Y += Main.rand.Next(-40, 41) * 0.02f;
				Projectile.NewProjectile(source, position, heading, type, damage * 2, knockback, player.whoAmI, 0f, ceilingLimit);
			}
			
			return false;
		}
        public override bool AltFunctionUse(Player player)
        {
			return true;
        }

    }
}