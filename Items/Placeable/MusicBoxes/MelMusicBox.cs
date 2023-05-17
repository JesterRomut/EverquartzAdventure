using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.Localization;
using EverquartzAdventure.Tiles.MusicBoxes;

namespace EverquartzAdventure.Items.Placeable.MusicBoxes
{
    public class MelanieMartinezMusicBox: EverquartzItem
    {
        //public override string Texture => "Item_1599";
        public static string TrolledKey => "Mods.EverquartzAdventure.Items.MelanieMartinezMusicBox.Trolled";
        public static string Trolled2Key => "Mods.EverquartzAdventure.Items.MelanieMartinezMusicBox.Trolled2";
        public override void SetStaticDefaults()
        {
            base.SacrificeTotal = 1;
            base.DisplayName.SetDefault("Music Box (Melanie Martinez)");
            DisplayName.AddTranslation(7, "音乐盒 (Melanie Martinez)");
            base.Tooltip.SetDefault("Plays Deimo's favorite song, EVIL, by Melanie Martinez");
            Tooltip.AddTranslation(7, "播放戴莫斯最爱的歌，EVIL，作者是Melanie Martinez");
            MusicLoader.AddMusicBox(base.Mod, MusicLoader.GetMusicSlot(base.Mod, "Sounds/Music/Rickroll"), ModContent.ItemType<MelanieMartinezMusicBox>(), ModContent.TileType<MelanieMartinezMusicBoxPlaced>());
        }
        public override void SetDefaults()
        {
            base.Item.useStyle = ItemUseStyleID.Swing;
            base.Item.useTurn = true;
            base.Item.useAnimation = 15;
            base.Item.useTime = 10;
            base.Item.autoReuse = true;
            base.Item.consumable = true;
            base.Item.createTile = ModContent.TileType<MelanieMartinezMusicBoxPlaced>();
            base.Item.width = 32;
            base.Item.height = 32;
            base.Item.rare = ModContent.RarityType<CelestialRarity>();
            base.Item.value = 0;
            base.Item.accessory = true;
        }

        public override bool CanUseItem(Player player)
        {
            //if (!player.Everquartz().trolled)
            //{
            //    CombatText.NewText(player.Hitbox, Color.White, Language.GetTextValue(TrolledKey), true);
            //    player.Everquartz().trolled = true;
            //    return false;
            //}
            return true;
        }
        public override bool? UseItem(Player player)
        {
            
            return base.UseItem(player);
        }

        public override bool PrePlaceThing_Tiles(Player player, bool canUse)
        {
            EverquartzPlayer modPlayer = player.Everquartz();
            if (modPlayer == null) return false;
            if (canUse && !modPlayer.musicBoxTrolled && Main.myPlayer == player.whoAmI)
            {
                if (modPlayer.musicBoxTrollAttempt == 0)
                {
                    CombatText.NewText(player.Hitbox, Color.White, Language.GetTextValue(TrolledKey), true);
                }
                if (modPlayer.musicBoxTrollAttempt > 150)
                {
                    CombatText.NewText(player.Hitbox, Color.White, Language.GetTextValue(Trolled2Key), true);
                    modPlayer.musicBoxTrolled = true;
                    return true;
                }
                modPlayer.musicBoxTrollAttempt++;
                
                    
                return false;
            }
            return canUse;
        }
    }
}
