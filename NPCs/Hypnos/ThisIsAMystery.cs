using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Humanizer;
using Terraria.Utilities;
using Terraria.GameContent.Events;
using Hypnos;
using Terraria.Chat;
using Terraria.Localization;
using Terraria.Audio;
using Terraria.GameContent.Personalities;

namespace EverquartzAdventure
{
    internal static partial class HypnosWeakRef
    {
        internal static bool downedHypnos => HypnosWorld.downedHypnos;
    }
}

namespace EverquartzAdventure.NPCs.Hypnos
{
    [AutoloadHead]
    public class Hypnos : ModNPC
    {
        public static readonly Asset<Texture2D> eyepatchTex = ModContent.Request<Texture2D>("EverquartzAdventure/NPCs/Hypnos/Eyepatch");
        public static readonly Asset<Texture2D> glowTex = ModContent.Request<Texture2D>("EverquartzAdventure/NPCs/Hypnos/Hypnos_Glow");

        public static readonly string ButtonTextKey = "Mods.EverquartzAdventure.NPCs.TownNPCs.Hypnos.ButtonText";
        public static readonly string BestiaryTextKey = "Mods.EverquartzAdventure.NPCs.TownNPCs.Hypnos.BestiaryText";

        public static readonly string ChatCommonKey = "Mods.EverquartzAdventure.NPCs.TownNPCs.Hypnos.Chat.Common";
        public static readonly string ChatBloodMoonKey = "Mods.EverquartzAdventure.NPCs.TownNPCs.Hypnos.Chat.BloodMoon";
        public static readonly string ChatPartyKey = "Mods.EverquartzAdventure.NPCs.TownNPCs.Hypnos.Chat.Party";
        public static readonly string ChatPreHypnosKey = "Mods.EverquartzAdventure.NPCs.TownNPCs.Hypnos.Chat.PreHypnos";
        public static readonly string ChatPostHypnosKey = "Mods.EverquartzAdventure.NPCs.TownNPCs.Hypnos.Chat.PostHypnos";
        public static readonly string ChatPrayKey = "Mods.EverquartzAdventure.NPCs.TownNPCs.Hypnos.Chat.Pray";
        public static readonly string ChatPrayWithoutMoneyKey = "Mods.EverquartzAdventure.NPCs.TownNPCs.Hypnos.Chat.PrayWithoutMoney";

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            if (AergiaNeuron.AllNeurons.Count >= 12)
            {
                return;
            }
            projType = ModContent.ProjectileType<AergiaNeuron>();
            attackDelay = 1;
        }

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            EverquartzGlobalNPC.hypnos = NPC.whoAmI;
            damage = 200;
            knockback = 0;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul of the Eternal Intellect of Infinite Verboten Knowledge");
            DisplayName.AddTranslation(7, "无限禁忌知识的永恒智慧之魂");
            //NPCID.Sets.ActsLikeTownNPC[Type] = true;
            //NPCID.Sets.SpawnsWithCustomName[Type] = true;
            Main.npcFrameCount[base.NPC.type] = 12;

            NPCID.Sets.TownNPCBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Velocity = 0f,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
            NPCID.Sets.BossBestiaryPriority.Add(base.Type);

