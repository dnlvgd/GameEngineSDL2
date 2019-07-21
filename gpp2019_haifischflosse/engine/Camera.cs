using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;

namespace gpp2019_haifischflosse
{ 
    public class Camera
    {
        public static SDL_Rect camera = new SDL_Rect { x = 0, y = 0, w = Window.CURRENT_SCREEN_WIDTH, h = Window.CURRENT_SCREEN_HEIGHT };
        public static SDL_Rect cameraReset = new SDL_Rect { x = 0, y = 0, w = Window.CURRENT_SCREEN_WIDTH, h = Window.CURRENT_SCREEN_HEIGHT };
    }
}
