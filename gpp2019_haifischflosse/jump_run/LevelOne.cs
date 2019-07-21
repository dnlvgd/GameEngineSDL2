using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XnaGeometry;
using static SDL2.SDL;

namespace gpp2019_haifischflosse
{
    public class LevelOne : IGameState
    {
        public List<GameObject> GameObjects { get; set; } = new List<GameObject>();

        /* TileMap */
        private int[,] tileMap = new int[,] { };
        private TileSet tileSet = new TileSet("assets/spritesheet/grass.png", 70, 70, 5, 7, 35);

        public void CleanUp()
        {
            Camera.camera = Camera.cameraReset;

            foreach (GameObject go in GameObjects)
            {
                Game.objectSys.DeleteGameObject(go.Name);
                go.IsActive = false;
            }

            Game.audioSys.StopMusic();
        }

        public void Init()
        {
            World world = new World(10000, 5080);

            /* Init of TileMap */
            tileMap = loadMapFromFile();
            for (int c = 0; c < tileMap.GetLength(0); c++)
            {
                for (int r = 0; r < tileMap.GetLength(1); r++)
                {
                    if(tileMap[c,r] != 0 && tileMap[c,r] <35)
                    {
                        int tileIndex = tileMap[c, r];
                        GameObject Tile = Game.objectSys.CreateGameObject("Tile" + c+"." + r, Tag.Tile, r * tileSet.TileWidth * Window.SCALEFACTOR_X, c * tileSet.TileHeight * Window.SCALEFACTOR_Y, 1, 1, 0, true);
                        Tile.AddComponent(true, Game.physicSys.CreateBoundingBoxComponent((int)(r * tileSet.TileWidth * Window.SCALEFACTOR_X), (int)(c * tileSet.TileHeight * Window.SCALEFACTOR_Y), tileSet.TileWidth, tileSet.TileHeight, false));
                        Tile.AddComponent(true, Game.physicSys.CreateRectangleColliderComponent(0, 0, 0, tileSet.TileWidth, tileSet.TileHeight));
                        Tile.AddComponent(true, Game.renderSys.CreateTextureComponent(tileSet.TileSheetTexture, tileSet.LoadedTiles[tileIndex].TileRect, 1));
                        GameObjects.Add(Tile);

                    } else if (tileMap[c,r] == 35)
                    {
                        GameObject Coin = Game.objectSys.CreateGameObject("Coin"+c+r, Tag.Coin, r*tileSet.TileWidth * Window.SCALEFACTOR_X, (c * tileSet.TileHeight +40) * Window.SCALEFACTOR_Y, 1,1, 0, true);
                        Coin.AddComponent(true, Game.physicSys.CreateBoundingBoxComponent((int)(Coin.Position.X-21*Window.SCALEFACTOR_X), (int)(Coin.Position.Y-21*Window.SCALEFACTOR_Y),42, 42, false));//(int)Coin.Position.X,(int)Coin.Position.Y,50,50, false));
                        Coin.AddComponent(true, Game.physicSys.CreateCircleColliderComponent(0, 0, 0, 20));
                        Coin.AddComponent(true, Game.renderSys.CreateSpriteComponent(Animation.CoinRotation, FilePath.Coin, 1, 6, 6, 3, 800));
                        Coin.AddComponent(true, Game.audioSys.CreateSoundFXComponent(FilePath.CoinPickup));
                        GameObjects.Add(Coin);
                    } else if (tileMap[c, r] == 36)
                    {
                        GameObject Flag = Game.objectSys.CreateGameObject("FlagPole", Tag.Flag, (r * tileSet.TileWidth + 30)* Window.SCALEFACTOR_X, (c * tileSet.TileHeight +50) * Window.SCALEFACTOR_Y, 1, 1, 0, true);
                        Flag.AddComponent(true, Game.physicSys.CreateBoundingBoxComponent((int)(Flag.Position.X-30*Window.SCALEFACTOR_X), (int)(Flag.Position.Y-30 * Window.SCALEFACTOR_Y), 60, 60, false));
                        Flag.AddComponent(true, Game.physicSys.CreateCircleColliderComponent(0, 0, 0, 40));
                        Flag.AddComponent(true, Game.renderSys.CreateSpriteComponent(Animation.FlagWaving, FilePath.WavingFlag, 1, 6, 6, 3, 1000));
                        GameObjects.Add(Flag);
                    } else if (tileMap[c, r] == 37)
                    {
                        GameObject Enemy = Game.objectSys.CreateGameObject("Enemy"+c+r,Tag.EnemyNormal, r * tileSet.TileWidth * Window.SCALEFACTOR_X, (c * tileSet.TileHeight + 40) * Window.SCALEFACTOR_Y, 1, 1, 0, true);
                        Enemy.AddComponent(true, Game.renderSys.CreateSpriteComponent(Animation.IdleRight, FilePath.Enemy, 6, 9, 51, 3, 10000));
                        Enemy.AddComponent(true, Game.physicSys.CreateBoundingBoxComponent((int)(Enemy.Position.X - 24 * Window.SCALEFACTOR_X), (int)(Enemy.Position.Y - 24 * Window.SCALEFACTOR_Y), 48, 48, false));
                        Enemy.AddComponent(true, Game.physicSys.CreateCircleColliderComponent(0, 0, 0, 24));
                        GameObjects.Add(Enemy);
                    }
                }
            }

            ObjectLoader gol = new ObjectLoader();
            GameObjects = gol.LoadGameObjectsFromFile(FilePath.level1_go, GameObjects);

            GameObject mainPlayer = Game.objectSys.CreateGameObject("mainPlayer", Tag.MainPlayer, 250 * Window.SCALEFACTOR_X, 400*Window.SCALEFACTOR_Y, 1, 1, 0, true);
            mainPlayer.AddComponent(true, Game.physicSys.CreateMoveComponent(0, 0, 0.075));           
            mainPlayer.AddComponent(true, Game.renderSys.CreateSpriteComponent(Animation.IdleRight, FilePath.PlayerIdle, 1, 4, 4, 1));
            mainPlayer.AddComponent(false, Game.renderSys.CreateSpriteComponent(Animation.CrouchRight, FilePath.PlayerCrouch, 1, 4, 4, 1));
            mainPlayer.AddComponent(false, Game.renderSys.CreateSpriteComponent(Animation.CrouchWalkRight, FilePath.PlayerCrouchWalk, 6, 1, 6, 1));
            mainPlayer.AddComponent(false, Game.renderSys.CreateSpriteComponent(Animation.CrouchWalkLeft, FilePath.PlayerCrouchWalk, 6, 1, 6, 1, rendererFlip: SDL_RendererFlip.SDL_FLIP_HORIZONTAL));
            mainPlayer.AddComponent(false, Game.renderSys.CreateSpriteComponent(Animation.WalkRight, FilePath.PlayerWalk, 6, 1, 6, 1));
            mainPlayer.AddComponent(false, Game.renderSys.CreateSpriteComponent(Animation.WalkLeft, FilePath.PlayerWalk, 6, 1, 6, 1, rendererFlip: SDL_RendererFlip.SDL_FLIP_HORIZONTAL));
            mainPlayer.AddComponent(false, Game.renderSys.CreateSpriteComponent(Animation.RunRight, FilePath.PlayerRun, 6, 1, 6, 1));
            mainPlayer.AddComponent(false, Game.renderSys.CreateSpriteComponent(Animation.RunLeft, FilePath.PlayerRun, 6, 1, 6, 1, rendererFlip: SDL_RendererFlip.SDL_FLIP_HORIZONTAL));
            mainPlayer.AddComponent(false, Game.renderSys.CreateSpriteComponent(Animation.SlideRight, FilePath.PlayerSlide, 1, 2, 2, 1));
            mainPlayer.AddComponent(false, Game.renderSys.CreateSpriteComponent(Animation.SlideLeft, FilePath.PlayerSlide, 1, 2, 2, 1, rendererFlip: SDL_RendererFlip.SDL_FLIP_HORIZONTAL));
            mainPlayer.AddComponent(false, Game.renderSys.CreateSpriteComponent(Animation.ShootBowRight, FilePath.PlayerShootBow, 1, 9, 9, 1, 600));
            mainPlayer.AddComponent(false, Game.renderSys.CreateSpriteComponent(Animation.ShootBowLeft, FilePath.PlayerShootBow, 1, 9, 9, 1, 600, rendererFlip: SDL_RendererFlip.SDL_FLIP_HORIZONTAL));
            mainPlayer.AddComponent(false, Game.renderSys.CreateSpriteComponent(Animation.JumpRight, FilePath.PlayerJump, 1, 4, 4, 1, 600));
            mainPlayer.AddComponent(false, Game.renderSys.CreateSpriteComponent(Animation.JumpLeft, FilePath.PlayerJump, 1, 4, 4, 1, 600, rendererFlip: SDL_RendererFlip.SDL_FLIP_HORIZONTAL));
            mainPlayer.AddComponent(false, Game.renderSys.CreateSpriteComponent(Animation.AirShootBowRight, FilePath.PlayerAirShootBow, 6, 1, 6, 1, 200));
            mainPlayer.AddComponent(false, Game.renderSys.CreateSpriteComponent(Animation.AirShootBowLeft, FilePath.PlayerAirShootBow, 6, 1, 6, 1, 200, rendererFlip: SDL_RendererFlip.SDL_FLIP_HORIZONTAL));      
            mainPlayer.AddComponent(false, Game.renderSys.CreateSpriteComponent(Animation.AirMeleeAttackStartRight, FilePath.PlayerAirMeleeAttackStart, 1, 1, 1, 1, 100));
            mainPlayer.AddComponent(false, Game.renderSys.CreateSpriteComponent(Animation.AirMeleeAttackStartLeft, FilePath.PlayerAirMeleeAttackStart, 1, 1, 1, 1, 100, rendererFlip: SDL_RendererFlip.SDL_FLIP_HORIZONTAL));
            mainPlayer.AddComponent(false, Game.renderSys.CreateSpriteComponent(Animation.AirMeleeAttackLoopRight, FilePath.PlayerAirMeleeAttackLoop, 1, 2, 2, 1, 300));
            mainPlayer.AddComponent(false, Game.renderSys.CreateSpriteComponent(Animation.AirMeleeAttackLoopLeft, FilePath.PlayerAirMeleeAttackLoop, 1, 2, 2, 1, 300, rendererFlip: SDL_RendererFlip.SDL_FLIP_HORIZONTAL));
            mainPlayer.AddComponent(false, Game.renderSys.CreateSpriteComponent(Animation.AirMeleeAttackEndRight, FilePath.PlayerAirMeleeAttackEnd, 3, 1, 3, 1, 200));
            mainPlayer.AddComponent(false, Game.renderSys.CreateSpriteComponent(Animation.AirMeleeAttackEndLeft, FilePath.PlayerAirMeleeAttackEnd, 3, 1, 3, 1, 200, rendererFlip: SDL_RendererFlip.SDL_FLIP_HORIZONTAL));
            mainPlayer.AddComponent(false, Game.renderSys.CreateSpriteComponent(Animation.AirFallRight, FilePath.PlayerAirFall, 1, 2, 2, 1, 100));
            mainPlayer.AddComponent(false, Game.renderSys.CreateSpriteComponent(Animation.AirFallLeft, FilePath.PlayerAirFall, 1, 2, 2, 1, 100, rendererFlip: SDL_RendererFlip.SDL_FLIP_HORIZONTAL));
            mainPlayer.AddComponent(false, Game.renderSys.CreateSpriteComponent(Animation.StandUpRight, FilePath.PlayerStandUp, 3, 1, 3, 1, 250));
            mainPlayer.AddComponent(false, Game.renderSys.CreateSpriteComponent(Animation.StandUpLeft, FilePath.PlayerStandUp, 3, 1, 3, 1, 250, rendererFlip: SDL_RendererFlip.SDL_FLIP_HORIZONTAL));
            mainPlayer.AddComponent(false, Game.renderSys.CreateSpriteComponent(Animation.DieRight, FilePath.PlayerDie, 1, 7, 7, 1));
            mainPlayer.AddComponent(false, Game.renderSys.CreateSpriteComponent(Animation.DieLeft, FilePath.PlayerDie, 1, 7, 7, 1, rendererFlip: SDL_RendererFlip.SDL_FLIP_HORIZONTAL));
            mainPlayer.AddComponent(true, Game.behaviorSys.CreateBehaviorComponent(new PlayerStateComponent()));
            mainPlayer.AddComponent(true, Game.behaviorSys.CreateBehaviorComponent(new PlayerManager()));
            mainPlayer.AddComponent(true, new ConstraintAreaComponent(0, 0, World.WORLD_WIDTH, World.WORLD_HEIGHT));
            mainPlayer.AddComponent(true, Game.physicSys.CreateBoundingBoxComponent((int)(21*Window.SCALEFACTOR_X), (int)(35 * Window.SCALEFACTOR_Y), (int)(21 * 2.1), (int)(60 * 2), true));// (int)mainPlayer.Position.X, (int)mainPlayer.Position.Y, (int)(20 * 2.1), (int)(33 * 2.1), false));
            mainPlayer.AddComponent(true, Game.physicSys.CreateCircleColliderComponent(0, 0, 1, (int)(20 * 1.7),new Vector2(-50,-110)));
            mainPlayer.AddComponent(true, Game.physicSys.CreatePhysicComponent(0, 0, 1));
            mainPlayer.AddComponent(true, Game.audioSys.CreateSoundFXComponent(FilePath.ArrowShoot));
            GameObjects.Add(mainPlayer);

            // new arrow gameobject
            GameObject arrowBullet = Game.objectSys.CreateGameObject("arrowBullet", Tag.PlayerBullet, -1000, -1000, 1, 1, 0, false);
            arrowBullet.AddComponent(true, Game.renderSys.CreateSpriteComponent(Animation.FlyingArrowRight, FilePath.ArrowRotation, 1, 4, 4, -1));
            arrowBullet.AddComponent(false, Game.renderSys.CreateSpriteComponent(Animation.FlyingArrowLeft, FilePath.ArrowRotation, 1, 4, 4, -1, rendererFlip: SDL_RendererFlip.SDL_FLIP_HORIZONTAL));
            arrowBullet.AddComponent(false, Game.renderSys.CreateSpriteComponent(Animation.FlyingArrowHit, FilePath.ArrowHit, 8, 8, 64, 4, 2000));
            arrowBullet.AddComponent(false, Game.renderSys.CreateSpriteComponent(Animation.FlyingArrowPuff, FilePath.ArrowPuff, 8, 8, 64, 2));
            arrowBullet.AddComponent(true, Game.physicSys.CreateMoveComponent(0, 0, 1));
            arrowBullet.AddComponent(true, Game.physicSys.CreateBoundingBoxComponent(0, 0, 20, 20, true));
            arrowBullet.AddComponent(true, Game.physicSys.CreateCircleColliderComponent(0,0,1,10));
            //arrowBullet.AddComponent(true, Game.behaviorSys.CreateBehaviorComponent(new BulletStateComponent(false)));
            //arrowBullet.AddComponent(true, Game.behaviorSys.CreateBehaviorComponent(new BulletStateComponent(shootLeft)));
            arrowBullet.AddComponent(true, Game.audioSys.CreateSoundFXComponent(FilePath.ArrowHitExplosion));
            GameObjects.Add(arrowBullet);

            GameObject explosion = Game.objectSys.CreateGameObject("explosion", Tag.GameBackground, -500, -500, 1, 1, 0, true);
            explosion.AddComponent(true, Game.renderSys.CreateSpriteComponent(Animation.None, FilePath.ArrowHit, 8, 8, 64, 4,2000));
            explosion.AddComponent(true, Game.behaviorSys.CreateBehaviorComponent(new ExplosionBehavior(1000)));

            GameObject screenFlash = Game.objectSys.CreateGameObject("screenFlash", Tag.GameBackground, -10000, -10000, 1, 1, 0, true);
            screenFlash.AddComponent(true, Game.renderSys.CreateSpriteComponent(Animation.None, FilePath.Flash, 1, 1, 1, 10));
            screenFlash.AddComponent(true, Game.behaviorSys.CreateBehaviorComponent(new ExplosionBehavior(100)));

            GameObject spawner = Game.objectSys.CreateGameObject("spawner", Tag.EnemySpawner, 0, 0, 1, 1, 0, true);
            spawner.AddComponent(true, Game.spawnSys.CreateSpawnComponent(Game.objectSys.FindGameObject("enemy")));
            spawner.AddComponent(true, Game.behaviorSys.CreateBehaviorComponent(new SpawnBehaviorComponent(explosion, screenFlash, mainPlayer)));

            for (int i = 0; i < 3; i++)
            {
                GameObject mapBackground = Game.objectSys.CreateGameObject("mapBackground" + i, Tag.GameBackground, 2048 * i * Window.SCALEFACTOR_X, 0, 1, 1, 0, true);
                mapBackground.AddComponent(true, Game.renderSys.CreateTextureComponent(FilePath.MapBackground,- 2));
                GameObjects.Add(mapBackground);
            }

            GameObject backgroundMusic = Game.objectSys.CreateGameObject("backgroundMusic", Tag.BackgroundMusic, -1000, -1000, 1, 1, 0, false);
            backgroundMusic.AddComponent(true, Game.audioSys.CreateMusicComponent(FilePath.GrasslandsTheme));
            GameObjects.Add(backgroundMusic);
            Game.audioSys.PlayMusic(backgroundMusic);
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

                if (InputHandler.D(e))
                {
                    Game.input = Input.D_Press;
                }
                else if (InputHandler.DRe(e))
                {
                    Game.input = Input.D_Release;
                }
                else if (InputHandler.A(e))
                {
                    Game.input = Input.A_Press;
                }
                else if (InputHandler.ARe(e))
                {
                    Game.input = Input.A_Release;
                }
                else if (InputHandler.S(e))
                {
                    Game.input = Input.S_Press;
                }
                else if (InputHandler.SRe(e))
                {
                    Game.input = Input.S_Release;
                }
                else if (InputHandler.Strg(e))
                {
                    Game.input = Input.Strg_Press;
                }
                else if (InputHandler.StrgRe(e))
                {
                    Game.input = Input.Strg_Release;
                }
                else if (InputHandler.Shift(e))
                {
                    Game.input = Input.Shift_Press;
                }
                else if (InputHandler.ShiftRe(e))
                {
                    Game.input = Input.Shift_Release;
                }
                else if (InputHandler.Space(e))
                {
                    Game.input = Input.Space_Press;
                }
                else if (InputHandler.SpaceRe(e))
                {
                    Game.input = Input.Space_Release;
                }
                else if (InputHandler.MouseButtonLeft(e))
                {
                    Game.input = Input.MouseButtonLeft_Press;
                }
                else if (InputHandler.MouseButtonLeftRe(e))
                {
                    Game.input = Input.MouseButtonLeft_Release;
                }
                else if (InputHandler.MouseButtonRight(e))
                {
                    Game.input = Input.MouseButtonRight_Press;
                }
                else if (InputHandler.MouseButtonRightRe(e))
                {
                    Game.input = Input.MouseButtonRight_Release;
                }
                else if(InputHandler.Escape(e))
                {
                    Game.gameStateSys.PushState(new MainMenue());
                }
                else if (InputHandler.R(e))
                {
                    GameObject mainPlayer = Game.objectSys.FindGameObject("mainPlayer");
                    if (mainPlayer != null)
                    {
                        PhysicComponent pc = mainPlayer.GetComponent<PhysicComponent>() as PhysicComponent;
                        MoveComponent mc = mainPlayer.GetComponent<MoveComponent>() as MoveComponent;
                        if (pc != null && mc != null)
                        {
                            mc.Velocity = Vector2.Zero;
                            pc.Force = Vector2.UnitY * 0.1D;
                        }
                        mainPlayer.Position = new Vector2(300 * Window.SCALEFACTOR_X, 800 * Window.SCALEFACTOR_Y);

                    }
                }
                //TODO Tab Button to open UpgradeMenue -> Game.gameStateSys.PushState(new UpgradeMenue());

                if (Window.isDebug)
                {
                    Console.WriteLine("GameInput: " + Game.input);
                    Console.WriteLine("------------------------");
                }
            }
            /*
            // Handle key states
            //InputHandler.UpdateKeyboardState();
            if (InputHandler.A())
            {
                Game.input = Input.RunLeftPressed;
            }
            else if (InputHandler.ARe())
            {
                Game.input = Input.RunLeftReleased;
            }
            else if (InputHandler.D())
            {
                Game.input = Input.RunRightPressed;
            }
            else if (InputHandler.DRe())
            {
                Game.input = Input.RunRightReleased;
            }
            else if (InputHandler.S())
            {
                Game.input = Input.CrouchPressed;
            }
            else if (InputHandler.SRe())
            {
                Game.input = Input.CrouchReleased;
            }
            else if (InputHandler.Space())
            {
                Game.input = Input.JumpPressed;
            }
            else if (InputHandler.SpaceRe())
            {
                Game.input = Input.JumpReleased;
            }
            /*
            //else if (InputHandler.MouseButtonLeft(e))
            //{
            //    Game.input = Input.AttackPressed;
            //}
            */
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

        public int[,] loadMapFromFile()
        {
            string filePath = FilePath.mapLevel1;

            string[] lines = File.ReadAllLines(filePath);

            string[] sizes = lines[0].Split(';');
            int x = Convert.ToInt32(sizes[0]);
            int y = Convert.ToInt32(sizes[1]);

            int[,] loadedMap = new int[y, x];

            for (int i = 0; i < y; i++)
            {
                string[] entries = lines[i + 1].Split(';');

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
            return loadedMap;
        }
    }
}
