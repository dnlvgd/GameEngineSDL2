using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static SDL2.SDL;

namespace gpp2019_haifischflosse
{
    public class TileSet
    {
        public Tile[] LoadedTiles { get; set; }
        public string TileSheetPath { get; set; }
        public Texture TileSheetTexture { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }

        public TileSet(string tileSheetPath, int tileWidth, int tileHeight, int quatityTileColumn, int quantityTileRow, int quantityTotalTiles)
        {
            this.TileSheetPath = tileSheetPath;
            this.TileHeight = (int)(tileHeight);
            this.TileWidth = (int)(tileWidth);
            this.TileSheetTexture = new Texture(TileSheetPath);

            /* Initialize the tiles */
            LoadedTiles = new Tile[quantityTotalTiles];

            int index = 1;
            LoadedTiles[0] = new Tile(-TileWidth, -TileHeight, TileWidth, TileHeight, "EmptyTile");
            for (int y = 0; y < quantityTileRow; y++)
            {
                for (int x = 0; x < quatityTileColumn; x++)
                {
                    LoadedTiles[index] = new Tile(x * TileWidth, y * TileHeight, TileWidth, TileHeight, "TODO");
                    
                    index++;

                    if (index >= LoadedTiles.Length)
                    {
                        break;
                    }
                }
            }
        }
    }
}
