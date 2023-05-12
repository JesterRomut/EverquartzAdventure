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

namespace EverquartzAdventure.NPCs.TownNPCs.Mystery
{
    [AutoloadHead]
    public class Hypnos: ModNPC
    {
        public static readonly Asset<Texture2D> eyepatchTex = ModContent.Request<Texture2D>("EverquartzAdventure/NPCs/TownNPCs/Mystery/Eyepatch");
        public static readonly Asset<Texture2D> glowTex = ModContent.Request<Texture2D>("EverquartzAdventure/NPCs/TownNPCs/Mystery/Hypnos_Glow");

        public static readonly string ButtonTextKey = "Mods.EverquartzAdventure.NPCs.TownNPCs.Hypnos.ButtonText";
        public static readonly string BestiaryTextKey = "Mods.EverquartzAdventure.NPCs.TownNPCs.Hypnos.BestiaryText";

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
                texture.Size() * 0.5f,
                NPC.scale,
                NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                0f
            );
        }

        //public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        //{
        //    if (NPC.direction == 1)
        //    {
        //        //Texture2D texture = TextureAssets.Npc[base.NPC.type].Value;
        //        //Vector2 vector11 = new Vector2((float)(texture.Width / 2), (float)(texture.Height / 2));
        //        //Vector2 vector12 = base.NPC.Center - screenPos;
        //        //vector12 -= new Vector2((float)texture.Width, (float)texture.Height) * base.NPC.scale / 2f;
        //        //vector12 += vector11 * base.NPC.scale + new Vector2(0f, base.NPC.gfxOffY);
        //        //spriteBatch.Draw(texture, vector12, (Rectangle?)base.NPC.frame, Color.White, base.NPC.rotation, vector11, base.NPC.scale, SpriteEffects.FlipHorizontally, 0f);
        //    }
        //    Texture2D texture = TextureAssets.Npc[base.NPC.type].Value;

        //    Draw(texture, spriteBatch, screenPos);
        //    //return false;
        //}

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
                NPC.frame.Size() * 0.5f,
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
