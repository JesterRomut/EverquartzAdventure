using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.Utilities;
using Terraria.ID;
using Terraria.Audio;
using EverquartzAdventure.NPCs.Hypnos;

namespace EverquartzAdventure.Projectiles.Hypnos
{
    public class AergiaNeuron : ModProjectile
    {
        #region ExtraAssets
        

        public static string readThisIfYouAreUsingADecompiler => "The aergia neuron and blue exo laser's sprites can both be found on fandom - absolutely bread fangirl";
        public static readonly Asset<Texture2D> glowTex = ModContent.Request<Texture2D>("EverquartzAdventure/Projectiles/Hypnos/AergiaNeuron_Glow");
        public static readonly Asset<Texture2D> glowRedTex = ModContent.Request<Texture2D>("EverquartzAdventure/Projectiles/Hypnos/AergiaNeuron_GlowRed");
        //public static readonly Asset<Texture2D> tubeTex = ModContent.Request<Texture2D>("EverquartzAdventure/Projectiles/Hypnos/HypnosPlugCable");
        #endregion

        #region Consts
        public static readonly int laserTimer = 60;
        public static readonly int refreshTimeLeft = 200;
        public static readonly int alphaChange = 10;
        public static readonly int laserSpeed = 20;
        #endregion

        #region BuffInfo
        public static List<int> VanillaDebuffs => new List<int>()
        {
            BuffID.Ichor,
            BuffID.BetsysCurse
            //ModContent.BuffType<Mindcrashed>(),
        };

        public static List<int> CalamityDebuffs => new List<int>()
        {
            CalamityWeakRef.BuffType.MarkedForDeath,
        CalamityWeakRef.BuffType.ArmorCrunch,
               CalamityWeakRef.BuffType.KamiFlu
        };

        public static List<int> Debuffs => (ModCompatibility.calamityEnabled ? VanillaDebuffs.Union(CalamityDebuffs) : VanillaDebuffs).ToList();

        //public static int Debuff => ModContent.BuffType<Mindcrashed>();

        public static readonly int buffDuration = 300;

        #endregion

        #region Fields
        public bool Landed
        {
            get
            {
                return Projectile.ai[0] != 0;
            }
            set
            {
                if (value == true)
                {
                    Projectile.ai[0] = 1;
                }
                else
                {
                    Projectile.ai[0] = 0;
                }
            }
        }

        public int ShootCooldown
        {
            get
            {
                return (int)Projectile.ai[1];
            }
            set
            {
                Projectile.ai[1] = value;
            }
        }
        public int AergiaIndex => AllNeurons.IndexOf(Projectile);


        #endregion

        #region Utils
        public static List<Projectile> AllNeurons => Main.projectile.Where(proj => proj != null && proj.active && proj.owner == 0 && proj.type == ModContent.ProjectileType<AergiaNeuron>()).ToList();

        public static int CalcDamage(NPC target) => (int)Math.Floor((float)target.lifeMax / (target.boss ? 96000 : 6));
        public static void AddElectricDusts(Entity proj, int count = 3) // hey hypons or whatever your name u coded quite a lot maybe it is time to stop that is not healthy for you, i can tell by myself
        {
            for (int i = 0; i < count; i++)
            {
                Dust.NewDust(proj.position, proj.width, proj.height, DustID.Electric);
            }
        }

        public void AddElectricDusts()
        {
            AddElectricDusts(Projectile);
        }
        #endregion


