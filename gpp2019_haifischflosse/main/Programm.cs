using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using static SDL2.SDL;
using static SDL2.SDL_ttf;
using static SDL2.SDL_image; 
using static SDL2.SDL_mixer;
using XnaGeometry;

namespace gpp2019_haifischflosse
{
    public class Programm
    {
        private GameObjectSystem objectSys = new GameObjectSystem();
        private RenderSystem renderSys = new RenderSystem();
        private PhysicsSystem physicSys = new PhysicsSystem();
        private UISystem uiSys = new UISystem();
        private AudioSystem audioSys = new AudioSystem();

        private int indexofLastEvent = 0;

        SDL_Event e;
        bool closeWindow = false;
        

        //main folder path
        private const string assets = "assets/";
        //sub folders
        private const string audios = assets + "audio/";
        private const string sprites = assets + "sprites/";
        private const string textures = assets + "texture/";
        //sub sub folders
        private const string buttonPath = sprites + "button/";
        private const string backgroundPath = textures + "backgrounds/";


        //Text mainMenu;

        //Music menuSong;

        public Programm()
        {
            InitGame();
        }
        
        static void Main(string[] args)
        {
            Programm menu = new Programm();
            menu.GameLoop();
        }

        private void InitGame()
        {
            //Create window
            Window.InitWindow("Game", 960, 540, SDL_WindowFlags.SDL_WINDOW_SHOWN);
            BuildGameObjects();
        }

        public void BuildGameObjects()
        {
            //GameObject menuMusic = objectSys.CreateGameObject("menuMusic");
            //menuMusic.AddComponent(audioSys.CreateMusicComponent(audios + "XXX.mp3"));

            bool isActive = true;
            GameObject menuBackground = objectSys.CreateGameObject("menuBackground", Tag.MenuBackground, 0, 0, 1, 1, 0, true);
            menuBackground.AddComponent(isActive, renderSys.CreateTextureComponent(backgroundPath + "stars.png", 0));

            int buttonWidth = (int)(222 * 2.0);
            int buttonHeight = (int)(39 * 2.0);
            int buttonTextSize = 48;
            GameObject buttonGameOne = objectSys.CreateGameObject("ButtonGameOne", Tag.LoadGameButton, Window.CURRENT_SCREEN_WIDTH / 2 - ((int)(buttonWidth * 0.25)), (int)((float)Window.CURRENT_SCREEN_HEIGHT * 0.5), 1, 1, 0, true);
            buttonGameOne.AddComponent(isActive, renderSys.CreateTextureComponent(buttonPath + "buttonBlue.png", 1, buttonWidth, buttonHeight));
            buttonGameOne.AddComponent(isActive, uiSys.CreateTextComponent(" [1] Air Pong", buttonTextSize, 0, 0, 0, 255));
            TextureComponent buttonGameOneTC = buttonGameOne.GetComponent<TextureComponent>() as TextureComponent;
            if (buttonGameOneTC != null)
            {
                buttonGameOne.AddComponent(isActive, uiSys.CreateButtonClickAreaComponent((int)buttonGameOne.Position.X, (int)buttonGameOne.Position.Y, buttonGameOneTC.DstRect.w, buttonGameOneTC.DstRect.h));
            }

            GameObject buttonGameTwo = objectSys.CreateGameObject("ButtonGameTwo", Tag.LoadGameButton, Window.CURRENT_SCREEN_WIDTH / 2 - ((int)(buttonWidth * 0.25)), (int)((float)Window.CURRENT_SCREEN_HEIGHT * 0.6), 1, 1, 0, true);
            buttonGameTwo.AddComponent(isActive, renderSys.CreateTextureComponent(buttonPath + "buttonBlue.png", 1, buttonWidth, buttonHeight));
            buttonGameTwo.AddComponent(isActive, uiSys.CreateTextComponent(" [2] Astroids", buttonTextSize, 0, 0, 0, 255));
            TextureComponent buttonGameTwoTC = buttonGameTwo.GetComponent<TextureComponent>() as TextureComponent;
            if (buttonGameTwoTC != null)
            {
                buttonGameTwo.AddComponent(isActive, uiSys.CreateButtonClickAreaComponent((int)buttonGameTwo.Position.X, (int)buttonGameTwo.Position.Y, buttonGameTwoTC.DstRect.w, buttonGameTwoTC.DstRect.h));
            }

            GameObject buttonGameThree = objectSys.CreateGameObject("ButtonGameThree", Tag.LoadGameButton, Window.CURRENT_SCREEN_WIDTH / 2 - ((int)(buttonWidth * 0.25)), (int)((float)Window.CURRENT_SCREEN_HEIGHT * 0.7), 1, 1, 0, true);
            buttonGameThree.AddComponent(isActive, renderSys.CreateTextureComponent(buttonPath + "buttonBlue.png", 1, buttonWidth, buttonHeight));
            buttonGameThree.AddComponent(isActive, uiSys.CreateTextComponent(" [3] TileGame", buttonTextSize, 0, 0, 0, 255));
            TextureComponent buttonGameThreeTC = buttonGameThree.GetComponent<TextureComponent>() as TextureComponent;
            if (buttonGameThreeTC != null)
            {
                buttonGameThree.AddComponent(isActive, uiSys.CreateButtonClickAreaComponent((int)buttonGameThree.Position.X, (int)buttonGameThree.Position.Y, buttonGameThreeTC.DstRect.w, buttonGameThreeTC.DstRect.h));
            }

            GameObject controlText = objectSys.CreateGameObject("cText", Tag.MenuBackground, 5, 10, 1, 1, 0, true);
            controlText.AddComponent(isActive, uiSys.CreateTextComponent("LMB: Shoot | RMB: Boost | Zero: DebugView | One: GodMode | Space: Fullscreen | Backspace: BackToMenu", 27, 255, 255, 255, 255));

            GameObject mainMenuMusic = objectSys.CreateGameObject("mainMenuMusic", Tag.BackgroundMusic, -1000, -1000, 1, 1, 0, true);
            mainMenuMusic.AddComponent(true, audioSys.CreateMusicComponent(FilePath.GrayTrip));
            audioSys.PlayMusic(mainMenuMusic);

            GameObject buttonClickSound = objectSys.CreateGameObject("buttonClickSound", Tag.SoundFX, -1000, -1000, 1, 1, 0, true);
            buttonClickSound.AddComponent(true, audioSys.CreateSoundFXComponent(FilePath.ButtonClick));
        }

