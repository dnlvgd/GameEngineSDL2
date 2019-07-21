using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;

namespace gpp2019_haifischflosse
{
    class MainMenue : IGameState
    {
        private int indexofLastEvent = 0;
        public List<GameObject> GameObjects { get; set; } = new List<GameObject>();

        public void CleanUp()
        {
            foreach (GameObject go in GameObjects)
            {
                Game.objectSys.DeleteGameObject(go.Name);
                go.IsActive = false;
            }
        }

        public void Init()
        {
            GameObject playButton1 = Game.objectSys.CreateGameObject("PlayButton1", Tag.Button, 100 * Window.SCALEFACTOR_X, 100 * Window.SCALEFACTOR_Y, 1, 1, 0, true);
            playButton1.AddComponent(true, Game.uiSys.CreateTextComponent("Level One", 30, 255, 255, 255, 255));
            playButton1.AddComponent(true, Game.uiSys.CreateButtonClickAreaComponent((int)playButton1.Position.X, (int)playButton1.Position.Y, (int)(300 * Window.SCALEFACTOR_X), (int)(100 * Window.SCALEFACTOR_Y)));
            GameObjects.Add(playButton1);

            GameObject playButton2 = Game.objectSys.CreateGameObject("PlayButton2", Tag.Button, 100 * Window.SCALEFACTOR_X, 200 * Window.SCALEFACTOR_Y, 1, 1, 0, true);
            playButton2.AddComponent(true, Game.uiSys.CreateTextComponent("Level Two", 30, 255, 255, 255, 255));
            playButton2.AddComponent(true, Game.uiSys.CreateButtonClickAreaComponent((int)playButton2.Position.X, (int)playButton2.Position.Y, (int)(300 * Window.SCALEFACTOR_X), (int)(100 * Window.SCALEFACTOR_Y)));
            GameObjects.Add(playButton2);

            GameObject upgradeButton = Game.objectSys.CreateGameObject("UpgradeButton", Tag.Button, 100 * Window.SCALEFACTOR_X, 300 * Window.SCALEFACTOR_Y, 1, 1, 0, true);
            upgradeButton.AddComponent(true, Game.uiSys.CreateTextComponent("Upgrade", 30, 255, 255, 255, 255));
            upgradeButton.AddComponent(true, Game.uiSys.CreateButtonClickAreaComponent((int)upgradeButton.Position.X, (int)upgradeButton.Position.Y, (int)(300 * Window.SCALEFACTOR_X), (int)(100 * Window.SCALEFACTOR_Y)));
            GameObjects.Add(upgradeButton);

            GameObject fullscreenButton = Game.objectSys.CreateGameObject("FullscreenButton", Tag.Button, 100 * Window.SCALEFACTOR_X, 400 * Window.SCALEFACTOR_Y, 1, 1, 0, true);
            fullscreenButton.AddComponent(true, Game.uiSys.CreateTextComponent("Fulscreen", 30, 255, 255, 255, 255));
            fullscreenButton.AddComponent(true, Game.uiSys.CreateButtonClickAreaComponent((int)fullscreenButton.Position.X, (int)fullscreenButton.Position.Y, (int)(300 * Window.SCALEFACTOR_X), (int)(100 * Window.SCALEFACTOR_Y)));
            GameObjects.Add(fullscreenButton);

            GameObject exitButton = Game.objectSys.CreateGameObject("ExitButton", Tag.Button, 100 * Window.SCALEFACTOR_X, 500 * Window.SCALEFACTOR_Y, 1, 1, 0, true);
            exitButton.AddComponent(true, Game.uiSys.CreateTextComponent("Exit", 30, 255, 255, 255, 255));
            exitButton.AddComponent(true, Game.uiSys.CreateButtonClickAreaComponent((int)exitButton.Position.X, (int)exitButton.Position.Y, (int)(300 * Window.SCALEFACTOR_X), (int)(100 * Window.SCALEFACTOR_Y)));
            GameObjects.Add(exitButton);

            GameObject buttonClickSound = Game.objectSys.CreateGameObject("buttonClickSound", Tag.SoundFX, -1000, -1000, 1, 1, 0, true);
            buttonClickSound.AddComponent(true, Game.audioSys.CreateSoundFXComponent(FilePath.ButtonClick));
        }