        #region Overrides
        public override void SetStaticDefaults()
        {
            // base.DisplayName.SetDefault("Aergia Neuron");
            //DisplayName.AddTranslation(7, "埃吉亚神经元");
            //DisplayName.AddTranslation(7, "Нейрон Агерии");
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.damage = 1;
            Projectile.ai[0] = 0;
            Projectile.ai[1] = 0;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.npcProj = true;
            Projectile.DamageType = DamageClass.MagicSummonHybrid;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
            Projectile.timeLeft = refreshTimeLeft;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            //damage = CalcDamage(target);
            //crit = true;
            modifiers.FinalDamage.Flat = CalcDamage(target);
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D sprite = Landed ? glowTex.Value : glowRedTex.Value;
            float originOffsetX = (sprite.Width - Projectile.width) * 0.5f + Projectile.width * 0.5f + DrawOriginOffsetX;

            Rectangle frame = new Rectangle(0, 0, sprite.Width, sprite.Height);

            Vector2 origin = new Vector2(originOffsetX, Projectile.height / 2 - DrawOriginOffsetY);


            Main.EntitySpriteDraw(sprite, Projectile.position - Main.screenPosition + new Vector2(originOffsetX + DrawOffsetX, Projectile.height / 2 + Projectile.gfxOffY), (Rectangle?)frame, Color.White, Projectile.rotation, origin, Projectile.scale, default(SpriteEffects), 0);
        }
        #endregion

        #region AI
        public override void AI()
        {
            NPC hypnos = NPCs.Hypnos.Hypnos.Instance;

            if (hypnos == null)
            {
                Projectile.Kill();
                return;
            }

            int aergiaIndex = AergiaIndex;
            int neuronCount = 12;
            float offset = Main.GlobalTimeWrappedHourly * 80;

            double rad6 = (double)((360f / neuronCount) * aergiaIndex + offset) * (Math.PI / 180.0);
            double dist4 = 200.0;
            float hyposx4 = hypnos.Center.X - (float)(int)(Math.Cos(rad6) * dist4) - (float)(Projectile.width / 2);
            float hyposy4 = hypnos.Center.Y - (float)(int)(Math.Sin(rad6) * dist4) - (float)(Projectile.height / 2);

            float dist = Vector2.Distance(
                Projectile.Center,
                ((float)Math.PI * 2f * aergiaIndex / neuronCount + offset).ToRotationVector2() * (float)dist4 + hypnos.Center
                );

            if (dist < 20f && Landed == false)
            {
                Landed = true;
                AddElectricDusts();
            }

            float idealx8;
            float idealy8;
            if (!Landed)
            {
                idealx8 = MathHelper.Lerp(Projectile.position.X, hyposx4, 0.4f);
                idealy8 = MathHelper.Lerp(Projectile.position.Y, hyposy4, 0.4f);
            }
            else
            {
                idealx8 = MathHelper.Lerp(Projectile.position.X, hyposx4, 0.8f);
                idealy8 = MathHelper.Lerp(Projectile.position.Y, hyposy4, 0.8f);

                NPC target = Projectile.Center.NearestEnemy(800f);
                if (target != null)
                {
                    //CombatText.NewText(Projectile.Hitbox, Color.White, $"{target.FullName} {target.CanBeChasedBy()} {target.chaseable}");
                    Projectile.timeLeft = refreshTimeLeft;
                    if (ShootCooldown > 0)
                    {
                        ShootCooldown--;
                    }
                    else
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.SafeDirectionTo(target.Center) * laserSpeed, ModContent.ProjectileType<BlueExoPulseLaser>(), 1, 0, 0, target.whoAmI);
                        SoundStyle style = NPCs.Hypnos.Hypnos.IPutTheSoundFileInLocalBecauseICouldntKnowCalamitysPathOfThis;
                        style.Volume = NPCs.Hypnos.Hypnos.IPutTheSoundFileInLocalBecauseICouldntKnowCalamitysPathOfThis.Volume - 0.1f;
                        SoundEngine.PlaySound(in style, Projectile.Center);
                        ShootCooldown = laserTimer;

                    }

                }
                else
                {
                    if (Projectile.timeLeft > refreshTimeLeft)
                    {
                        Projectile.timeLeft = refreshTimeLeft;
                    }
                }
            }

            Projectile.position = new Vector2(idealx8, idealy8);

            if (Projectile.timeLeft < 100)
            {
                Projectile.alpha = Math.Min(Projectile.alpha + alphaChange, 255);
            }
            else
            {
                if (Projectile.alpha > 0)
                {
                    Projectile.alpha = Math.Max(Projectile.alpha - alphaChange, 0);
                }
            }

