using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using static Humanizer.In;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Steamworks;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace EverquartzAdventure.Items.Weapons
{
    public class StarFlare : ModItem
    {
        public override void SetStaticDefaults()
        {
            // These are all related to gamepad controls and don't seem to affect anything else
            ItemID.Sets.Yoyo[Item.type] = true;
            ItemID.Sets.GamepadExtraRange[Item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Item.type] = true;

            // DisplayName.SetDefault("Star Flare");
            //DisplayName.AddTranslation(7, "星耀");
            //DisplayName.AddTranslation(6, "Звёздная Вспышка");
            //// Tooltip.SetDefault("'Dead Deimos attack me in the Hallow!'");
            //Tooltip.AddTranslation(7, "'死去的戴莫斯突然开始攻击我！'");
            //Tooltip.AddTranslation(6, "'Мёртвая Деймос атакует меня в Освещении！'"); // it sounds funky at russian
        }

        public override void SetDefaults()
        {
            Item.width = 24; // The width of the item's hitbox.
            Item.height = 24; // The height of the item's hitbox.

            Item.useStyle = ItemUseStyleID.Shoot; // The way the item is used (e.g. swinging, throwing, etc.)
            Item.useTime = 25; // All vanilla yoyos have a useTime of 25.
            Item.useAnimation = 25; // All vanilla yoyos have a useAnimation of 25.
            Item.noMelee = true; // This makes it so the item doesn't do damage to enemies (the projectile does that).
            Item.noUseGraphic = true; // Makes the item invisible while using it (the projectile is the visible part).
            Item.UseSound = SoundID.Item1; // The sound that will play when the item is used.

            Item.damage = 12; // The amount of damage the item does to an enemy or player.
            Item.DamageType = DamageClass.MeleeNoSpeed; // The type of damage the weapon does. MeleeNoSpeed means the item will not scale with attack speed.
            Item.knockBack = 2.5f; // The amount of knockback the item inflicts.
            Item.crit = 8; // The percent chance for the weapon to deal a critical strike. Defaults to 4.
            Item.channel = true; // Set to true for items that require the attack button to be held out (e.g. yoyos and magic missile weapons)
            Item.rare = ModContent.RarityType<CelestialRarity>(); // The item's rarity. This changes the color of the item's name.
            Item.value = Item.buyPrice(gold: 1); // The amount of money that the item is can be bought for.

            Item.shoot = ModContent.ProjectileType<DeimosYoYoProjectile>(); // Which projectile this item will shoot. We set this to our corresponding projectile.
            Item.shootSpeed = 16f; // The velocity of the shot projectile.			
        }
    }


    public class DeimosYoYoProjectile : ModProjectile
    {
        public int shootCounter = 0;
        public const int shootRate = 10;
        public override void SetStaticDefaults()
        {
            // The following sets are only applicable to yoyo that use aiStyle 99.

            // YoyosLifeTimeMultiplier is how long in seconds the yoyo will stay out before automatically returning to the player. 
            // Vanilla values range from 3f (Wood) to 16f (Chik), and defaults to -1f. Leaving as -1 will make the time infinite.
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = -1f;

            // YoyosMaximumRange is the maximum distance the yoyo sleep away from the player. 
            // Vanilla values range from 130f (Wood) to 400f (Terrarian), and defaults to 200f.
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 800f;

            // YoyosTopSpeed is top speed of the yoyo Projectile.
            // Vanilla values range from 9f (Wood) to 17.5f (Terrarian), and defaults to 10f.
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 50f;
        }
        public override void SetDefaults()
        {
            // This method right here is the backbone of what we're doing here; by using this method, we copy all of
            // the Meowmere Projectile's SetDefault stats (such as projectile.friendly and projectile.penetrate) on to our projectile,
            // so we don't have to go into the source and copy the stats ourselves. It saves a lot of time and looks much cleaner;
            // if you're going to copy the stats of a projectile, use CloneDefaults().

            Projectile.CloneDefaults(ProjectileID.Terrarian);
            Projectile.aiStyle = ProjAIStyleID.Yoyo;
            // To further the Cloning process, we can also copy the ai of any given projectile using AIType, since we want
            // the projectile to essentially behave the same way as the vanilla projectile.
            //AIType = ProjectileID.Terrarian;

        }

        public override void AI()
        {
            if (++shootCounter >= shootRate)
            {
                shootCounter %= shootRate;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X - Projectile.velocity.X, Projectile.Center.Y - Projectile.velocity.Y, Projectile.velocity.X, Projectile.velocity.Y, ModContent.ProjectileType<DeimosYoyoHeadProjectile>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
            
        }

        public override void ModifyFishingLine(ref Vector2 lineOriginOffset, ref Color lineColor)
        {
            lineColor = CelestialRarity.ColorSwap();
        }

        //public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        //{
        //    //damage *= 1000;
        //    //damage *= 100; this stuff makes it WAY too unbalanced, i changed the stats to a point where it's balanced enough -gat
        //    crit = true;
        //}

    }

    public class DeimosYoyoHeadProjectile: ModProjectile
    {
        public override string Texture => "EverquartzAdventure/NPCs/TownNPCs/StarbornPrincess_Head";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Decap Attack");
            //DisplayName.AddTranslation(7, "飞头攻击");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.TerrarianBeam);
            Projectile.width = 13;
            Projectile.height = 18;
            //Projectile.aiStyle = ProjectileID.TerrarianBeam;
            Projectile.tileCollide = false;
            Projectile.scale = 1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
            Projectile.ignoreWater = true;
            //AIType = ProjectileID.TerrarianBeam;

        }

        public override void AI()
        {
            if (++Projectile.alpha >= 255)
            {
                Projectile.Kill();
            }
            Projectile.rotation = Projectile.velocity.ToRotation();

            Vector2 center = base.Projectile.Center;
            float homingRange = 325f;
            bool homeIn = false;
            float inertia = 25f;
            float homingSpeed = 23f;

            for (int i = 0; i < 200; i++)
            {
                if (Main.npc[i].CanBeChasedBy(base.Projectile))
                {
                    float extraDistance = (float)(Main.npc[i].width / 2) + (float)(Main.npc[i].height / 2);
                    if (Vector2.Distance(Main.npc[i].Center, base.Projectile.Center) < homingRange + extraDistance)
                    {
                        center = Main.npc[i].Center;
                        homeIn = true;
                        break;
                    }
                }
            }
            if (homeIn)
            {
                base.Projectile.extraUpdates = 1;
                Vector2 homeInVector = (center - Projectile.Center).SafeNormalize(Vector2.UnitY);//base.Projectile.SafeDirectionTo(center, Vector2.UnitY);
                base.Projectile.velocity = (base.Projectile.velocity * inertia + homeInVector * homingSpeed) / (inertia + 1f);
            }
            else
            {
                base.Projectile.extraUpdates = 0;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= 2000;
            modifiers.SetCrit();
            if (ModCompatibility.calamityEnabled)
            {
                target.AddBuff(CalamityWeakRef.BuffType.GodSlayerInferno, 120);
                target.AddBuff(CalamityWeakRef.BuffType.HolyFlames, 120);
            }
            
        }
    }
}

