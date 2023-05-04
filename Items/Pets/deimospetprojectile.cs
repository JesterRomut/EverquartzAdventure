using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EverquartzAdventure.Items.Pets
{
	public class deimospetprojectile : ModProjectile
	{
		public override void SetStaticDefaults() {
			Main.projFrames[Projectile.type] = 6;
			Main.projPet[Projectile.type] = true;
		}

		public override void SetDefaults() {
			Projectile.CloneDefaults(ProjectileID.ZephyrFish); // Copy the stats of the Zephyr Fish

			AIType = ProjectileID.ZephyrFish; // Mimic as the Zephyr Fish during AI.
            Projectile.width = 50; 
			Projectile.height = 62;
		}

		public override bool PreAI() {
			Player player = Main.player[Projectile.owner];

			player.zephyrfish = false; // Relic from AIType

			return true;
		}

		public override void AI() {
			Player player = Main.player[Projectile.owner];

			// Keep the projectile from disappearing as long as the player isn't dead and has the pet buff.
			if (!player.dead && player.HasBuff(ModContent.BuffType<DeimosPetBuff>())) {
				Projectile.timeLeft = 2;
			}
		}
	}
}