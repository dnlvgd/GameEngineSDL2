using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;

namespace gpp2019_haifischflosse
{
    class Level : IGameState
    {
        public List<GameObject> GameObjects { get; set; } = new List<GameObject>();

        /* TileMap */
        private int[,] tileMap = new int[,] { { 14, 17, 1, 4, 8, 11, 22 }, { 34, 8, 25, 2, 18, 4, 30 } };
        private int[,] loadedMap;
        private TileSystem tilesystem = new TileSystem("assets/spritesheet/grass.png", 70, 70, 5, 7, 35);

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
            loadMapFromFile();

            GameObject text = Game.objectSys.CreateGameObject("Game", Tag.Text, 12, 50, 1, 1, 0, true);
            text.AddComponent(true, Game.uiSys.CreateTextComponent("GAME", 20, 255, 255, 255, 255));
            GameObjects.Add(text);

            GameObject mainPlayer = Game.objectSys.CreateGameObject("mainPlayer", Tag.MainPlayer, Window.CURRENT_SCREEN_WIDTH / 2, Window.CURRENT_SCREEN_HEIGHT / 2 - 200, 1, 1, 0, true);
            mainPlayer.AddComponent(false, Game.renderSys.CreateSpriteComponent(Animation.Shoot, FilePath.knightAttack, 1, 22, 22, 1));
            mainPlayer.AddComponent(false, Game.renderSys.CreateSpriteComponent(Animation.Death, FilePath.knightDeath, 1, 15, 15, 1));
            mainPlayer.AddComponent(true, Game.renderSys.CreateSpriteComponent(Animation.Idle, FilePath.knightIdle, 1, 15, 15, 1));
            mainPlayer.AddComponent(false, Game.renderSys.CreateSpriteComponent(Animation.JumpAndFall, FilePath.knightJumpAndFall, 1, 14, 14, 1));
            mainPlayer.AddComponent(false, Game.renderSys.CreateSpriteComponent(Animation.Roll, FilePath.KnightRoll, 1, 15, 15, 1));
            mainPlayer.AddComponent(false, Game.renderSys.CreateSpriteComponent(Animation.RunRight, FilePath.KnightRun, 1, 8, 8, 1));
            mainPlayer.AddComponent(false, Game.renderSys.CreateSpriteComponent(Animation.Shield, FilePath.KnightShield, 1, 7, 7, 1));
            mainPlayer.AddComponent(true, Game.behaviorSys.CreateBehaviorComponent(new PlayerStateComponent()));
            GameObjects.Add(mainPlayer);

            //GameObject animationTest1 = Game.objectSys.CreateGameObject("test1", Tag.Test, Window.CURRENT_SCREEN_WIDTH / 2, Window.CURRENT_SCREEN_HEIGHT / 2 - 150, 1, 1, 0, true);
            //animationTest1.AddComponent(true, Game.renderSys.CreateSpriteComponent(Animation.Death, FilePath.knightDeath, 1, 15, 15, 1));
            //GameObjects.Add(animationTest1);

            //GameObject animationTest2 = Game.objectSys.CreateGameObject("test2", Tag.Test, Window.CURRENT_SCREEN_WIDTH / 2, Window.CURRENT_SCREEN_HEIGHT / 2 - 100, 1, 1, 0, true);
            //animationTest2.AddComponent(true, Game.renderSys.CreateSpriteComponent(Animation.Idle, FilePath.knightIdle, 1, 15, 15, 1));
            //GameObjects.Add(animationTest2);

            //GameObject animationTest3 = Game.objectSys.CreateGameObject("test3", Tag.Test, Window.CURRENT_SCREEN_WIDTH / 2, Window.CURRENT_SCREEN_HEIGHT / 2 - 50, 1, 1, 0, true);
            //animationTest3.AddComponent(true, Game.renderSys.CreateSpriteComponent(Animation.JumpAndFall, FilePath.knightJumpAndFall, 1, 14, 14, 1));
            //GameObjects.Add(animationTest3);

            //GameObject animationTest4 = Game.objectSys.CreateGameObject("test4", Tag.Test, Window.CURRENT_SCREEN_WIDTH / 2, Window.CURRENT_SCREEN_HEIGHT / 2 - 0, 1, 1, 0, true);
            //animationTest4.AddComponent(true, Game.renderSys.CreateSpriteComponent(Animation.Roll, FilePath.KnightRoll, 1, 15, 15, 1));
            //GameObjects.Add(animationTest4);

            //GameObject animationTest5 = Game.objectSys.CreateGameObject("test5", Tag.Test, Window.CURRENT_SCREEN_WIDTH / 2, Window.CURRENT_SCREEN_HEIGHT / 2 + 50, 1, 1, 0, true);
            //animationTest5.AddComponent(true, Game.renderSys.CreateSpriteComponent(Animation.RunRight, FilePath.KnightRun, 1, 8, 8, 1));
            //GameObjects.Add(animationTest5);

            //GameObject animationTest6 = Game.objectSys.CreateGameObject("test6", Tag.Test, Window.CURRENT_SCREEN_WIDTH / 2, Window.CURRENT_SCREEN_HEIGHT / 2 + 100, 1, 1, 0, true);
            //animationTest6.AddComponent(true, Game.renderSys.CreateSpriteComponent(Animation.Shield, FilePath.KnightShield, 1, 7, 7, 1));
            //GameObjects.Add(animationTest6);
        }

        public void Pause()
        {
            foreach (GameObject go in GameObjects)
            {
                Game.objectSys.DeactivateGameObject(go.Name);
            }
        }

        public void ProcessInput(SDL_Event e)
        {
            while (SDL_PollEvent(out e) != 0)
            {
                if (InputHandler.A(e))
                {
                    Game.input = Input.RunLeftPressed;
                }
                else if (InputHandler.ARe(e))
                {
                    Game.input = Input.RunLeftReleased;
                }
                else if (InputHandler.D(e))
                {
                    Game.input = Input.RunRightPressed;
                }
                else if (InputHandler.DRe(e))
                {
                    Game.input = Input.RunRightReleased;
                }
                else if (InputHandler.S(e))
                {
                    Game.input = Input.CrouchPressed;
                }
                else if (InputHandler.SRe(e))
                {
                    Game.input = Input.CrouchReleased;
                }
                else if (InputHandler.Space(e))
                {
                    Game.input = Input.JumpPressed;
                }
                else if (InputHandler.SpaceRe(e))
                {
                    Game.input = Input.JumpReleased;
                }
                else if (InputHandler.MouseButtonLeft(e))
                {
                    Game.input = Input.AttackPressed;
                }
            }
            //Console.WriteLine("GameInput: " + Game.input + "\n");
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
            //Game Update
        }

        public void loadMapFromFile()
        {
            string filePath = FilePath.mapLevel1;

            string[] lines = File.ReadAllLines(filePath);

            string[] sizes = lines[0].Split(',');
            int x = Convert.ToInt32(sizes[0]);
            int y = Convert.ToInt32(sizes[1]);

            int[,] loadedMap = new int[y, x];

            for (int i = 0; i < y; i++)
            {
                string[] entries = lines[i + 1].Split(',');

                for (int j = 0; j < x; j++)
                {
                    loadedMap[i, j] = Convert.ToInt32(entries[j]);
                }
            }
            for (int i = 0; i < loadedMap.GetLength(0); i++)
            {
                for (int j = 0; j < loadedMap.GetLength(1); j++)
                {
                    Console.Write(loadedMap[i, j] + " ");
                }
                Console.WriteLine("");
            }
            //Console.WriteLine(loadedMap[5,2]);
        }

    }
}