            NPCID.Sets.AttackType[Type] = 2;
            NPCID.Sets.MagicAuraColor[Type] = Color.Purple;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[2]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheHallow,
            new FlavorTextBestiaryInfoElement(BestiaryTextKey)
            });
        }

        public override void SetDefaults()
        {
            NPC.friendly = true;
            NPC.townNPC = true;
            NPC.width = 15;
            NPC.height = 22;
            NPC.aiStyle = NPCAIStyleID.Passive;
            NPC.damage = 200;
            NPC.defense = 90;
            
            
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            TownNPCStayingHomeless = true;
            NPC.trapImmune = true;
            base.NPC.lavaImmune = true;
            //NPC.rarity = 2;//设置稀有度
            //AnimationType = npcID;

            base.NPC.Happiness.SetBiomeAffection<HallowBiome>(AffectionLevel.Like);

            NPC.lifeMax = 1320000;
        }

        public override bool CanTownNPCSpawn(int numTownNPCs, int money)
        {
            return false;
        }

        public override void AI()
        {
            NPC.homeless = true;
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.velocity.X == 0)
            {
                NPC.frame.Y = 0;
                base.NPC.frameCounter = 0;
            }
            else
            {
                base.NPC.frameCounter += 0.3;
                base.NPC.frameCounter %= Main.npcFrameCount[base.NPC.type] - 1;
                int frame = (int)base.NPC.frameCounter;
                base.NPC.frame.Y = (frame + 1) * frameHeight;
            }
            NPC.spriteDirection = NPC.direction;
        }

        public override bool CanGoToStatue(bool toKingStatue)
        {
            return false;
        }

        public override bool CanChat()
        {
            return true;
        }

        private void Draw(Texture2D texture, SpriteBatch spriteBatch, Vector2 screenPos, Vector2? position = null, Color? drawColor = null)
        {
            spriteBatch.Draw
            (
                texture,
                position ?? (NPC.Center - screenPos - new Vector2(0, 8f)),
                new Rectangle(0, 0, texture.Width, texture.Height),
                drawColor ?? Color.White,
                NPC.rotation,
                NPC.frame.Size() / 2,
                NPC.scale,
                NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                0f
            );
        }

        //public override void OnKill()
        //{
        //    NetworkText networkText = NetworkText.FromKey("Announcement.HasArrived", NPC.GetFullNetName());
        //    if (NPC.ai[3] == 1)
        //    {
        //        ChatHelper.BroadcastChatMessage(networkText, new Color(50, 125, 255));
        //    }
        //}

        public void Kill()
        {
            NPC.life = 0;
            NPC.checkDead();
        }

        public void DropCoins()
        {
            Item.NewItem(NPC.GetSource_Death(), NPC.Center, ItemID.GoldCoin, EverquartzSystem.hypnoCoins);
            EverquartzSystem.hypnoCoins = 0;
        }

        public void KillWithCoins()
        {
            DropCoins();
            Kill();
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue(ButtonTextKey, Lang.inter[16].Value);
        }

        public string GetPrayChat(bool hasEnoughMoney)
        {
            WeightedRandom<string> textSelector = new WeightedRandom<string>(Main.rand);
            if (hasEnoughMoney)
            {
                EverquartzUtils.GetTextListFromKey(ChatPrayKey).ForEach(st => textSelector.Add(st));
            }
            else
            {
                EverquartzUtils.GetTextListFromKey(ChatPrayWithoutMoneyKey).ForEach(st => textSelector.Add(st));
            }
            string thingToSay = textSelector.Get();
            return thingToSay;
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            Player player = Main.player[Main.myPlayer];
            if (player.Everquartz().IsPraisingHypnos)
            {
                return;
            }
            Pray(player);
        }

        public void GetPrayInfo(Player player, out int targetDirection, out Vector2 playerPositionWhenPetting)
        {
            targetDirection = ((NPC.Center.X > player.Center.X) ? 1 : (-1));
            int num = 36;
            playerPositionWhenPetting = NPC.Bottom + new Vector2((float)(-targetDirection * num), 0f);
            playerPositionWhenPetting = playerPositionWhenPetting.Floor();
        }

        public void Pray(Player player)
        {
            GetPrayInfo(player, out int targetDirection, out Vector2 playerPositionWhenPetting);
            Vector2 offset = playerPositionWhenPetting - player.Bottom;
            if (!player.CanSnapToPosition(offset) || !WorldGen.SolidTileAllowBottomSlope((int)playerPositionWhenPetting.X / 16, (int)playerPositionWhenPetting.Y / 16))
            {
                return;
            }

            bool buyResult = player.BuyItem(Item.buyPrice(gold: 1));
            if (buyResult)
            {
                EverquartzSystem.hypnoCoins++;
                player.StopVanityActions();
                player.RemoveAllGrapplingHooks();
                if (player.mount.Active)
                {
                    player.mount.Dismount(player);
                }
                player.Bottom = playerPositionWhenPetting;
                player.ChangeDir(targetDirection);
                player.Everquartz().praisingTimer = 100;
                player.velocity = Vector2.Zero;
                player.gravDir = 1f;
            }
            
            

            Main.npcChatText = GetPrayChat(buyResult);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                SoundEngine.PlaySound(NPC.DeathSound, NPC.position);
                for (int num585 = 0; num585 < 25; num585++)
                {
                    int num586 = Dust.NewDust(NPC.position, NPC.width, NPC.height, 31, 0f, 0f, 100, default(Color), 2f);
                    Dust dust30 = Main.dust[num586];
                    Dust dust187 = dust30;
                    dust187.velocity *= 1.4f;
                    Main.dust[num586].noLight = true;
                    Main.dust[num586].noGravity = true;
                }
            }
        }

        public override bool CheckDead()
        {
            NPC.active = false;
            return true;
        }

        public override string GetChat()
        {
            WeightedRandom<string> textSelector = new WeightedRandom<string>(Main.rand);
            EverquartzUtils.GetTextListFromKey(ChatCommonKey).ForEach(st => textSelector.Add(st));
            if (!Main.dayTime && Main.bloodMoon)
            {
                EverquartzUtils.GetTextListFromKey(ChatBloodMoonKey).ForEach(st => textSelector.Add(st, 5.15));
            }
            if (BirthdayParty.PartyIsUp)
            {
                EverquartzUtils.GetTextListFromKey(ChatPartyKey).ForEach(st => textSelector.Add(st, 5.5));
            }
            if (ModCompatibility.hypnosEnabled && HypnosWeakRef.downedHypnos)
            {
                    EverquartzUtils.GetTextListFromKey(ChatPostHypnosKey).ForEach(st => textSelector.Add(st));

            }
            else
            {
                EverquartzUtils.GetTextListFromKey(ChatPreHypnosKey).ForEach(st => textSelector.Add(st));
            }

            string thingToSay = textSelector.Get();
            return thingToSay;
        }



        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 position = NPC.Center - screenPos - new Vector2(0, 9f);
            spriteBatch.Draw
            (
                TextureAssets.Npc[base.NPC.type].Value,
                position,
                NPC.frame,
                drawColor,
                NPC.rotation,
                NPC.frame.Size() / 2,
                NPC.scale,
                NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                0f
            );

            Draw(glowTex.Value, spriteBatch, screenPos, position, Color.White);
            if (NPC.spriteDirection == 1)
            {
                Draw(eyepatchTex.Value, spriteBatch, screenPos, position, drawColor);
            }

            return false;
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string> { "Hypnos" };
        }
    }
}
