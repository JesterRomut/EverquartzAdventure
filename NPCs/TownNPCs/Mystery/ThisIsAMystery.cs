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

namespace EverquartzAdventure
{
    internal static partial class HypnosWeakRef
    {
        internal static bool downedHypnos => HypnosWorld.downedHypnos;
    }
}

namespace EverquartzAdventure.NPCs.TownNPCs.Mystery
{
    [AutoloadHead]
    public class Hypnos : ModNPC
    {
        public static readonly Asset<Texture2D> eyepatchTex = ModContent.Request<Texture2D>("EverquartzAdventure/NPCs/TownNPCs/Mystery/Eyepatch");
        public static readonly Asset<Texture2D> glowTex = ModContent.Request<Texture2D>("EverquartzAdventure/NPCs/TownNPCs/Mystery/Hypnos_Glow");

        public static readonly string ButtonTextKey = "Mods.EverquartzAdventure.NPCs.TownNPCs.Hypnos.ButtonText";
        public static readonly string BestiaryTextKey = "Mods.EverquartzAdventure.NPCs.TownNPCs.Hypnos.BestiaryText";

        public static readonly string ChatCommonKey = "Mods.EverquartzAdventure.NPCs.TownNPCs.Hypnos.Chat.Common";
        public static readonly string ChatBloodMoonKey = "Mods.EverquartzAdventure.NPCs.TownNPCs.Hypnos.Chat.BloodMoon";
        public static readonly string ChatPartyKey = "Mods.EverquartzAdventure.NPCs.TownNPCs.Hypnos.Chat.Party";
        public static readonly string ChatPreHypnosKey = "Mods.EverquartzAdventure.NPCs.TownNPCs.Hypnos.Chat.PreHypnos";
        public static readonly string ChatPostHypnosKey = "Mods.EverquartzAdventure.NPCs.TownNPCs.Hypnos.Chat.PostHypnos";
        public static readonly string ChatPrayKey = "Mods.EverquartzAdventure.NPCs.TownNPCs.Hypnos.Chat.Pray";
        public static readonly string ChatPrayWithoutMoneyKey = "Mods.EverquartzAdventure.NPCs.TownNPCs.Hypnos.Chat.PrayWithoutMoney";

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
                Velocity = 1f,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
            NPCID.Sets.BossBestiaryPriority.Add(base.Type);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[1]
            {

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
            NPC.damage = 10;
            NPC.defense = 90;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            TownNPCStayingHomeless = true;
            NPC.trapImmune = true;
            base.NPC.dontTakeDamageFromHostiles = true;
            base.NPC.lavaImmune = true;
            //NPC.rarity = 2;//设置稀有度
            //AnimationType = npcID;
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
                base.NPC.frameCounter += 0.2;
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
