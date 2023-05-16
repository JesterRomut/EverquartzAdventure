using System.Collections.Generic;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using EverquartzAdventure;
using Terraria;
using CalamityMod;
using log4net.Repository.Hierarchy;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System;
using ReLogic.Content;
using Terraria.GameContent;

namespace EverquartzAdventure.Buffs.Hypnos
{
	public class Mindcrashed : ModBuff
	{
		
		public override string Texture => "EverquartzAdventure/Buffs/Hypnos/CityPop";
		public static readonly string RandomDescriptionKey = "Mods.EverquartzAdventure.Buffs.Mindcrashed.Descriptions";
		public static List<string> Descriptions => EverquartzUtils.GetTextListFromKey(RandomDescriptionKey);

		public static List<Color> Palette => new List<Color>()
		{
			new Color(255,113,206),
			new Color(1,205,254),
			new Color(5,255,161),
			new Color(185,103,255),
			new Color(255,251,150),
		};

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("M I N D C R A S H E D 2000");
			DisplayName.AddTranslation(7, "思 维 大 崩 溃 2000");
			Description.SetDefault("");
			Main.debuff[base.Type] = true;
			Main.pvpBuff[base.Type] = true;
			Main.buffNoSave[base.Type] = true;
			//BuffID.Sets.LongerExpertDebuff[base.Type] = true;
		}

		public override void ModifyBuffTip(ref string tip, ref int rare)
		{
			tip = string.Concat(tip, Descriptions.Random());
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.Everquartz().mindcrashed = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			//reduce enemy damage reduction(if cal), defense, and make vulunerable, and reduce enemy contact damage
			if (npc.Everquartz().mindcrashed < npc.buffTime[buffIndex])
			{
				//EverquartzAdventureMod.Instance.Logger.Info($"{npc.buffTime[buffIndex]}");
				npc.Everquartz().mindcrashed = npc.buffTime[buffIndex];
			}
			npc.DelBuff(buffIndex);
			buffIndex--;
		}

		public static void DrawBackAfterimages(SpriteBatch spriteBatch, NPC npc, Vector2 screenPos)
		{
			DrawBackAfterImage(spriteBatch, npc, screenPos);
			for (int i = 1; i < 3; i++)
			{
				Vector2 oldpos = npc.oldPos.ElementAtOrDefault(i);
				if (oldpos == default)
				{
					break;
				}
				DrawBackAfterImage(spriteBatch, npc, screenPos, oldpos);
			}
		}

		public static void DrawBackAfterImage(SpriteBatch spriteBatch, NPC npc, Vector2 screenPos, Vector2? position = null)
		{
			float pulse = Main.GlobalTimeWrappedHourly * 0.75f % 1f;
			float outwardnessFactor = MathHelper.Lerp(0.9f, 1.3f, pulse);
			Color drawColor = EverquartzUtils.ColorSwap(Mindcrashed.Palette, 2) * (1f - pulse) * 0.27f;
			Texture2D asset = TextureAssets.Npc[npc.type].Value;
			Rectangle frame = npc.frame;
			Vector2 baseDrawPosition = (position ?? npc.position) - screenPos + npc.frame.Size() * npc.scale / 2;
			drawColor.A = ((byte)0);
			SpriteEffects spriteEffects = (SpriteEffects)0;
			if (npc.spriteDirection == 1)
			{
				spriteEffects = (SpriteEffects)1;
			}
			for (int i = 0; i < 5; i+=2)
			{
				float scale = npc.scale * outwardnessFactor;
				Vector2 drawPosition = baseDrawPosition + ((float)Math.PI * 2f * (float)i / 4f).ToRotationVector2() * 4f + new Vector2(Main.rand.Next(-2, 2));
				spriteBatch.Draw(asset, drawPosition, (Rectangle?)frame, drawColor, npc.rotation, frame.Size() * 0.5f, scale, spriteEffects, 0f);
			}
		}
	}
}