        private void GameLoop()
        {
            while (true)
            {
                if (closeWindow)
                {
                    break;
                }
                ProcessInput();
                UpdateSimulation();
                RenderFrame();
            }
            CleanUp();
            CleanUpInitializedSubsystems();
        }

        void ProcessInput()
        {
            while (SDL_PollEvent(out e) != 0)
            {
                //Key Event for Window X Button to quit the game
                if (InputHandler.QuitWindowsRedX(e))
                {
                    closeWindow = true;
                }
                else if (InputHandler.MouseButtonLeft(e))
                {
                    uiSys.CheckAllButtonsForClickEvent(InputHandler.MousePosition());
                    if (Window.isDebug)
                    {
                        Console.WriteLine("Mouse-Position: " + InputHandler.MousePosition());
                    }
                }
                else if (InputHandler.Escape(e))
                {
                    PlayClickSound();
                    closeWindow = true;
                }
                else if (InputHandler.One(e))
                {
                    PlayClickSound();
                    StartGameOne();
                }
                else if (InputHandler.Two(e))
                {
                    PlayClickSound();
                    StartGameTwo();
                }
                else if (InputHandler.Three(e))
                {
                    PlayClickSound();
                    StartGameThree();
                }
                else if (InputHandler.Space(e))
                {
                    PlayClickSound();
                    ToggleFullscreen();
                }
                else if (InputHandler.Zero(e))
                {
                    PlayClickSound();
                    Window.isDebug = !Window.isDebug;
                    Console.WriteLine("(Programm.cs) isDebug: " + Window.isDebug);
                }
            }

            /*
            InputHandler.UpdateKeyboardState();
            if (InputHandler.Escape())
            {
                closeWindow = true;
            }
            else if (InputHandler.One())
            {
                StartGameOne();
            }
            else if (InputHandler.Two())
            {
                StartGameTwo();
            }
            else if (InputHandler.Three())
            {
                StartGameThree();
            }
            else if (InputHandler.Space())
            {
                if (!Window.isFullscreen)
                {
                    Window.isFullscreen = true;
                    Window.FullscreenMode();
                    Rescale();
                }
                else
                {
                    Window.isFullscreen = false;
                    Window.WindowMode(960, 540);
                    Rescale();
                }
            }
            else if (InputHandler.Zero())
            {
                Window.isDebug = !Window.isDebug;
                Console.WriteLine("Programm Window.isDebug: " + Window.isDebug);
            }
            */
        }

