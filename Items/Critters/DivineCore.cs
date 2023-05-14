using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using System.Linq;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using CalamityMod.Buffs.Potions;
using System;
using Terraria.Audio;

namespace EverquartzAdventure
{
	internal static partial class CalamityWeakRef
	{
        internal static int GravityNormalizerBuff => ModContent.BuffType<GravityNormalizerBuff>();
        internal static int OmniscienceBuff => ModContent.BuffType<Omniscience>();
        internal static int SoaringBuff => ModContent.BuffType<Soaring>();
    }
}

namespace EverquartzAdventure.Items.Critters
{
	public class DivineCore : ModItem
	{
        public override string Texture => "EverquartzAdventure/NPCs/Critters/DivineCore";
        public static readonly int buffDuration = 126000;
        public static List<int> Buffs => new List<int>() {
            BuffID.Rage,
            BuffID.Wrath,
            BuffID.Lifeforce,
            BuffID.Endurance,
            BuffID.Ironskin,
            BuffID.ObsidianSkin,
            BuffID.Regeneration,
            BuffID.Swiftness
        };
        public static List<int> CalamityBuffs => new List<int>()
        {
            CalamityWeakRef.SoaringBuff,
            CalamityWeakRef.OmniscienceBuff,
            CalamityWeakRef.GravityNormalizerBuff
        };

        public override void SetStaticDefaults() {			
            DisplayName.SetDefault("Divine Core");
			DisplayName.AddTranslation(7, "神圣核心");
			DisplayName.AddTranslation(6, "Божественное Ядро");
			Tooltip.SetDefault("You feel such a divine warmth seethe through your body...\nSomething tells you that you should not have this item in your hands.");
			Tooltip.AddTranslation(7, "你感到如此神圣的温度在你的体中沸腾...\n你最好不要把这个拿在手里。");
			Tooltip.AddTranslation(6, "Вы чувствуете, как божественная теплота бурлит в вашем теле...\nЧто то говорит тебе, что этот предмет не должен быть в твоих руках.");
		}
		public override void SetDefaults() {
			Item.width = 42; 
			Item.height = 102; 
			Item.maxStack = 1; 
			Item.value = 0; 
            Item.rare = ItemRarityID.Lime;

            base.Item.consumable = true;
            base.Item.useAnimation = 17;
            base.Item.useTime = 17;
            base.Item.useStyle = ItemUseStyleID.EatFood;
            base.Item.useTurn = true;
            base.Item.buffType = BuffID.WellFed3;
            //base.Item.buffTime = 3600 * 60;
        }

        public override void HoldItem(Player player)
        {

            if (ModCompatibility.calamityEnabled)
            {
                player.AddBuff(CalamityWeakRef.BuffType.HolyFlames, 120);
            }
			
        }

        public static void ReleaseProvCoreServer(Player player)
        {
            NPC.NewNPC(new EntitySource_WorldEvent(), (int)Math.Floor(player.Center.X), (int)Math.Floor(player.Center.Y), ModContent.NPCType<NPCs.Critters.DivineCore>());
        }

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                base.Item.useStyle = ItemUseStyleID.Swing;
                base.Item.noUseGraphic = true;
                if (Main.myPlayer == player.whoAmI)
                {
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        ReleaseProvCoreServer(player);
                    }
                    else
                    {
                        ModPacket packet = base.Mod.GetPacket();
                        packet.Write((byte)EverquartzMessageType.ReleaseProvCore);
                        packet.Write(player.whoAmI);
                        packet.Send();
                    }
                }
            }
            else
            {
                base.Item.useStyle = ItemUseStyleID.EatFood;
                base.Item.noUseGraphic = false;
                SoundEngine.PlaySound(SoundID.Item2, player.Center);
                Buffs.ForEach(buff => player.AddBuff(buff, buffDuration));
                if (ModCompatibility.calamityEnabled)
                {
                    CalamityBuffs.ForEach(buff => player.AddBuff(buff, buffDuration));
                }
                player.Heal(20);
                player.HealEffect(20);
            }
            

            return base.UseItem(player);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool? CanBurnInLava()
        {
            return false;
        }

        public override bool ConsumeItem(Player player)
        {
            
            //buffs.ForEach(buff => player.AddBuff(buff, buffDuration));
            return player.altFunctionUse == 2;
        }

    }
}