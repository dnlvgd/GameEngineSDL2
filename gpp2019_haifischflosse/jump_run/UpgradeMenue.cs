using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;

namespace gpp2019_haifischflosse
{
    class UpgradeMenue : IGameState
    {
        public List<GameObject> GameObjects { get; set; } = new List<GameObject>();

        public void CleanUp()
        {
            Camera.camera = Camera.cameraReset;

            foreach (GameObject go in GameObjects)
            {
                Game.objectSys.DeleteGameObject(go.Name);
                go.IsActive = false;
            }
        }

        public void Init()
        {
            int gold = loadGold();
            GameObject text = Game.objectSys.CreateGameObject("Text", Tag.Text, 12, 50, 1, 1, 0, true);
            text.AddComponent(true, Game.uiSys.CreateTextComponent("Your Gold: "+ gold, 50, 255, 100, 25, 255));
            GameObjects.Add(text);

            //TODO Zurück Button mit PopGameState -> Game.gameStateSys.PopState();
        }

        private int loadGold()
        {
            int myGold;
            string[] lines = File.ReadAllLines(FilePath.playerSafe);
            string[] goldsafe = lines[0].Split(',');

            Int32.TryParse(goldsafe[1], out myGold);
            return myGold;
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

                if (InputHandler.Escape(e))
                {
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
            
        }
    }
}
