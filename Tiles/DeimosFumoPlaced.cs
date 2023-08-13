using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using EverquartzAdventure.Items;

namespace EverquartzAdventure.Tiles
{
    public class DeimosFumoPlaced : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[base.Type] = true;
            Main.tileObsidianKill[base.Type] = true;
            TileID.Sets.DisableSmartCursor[base.Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.newTile.CoordinateHeights = new int[2] { 16, 16 };
            TileObjectData.newTile.AnchorInvalidTiles = new int[1] { 127 };
            //TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            //TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(base.Type);
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(200, 200, 200), name);
            base.DustType = 11;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ModContent.ItemType<DeimosFumo>());
        }
    }
}
