using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;
using static SDL2.SDL_mixer;
using static SDL2.SDL_image;
using static SDL2.SDL_ttf;

namespace gpp2019_haifischflosse
{
    public class Window
    {
        public static IntPtr window = IntPtr.Zero;
        public static IntPtr surface = IntPtr.Zero;

        public static int CURRENT_SCREEN_WIDTH;
        public static int CURRENT_SCREEN_HEIGHT;

        public static int OLD_SCREEN_WIDTH;
        public static int OLD_SCREEN_HEIGHT;

        public static float SCALEFACTOR_X = 0.5f;
        public static float SCALEFACTOR_Y = 0.5f;

        public static double WINDOW_SCALEFACTOR_X = 0;
        public static double WINDOW_SCALEFACTOR_Y = 0;

        public static bool isFullscreen = false;
        public static bool isDebug = false;

        public static void InitWindow(string windowName, int width, int height, SDL_WindowFlags flag)
        {
            InitSDL();

            CURRENT_SCREEN_WIDTH = width;
            CURRENT_SCREEN_HEIGHT = height;

            window = SDL_CreateWindow(windowName, SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED, Window.CURRENT_SCREEN_WIDTH, Window.CURRENT_SCREEN_HEIGHT, flag);

            if (window == System.IntPtr.Zero)
            {
                Console.WriteLine(SDL_GetError());
            }

            Renderer.CreateRender();

            surface = SDL_GetWindowSurface(window);

            if (surface == System.IntPtr.Zero)
            {
                Console.WriteLine(SDL_GetError());
                return;
            }
            SDL_UpdateWindowSurface(surface);
        }

        public static void FullscreenMode()
        {
            SDL_SetWindowFullscreen(window, (uint) SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP);
            SetWindowData();
        }

        public static void WindowMode(int width, int height)
        {
            SDL_SetWindowSize(window, width, height);
            SDL_SetWindowFullscreen(window, 0);
            SetWindowData();          
        }

        public static void SetWindowData()
        {
            OLD_SCREEN_WIDTH = CURRENT_SCREEN_WIDTH;
            OLD_SCREEN_HEIGHT = CURRENT_SCREEN_HEIGHT;
            SDL_GetWindowSize(window, out CURRENT_SCREEN_WIDTH, out CURRENT_SCREEN_HEIGHT);

            SCALEFACTOR_X = (SCALEFACTOR_X / OLD_SCREEN_WIDTH) * CURRENT_SCREEN_WIDTH;
            SCALEFACTOR_Y = (SCALEFACTOR_Y / OLD_SCREEN_HEIGHT) * CURRENT_SCREEN_HEIGHT;

            WINDOW_SCALEFACTOR_X = (double)(CURRENT_SCREEN_WIDTH / OLD_SCREEN_WIDTH);
            WINDOW_SCALEFACTOR_Y = (double)(CURRENT_SCREEN_HEIGHT / OLD_SCREEN_HEIGHT);
        }

        public static void ToggleFullscreen(GameObjectSystem objectSys, PhysicsSystem physicSys, UISystem uiSys)
        {
            if (!Window.isFullscreen)
            {
                Window.isFullscreen = true;
                Window.FullscreenMode();
                Rescale(objectSys, physicSys, uiSys);
            }
            else
            {
                Window.isFullscreen = false;
                Window.WindowMode(960, 540);
                Rescale(objectSys, physicSys, uiSys);
            }
        }

        private static void Rescale(GameObjectSystem objectSys, PhysicsSystem physicSys, UISystem uiSys)
        {
            //Rescale
            objectSys.RescaleGameObjectPositions();
            physicSys.RescaleCollider();
            uiSys.RescaleCollider();
            World.WORLD_HEIGHT = (int)(World.WORLD_HEIGHT * Window.WINDOW_SCALEFACTOR_X);
            World.WORLD_WIDTH = (int)(World.WORLD_WIDTH * Window.WINDOW_SCALEFACTOR_Y);
        }

        public static void PrintWindowData()
        {
            Console.WriteLine(
                "CURRENT_SCREEN_WIDTH: {0}" +
                " CURRENT_SCREEN_HEIGHT: {1}" +
                " SCALEFACTOR_X: {2}" +
                " SCALEFACTOR_Y: {3}" +
                " OLD_SCREEN_WIDTH: {4}" +
                " OLD_SCREEN_HEIGHT: {5}",
                CURRENT_SCREEN_WIDTH,
                CURRENT_SCREEN_HEIGHT,
                SCALEFACTOR_X,
                SCALEFACTOR_Y,
                OLD_SCREEN_WIDTH,
                OLD_SCREEN_HEIGHT
                );

        }

        public static void InitSDL()
        {
            //Init SimpleDirectMediaLayer2 + SDL Audio
            if (SDL_Init(SDL_INIT_EVERYTHING | SDL_INIT_AUDIO) < 0)
            {
                Console.WriteLine(SDL_GetError());
                return;
            }

            //Initialize SDL_mixer
            if (Mix_OpenAudio(44100, MIX_DEFAULT_FORMAT, 2, 2048) < 0)
            {
                Console.WriteLine("SDL_mixer could not initialize!");
            }

            //Init SDL2_TrueTypeText 
            if (TTF_Init() < 0)
            {
                Console.WriteLine("Failed to load TTF!");
                return;
            }
        }
    }
}
