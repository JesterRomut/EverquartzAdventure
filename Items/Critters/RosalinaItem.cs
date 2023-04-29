﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.Rarities;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using EverquartzAdventure.NPCs.TownNPCs;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using EverquartzAdventure.Items.Weapons;
using Terraria.Audio;
using CalamityMod.NPCs.Providence;
using Terraria.Localization;

namespace EverquartzAdventure.Items.Critters
{
    public class StarbornPrincessItem : ModItem
    {
        public override string Texture => "EverquartzAdventure/NPCs/TownNPCs/StarbornPrincess";
        public override void SetStaticDefaults()
        {
            base.SacrificeTotal = 5;
            base.DisplayName.SetDefault("Deimos the Starborn Princess");
            base.Tooltip.SetDefault("Right click to murder");
            Main.RegisterItemAnimation(base.Item.type, new DrawAnimationVertical(5, 6));
            ItemID.Sets.AnimatesAsSoul[base.Type] = true;
        }

        public override void SetDefaults()
        {
            base.Item.useStyle = ItemUseStyleID.Swing;
            base.Item.autoReuse = true;
            base.Item.useTurn = true;
            base.Item.useAnimation = 25;
            base.Item.useTime = 25;
            base.Item.maxStack = 1;
            base.Item.consumable = true;
            base.Item.noUseGraphic = true;
            base.Item.value = Item.buyPrice(platinum: 10);
            base.Item.width = 18;
            base.Item.height = 40;
            base.Item.makeNPC = (short)ModContent.NPCType<StarbornPrincess>();
            base.Item.rare = ItemRarityID.Purple;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void RightClick(Player player)
        {
            int helptext = Main.rand.Next(EverquartzUtils.GetTextListFromKey(StarbornPrincess.HelpListKey).Count());
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                StarbornPrincess.ItemDeathEffectServer(player, helptext);
            }
            else
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)EverquartzMessageType.DeimosItemKilled);
                packet.Write(player.whoAmI);
                packet.Write(helptext);
                packet.Send();
            }

            


        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            StarbornPrincess.ModifyLoot(itemLoot);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            //tooltips.ForEach(line => line.Text += tooltips.IndexOf(line));
        }
    }
}
