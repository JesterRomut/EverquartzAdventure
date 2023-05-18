using System.Collections.Generic;
using Terraria.ModLoader;
using System.Linq;
using Terraria;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria.ID;
using EverquartzAdventure.NPCs.TownNPCs;
using EverquartzAdventure.NPCs;
using System;
using Terraria.Localization;
using EverquartzAdventure.Items.Critters;
using Terraria.ModLoader.IO;
using EverquartzAdventure.NPCs.Hypnos;
using System.Collections;
using EverquartzAdventure.ILEditing;
using EverquartzAdventure.UI.Transmogrification;

namespace EverquartzAdventure
{
    internal static class EverquartzUtils
    {
        internal static bool ActiveTiles(int startX, int endX, int startY, int endY)
        {
            for (int i = startX; i < endX + 1; i++)
            {
                for (int j = startY; j < endY + 1; j++)
                {
                    if (Main.tile[i, j] == null)
                    {
                        return false;
                    }
                    if (!Main.tile[i, j].HasUnactuatedTile || !Main.tileSolid[Main.tile[i, j].TileType])
                    {
                        Dust.NewDustPerfect(new Point(i, j).ToWorldCoordinates(), DustID.Water, Vector2.Zero);
                        return false;
                    }
                }
            }
            return true;
        }

        internal static bool TileCapable(int tileX, int tileY)
        {
            if (tileX < 0 || tileY < 0 || tileX >= Main.maxTilesX || tileY >= Main.maxTilesY)
            {
                //EverquartzAdventureMod.Instance.Logger.Info("not pass: tileX < 0 || tileY < 0 || tileX > Main.maxTilesX || tileY > Main.maxTilesY");
                return false;
            }

            //Dust.NewDustPerfect(new Vector2(tileX * 16, tileY * 16), DustID.Water, Vector2.Zero);

            if (!ActiveTiles(tileX, tileX + 1, tileY + 3, tileY + 3))//!Main.tile[tileX, tileY + 4].Solid() && !Main.tile[tileX + 1, tileY + 4].Solid())
            {
                //EverquartzAdventureMod.Instance.Logger.Info("not pass: !Collision.SolidTiles(tileX, tileX + 1, tileY + 4, tileY + 4)");
                return false;
            }
            if (Main.tile[tileX, tileY].HasLiquid() || Main.tile[tileX, tileY + 1].HasLiquid() || Main.tile[tileX, tileY + 2].HasLiquid() || ActiveTiles(tileX, tileX + 1, tileY, tileY + 2))
            {
                //EverquartzAdventureMod.Instance.Logger.Info("not pass: hasliquid or");
                return false;
            }
            return true;
        }

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

        internal static int EnumCount<T>() where T : Enum
        {
            return Enum.GetNames(typeof(T)).Length;
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

        public static Color ColorSwap(List<Color> colors, float seconds)
        {
            float colorMePurple = (float)((Math.Sin((double)((float)Math.PI * 2f / seconds) * (double)Main.GlobalTimeWrappedHourly) + 1.0) * 0.5);
            //ModContent.GetInstance<EverquartzAdventureMod>().Logger.Info(colorMePurple);
            int count = colors.Count();

            for (int i = 1; i <= count; i++)
            {
                double level = (double)i / (double)count;
                //EverquartzAdventureMod.Instance.Logger.Info($"{i} {count} {level} {(double)(i - 1) / count}");
                if (colorMePurple < level)
                {
                    double lerpAmount = (double)(i - 1) / count;
                    if (i == count)
                    {
                        return Color.Lerp(colors[i - 1], colors[0], (float)(((double)colorMePurple - lerpAmount) * count));
                    }
                    return Color.Lerp(colors[i - 1], colors[i], (float)(((double)colorMePurple - lerpAmount) * count));
                }
            }
            return Color.White;
        }
    }

}