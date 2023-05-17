using Terraria.ModLoader;
using Terraria;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using EverquartzAdventure.Items.Placeable.MusicBoxes;

namespace EverquartzAdventure.Tiles.MusicBoxes
{
    public class MelanieMartinezMusicBoxPlaced : ModTile
    {
        //public override string Texture => "Item_1599";
        public override void SetStaticDefaults()
        {
            //IL_0073: Unknown result type (might be due to invalid IL or missing references)
            Main.tileFrameImportant[base.Type] = true;
            Main.tileObsidianKill[base.Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(base.Type);
            ModTranslation name = CreateMapEntryName();
            AddMapEntry(new Color(200, 200, 200), name);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 48, ModContent.ItemType<MelanieMartinezMusicBox>());
        }

        public override void MouseOver(int i, int j)
        {
            Player localPlayer = Main.LocalPlayer;
            localPlayer.noThrow = 2;
            localPlayer.cursorItemIconEnabled = true;
            localPlayer.cursorItemIconID = ModContent.ItemType<MelanieMartinezMusicBox>();
        }
    }
}