        public void Pause()
        {
            foreach (GameObject go in GameObjects)
            {
                Game.objectSys.DeactivateGameObject(go.Name);
            }
        }

        public void ProcessInput()
        {
            Queue<SDL_Event> tmp = InputHandler.keyEventsPerTick;
            while (tmp.Count > 0)
            {
                SDL_Event e = tmp.Dequeue();

                if (InputHandler.One(e))
                {
                    /* Changes GameState to LevelOne */
                    ClickPlayButtonOne();
                }
                else if (InputHandler.Two(e))
                {
                    /* Changes GameState to LevelTwo */
                    ClickPlayButtonTwo();
                }
                else if (InputHandler.Three(e))
                {
                    /* Changes GameState to UpgradeMenue */                   
                    ClickUpgradeButton();
                }
                else if (InputHandler.Four(e))
                {
                    /* Change Game in Fullscreen Mode and rescale all GameObjects */
                    ClickFullscreenButton();
                }
                else if (InputHandler.Five(e))
                {
                    /* Exit Game */
                    ClickExitButton();
                }
                else if (InputHandler.Escape(e))
                {
                    Game.gameStateSys.PopState();
                }
                else if (InputHandler.MouseButtonLeft(e))
                {
                    Game.uiSys.CheckAllButtonsForClickEvent(InputHandler.MousePosition());
                    if (Window.isDebug)
                    {
                        Console.WriteLine("Mouse-Position: " + InputHandler.MousePosition());
                    }
                }
            }
        }

        public void Resume()
        {
            foreach (GameObject go in GameObjects)
            {
                Game.objectSys.ActivateGameObject(go.Name);
            }
        }

        public void Update()
        {
            CheckForButtonClickedEvents();
            Game.audioSys.Update();
        }

        private void CheckForButtonClickedEvents()
        {
            if (EventSystem.Instance.CheckEvent("PlayButtonOne", indexofLastEvent))
            {
                ClickPlayButtonOne();
            }
            else if (EventSystem.Instance.CheckEvent("PlayButtonTwo", indexofLastEvent))
            {
                ClickPlayButtonTwo();
            }
            else if (EventSystem.Instance.CheckEvent("UpgradeButton", indexofLastEvent))
            {
                ClickUpgradeButton();
            }
            else if (EventSystem.Instance.CheckEvent("FullscreenButton", indexofLastEvent))
            {
                ClickFullscreenButton();
            }
            else if(EventSystem.Instance.CheckEvent("ExitButton", indexofLastEvent))
            {
                ClickExitButton();
            }

            indexofLastEvent = EventSystem.Instance.GetMyIndex();
        }
        private void ClickPlayButtonOne()
        {
            PlayClickSound();
            Game.gameStateSys.ChangeState(new LevelOne());
        }

        private void ClickPlayButtonTwo()
        {
            PlayClickSound();
            Game.gameStateSys.ChangeState(new LevelTwo());
        }

        private void ClickUpgradeButton()
        {
            PlayClickSound();
            Game.gameStateSys.ChangeState(new UpgradeMenue());
        }

        private void ClickFullscreenButton()
        {
            PlayClickSound();
            if (!Window.isFullscreen)
            {
                Game.FullscreenMode();
            }
            else
            {
                Game.WindowMode();
            }    
        }

        private void ClickExitButton()
        {
            PlayClickSound();
            Game.closeWindow = true;
        }

        private void PlayClickSound()
        {
            GameObject buttonClick = Game.objectSys.FindGameObject("buttonClickSound");
            EventSystem.Instance.AddEvent("ButtonClicked", buttonClick);
        }
    }
}
