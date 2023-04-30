using System.Collections.Generic;
using Terraria.ModLoader;
using System.Linq;
using Terraria;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.DataStructures;
using EverquartzAdventure.NPCs.TownNPCs;
using static Terraria.ModLoader.PlayerDrawLayer;
using System;
using Terraria.Audio;
using Terraria.Localization;
using EverquartzAdventure.Items.Weapons;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI.Chat;

namespace EverquartzAdventure
{
	public class EverquartzAdventureMod : Mod
	{
        public override void PostSetupContent()
        {
			ModCompatibility.calamityEnabled = ModLoader.HasMod("CalamityMod");
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            EverquartzMessageType msgType = (EverquartzMessageType)reader.ReadByte();
            switch (msgType)
            {
                case EverquartzMessageType.DeimosItemKilled:
                    //Main.player[reader.ReadInt32()];
                    Player murderer = Main.player[reader.ReadInt32()];
                    int helptext = reader.ReadInt32();
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        StarbornPrincess.ItemDeathEffectServer(murderer, helptext);
                    }
                    else
                    {
                        
                        StarbornPrincess.ItemDeathEffectClient(murderer.position, murderer.width, murderer.height, helptext);
                    }
                        
                    break;
            }
        }
    }

    public class CelestialRarity : ModRarity
    {
        // FFD79F F6809F A063B9
        public override Color RarityColor => firstColor;

        public static readonly Color firstColor = new Color(255, 215, 159);
        public static readonly Color secondColor = new Color(246, 128, 159);
        public static readonly Color thirdColor = new Color(160, 99, 185);
        //public static readonly Color fourthColor

        public static Color ColorSwap()
        { // 0 < colormepurple < 1
            return EverquartzUtils.ColorSwap(firstColor, secondColor, thirdColor, 3);
        }
        public static void PreDraw(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            Color rarityColor = (Color)((line.OverrideColor) ?? line.Color);
            Vector2 basePosition = new Vector2((float)line.X, (float)line.Y);

            float backInterpolant;
            float backInterpolant2;
            float changingTime = Main.GlobalTimeWrappedHourly * 1.5f;
            if (Math.Floor(changingTime) % 2 == 0)
            {
                backInterpolant = 0;
                backInterpolant2 = changingTime % 1;
            }
            else
            {
                backInterpolant = changingTime % 1;
                backInterpolant2 = 0;
            }
            Vector2 backPosition = basePosition + new Vector2(2f, 4f);
            //ModContent.GetInstance<EverquartzAdventureMod>().Logger.Info(Main.GlobalTimeWrappedHourly);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin((SpriteSortMode)1, BlendState.Additive, (SamplerState)null, (DepthStencilState)null, (RasterizerState)null, (Effect)null, Main.UIScaleMatrix);
            for (int i = 0; i < 2; i++)
            {
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, line.Font, line.Text, backPosition, EverquartzUtils.ColorSwap(new Color(246, 128, 159), Color.Orange , 1) * (Main.rand.Next(3, 7) / 10f), backInterpolant * 2 * (float)Math.PI, line.Origin, line.BaseScale, line.MaxWidth, line.Spread);
            }
            for (int i = 0; i < 2; i++)
            {
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, line.Font, line.Text, backPosition + new Vector2(14f, 4f), EverquartzUtils.ColorSwap(Color.Cyan, new Color(160, 99, 185), 1) * (Main.rand.Next(3, 7) / 10f), backInterpolant2 * -2 * (float)Math.PI, line.Origin, line.BaseScale, line.MaxWidth, line.Spread);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin((SpriteSortMode)0, BlendState.AlphaBlend, (SamplerState)null, (DepthStencilState)null, (RasterizerState)null, (Effect)null, Main.UIScaleMatrix);
            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, line.Font, line.Text, basePosition + new Vector2(Main.rand.Next(-2, 2)), rarityColor, line.Rotation, line.Origin, line.BaseScale, line.MaxWidth, line.Spread);

        }
    }

    public class EverquartzGlobalItem: GlobalItem
    {
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine nameLine = tooltips.FirstOrDefault((TooltipLine x) => x.Name == "ItemName" && x.Mod == "Terraria");
            if (nameLine != null)
            {
                ApplyRarityColor(item, nameLine);
            }
        }

        public void ApplyRarityColor(Item item, TooltipLine nameLine)
        {
            if(item.rare == ModContent.RarityType<CelestialRarity>())
            {
                nameLine.OverrideColor = CelestialRarity.ColorSwap();
            }
        }

        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Name == "ItemName" && line.Mod == "Terraria" && item.rare == ModContent.RarityType<CelestialRarity>())
            {
                CelestialRarity.PreDraw(item, line, ref yOffset);
                return false;
            }
            return true;
        }
    }

    public class EverquartzPlayer: ModPlayer
    {
        public void HandleDeadDeimos(Player murderer)
        {
            StarbornPrincess.DeathEffectClient(murderer.position, murderer.width, murderer.height);
        }
    }

    public enum EverquartzMessageType
    {
        DeimosItemKilled // id, player, helptext
    }

	public static class ModCompatibility
	{
		public static bool calamityEnabled = false;
	}

	[JITWhenModsEnabled("CalamityMod")]
	internal static partial class CalamityWeakRef
	{

	}

    internal static class EverquartzExtensions
    {
        internal static T Random<T>(this IEnumerable<T> li)
        {
            return li.ElementAt(Main.rand.Next(li.Count()));
        }

        internal static void AddShopItem(this Chest shop, ref int nextSlot, int item, int price = -1)
        {
            shop.item[nextSlot].SetDefaults(item);
            if (price != -1)
            {
                shop.item[nextSlot].shopCustomPrice = price;
            }
            nextSlot++;
        }

        internal static Vector2 NearestPos(this Vector2 here, Vector2 dest, float radius)
        {
            double angle = Math.Atan2(here.Y - dest.Y, here.X - dest.X);
            return new Vector2((float)(dest.X + radius * Math.Cos(angle)), (float)(dest.Y + radius * Math.Sin(angle)));
        }

        internal static Vector2 NearestPos(this Vector2 here, double angle, float radius)
        {
            return new Vector2((float)(here.X + radius * Math.Cos(angle)), (float)(here.Y + radius * Math.Sin(angle)));
        }
    }

    internal static class EverquartzUtils
    {
        
        internal static List<string> GetTextListFromKey(string key)
        {
            int index = 0;
            List<string> li = new List<string>();
            while (true)
            {
                if (index > 300)
                {
                    break;
                }
                string lineKey = $"{key}.{index}";
                string line = Language.GetTextValue(lineKey);
                if (line == lineKey || line == null || line == string.Empty)
                {
                    break;
                }
                else
                {
                    li.Add(line);
                }
                index++;
            }
            return li;
        }

        public static Color ColorSwap(Color firstColor, Color secondColor, float seconds)
        {
            float colorMePurple = (float)((Math.Sin((double)((float)Math.PI * 2f / seconds) * (double)Main.GlobalTimeWrappedHourly) + 1.0) * 0.5);
            return Color.Lerp(firstColor, secondColor, colorMePurple);
        }

        public static Color ColorSwap(Color firstColor, Color secondColor, Color thirdColor, float seconds)
        { // 0 < colormepurple < 1
            float colorMePurple = (float)((Math.Sin((double)((float)Math.PI * 2f / seconds) * (double)Main.GlobalTimeWrappedHourly) + 1.0) * 0.5);
            //ModContent.GetInstance<EverquartzAdventureMod>().Logger.Info(colorMePurple);
            if (colorMePurple < 0.33f)
            {
                return Color.Lerp(firstColor, secondColor, colorMePurple * 3);
            }
            else if (colorMePurple < 0.66f)
            {
                return Color.Lerp(secondColor, thirdColor, (colorMePurple - 0.33f) * 3);
            }
            else
            {
                return Color.Lerp(thirdColor, firstColor, (colorMePurple - 0.66f) * 3);
            }

        }
    }
    }