using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;

namespace gpp2019_haifischflosse
{
    public class TitleScreen : IGameState
    { 
        public List<GameObject> GameObjects { get; set; } = new List<GameObject>();

        public void CleanUp()
        {
            foreach(GameObject go in GameObjects)
            {
                Game.objectSys.DeleteGameObject(go.Name);
                go.IsActive = false;
            }
        }

        public void Init()
        {
            GameObject text = Game.objectSys.CreateGameObject("Text", Tag.Text, 12, 50, 1, 1, 0, true);
            text.AddComponent(true, Game.uiSys.CreateTextComponent("Jump&Run", 50, 255, 255, 255, 255));
            GameObjects.Add(text);

            GameObject start = Game.objectSys.CreateGameObject("Start", Tag.Text, 12, 100, 1, 1, 0, true);
            start.AddComponent(true, Game.uiSys.CreateTextComponent("Press SPACE to start the Game", 50, 255, 255, 255, 255));
            GameObjects.Add(start);

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

                if (InputHandler.Space(e))
                {
                    PlayClickSound();
                    Game.gameStateSys.ChangeState(new MainMenue());
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
            Game.audioSys.Update();
        }

        private void PlayClickSound()
        {
            GameObject buttonClick = Game.objectSys.FindGameObject("buttonClickSound");
            EventSystem.Instance.AddEvent("ButtonClicked", buttonClick);
        }
    }
}
