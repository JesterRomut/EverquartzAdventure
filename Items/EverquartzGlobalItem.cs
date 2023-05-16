using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria;
using Terraria.Chat;
using Terraria.UI.Chat;
using Microsoft.Xna.Framework;
using EverquartzAdventure.UI;
using EverquartzAdventure.UI.Transmogrification;

namespace EverquartzAdventure.Items
{
    public class EverquartzGlobalItem : GlobalItem
    {
        public static List<RarityAdditiveText> rarityAdditiveTexts = new List<RarityAdditiveText>();

        public void PruneTexts(Item item)
        {
            rarityAdditiveTexts.RemoveAll(text => text.Belong != item.rare || text.shouldBeKilled);
        }

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
            if (item.rare == ModContent.RarityType<CelestialRarity>())
            {
                nameLine.OverrideColor = CelestialRarity.ColorSwap();
            }
        }
        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (EverquartzUI.instance.userInterface.CurrentState is TransmogrificationUI && TransmogrificationManager.CanTrans(item.type))
            {
                //Main.NewText("111");
                TransmogrificationUI.DrawItemAura(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
                //return false;
            }
            return true;
        }
        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {

            if (line.Name == "ItemName" && line.Mod == "Terraria")
            {
                PruneTexts(item);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin((SpriteSortMode)1, BlendState.Additive, (SamplerState)null, (DepthStencilState)null, (RasterizerState)null, (Effect)null, Main.UIScaleMatrix);
                foreach (RarityAdditiveText text in rarityAdditiveTexts)
                {
                    text.Update();
                    ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, line.Font, line.Text, text.position, text.color, text.rotation, line.Origin, line.BaseScale, line.MaxWidth, line.Spread);
                }
                Main.spriteBatch.End();
                Main.spriteBatch.Begin((SpriteSortMode)0, BlendState.AlphaBlend, (SamplerState)null, (DepthStencilState)null, (RasterizerState)null, (Effect)null, Main.UIScaleMatrix);
                if (item.rare == ModContent.RarityType<CelestialRarity>())
                {
                    CelestialRarity.PreDraw(item, line, ref yOffset);
                    return false;
                }

            }
            return true;
        }
    }
    public abstract class RarityAdditiveText
    {
        public Vector2 position;
        public float rotation;

        public Color color;

        public abstract int Belong { get; }

        public bool shouldBeKilled = false;

        public int time;

        public RarityAdditiveText(Vector2 position, float rotation, Color color)
        {
            this.position = position;
            this.rotation = rotation;
            this.color = color;
        }

        public virtual void Update()
        {
            time++;
        }

        public virtual void Kill()
        {
            shouldBeKilled = true;
        }

    }

    public class CelestialRarity : ModRarity
    {
        // FFD79F F6809F A063B9
        public class CelestialRarityAdditiveText : RarityAdditiveText
        {
            public override int Belong => ModContent.RarityType<CelestialRarity>();


            public int lastChangingTime = -1;

            public CelestialRarityAdditiveText(Vector2 position, float rotation, Color color) : base(position, rotation, color)
            {
            }

            public override void Update()
            {
                base.Update();
                position.Y -= 2 ^ (time / 10);
                float changingTime = Main.GlobalTimeWrappedHourly * 1.5f;
                if (Math.Floor(changingTime) % 2 != 0)
                {
                    if ((int)Math.Floor(changingTime) != lastChangingTime)
                    {
                        lastChangingTime = (int)Math.Floor(changingTime);

                    }
                    rotation += 0.01f;
                }

                AdditiveTextUniversalUpdate(this);
            }
        }

        public class CelestialRarityAdditiveText2 : RarityAdditiveText
        {
            public override int Belong => ModContent.RarityType<CelestialRarity>();

            public CelestialRarityAdditiveText2(Vector2 position, float rotation, Color color) : base(position, rotation, color) { }

            public override void Update()
            {
                base.Update();
                position.Y += 2;
                AdditiveTextUniversalUpdate(this);
            }
        }

        public override Color RarityColor => firstColor;

        public static readonly Color firstColor = new Color(255, 215, 159);
        public static readonly Color secondColor = new Color(246, 128, 159);
        public static readonly Color thirdColor = new Color(160, 99, 185);
        //public static readonly Color fourthColor

        public static void AdditiveTextUniversalUpdate(RarityAdditiveText text)
        {
            if (text.color == new Color(0, 0, 0))
            {
                text.Kill();
            }
            text.color *= 0.98f;
        }

        public static Color ColorSwap()
        { // 0 < colormepurple < 1
            return EverquartzUtils.ColorSwap(firstColor, secondColor, thirdColor, 3);//EverquartzUtils.ColorSwap(Buffs.Hypnos.Mindcrashed.Palette, 5); //
        }
        public static void PreDraw(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            Color rarityColor = (Color)((line.OverrideColor) ?? line.Color);
            Vector2 basePosition = new Vector2((float)line.X, (float)line.Y);

            float changingTime = Main.GlobalTimeWrappedHourly * 1.5f;
            if (Math.Floor(changingTime) % 2 == 0)
            {
                EverquartzGlobalItem.rarityAdditiveTexts.Add(new CelestialRarityAdditiveText(basePosition, line.Rotation, EverquartzUtils.ColorSwap(new Color(246, 128, 159), Color.Orange, 1) * (Main.rand.Next(3, 7) / 10f)));
                EverquartzGlobalItem.rarityAdditiveTexts.Add(new CelestialRarityAdditiveText2(basePosition, line.Rotation, EverquartzUtils.ColorSwap(new Color(246, 128, 159), Color.Orange, 1) * (Main.rand.Next(2, 4) / 10f)));
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin((SpriteSortMode)0, BlendState.AlphaBlend, (SamplerState)null, (DepthStencilState)null, (RasterizerState)null, (Effect)null, Main.UIScaleMatrix);
            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, line.Font, line.Text, basePosition + new Vector2(Main.rand.Next(-2, 2)), rarityColor, line.Rotation, line.Origin, line.BaseScale, line.MaxWidth, line.Spread);

        }
    }

}
