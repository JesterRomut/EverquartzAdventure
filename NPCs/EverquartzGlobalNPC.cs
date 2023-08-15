using EverquartzAdventure.NPCs.TownNPCs;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using System.Linq;
using CalamityMod;

namespace EverquartzAdventure
{
    internal static partial class CalamityWeakRef
    {
        //internal static void RemoveDR(NPC npc)
        //{
            
        //    npc.Calamity().DR = 0;
        //}
        //internal static void Test(NPC npc)
        //{
        //    CombatText.NewText(npc.Hitbox, Color.White, npc.Calamity().DR.ToString());
        //}
    }
}

namespace EverquartzAdventure.NPCs
{
    
    public class EverquartzGlobalNPC : GlobalNPC
    {
        

        //public override bool StrikeNPC(NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        //{

        //}

        //public int mindcrashed = 0;

        public override bool InstancePerEntity => true;

        //public override void DrawEffects(NPC npc, ref Color drawColor)
        //{
        //    if (Main.LocalPlayer.Everquartz().mindcrashed)
        //    {
        //        //EverquartzAdventureMod.Instance.Logger.Info($"{npc.FullName}");
        //        drawColor = EverquartzUtils.ColorSwap(Mindcrashed.Palette, 2);
        //    }
        //}

        //public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        //{
        //    if (Main.LocalPlayer.Everquartz().mindcrashed)
        //    {
        //        SpriteEffects spriteEffects = (SpriteEffects)0;
        //        if (npc.spriteDirection == 1)
        //        {
        //            spriteEffects = (SpriteEffects)1;
        //        }
        //        float num216 = 0f;
        //        Vector2 vector11 = new Vector2((float)(TextureAssets.Npc[npc.type].Value.Width / 2), (float)(TextureAssets.Npc[npc.type].Value.Height / Main.npcFrameCount[npc.type] / 2));
        //        Color color9 = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0);
        //        Color alpha15 = npc.GetAlpha(color9);
        //        float num212 = 0.99f;
        //        alpha15.R = ((byte)((float)(int)alpha15.R * num212));
        //        alpha15.G = ((byte)((float)(int)alpha15.G * num212));
        //        alpha15.B = ((byte)((float)(int)alpha15.B * num212));
        //        alpha15.A = ((byte)((float)(int)alpha15.A * num212));
        //        for (int num213 = 0; num213 < 4; num213++)
        //        {
        //            Vector2 position9 = npc.position;
        //            float num214 = Math.Abs(npc.Center.X - Main.LocalPlayer.Center.X);
        //            float num215 = Math.Abs(npc.Center.Y - Main.LocalPlayer.Center.Y);
        //            if (num213 == 0 || num213 == 2)
        //            {
        //                position9.X = Main.LocalPlayer.Center.X + num214;
        //            }
        //            else
        //            {
        //                position9.X = Main.LocalPlayer.Center.X - num214;
        //            }
        //            position9.X -= npc.width / 2;
        //            if (num213 == 0 || num213 == 1)
        //            {
        //                position9.Y = Main.LocalPlayer.Center.Y + num215;
        //            }
        //            else
        //            {
        //                position9.Y = Main.LocalPlayer.Center.Y - num215;
        //            }
        //            position9.Y -= npc.height / 2;
        //            Main.spriteBatch.Draw(TextureAssets.Npc[npc.type].Value, new Vector2(position9.X - screenPos.X + (float)(npc.width / 2) - (float)TextureAssets.Npc[npc.type].Value.Width * npc.scale / 2f + vector11.X * npc.scale, position9.Y - screenPos.Y + (float)npc.height - (float)TextureAssets.Npc[npc.type].Value.Height * npc.scale / (float)Main.npcFrameCount[npc.type] + 4f + vector11.Y * npc.scale + num216 + npc.gfxOffY), (Rectangle?)npc.frame, alpha15, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
        //        }
        //    }
        //    if (npc.Everquartz().mindcrashed > 0)
        //    {

        //        //SpriteEffects spriteEffects = (SpriteEffects)0;
        //        //if (npc.spriteDirection == 1)
        //        //{
        //        //    spriteEffects = (SpriteEffects)1;
        //        //}
        //        //Vector2 position9 = npc.position;
        //        //float num216 = 0f;
        //        //Color color9 = EverquartzUtils.ColorSwap(Mindcrashed.Palette, 2);
        //        //Color alpha15 = npc.GetAlpha(color9);
        //        //float scale = npc.scale * 2;
        //        //Vector2 vector11 = new Vector2((float)(TextureAssets.Npc[npc.type].Value.Width / 2), (float)(TextureAssets.Npc[npc.type].Value.Height / Main.npcFrameCount[npc.type] / 2));
        //        //Main.spriteBatch.Draw(TextureAssets.Npc[npc.type].Value, new Vector2(position9.X - screenPos.X + (float)(npc.width / 2) - (float)TextureAssets.Npc[npc.type].Value.Width * scale / 2f + vector11.X * scale, position9.Y - screenPos.Y + (float)npc.height - (float)TextureAssets.Npc[npc.type].Value.Height * scale / (float)Main.npcFrameCount[npc.type] + 4f + vector11.Y * npc.scale + num216 + npc.gfxOffY), (Rectangle?)npc.frame, alpha15, npc.rotation, vector11, scale, spriteEffects, 0f);
        //        Mindcrashed.DrawBackAfterimages(spriteBatch, npc, screenPos);
        //    }
        //    return true;
        //}

        //public override GlobalNPC Clone(NPC from, NPC to)
        //{
        //    EverquartzGlobalNPC myClone = (EverquartzGlobalNPC)base.Clone(from, to);
        //    myClone.mindcrashed = mindcrashed;
        //    return myClone;
        //}

        //public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        //{
        //    if (npc.Everquartz().mindcrashed > 0)
        //    {
        //        //if (modifiers.FinalDamage.Multiplicative == 0)
        //        //{
        //        //    modifiers.FinalDamage.Flat += modifiers.FinalDamage.Flat;
        //        //}
        //        //else
        //        //if (modifiers.FinalDamage.Multiplicative < 1)
        //        //{
        //        //    modifiers.FinalDamage *= (1 / modifiers.FinalDamage.Multiplicative);
        //        //}
        //        //modifiers.FinalDamage *= 0;
        //        modifiers.FinalDamage.Flat += modifiers.SourceDamage.Flat / 2;
        //        //modifiers.SetCrit();
        //    }
        //}

        //public override void PostAI(NPC npc)
        //{
        //    if (mindcrashed > 0)
        //    {
        //        npc.defense = 0;
        //        //if (ModCompatibility.calamityEnabled)
        //        //{
        //        //    CalamityWeakRef.RemoveDR(npc);
        //        //}
        //        mindcrashed--;
        //    }
        //}

        //public override void OnKill(NPC npc)
        //{
        //    if (npc.boss && ModCompatibility.hypnosEnabled && ModCompatibility.HypnosBossType.HasValue && npc.type == ModCompatibility.HypnosBossType.Value)
        //    {
        //        int hypNpcType = ModContent.NPCType<NPCs.Hypnos.Hypnos>();
        //        Main.npc.Where(npc2 => npc2.active && npc2.type == hypNpcType).ToList().ForEach(hypno => ((NPCs.Hypnos.Hypnos)hypno.ModNPC).KillWithCoins());
        //    }
        //}
    }

}