        private void PlayClickSound()
        {
            GameObject buttonClick = objectSys.FindGameObject("buttonClickSound");
            EventSystem.Instance.AddEvent("ButtonClicked", buttonClick);
        }

        public void ToggleFullscreen()
        {
            Window.ToggleFullscreen(objectSys, physicSys, uiSys);
            /*
            //Rescale
            objectSys.RescaleGameObjectPositions();
            physicSys.RescaleCollider();
            uiSys.RescaleCollider();
            World.WORLD_HEIGHT = (int)(World.WORLD_HEIGHT * Window.WINDOW_SCALEFACTOR_X);
            World.WORLD_WIDTH = (int)(World.WORLD_WIDTH * Window.WINDOW_SCALEFACTOR_Y);
            */
        }

        void UpdateSimulation()
        {
            renderSys.Update();
            CheckForButtonClickedEvents();
            audioSys.Update();

            EventSystem.Instance.Update(); //Resets the EventQueue  should be last step in update!
        }

        private void CheckForButtonClickedEvents()
        {
            if (EventSystem.Instance.CheckEvent("ButtonGameOne", indexofLastEvent))
            {
                PlayClickSound();
                StartGameOne();
            }
            else if (EventSystem.Instance.CheckEvent("ButtonGameTwo", indexofLastEvent))
            {
                PlayClickSound();
                StartGameTwo();
            }
            else if (EventSystem.Instance.CheckEvent("ButtonGameThree", indexofLastEvent))
            {
                PlayClickSound();
                StartGameThree();
            }
            indexofLastEvent = EventSystem.Instance.GetMyIndex();
        }

        void RenderFrame()
        {
            SDL_RenderClear(Renderer.renderer);

            renderSys.Render(TimeInfo.DeltaTime);
            uiSys.Render();

            if (Window.isDebug)
            {
                physicSys.RenderColliderForDebug();
                uiSys.RenderColliderForDebug();
                renderSys.RenderOnceEventHandling();
            }

            SDL_RenderPresent(Renderer.renderer);
            //TODO: implement a .Free() method for each System and call their .Free() method
            //uiSys.Free();
        }

        private void CleanUp()
        {
            //throw new NotImplementedException();
        }

        private void CleanUpInitializedSubsystems()
        {
            IMG_Quit();
            TTF_Quit();
            Mix_Quit();
            SDL_Quit();
        }   

        private void StartGameOne()
        {
            Console.WriteLine("Starting AirPong ...");
            StartAirPong();
            Console.WriteLine("Closed AirPong.");
        }

        private void StartGameTwo()
        {
            Console.WriteLine("Starting ShootEmUp ...");
            StartShootEmUp();
            Console.WriteLine("Closed ShootEmUp.");
        }

        private void StartGameThree()
        {
            Console.WriteLine("Starting ThirdGame ...");
            StartJumpRun();
            Console.WriteLine("Closed ThirdGame.");
        }

        void StartAirPong()
        {
            AirPong apGame = new AirPong();
            //apGame.GameLoop();
        }

        void StartShootEmUp()
        {
            ShootEmUp ssuGame = new ShootEmUp();
            ssuGame.GameLoop();
        }

        void StartJumpRun()
        {
            JumpRun jrGame = new JumpRun();
            jrGame.GameLoop();
        }

        private int Scale(int x)
        {
            return x *= (int)Window.SCALEFACTOR_X;
        }
    }

}
