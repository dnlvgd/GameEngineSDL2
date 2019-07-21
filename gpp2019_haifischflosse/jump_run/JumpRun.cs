using System;
using System.IO;
using System.Runtime.InteropServices;
using static SDL2.SDL;

namespace gpp2019_haifischflosse
{
    public class JumpRun : Game
    {
        public JumpRun()
        {
            InitGame();
        }

        public static void Main(string[] args)
        {
            Window.InitWindow("JumpRun", 960, 540, SDL_WindowFlags.SDL_WINDOW_SHOWN);
            JumpRun game = new JumpRun();
            game.GameLoop();
        }

        public void InitGame()
        {
            gameStateSys.ChangeState(new TitleScreen());
        }       

        public void GameLoop()
        {
            while (true)
            {
                if (closeWindow)
                {
                    closeWindow = false;
                    break;
                }
                TimeInfo.Begin();
                ProcessInput();
                UpdateSimulation();
                RenderFrame();
                //TimeInfo.Sleep(16.6);
                TimeInfo.End();
                FrameInfo.CalculateFrameInfo(TimeInfo.Time, TimeInfo.DeltaTime);
            }
            CleanUp();
            Game.ResetSystems();
        }

        public void ProcessInput()
        {
            // Handle all Key Events
            InputHandler.keyEventsPerTick.Clear();
            while (SDL_PollEvent(out e) != 0)
            {
                InputHandler.keyEventsPerTick.Enqueue(e);
                if (InputHandler.QuitWindowsRedX(e))
                {
                    Game.closeWindow = true;
                }
                else if (InputHandler.Zero(e))
                {
                    Window.isDebug = !Window.isDebug;
                    Console.WriteLine("(JumpRun.cs) isDebug: " + Window.isDebug);
                }
            }
      
            /*
            // Handle all Key States
            InputHandler.UpdateKeyboardState();
            if(InputHandler.Escape())
            {
                Game.closeWindow = true;
            }
            else if (InputHandler.Zero())
            {
                Window.isDebug = !Window.isDebug;
                Console.WriteLine("Jump&Run Window.isDebug: " + Window.isDebug);
            }
            else if (InputHandler.One())
            {
                gameStateSys.ChangeState(new MainMenue());
            }
            */
            /* Process GameState Inputs */
            Game.gameStateSys.ProcessInput();
        }

        public void UpdateSimulation()
        {
            gameStateSys.Update();
            physicSys.CheckOverlapAABB();
            physicSys.Move(TimeInfo.DeltaTime);
            audioSys.Update();
            weaponSys.Update();
            spawnSys.Update(TimeInfo.DeltaTime);
            objectSys.Update(objectSys.GlobalMousePos(InputHandler.MousePosition()));
            behaviorSys.Update(TimeInfo.DeltaTime, Game.input);
            renderSys.Update();
            physicSys.Update(TimeInfo.DeltaTime);

            EventSystem.Instance.Update(); //Resets the EventQueue  should be last step in update!
        }

        public void RenderFrame()
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
        }

        private void CleanUp()
        {
            SDL_RenderClear(Renderer.renderer);
            SDL_RenderPresent(Renderer.renderer);
            audioSys.StopMusic();
        }

    }
}
