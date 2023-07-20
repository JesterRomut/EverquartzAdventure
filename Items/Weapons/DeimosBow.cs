using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.DataStructures;
using CalamityMod.Projectiles.Ranged;

namespace EverquartzAdventure
{
    internal static partial class CalamityWeakRef
    {
		
    }
}

namespace EverquartzAdventure.Items.Weapons
{
	public class DeimosBow : ModItem
	{
        //public override string Texture => "EverquartzAdventure/NPCs/TownNPCs/StarbornPrincess_Head";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Celestial Heavenfire");
            //DisplayName.AddTranslation(7, "天火");
        }
        public override void SetDefaults() {
			Item.width = 20; 
			Item.height = 39; 
			//Item.scale = 0.75f;
			Item.rare = ItemRarityID.Green; 
			Item.useTime = 4; 
			Item.useAnimation = 16; 
			Item.useStyle = ItemUseStyleID.Shoot; 
			Item.autoReuse = true; 
			Item.DamageType = DamageClass.Ranged; 
			Item.damage = 520; 
			Item.knockBack = 5f; 
			Item.noMelee = true;
			if (ModCompatibility.calamityEnabled)
			{
				Item.shoot = CalamityWeakRef.ProjectileType.TelluricGlareArrow;

            }
                
			Item.shootSpeed = 45f; 
			Item.reuseDelay = 18;
		}
        //we deimos in this bitch!! !!! !! ! !! GGGGGGRRRRRRRAAAAAAAAHHHHHH
		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			velocity = velocity.RotatedByRandom(MathHelper.ToRadians(4));
			Vector2 muzzleOffset = Vector2.Normalize(velocity) * -45f;
		}
		int n = 0;
        public override bool CanShoot(Player player)
        {
            return ModCompatibility.calamityEnabled;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			for (int i = 0; i < 5; i++)
			{
    		Projectile.NewProjectile(source, position, velocity, CalamityWeakRef.ProjectileType.DWArrow, damage, knockback, player.whoAmI);
			}
			if (n == 7){
    			float numberProjectiles = 3 + Main.rand.Next(3); // 3, 4, or 5 shots
				float rotation = MathHelper.ToRadians(12);

				position += Vector2.Normalize(velocity) * 12f;

				for (int i = 0; i < numberProjectiles; i++) {
				Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .2f; // Watch out for dividing by 0 if there is only 1 projectile.
				Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, knockback, player.whoAmI);
				}
				n = 0;
			}
			n++;
			
			return false;
		}
		}}

		