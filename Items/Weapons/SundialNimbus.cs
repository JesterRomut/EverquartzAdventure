using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using static Terraria.ModLoader.PlayerDrawLayer;
using Terraria.Audio;
using Terraria.Localization;

namespace EverquartzAdventure.Items.Weapons
{

    public class SundialNimbus : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sundial's Nimbus");
            DisplayName.AddTranslation(7, "日晷之灵光");                                            
            DisplayName.AddTranslation(6, "Нимб Сандиала");
            Tooltip.SetDefault("THEY PUT HER ON A FUCKING STAFF"); // is that everquartz who is "they"?
            Tooltip.AddTranslation(7, "他们把她放在一个她妈的法杖上！");
            Tooltip.AddTranslation(6, "ТЫ ПОМЕСТИЛ ЕЁ НА ЕБУЧИЙ ПОСОХ");
        }
        public override void SetDefaults()
        {

            Item.width = 52;
            Item.height = 84;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.autoReuse = true;

            Item.DamageType = DamageClass.Magic;
            Item.damage = 75;
            Item.knockBack = 1;
            Item.crit = 10;

            Item.value = 0;
            Item.rare = ModContent.RarityType<CelestialRarity>();
            Item.shoot = ModContent.ProjectileType<EverquartzProjectile>();
            Item.shootSpeed = 50f;


        }
        // here we see the magic that is ExampleMod. i don't code, i steal
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                //List<Item> li = player.inventory.ToList();
                //player.inventory = li.Except(li.Where(item => item.type == ModContent.ItemType<EverquartzItem>())).ToArray();

                return false;
            }

            Vector2 target = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY);
            float ceilingLimit = target.Y;
            if (ceilingLimit > player.Center.Y - 200f)
            {
                ceilingLimit = player.Center.Y - 200f;
            }

            for (int i = 0; i < 10; i++)
            {
                position = player.Center - new Vector2(Main.rand.NextFloat(401) * player.direction, 600f);
                position.Y -= 100 * i;
                Vector2 heading = target - position;

                if (heading.Y < 0f)
                {
                    heading.Y *= -1f;
                }

                if (heading.Y < 20f)
                {
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
    public class EverquartzProjectile : ModProjectile
    {
        //public override string Texture => "Projectile_931";
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.FairyQueenMagicItemShot);

            Projectile.tileCollide = false; //keeping collision off is a good thing i think
            Projectile.usesLocalNPCImmunity= true;
            Projectile.localNPCHitCooldown = 25;
            Projectile.penetrate = -1;
            AIType = ProjectileID.FairyQueenMagicItemShot;

        }

        public override void AI()
        {
            int num4 = Projectile.alpha;
            Projectile.alpha = 0;
            Color fairyQueenWeaponsColor = Projectile.GetFairyQueenWeaponsColor();
            Projectile.alpha = num4;
            for (int i = 0; i < 3; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, 267, Main.rand.NextVector2CircularEdge(3f, 3f) * (Main.rand.NextFloat() * 0.5f + 0.5f), 0, fairyQueenWeaponsColor);
                dust.scale *= 1.2f;
                dust.noGravity = true;

            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            //damage *= 1000;
            //damage *= 100; this stuff makes it WAY too unbalanced, i changed the stats to a point where it's balanced enough -gat
            //crit = true;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White* Projectile.Opacity;

        public override void Kill(int timeLeft)
        {
            Color fairyQueenWeaponsColor2 = Projectile.GetFairyQueenWeaponsColor();
            SoundEngine.PlaySound(in SoundID.Item10, Projectile.Center);
            Vector2 target2 = Projectile.Center;
            Main.rand.NextFloat();
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 vector12 = Projectile.oldPos[k];
                if (vector12 == Vector2.Zero)
                {
                    break;
                }
                int num35 = Main.rand.Next(1, 3);
                float num46 = MathHelper.Lerp(0.3f, 1f, Utils.GetLerpValue(Projectile.oldPos.Length, 0f, k, clamped: true));
                if ((float)k >= (float)Projectile.oldPos.Length * 0.3f)
                {
                    num35--;
                }
                if ((float)k >= (float)Projectile.oldPos.Length * 0.75f)
                {
                    num35 -= 2;
                }
                vector12.DirectionTo(target2).SafeNormalize(Vector2.Zero);
                target2 = vector12;
                for (float num57 = 0f; num57 < (float)num35; num57++)
                {
                    int num68 = Dust.NewDust(vector12, Projectile.width, Projectile.height, 267, 0f, 0f, 0, fairyQueenWeaponsColor2);
                    Dust dust19 = Main.dust[num68];
                    Dust dust315 = dust19;
                    dust315.velocity *= Main.rand.NextFloat() * 0.8f;
                    Main.dust[num68].noGravity = true;
                    Main.dust[num68].scale = 0.9f + Main.rand.NextFloat() * 1.2f;
                    Main.dust[num68].fadeIn = Main.rand.NextFloat() * 1.2f * num46;
                    dust19 = Main.dust[num68];
                    dust315 = dust19;
                    dust315.scale *= num46;
                    if (num68 != 6000)
                    {
                        Dust dust306 = Dust.CloneDust(num68);
                        dust19 = dust306;
                        dust315 = dust19;
                        dust315.scale /= 2f;
                        dust19 = dust306;
                        dust315 = dust19;
                        dust315.fadeIn *= 0.85f;
                        dust306.color = new Color(255, 255, 255, 255);
                    }
                }
            }
        }
    }
}