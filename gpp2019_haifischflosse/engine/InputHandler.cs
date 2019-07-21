using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;
using XnaGeometry;
using SDL2;
using System.Runtime.InteropServices;

namespace gpp2019_haifischflosse
{
    public static class InputHandler
    {
        // Key Events
        public static Queue<SDL_Event> keyEventsPerTick = new Queue<SDL_Event>();   //used as cache for all SDL_Events

        // Key States
        private static byte[] CurrentKeyStates;
        private static bool isInit = false; // used to check if the CurrentKeyStates array is valid to use

        /* ################# */
        /* Handle Key Events */
        /* ################# */

        public static bool ButtonPressed(SDL_Event e)
        {
            if(e.type == SDL_EventType.SDL_KEYDOWN)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("Keyboard", KeyState.Down);
                }
                return true;
            }
            return false;
        }

        public static bool ButtonReleased(SDL_Event e)
        {
            if (e.type == SDL_EventType.SDL_KEYUP)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("Keyboard", KeyState.Up);
                }
                return true;
            }
            return false;
        }
        
        public static bool QuitWindowsRedX(SDL_Event e)
        {
            if(e.type == SDL_EventType.SDL_QUIT && e.key.repeat == 0)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("QuitWindowsRedX", KeyState.Down);
                }
                return true;
            }
            return false;
        }

        public static bool Escape(SDL_Event e)
        {
            if (e.type == SDL_EventType.SDL_KEYDOWN && e.key.repeat == 0 && e.key.keysym.sym == SDL_Keycode.SDLK_ESCAPE)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("Escape", KeyState.Down);
                }
                return true;
            }
            return false;
        }

        public static bool Zero(SDL_Event e)
        {
            if (e.type == SDL_EventType.SDL_KEYDOWN && e.key.repeat == 0 && e.key.keysym.sym == SDL_Keycode.SDLK_0)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("0", KeyState.Down);
                }
                return true;
            }
            return false;
        }

        public static bool One(SDL_Event e)
        {
            if (e.type == SDL_EventType.SDL_KEYDOWN && e.key.repeat == 0 && e.key.keysym.sym == SDL_Keycode.SDLK_1)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("1", KeyState.Down);
                }
                return true;
            }
            return false;
        }

        public static bool Two(SDL_Event e)
        {
            if (e.type == SDL_EventType.SDL_KEYDOWN && e.key.repeat == 0 && e.key.keysym.sym == SDL_Keycode.SDLK_2)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("2", KeyState.Down);
                }
                return true;
            }
            return false;
        }

        public static bool Three(SDL_Event e)
        {
            if (e.type == SDL_EventType.SDL_KEYDOWN && e.key.repeat == 0 && e.key.keysym.sym == SDL_Keycode.SDLK_3)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("3", KeyState.Down);
                }
                return true;
            }
            return false;
        }

        public static bool Four(SDL_Event e)
        {
            if (e.type == SDL_EventType.SDL_KEYDOWN && e.key.repeat == 0 && e.key.keysym.sym == SDL_Keycode.SDLK_4)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("4", KeyState.Down);
                }
                return true;
            }
            return false;
        }

        public static bool Five(SDL_Event e)
        {
            if (e.type == SDL_EventType.SDL_KEYDOWN && e.key.repeat == 0 && e.key.keysym.sym == SDL_Keycode.SDLK_5)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("5", KeyState.Down);
                }
                return true;
            }
            return false;
        }

        public static bool Space(SDL_Event e)
        {
            if (e.type == SDL_EventType.SDL_KEYDOWN && e.key.repeat == 0 && e.key.keysym.sym == SDL_Keycode.SDLK_SPACE)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("Space", KeyState.Down);
                }
                return true;
            }
            return false;
        }

        public static bool SpaceRe(SDL_Event e)
        {
            if (e.type == SDL_EventType.SDL_KEYUP && e.key.repeat == 0 && e.key.keysym.sym == SDL_Keycode.SDLK_SPACE)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("space", KeyState.Up);
                }
                return true;
            }
            return false;
        }

        public static bool BackSpace(SDL_Event e)
        {
            if (e.type == SDL_EventType.SDL_KEYDOWN && e.key.repeat == 0 && e.key.keysym.sym == SDL_Keycode.SDLK_BACKSPACE)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("BackSpace", KeyState.Down);
                }
                return true;
            }
            return false;
        }

        public static bool MouseMoved(SDL_Event e)
        {
            if(e.type == SDL_EventType.SDL_MOUSEMOTION)
            {
                return true;
            }
            return false;
        }

        public static Vector2 MousePosition()
        {
            SDL_GetMouseState(out int mousePosX, out int mousePosY);
            return new Vector2(mousePosX, mousePosY);
        }

        public static bool MouseButtonLeft(SDL_Event e)
        {
            if (e.type == SDL_EventType.SDL_MOUSEBUTTONDOWN && e.key.repeat == 0 && e.button.button == SDL_BUTTON_LEFT)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("left mouse button", KeyState.Down);
                }
                return true;
            }
            return false;
        }

        public static bool MouseButtonLeftRe(SDL_Event e)
        {
            if (e.type == SDL_EventType.SDL_MOUSEBUTTONUP && e.key.repeat == 0 && e.button.button == SDL_BUTTON_LEFT)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("left mouse button", KeyState.Up);
                }
                return true;
            }
            return false;
        }

        public static bool MouseButtonRight(SDL_Event e)
        {
            if (e.type == SDL_EventType.SDL_MOUSEBUTTONDOWN && e.key.repeat == 0 && e.button.button == SDL_BUTTON_RIGHT )
            {
                if (Window.isDebug)
                {
                    PrintButtonState("right mouse button", KeyState.Down);
                }
                return true;
            }
            return false;
        }

        public static bool MouseButtonRightRe(SDL_Event e)
        {
            if (e.type == SDL_EventType.SDL_MOUSEBUTTONUP && e.key.repeat == 0 && e.button.button == SDL_BUTTON_RIGHT)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("right mouse button", KeyState.Up);
                }
                return true;
            }
            return false;
        }

        public static bool W(SDL_Event e)
        {
            if(e.type == SDL_EventType.SDL_KEYDOWN && e.key.repeat == 0 && e.key.keysym.sym == SDL_Keycode.SDLK_w)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("w", KeyState.Down);
                }
                return true;
            }
            return false;
        }

        public static bool WRe(SDL_Event e)
        {
            if (e.type == SDL_EventType.SDL_KEYUP && e.key.repeat == 0 && e.key.keysym.sym == SDL_Keycode.SDLK_w)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("w", KeyState.Up);
                }
                return true;
            }
            return false;
        }

        public static bool S(SDL_Event e)
        {
            if (e.type == SDL_EventType.SDL_KEYDOWN && e.key.repeat == 0 && e.key.keysym.sym == SDL_Keycode.SDLK_s)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("s", KeyState.Down);
                }
                return true;
            }
            return false;
        }

        public static bool SRe(SDL_Event e)
        {
            if (e.type == SDL_EventType.SDL_KEYUP && e.key.repeat == 0 && e.key.keysym.sym == SDL_Keycode.SDLK_s)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("s", KeyState.Up);
                }
                return true;
            }
            return false;
        }

        public static bool A(SDL_Event e)
        {
            if (e.type == SDL_EventType.SDL_KEYDOWN && e.key.repeat == 0 && e.key.keysym.sym == SDL_Keycode.SDLK_a)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("a", KeyState.Down);
                }
                return true;
            }
            return false;
        }

        public static bool ARe(SDL_Event e)
        {
            if (e.type == SDL_EventType.SDL_KEYUP && e.key.repeat == 0 && e.key.keysym.sym == SDL_Keycode.SDLK_a)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("a", KeyState.Up);
                }
                return true;
            }
            return false;
        }

        public static bool D(SDL_Event e)
        {
            if (e.type == SDL_EventType.SDL_KEYDOWN && e.key.repeat == 0 && e.key.keysym.sym == SDL_Keycode.SDLK_d)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("d", KeyState.Down);
                }
                return true;
            }
            return false;
        }

        public static bool DRe(SDL_Event e)
        {
            if (e.type == SDL_EventType.SDL_KEYUP && e.key.repeat == 0 && e.key.keysym.sym == SDL_Keycode.SDLK_d)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("d", KeyState.Up);
                }
                return true;
            }
            return false;
        }

        public static bool ArrowUp(SDL_Event e)
        {
            if (e.type == SDL_EventType.SDL_KEYDOWN && e.key.repeat == 0 && e.key.keysym.sym == SDL_Keycode.SDLK_UP)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("up arrow key", KeyState.Down);
                }
                return true;
            }
            return false;
        }

        public static bool ArrowUpRe(SDL_Event e)
        {
            if (e.type == SDL_EventType.SDL_KEYUP && e.key.repeat == 0 && e.key.keysym.sym == SDL_Keycode.SDLK_UP)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("up arrow key", KeyState.Up);
                }
                return true;
            }
            return false;
        }

        public static bool ArrowDown(SDL_Event e)
        {
            if (e.type == SDL_EventType.SDL_KEYDOWN && e.key.repeat == 0 && e.key.keysym.sym == SDL_Keycode.SDLK_DOWN)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("down arrow key", KeyState.Down);
                }
                return true;
            }
            return false;
        }

        public static bool ArrowDownRe(SDL_Event e)
        {
            if (e.type == SDL_EventType.SDL_KEYUP && e.key.repeat == 0 && e.key.keysym.sym == SDL_Keycode.SDLK_DOWN)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("down arrow key", KeyState.Up);
                }
                return true;
            }
            return false;
        }

        public static bool ArrowLeft(SDL_Event e)
        {
            if (e.type == SDL_EventType.SDL_KEYDOWN && e.key.repeat == 0 && e.key.keysym.sym == SDL_Keycode.SDLK_LEFT)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("left arrow key", KeyState.Down);
                }
                return true;
            }
            return false;
        }

        public static bool ArrowLeftRe(SDL_Event e)
        {
            if (e.type == SDL_EventType.SDL_KEYUP && e.key.repeat == 0 && e.key.keysym.sym == SDL_Keycode.SDLK_LEFT)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("left arrow key", KeyState.Up);
                }
                return true;
            }
            return false;
        }

        public static bool ArrowRight(SDL_Event e)
        {
            if (e.type == SDL_EventType.SDL_KEYDOWN && e.key.repeat == 0 && e.key.keysym.sym == SDL_Keycode.SDLK_RIGHT)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("right arrow key", KeyState.Down);
                }
                return true;
            }
            return false;
        }

        public static bool ArrowRightRe(SDL_Event e)
        {
            if (e.type == SDL_EventType.SDL_KEYUP && e.key.repeat == 0 && e.key.keysym.sym == SDL_Keycode.SDLK_RIGHT)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("right arrow key", KeyState.Up);
                }
                return true;
            }
            return false;
        }

        public static bool R(SDL_Event e)
        {
            if (e.type == SDL_EventType.SDL_KEYDOWN && e.key.repeat == 0 && e.key.keysym.sym == SDL_Keycode.SDLK_r)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("r", KeyState.Down);
                }
                return true;
            }
            return false;
        }

        public static bool RRe(SDL_Event e)
        {
            if (e.type == SDL_EventType.SDL_KEYUP && e.key.repeat == 0 && e.key.keysym.sym == SDL_Keycode.SDLK_r)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("r", KeyState.Up);
                }
                return true;
            }
            return false;
        }

        public static bool Strg(SDL_Event e)
        {
            if (e.type == SDL_EventType.SDL_KEYDOWN && e.key.repeat == 0 && e.key.keysym.sym == SDL_Keycode.SDLK_LCTRL)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("L-Strg", KeyState.Down);
                }
                return true;
            }
            return false;
        }

        public static bool StrgRe(SDL_Event e)
        {
            if (e.type == SDL_EventType.SDL_KEYUP && e.key.repeat == 0 && e.key.keysym.sym == SDL_Keycode.SDLK_LCTRL)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("L-Strg", KeyState.Up);
                }
                return true;
            }
            return false;
        }

        public static bool Shift(SDL_Event e)
        {
            if (e.type == SDL_EventType.SDL_KEYDOWN && e.key.repeat == 0 && e.key.keysym.sym == SDL_Keycode.SDLK_LSHIFT)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("L-Shift", KeyState.Down);
                }
                return true;
            }
            return false;
        }

        public static bool ShiftRe(SDL_Event e)
        {
            if (e.type == SDL_EventType.SDL_KEYUP && e.key.repeat == 0 && e.key.keysym.sym == SDL_Keycode.SDLK_LSHIFT)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("L-Strg", KeyState.Up);
                }
                return true;
            }
            return false;
        }

        /* ################# */
        /* Handle Key States */
        /* ################# */

        public static bool Escape()
        {
            if (isInit && CurrentKeyStates[(int)SDL_Scancode.SDL_SCANCODE_ESCAPE] == 1)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("Escape", KeyState.Down);
                }
                return true;
            }
            return false;
        }

        public static bool Zero()
        {
            if (isInit && CurrentKeyStates[(int)SDL_Scancode.SDL_SCANCODE_0] == 1)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("0", KeyState.Down);
                }
                return true;
            }
            return false;
        }

        public static bool One()
        {
            if (isInit && CurrentKeyStates[(int)SDL_Scancode.SDL_SCANCODE_1] == 1)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("1", KeyState.Down);
                }
                return true;
            }
            return false;
        }

        public static bool Two()
        {
            if (isInit && CurrentKeyStates[(int)SDL_Scancode.SDL_SCANCODE_2] == 1)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("2", KeyState.Down);
                }
                return true;
            }
            return false;
        }

        public static bool Three()
        {
            if (isInit && CurrentKeyStates[(int)SDL_Scancode.SDL_SCANCODE_3] == 1)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("3", KeyState.Down);
                }
                return true;
            }
            return false;
        }

        public static bool Space()
        {
            if (isInit && CurrentKeyStates[(int)SDL_Scancode.SDL_SCANCODE_SPACE] == 1)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("spacebar", KeyState.Down);
                }
                return true;
            }
            return false;
        }

        public static bool SpaceRe()
        {
            if (isInit && CurrentKeyStates[(int)SDL_Scancode.SDL_SCANCODE_SPACE] == 0)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("spacebar", KeyState.Up);
                }
                return true;
            }
            return false;
        }

        public static bool W()
        {
            if (isInit && CurrentKeyStates[(int)SDL_Scancode.SDL_SCANCODE_W] == 1)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("w", KeyState.Down);
                }
                return true;
            }
            return false;
        }

        public static bool WRe()
        {
            if (isInit && CurrentKeyStates[(int)SDL_Scancode.SDL_SCANCODE_W] == 0)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("w", KeyState.Up);
                }
                return true;
            }
            return false;
        }

        public static bool S()
        {
            if (isInit && CurrentKeyStates[(int)SDL_Scancode.SDL_SCANCODE_S] == 1)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("s", KeyState.Down);
                }
                return true;
            }
            return false;
        }

        public static bool SRe()
        {
            if (isInit && CurrentKeyStates[(int)SDL_Scancode.SDL_SCANCODE_S] == 0)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("s", KeyState.Up);
                }
                return true;
            }
            return false;
        }

        public static bool A()
        {
            if (isInit && CurrentKeyStates[(int)SDL_Scancode.SDL_SCANCODE_A] == 1)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("a", KeyState.Down);
                }
                return true;
            }
            return false;
        }

        public static bool ARe()
        {
            if (isInit && CurrentKeyStates[(int)SDL_Scancode.SDL_SCANCODE_A] == 0)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("a", KeyState.Up);
                }
                return true;
            }
            return false;
        }

        public static bool D()
        {
            if (isInit && CurrentKeyStates[(int)SDL_Scancode.SDL_SCANCODE_D] == 1)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("d", KeyState.Down);
                }
                return true;
            }
            return false;
        }

        public static bool DRe()
        {
            if (isInit && CurrentKeyStates[(int)SDL_Scancode.SDL_SCANCODE_A] == 0)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("d", KeyState.Up);
                }
                return true;
            }
            return false;
        }

        public static bool ArrowUp()
        {
            if (isInit && CurrentKeyStates[(int)SDL_Scancode.SDL_SCANCODE_UP] == 1)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("up arrow key", KeyState.Down);
                }
                return true;
            }
            return false;
        }

        public static bool ArrowUpRe()
        {
            if (isInit && CurrentKeyStates[(int)SDL_Scancode.SDL_SCANCODE_UP] == 0)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("up arrow key", KeyState.Up);
                }
                return true;
            }
            return false;
        }

        public static bool ArrowDown()
        {
            if (isInit && CurrentKeyStates[(int)SDL_Scancode.SDL_SCANCODE_DOWN] == 1)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("down arrow key", KeyState.Down);
                }
                return true;
            }
            return false;
        }

        public static bool ArrowDownRe()
        {
            if (isInit && CurrentKeyStates[(int)SDL_Scancode.SDL_SCANCODE_DOWN] == 0)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("down arrow key", KeyState.Up);
                }
                return true;
            }
            return false;
        }

        public static bool ArrowLeft()
        {
            if (isInit && CurrentKeyStates[(int)SDL_Scancode.SDL_SCANCODE_LEFT] == 1)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("left arrow key", KeyState.Down);
                }
                return true;
            }
            return false;
        }

        public static bool ArrowLeftRe()
        {
            if (isInit && CurrentKeyStates[(int)SDL_Scancode.SDL_SCANCODE_LEFT] == 0)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("left arrow key", KeyState.Up);
                }
                return true;
            }
            return false;
        }

        public static bool ArrowRight()
        {
            if (isInit && CurrentKeyStates[(int)SDL_Scancode.SDL_SCANCODE_RIGHT] == 1)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("right arrow key", KeyState.Down);
                }
                return true;
            }
            return false;
        }
        
        public static bool ArrowRightRe()
        {
            if (isInit && CurrentKeyStates[(int)SDL_Scancode.SDL_SCANCODE_RIGHT] == 0)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("right arrow key", KeyState.Up);
                }
                return true;
            }
            return false;
        }
        
        public static bool R()
        {
            if (isInit && CurrentKeyStates[(int)SDL_Scancode.SDL_SCANCODE_R] == 1)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("r", KeyState.Down);
                }
                return true;
            }
            return false;
        }

        public static bool RRe()
        {
            if (isInit && CurrentKeyStates[(int)SDL_Scancode.SDL_SCANCODE_R] == 0)
            {
                if (Window.isDebug)
                {
                    PrintButtonState("r", KeyState.Up);
                }
                return true;
            }
            return false;
        }

        public static void UpdateKeyboardState()
        {
            //SDL's internal keystates are updated every time SDL_PollEvent is called,
            //so make sure you polled all events on queue before checking key states.
            //InputHandler.CurrentKeyStates = SDL_GetKeyboardState(out int numKeys);

            IntPtr currentKeyStatesPtr = SDL_GetKeyboardState(out int length);
            byte[] currentKeyStatesArray = new byte[length];
            Marshal.Copy(currentKeyStatesPtr, currentKeyStatesArray, 0, length);
            CurrentKeyStates = currentKeyStatesArray;

            isInit = KeyBoardStateIsInitialized(CurrentKeyStates);

            /*
            Console.WriteLine("Anfang");
            for(int i = 0; i < currentKeyStatesArray.Length; i++)
            {

                if (currentKeyStatesArray[i] == 0)
                    continue;

                Console.WriteLine("{0}:\t {1} {2}", i, (SDL.SDL_Scancode)i, currentKeyStatesArray[i]);
            }
            Console.WriteLine("Ende");
            Console.WriteLine("");
            */

        }

        public static bool KeyBoardStateIsInitialized(byte[] currentKeyStates)
        {
            if (currentKeyStates != null)
            {
                return true;
            }
            return false;
        }

        private static void PrintButtonState(string buttonName, KeyState keyState)
        {
            switch (keyState)
            {
                case KeyState.Down:
                    Console.WriteLine(buttonName + " pressed");
                    break;
                case KeyState.Up:
                    Console.WriteLine(buttonName + " released");
                    break;
                case KeyState.Request:
                    Console.WriteLine(buttonName + " requested");
                    break;
            }
        }

        enum KeyState
        {
            Down,
            Up,
            Request
        }
    }
}