            if (Projectile.alpha >= 255)
            {
                Projectile.Kill();
                return;
            }




        }
        #endregion




    }

    public class BlueExoPulseLaser : ModProjectile
    {
        #region Fields
        public int TargetInt => (int)Projectile.ai[0];
        public NPC Target
        {
            get
            {
                NPC npc = Main.npc.ElementAtOrDefault((int)Projectile.ai[0]);
                if (npc != null && npc.active && npc.CanBeChasedBy() && npc.chaseable)
                {
                    return npc;
                }
                return null;
            }
        }
        #endregion

        #region Overrides
        public override void SetStaticDefaults()
        {
            // base.DisplayName.SetDefault("Blue Exo Pulse Laser");
            //DisplayName.AddTranslation(7, "蓝色星流脉冲激光");
            //DisplayName.AddTranslation(6, "Синий Экзо-Пульсовой Лазер");
            Main.projFrames[base.Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[base.Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[base.Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            base.Projectile.width = 22;
            base.Projectile.height = 22;
            base.Projectile.hostile = false;
            Projectile.friendly = true;
            base.Projectile.timeLeft = 480;
            base.Projectile.tileCollide = false;
            base.Projectile.ignoreWater = true;
            base.Projectile.alpha = 255;
            Projectile.npcProj = true;
            Projectile.penetrate = 1;
            base.Projectile.usesLocalNPCImmunity = true;
            base.Projectile.localNPCHitCooldown = 0;
            Projectile.DamageType = DamageClass.MagicSummonHybrid;
        }

        

        public override void Kill(int timeLeft)
        {
            AergiaNeuron.AddElectricDusts(Projectile, 1);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            //damage = AergiaNeuron.CalcDamage(target);
            modifiers.FinalDamage.Flat += AergiaNeuron.CalcDamage(target);
            //crit = true;
            //target.Everquartz().mindcrashed = AergiaNeuron.buffDuration;
            AergiaNeuron.Debuffs.ForEach(buff => { target.AddBuff(buff, AergiaNeuron.buffDuration); });

            NPC target2 = Projectile.Center.NearestEnemy(800f);
            if (target2 != null && target2.whoAmI != TargetInt)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center, Projectile.SafeDirectionTo(target2.Center) * AergiaNeuron.laserSpeed, ModContent.ProjectileType<BlueExoPulseLaser>(), 1, 0, 0, target.whoAmI);



            }
        }
        #endregion

        #region AI
        public override void AI()
        {
            base.Projectile.frameCounter++;
            if (base.Projectile.frameCounter > 6)
            {
                base.Projectile.frame++;
                base.Projectile.frameCounter = 0;
            }
            if (base.Projectile.frame > 3)
            {
                base.Projectile.frame = 0;
            }
            base.Projectile.alpha -= 30;
            Lighting.AddLight(base.Projectile.Center, 0f, 0f, 0.6f);
            NPC target = Target;
            if (target == null)
            {
                if (Projectile.timeLeft > 60)
                {
                    Projectile.timeLeft = 60;
                }

            }
            else
            {
                Projectile.velocity = Projectile.SafeDirectionTo(target.Center) * AergiaNeuron.laserSpeed;
            }
            if (base.Projectile.velocity.X < 0f)
            {
                base.Projectile.spriteDirection = -1;
                base.Projectile.rotation = (float)Math.Atan2(0.0 - (double)base.Projectile.velocity.Y, 0.0 - (double)base.Projectile.velocity.X);
            }
            else
            {
                base.Projectile.spriteDirection = 1;
                base.Projectile.rotation = (float)Math.Atan2(base.Projectile.velocity.Y, base.Projectile.velocity.X);
            }
            if (base.Projectile.timeLeft <= 60)
            {
                base.Projectile.alpha += 10;
            }
            if (base.Projectile.alpha >= 255)
            {
                base.Projectile.Kill();
            }
        }
        #endregion
    }
}
