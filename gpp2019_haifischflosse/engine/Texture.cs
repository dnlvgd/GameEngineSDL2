using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;
using static SDL2.SDL_image;
using XnaGeometry;

namespace gpp2019_haifischflosse
{
    public class Texture
    {
        protected IntPtr loadedSurface = IntPtr.Zero;
        protected IntPtr scaledSurface = IntPtr.Zero;

        public int Width { get; set; } = 0;
        public int Height { get; set; } = 0;
        public IntPtr Textur { get; set; }

        public Texture(string path)
        {
            LoadImageToSurface(path, true);
        }

        public Texture(string path, int w, int h)
        {
            this.Width = w;
            this.Height = h;
            LoadImageToSurface(path, false);
        }

        private void LoadImageToSurface(string path, bool defaultSize)
        {
            //Load image from specified path
            loadedSurface = IMG_Load(path);

            if (loadedSurface == IntPtr.Zero)
            {
                Console.WriteLine("Unable to load image {0}! SDL Error: {1}", path, SDL_GetError());
                return;
            }
            var s = Marshal.PtrToStructure<SDL_Surface>(loadedSurface);
            
            SDL_FreeSurface(scaledSurface);
            scaledSurface = IntPtr.Zero;

            
            SDL_Rect scaledRect;
            scaledRect.x = 0;
            scaledRect.y = 0;

            if (defaultSize)
            {
                this.Width = scaledRect.w = (int)(s.w);
                this.Height = scaledRect.h = (int)(s.h);
            }
            else
            {
                scaledRect.w = (int)(Width);
                scaledRect.h = (int)(Height);
            }

            uint rmask, gmask, bmask, amask;
            rmask = 0x000000ff;
            gmask = 0x0000ff00;
            bmask = 0x00ff0000;
            amask = 0xff000000;

            scaledSurface = SDL_CreateRGBSurface(0, Width, Height, 32, rmask, gmask, bmask, amask);

            SDL_BlitScaled(loadedSurface, IntPtr.Zero, scaledSurface, ref scaledRect);
            
            //Color key image
            SDL_SetColorKey(scaledSurface, (int)SDL_bool.SDL_TRUE, SDL_MapRGB(s.format, 0, 0xFF, 0xFF));
            var newTexture = SDL_CreateTextureFromSurface(Renderer.renderer, scaledSurface);
            if (newTexture == IntPtr.Zero)
            {
                Console.WriteLine("Unable to create texture! SDL Error: {0}", SDL_GetError());
            }

            Textur = newTexture;
        }
    }
}
