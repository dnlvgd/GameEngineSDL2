using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;
using XnaGeometry;

namespace gpp2019_haifischflosse
{   
    public class Sprite
    {
        protected uint quantitySpriteRow;
        protected uint quantitySpriteColumn;
        public uint TotalQuantitySprites { get; private set; }

        protected SDL2.SDL.SDL_Rect[] spriteClips;

        public uint CurrentSpriteClipIndex { get; set; } = 0;
        public SDL_Rect[] SpriteClips { get; set; }
        public Texture Texture { get; set; }
        public uint DelayBtwnFrames { get; set; }   // in ms
        public double AnimationTime { get; set; }
        public SDL_RendererFlip RendererFlip { get; set; } // used to flip textures vertically/horizontally during rendering

        /* Constructor for sprites */
        public Sprite(string path, uint quantitySpriteRow, uint quantitySpriteColumn, uint totalQuantitySprites, double animationDuration = 1000.0 /*ms*/, SDL_RendererFlip rendererFlip = SDL_RendererFlip.SDL_FLIP_NONE)
        {
            this.quantitySpriteRow = quantitySpriteRow;
            this.quantitySpriteColumn = quantitySpriteColumn;
            this.TotalQuantitySprites = totalQuantitySprites;
            this.AnimationTime = 0;
            // calculates the delay amount between two frames of an animation 
            // (amount of frames to skip till next frame animation draw)
            // DEFAULT: tries to display ALL frames of an animation in ONE second (1000ms)
            this.DelayBtwnFrames = (uint)(animationDuration / totalQuantitySprites);
            this.RendererFlip = rendererFlip;
            this.Texture = new Texture(path);
            SeperateIntoSpriteClips();
        }

        /* Method for spritesheets with sprites on x and y axis */
        public void SeperateIntoSpriteClips()
        {
            int spriteWidth = (int) ((Texture.Width / quantitySpriteColumn));
            int spriteHeight = (int)((Texture.Height / quantitySpriteRow));
            
            SDL_Rect[] spriteClips = new SDL_Rect[TotalQuantitySprites];

            int tempX = 0;
            int tempY = 0;
            int help = 0;

            for (int i = 0; i < quantitySpriteRow; i++)
            {
                for (int j = 0; j < quantitySpriteColumn; j++)
                {
                    spriteClips[help] = new SDL_Rect() { x = tempX, y = tempY, h = spriteHeight, w = spriteWidth };
                    tempX += spriteWidth;
                    help++;

                    //If all sprites are loaded, stop
                    if (help >= spriteClips.Length)
                    {
                        break;
                    }
                }

                tempX = 0;
                tempY += spriteHeight;
            }

            this.SpriteClips = spriteClips;
        }
    }
}
