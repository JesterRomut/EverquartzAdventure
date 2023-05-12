using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace EverquartzAdventure.Items.Pets
{

    /// <summary>
    /// Methods that are not used should be sealed. The base of a pet
    /// </summary>
    public abstract class CalValEXPet : ModProjectile
    {
        public static readonly string readThisIfYouAreUsingADecompiler = "this is calvalex's code and can be deleted by calvalex's request, follow calvalex on https://github.com/PotatoPersonThing/CalValEX!";
        /// <summary>
        /// Implement most, if not all, methods in here
        /// </summary>
        public abstract void PetBehaviour();

        /// <summary>
        /// The current state of the Projectile
        /// </summary>
        public int state = 0;

        /// <summary>
        /// The rotation (in radians) of the aura. Automatically gets it's rotation wrapped
        /// </summary>
        public float auraRotation = 0f;

        /// <summary>
        /// The usual static defaults for a pet
        /// </summary>
        /// <param name="lightPet">If it is a light pet or not</param>
        public virtual void PetSetStaticDefaults(bool lightPet)
        {
            Main.projPet[Projectile.type] = true;

            if (lightPet)
            {
                ProjectileID.Sets.LightPet[Projectile.type] = true;
            }
        }

        /// <summary>
        /// The usual defaults for a pet
        /// </summary>
        public virtual void PetSetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.timeLeft *= 5;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.aiStyle = -1;
        }

        /// <summary>
        /// Default value is 20f
        /// </summary>
        public virtual float FlyingArea => 20f;

        /// <summary>
        /// Default is 2400f
        /// </summary>
        public virtual float TeleportThreshold => 2400f;

        /// <summary>
        /// Default is 560f
        /// </summary>
        public virtual float SpeedupThreshold => 560f;

        /// <summary>
        /// If this pet can speedup after the threshold is hit. Default is true
        /// </summary>
        public virtual bool ShouldSpeedup => true;

        /// <summary>
        /// Default is 12f
        /// </summary>
        public virtual float FlyingSpeed => 12f;

        /// <summary>
        /// Default is 80f
        /// </summary>
        public virtual float FlyingInertia => 80f;

        /// <summary>
        /// Default is 1f
        /// </summary>
        public virtual float FlyingDrag => 1f;

        /// <summary>
        /// Default is 48 pixels (3 tiles) on the back of the player and 50 pixels (3.125 tiles) up
        /// </summary>
        public virtual Vector2 FlyingOffset => new Vector2(48f * -Main.player[Projectile.owner].direction, -50f);

        /// <summary>
        /// Default is true
        /// </summary>
        public virtual bool FacesLeft => true;

        /// <summary>
        /// Default is true
        /// </summary>
        public virtual bool ShouldFlip => true;

        /// <summary>
        /// Default is true
        /// </summary>
        public virtual bool ShouldFlyRotate => true;

        /// <summary>
        /// Default is true
        /// </summary>
        public virtual bool CanTeleport => true;

        /// <summary>
        /// Default is true
        /// </summary>
        public virtual bool AllowRotationReset => true;

        /// <summary>
        /// Set the pet bool and the despawn logic in here
        /// </summary>
        /// <param name="player">The owner of the pet</param>
        public abstract void PetFunctionality(Player player);

        /// <summary>
        /// Put the animation code here. This is called before any movement or state changes happen. Called before CustomBehaviour and after teleportation
        /// </summary>
        /// <param name="state">The current state the pet is in</param>
        public abstract void Animation(int state);

        /// <summary>
        /// Put the dust spawning code here. Allows you to modify the dust type. Return <see langword="false"/> to disable normal spawning. Return <see langword="true"/> for normal code. Returns <see langword="false"/> by default
        /// </summary>
        /// <param name="dustType">The dust type</param>
        public virtual bool ModifyDustSpawn(ref int dustType)
        {
            return false;
        }

        /// <summary>
        /// Allows you to add custom behaviour to this pet. Called right before the normal AI is done
        /// </summary>
        /// <param name="player">The owner of the pet</param>
        /// <param name="state">The state of the pet</param>
        public virtual void CustomBehaviour(Player player, ref int state)
        {
        }

        /// <summary>
        /// Allows you to add custom logic when this pet teleports, like adding dust
        /// </summary>
        public virtual void OnTeleport()
        {
        }

        /// <summary>
        /// Allows you to add custom logic when this pet is reset to a different state, like resetting values
        /// </summary>
        /// <param name="state">The state it got reset to</param>
        public virtual void OnReset(int state)
        {
        }

        /// <summary>
        /// Allows you to reset the pet
        /// </summary>
        /// <param name="state">The state to reset to</param>
        public void ResetMe(int state)
        {
            Projectile.ai[0] = 0;
            Projectile.ai[1] = 0;
            Projectile.localAI[0] = 0;
            Projectile.localAI[1] = 0;
            this.state = state;
            OnReset(state);
            Projectile.netUpdate = true;
        }

        /// <summary>
        /// <inheritdoc cref="ModProjectile.SendExtraAI(BinaryWriter)"/> It is important that you call base.SendExtraAI(writer) at the end.
        /// </summary>
        /// <param name="writer"></param>
        public new virtual void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
            writer.Write(state);
            base.SendExtraAI(writer);
        }

        /// <summary>
        /// <inheritdoc cref="ModProjectile.ReceiveExtraAI(BinaryReader)"/> It is important that you call base.ReceiveExtraAI(reader) at the end.
        /// </summary>
        /// <param name="writer"></param>
        public new virtual void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
            state = reader.ReadInt32();
            base.ReceiveExtraAI(reader);
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public sealed override void AI()
        {
            PetBehaviour();
        }

        public void SimpleAnimation(int speed)
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > speed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame > Main.projFrames[Projectile.type] - 1)
                    Projectile.frame = 0;
            }
        }

        public void SimpleGlowmask(SpriteBatch spriteBatch, string path)
        {
            Texture2D glowTexture = ModContent.Request<Texture2D>(path).Value;
            spriteBatch.Draw(glowTexture, Projectile.Center - Main.screenPosition, new Rectangle(0, (glowTexture.Height / Main.projFrames[Projectile.type]) * Projectile.frame, glowTexture.Width, glowTexture.Height / Main.projFrames[Projectile.type]), Color.White, Projectile.rotation, Projectile.Size / 2f, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public void SimpleGlowmask(SpriteBatch spriteBatch)
        {
            SimpleGlowmask(spriteBatch, Texture + "_Glow");
        }

        public void SimpleAura(SpriteBatch spriteBatch, string[] paths, bool glowing)
        {
            if (paths.Length < 1)
                return;

            Texture2D auraTexture = ModContent.Request<Texture2D>(paths[0]).Value;
            Color color = Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16);

            spriteBatch.Draw(auraTexture, Projectile.Center - Main.screenPosition, auraTexture.Bounds, color, auraRotation, auraTexture.Size() / 2f, 1f, SpriteEffects.None, 0);

            if (glowing)
            {
                if (paths.Length < 2)
                    return;

                Texture2D glowTexture = ModContent.Request<Texture2D>(paths[0]).Value;
                spriteBatch.Draw(glowTexture, Projectile.Center - Main.screenPosition, glowTexture.Bounds, Color.White, auraRotation, glowTexture.Size() / 2f, 1f, SpriteEffects.None, 0);
            }
        }

        public void SimpleAura(SpriteBatch spriteBatch, bool glowing)
        {
            SimpleAura(spriteBatch, new string[] { Texture + "_Aura", Texture + "_Aura_Glow" }, glowing);
        }

        public void AddLight(Vector3 RGB, float intensity, int abyssLight)
        {
            AddLight(Projectile.Center, RGB, intensity, abyssLight);
        }

        public void AddLight(Color color, float intensity, int abyssLight)
        {
            AddLight(color.ToVector3(), intensity, abyssLight);
        }

        public void AddLight(byte R, byte G, byte B, float intensity, int abyssLight)
        {
            AddLight(new Vector3(R, G, B), intensity, abyssLight);
        }

        public void AddLight(Vector2 position, byte R, byte G, byte B, float intensity, int abyssLight)
        {
            AddLight(position, new Vector3(R, G, B), intensity, abyssLight);
        }

        public void AddLight(Vector2 position, Color color, float intensity, int abyssLight)
        {
            AddLight(position, color.ToVector3(), intensity, abyssLight);
        }

        public void AddLight(Vector2 position, Vector3 RGB, float intensity, int abyssLight)
        {
            Vector3 lightColor = RGB * intensity;

            Lighting.AddLight(position, lightColor);

            /*Mod calamityMod = ModLoader.GetMod("CalamityMod");
            if (calamityMod != null)
            {
                calamityMod.Call("AddAbyssLightStrength", Main.player[Projectile.owner], abyssLight);
            }*/
        }
    }


    /// <summary>
    /// The base of a walking pet
    /// </summary>
    public abstract class CalValEXWalkingPet : CalValEXPet
    {
        public class States
        {
            public const int Walking = 0;
            public const int Flying = 1;
        }

        /// <summary>
        /// Default is 80f
        /// </summary>
        public virtual float WalkingThreshold => 80f;

        /// <summary>
        /// Default is 16f
        /// </summary>
        public virtual float WalkingSpeed => 16f;

        /// <summary>
        /// Default value is 0.4f
        /// </summary>
        public virtual float Gravity => 0.4f;

        /// <summary>
        /// Default value is 20f
        /// </summary>
        public virtual float WalkingInertia => 20f;

        /// <summary>
        /// Default is 0.95f
        /// </summary>
        public virtual float WalkingDrag => 0.95f;

        /// <summary>
        /// Default is 6.5f
        /// </summary>
        public virtual float MaxWalkingSpeed => 6.5f;

        /// <summary>
        /// Default is 250f
        /// </summary>
        public virtual float BackToWalkingThreshold => 250f;

        /// <summary>
        /// Default is 500f
        /// </summary>
        public virtual float BackToFlyingThreshold => 500f;

        /// <summary>
        /// Make false so you can apply your own gravity. Default is true
        /// </summary>
        public virtual bool HasGravity => true;

        /// <summary>
        /// Makes this WalkingPet always jump. Default is false. Only works if CanJump is true
        /// </summary>
        public virtual bool AlwaysJumping => false;

        /// <summary>
        /// Allows this WalkingPet to jump. Default is true
        /// </summary>
        public virtual bool CanJump => true;

        /// <summary>
        /// Default is true
        /// </summary>
        public virtual bool CanFly => true;

        /// <summary>
        /// Default value is 0
        /// </summary>
        public virtual int JumpOffset => 0;

        /// <summary>
        /// Counter for jumps
        /// </summary>
        public int jumpCounter = 0;

        /// <summary>
        /// Allows you to add custom logic when this pet jumps
        /// </summary>
        public virtual void OnJump()
        {
        }

        /// <summary>
        /// Allows you to change the jump height (strength) of this pet
        /// </summary>
        /// <param name="oneTileHigherAndNotTwoTilesHigher"></param>
        /// <param name="twoTilesHigher"></param>
        /// <param name="fiveTilesHigher"></param>
        /// <param name="fourTilesHigher"></param>
        /// <param name="anyOtherJump"></param>
        public virtual void ModifyJumpHeight(ref float oneTileHigherAndNotTwoTilesHigher, ref float twoTilesHigher, ref float fourTilesHigher, ref float fiveTilesHigher, ref float anyOtherJump)
        {
        }

        /// <summary>
        /// Allows you to modify this pets' speeds
        /// </summary>
        /// <param name="walkingSpeed">The current walking speed</param>
        /// <param name="flyingSpeed">The current flying speed</param>
        public virtual void ModifyPetSpeeds(ref float walkingSpeed, ref float flyingSpeed)
        {
        }

        /// <summary>
        /// Allows you to modify this pets' inertias
        /// </summary>
        /// <param name="walkingInertia">The current walking inertia</param>
        /// <param name="flyingInertia">the current flying inertia</param>
        public virtual void ModifyPetInertias(ref float walkingInertia, ref float flyingInertia)
        {
        }

        public sealed override void CustomBehaviour(Player player, ref int state)
        {
            base.CustomBehaviour(player, ref state);
        }

        /// <summary>
        /// <inheritdoc cref="ModPet.CustomBehaviour(Player, ref int)"/>
        /// </summary>
        /// <param name="player">The owner of this pet</param>
        /// <param name="state">The state this Walking Pet is in</param>
        /// <param name="walkingSpeed">The current walking speed</param>
        /// <param name="walkingInertia">The current walking inertia</param>
        /// <param name="flyingSpeed">The current flying speed</param>
        /// <param name="flyingInertia">The current flying inertia</param>
        public virtual void CustomBehaviour(Player player, ref int state, float walkingSpeed, float walkingInertia, float flyingSpeed, float flyingInertia)
        {

        }

        public sealed override void PetBehaviour()
        {
            if (!Main.player[Projectile.owner].active)
            {
                Projectile.active = false;
                return;
            }

            bool leftOfPlayer = false;
            bool rightOfPlayer = false;
            bool isBelowPlayer = false;
            bool wantsToJump = false;

            Player player = Main.player[Projectile.owner];

            PetFunctionality(player);

            if (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y) > 0.05f)
            {
                if (FacesLeft && ShouldFlip)
                    Projectile.spriteDirection = Projectile.velocity.X > 0 ? -1 : 1;
                else if (!FacesLeft && ShouldFlip)
                    Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;
            }

            if (player.position.X + (player.width / 2) < Projectile.position.X + (Projectile.width / 2) - WalkingThreshold)
            {
                leftOfPlayer = true;
            }
            else if (player.position.X + (player.width / 2) > Projectile.position.X + (Projectile.width / 2) + WalkingThreshold)
            {
                rightOfPlayer = true;
            }

            if (player.rocketDelay2 > 0 && state == States.Walking && CanFly)
                ResetMe(States.Flying);

            Vector2 ProjectileCenter = Projectile.Center;
            Vector2 vectorToPlayer = player.Center - ProjectileCenter;
            float lengthToPlayer = vectorToPlayer.Length();

            if (lengthToPlayer > TeleportThreshold && CanTeleport)
            {
                Projectile.position = player.Center;
                Projectile.velocity *= 0.1f;
                OnTeleport();
            }
            else if ((lengthToPlayer > BackToFlyingThreshold || (Math.Abs(vectorToPlayer.Y) > 300f)) && (state == States.Walking || state == States.Flying) && CanFly)
            {
                if (vectorToPlayer.Y > 0f && Projectile.velocity.Y < 0f)
                    Projectile.velocity.Y = 0f;

                if (vectorToPlayer.Y < 0f && Projectile.velocity.Y > 0f)
                    Projectile.velocity.Y = 0f;

                ResetMe(States.Flying);
            }

            Animation(state);

            float walkingSpeed = WalkingSpeed;
            float flyingSpeed = FlyingSpeed;
            float flyingInertia = FlyingInertia;
            float walkingInertia = WalkingInertia;

            if ((lengthToPlayer > SpeedupThreshold) && ShouldSpeedup)
            {
                walkingSpeed *= 1.25f;
                walkingInertia *= 0.75f;
                flyingSpeed *= 1.25f;
                flyingInertia *= 0.75f;
            }

            ModifyPetSpeeds(ref walkingSpeed, ref flyingSpeed);

            ModifyPetInertias(ref walkingInertia, ref flyingInertia);

            CustomBehaviour(player, ref state, walkingSpeed, walkingInertia, flyingSpeed, flyingInertia);

            switch (state)
            {
                case States.Walking:

                    if (AllowRotationReset)
                        Projectile.rotation = 0f;

                    Projectile.tileCollide = true;

                    if (leftOfPlayer || rightOfPlayer)
                    {
                        vectorToPlayer.Normalize();
                        vectorToPlayer *= walkingSpeed;

                        if (walkingInertia != 0f && walkingSpeed != 0f)
                            Projectile.velocity.X = (Projectile.velocity.X * (walkingInertia - 1) + vectorToPlayer.X) / walkingInertia;
                    }
                    else
                    {
                        Projectile.velocity.X *= WalkingDrag;
                        if (Projectile.velocity.X >= 0f - 0.2f && Projectile.velocity.X <= 0.2f)
                            Projectile.velocity.X = 0f;
                    }

                    if (leftOfPlayer || rightOfPlayer)
                    {
                        int XTileToPlayer = (int)(Projectile.position.X + (Projectile.width / 2)) / 16;
                        int YTileToPlayer = (int)(Projectile.position.Y + (Projectile.height / 2)) / 16;

                        if (leftOfPlayer)
                            XTileToPlayer--;

                        if (rightOfPlayer)
                            XTileToPlayer++;

                        XTileToPlayer += (int)Projectile.velocity.X;
                        if (WorldGen.SolidTile(XTileToPlayer, YTileToPlayer))
                            wantsToJump = true;
                    }

                    if (player.position.Y + player.height - 8f > Projectile.position.Y + Projectile.height)
                        isBelowPlayer = true;

                    Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);
                    if (Projectile.velocity.Y == 0f)
                    {
                        if (!isBelowPlayer && (Projectile.velocity.X < 0f || Projectile.velocity.X > 0f))
                        {
                            int XCenterTile = (int)(Projectile.position.X + (Projectile.width / 2)) / 16;
                            int YCenterTile = (int)(Projectile.position.Y + (Projectile.height / 2)) / 16 + 1;
                            if (leftOfPlayer)
                                XCenterTile--;

                            if (rightOfPlayer)
                                XCenterTile++;

                            WorldGen.SolidTile(XCenterTile, YCenterTile);
                        }

                        if ((wantsToJump || AlwaysJumping) && CanJump)
                        {
                            if (jumpCounter < JumpOffset)
                            {
                                jumpCounter++;
                                goto SkipJump;
                            }

                            jumpCounter = 0;
                            int XCenterTile = (int)(Projectile.position.X + (Projectile.width / 2)) / 16;
                            int YBottomTile = (int)(Projectile.position.Y + Projectile.height) / 16 + 1;
                            if (WorldGen.SolidTile(XCenterTile, YBottomTile) || Main.tile[XCenterTile, YBottomTile].IsHalfBlock || Main.tile[XCenterTile, YBottomTile].Slope > 0)
                            {
                                float oneAndNotTwo = -5.1f;
                                float two = -7.1f;
                                float five = -11.1f;
                                float four = -10.1f;
                                float anyOther = -9.1f;

                                ModifyJumpHeight(ref oneAndNotTwo, ref two, ref four, ref five, ref anyOther);

                                try
                                {
                                    XCenterTile = (int)(Projectile.position.X + (Projectile.width / 2)) / 16;
                                    YBottomTile = (int)(Projectile.position.Y + (Projectile.height / 2)) / 16;
                                    if (leftOfPlayer)
                                        XCenterTile--;

                                    if (rightOfPlayer)
                                        XCenterTile++;

                                    XCenterTile += (int)Projectile.velocity.X;
                                    if (!WorldGen.SolidTile(XCenterTile, YBottomTile - 1) && !WorldGen.SolidTile(XCenterTile, YBottomTile - 2))
                                        Projectile.velocity.Y = oneAndNotTwo;
                                    else if (!WorldGen.SolidTile(XCenterTile, YBottomTile - 2))
                                        Projectile.velocity.Y = two;
                                    else if (WorldGen.SolidTile(XCenterTile, YBottomTile - 5))
                                        Projectile.velocity.Y = five;
                                    else if (WorldGen.SolidTile(XCenterTile, YBottomTile - 4))
                                        Projectile.velocity.Y = four;
                                    else
                                        Projectile.velocity.Y = anyOther;
                                }
                                catch
                                {
                                    Projectile.velocity.Y = anyOther;
                                }

                                OnJump();
                            }
                        }
                        else
                        {
                            jumpCounter = 0;
                        }
                    }

                SkipJump:
                    if (Projectile.velocity.X > MaxWalkingSpeed)
                        Projectile.velocity.X = MaxWalkingSpeed;

                    if (Projectile.velocity.X < 0f - MaxWalkingSpeed)
                        Projectile.velocity.X = 0f - MaxWalkingSpeed;
                    break;

                case States.Flying:

                    Projectile.tileCollide = false;

                    vectorToPlayer += FlyingOffset;
                    lengthToPlayer = vectorToPlayer.Length();

                    if (lengthToPlayer < BackToWalkingThreshold && player.velocity.Y == 0f && Projectile.position.Y + Projectile.height <= player.position.Y + player.height && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                    {
                        ResetMe(States.Walking);
                        if (Projectile.velocity.Y < -6f)
                            Projectile.velocity.Y = -6f;
                    }

                    if (lengthToPlayer > FlyingArea)
                    {
                        vectorToPlayer.Normalize();
                        vectorToPlayer *= flyingSpeed;

                        if (Projectile.velocity == Vector2.Zero)
                        {
                            Projectile.velocity = new Vector2(-0.15f);
                        }
                        if (flyingInertia != 0f && flyingSpeed != 0f)
                            Projectile.velocity = (Projectile.velocity * (flyingInertia - 1) + vectorToPlayer) / flyingInertia;
                    }
                    else
                    {
                        Projectile.velocity *= FlyingDrag;
                    }

                    if (ShouldFlyRotate)
                    {
                        Projectile.rotation = Projectile.velocity.X * 0.1f;
                    }

                    int dustType = 16;
                    if (ModifyDustSpawn(ref dustType))
                    {
                        int theDust = Dust.NewDust(new Vector2(Projectile.position.X + (Projectile.width / 2) - 4f, Projectile.position.Y + (Projectile.height / 2) - 4f) - Projectile.velocity, 8, 8, dustType, (0f - Projectile.velocity.X) * 0.5f, Projectile.velocity.Y * 0.5f, 50, default, 1.7f);
                        Main.dust[theDust].velocity.X = Main.dust[theDust].velocity.X * 0.2f;
                        Main.dust[theDust].velocity.Y = Main.dust[theDust].velocity.Y * 0.2f;
                        Main.dust[theDust].noGravity = true;
                    }

                    break;
            }

            if (state == States.Walking && HasGravity)
            {
                Projectile.velocity.Y += Gravity;
            }

            auraRotation = MathHelper.WrapAngle(auraRotation);
        }

        /// <summary>
        /// <inheritdoc cref="ModPet.SendExtraAI(BinaryWriter)"/>
        /// </summary>
        /// <param name="writer"></param>
        public new virtual void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(jumpCounter);
            base.SendExtraAI(writer);
        }

        /// <summary>
        /// <inheritdoc cref="ModPet.ReceiveExtraAI(BinaryReader)"/>
        /// </summary>
        /// <param name="writer"></param>
        public new virtual void ReceiveExtraAI(BinaryReader reader)
        {
            jumpCounter = reader.ReadInt32();
            base.ReceiveExtraAI(reader);
        }
    }

}
