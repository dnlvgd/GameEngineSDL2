using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;
using static SDL2.SDL_ttf;
using static SDL2.SDL_image;
using static SDL2.SDL_mixer;

namespace gpp2019_haifischflosse
{ 
    public static class Renderer
    {
        public static IntPtr renderer;

        public static bool CreateRender()
        { 
            renderer = SDL_CreateRenderer(Window.window, -1, SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
            if (renderer == System.IntPtr.Zero)
            {
                Console.WriteLine("Failed to create Renderer!");
                return false;
            }
            return true;
        }
        
}
}
