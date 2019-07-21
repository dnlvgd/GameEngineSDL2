using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;

namespace gpp2019_haifischflosse
{
    public class Tile
    {
        public SDL_Rect TileRect { get; set; }
        public string TileName { get; set; }

        public Tile(int posX, int posY, int tileWidth, int tileHeight, string tileName)
        {
            this.TileRect = new SDL_Rect
            {
                x = posX,
                y = posY,
                h = tileHeight,
                w = tileWidth

            };

            this.TileName = tileName;
        }
    }
}
