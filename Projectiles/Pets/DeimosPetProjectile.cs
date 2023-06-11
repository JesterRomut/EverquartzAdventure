using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using EverquartzAdventure;
using EverquartzAdventure.Items.Pets;
using EverquartzAdventure.Buffs.Pets;

namespace EverquartzAdventure.Projectiles.Pets
{
    public class DeimosPetProjectile : CalValEXWalkingPet
    {
        public override string Texture => "EverquartzAdventure/Projectiles/Pets/DeimosPetProjectileFull";
        public override float BackToFlyingThreshold => 600f;
        public override int JumpOffset => 10;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 8;
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.ZephyrFish); // Copy the stats of the Zephyr Fish

            //AIType = ProjectileID.ZephyrFish; // Mimic as the Zephyr Fish during AI.
            Projectile.tileCollide = true;
            Projectile.width = 50;
            Projectile.height = 62;
            //Projectile.aiStyle = 26;
        }

        //public override bool PreAI() {
        //	Player player = Main.player[Projectile.owner];

        //	player.zephyrfish = false; // Relic from AIType

        //	return true;
        //}

        public int realFrame = 0;

        public override void Animation(int state)
        {
            switch (state)
            {
                case States.Walking:
                    if (Projectile.velocity.X < 1 && Projectile.velocity.X > -1)
                    {
                        Projectile.frame = 0;
                        realFrame = 0;
                    }
                    else
                    {
                        if (++Projectile.frameCounter >= 5)
                        {
                            Projectile.frameCounter = 0;

                            Projectile.frame = (++realFrame % 4) + 1;
                        }
                    }
                    break;
                //base.Projectile.velocity.Y += Gravity;
                case States.Flying:
                    if (++Projectile.frameCounter >= 5)
                    {
                        Projectile.frameCounter = 0;

                        Projectile.frame = (++realFrame % 3) + 5;
                    }
                    break;
            }

        }

        public override void PetFunctionality(Player player)
        {
            // Keep the projectile from disappearing as long as the player isn't dead and has the pet buff.
            if (player.dead || !player.HasBuff(ModContent.BuffType<DeimosPetBuff>()))
            {
                Projectile.Kill();
            }
            Projectile.timeLeft = 2;
        }
    }
}