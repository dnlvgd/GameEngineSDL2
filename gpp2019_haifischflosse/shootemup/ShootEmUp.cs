using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;
using static SDL2.SDL_mixer;
using static SDL2.SDL_image;
using static SDL2.SDL_ttf;
using System.IO;
using System.Globalization;

namespace gpp2019_haifischflosse
{
    public class ShootEmUp
    {
        public SDL_Event events;
        public static bool godmode;

        private RenderSystem renderSys = new RenderSystem();
        private PhysicsSystem physicSys = new PhysicsSystem();
        private UISystem uiSys = new UISystem();
        private SpawnSystem spawnSys = new SpawnSystem();
        private WeaponSystem weaponSys = new WeaponSystem();
        private BehaviorSystem behaviorSys = new BehaviorSystem();
        //private EventSystem evSys = new EventSystem()

        private AudioSystem audioSys = new AudioSystem();

        private GameObjectSystem objectSys = new GameObjectSystem();

        //main folder path
        private const string assets = "assets/";
        //sub folders
        private const string audios = assets + "audio/";
        private const string sprites = assets + "sprites/";
        private const string textures = assets + "texture/";
        //sub sub folders
        private const string buttonPath = sprites + "button/";
        private const string colliderPath = sprites + "collider/";
        private const string enemyPath = sprites + "enemy/";
        private const string heartPath = sprites + "heart/";
        private const string laserPath = sprites + "laser/";
        private const string playerPath = sprites + "player/";
        private const string backgroundPath = textures + "backgrounds/";
        private const string explosions = sprites + "explosions/";

        private bool backToMainLoop = false;

        //The surface contained by the window
        public static IntPtr surface;
        

        public static void Main(string[] args)
        {
            Window.InitWindow("ShootEmUp", 960, 540, SDL_WindowFlags.SDL_WINDOW_SHOWN);
            ShootEmUp game = new ShootEmUp();
            game.GameLoop();
        }

        public ShootEmUp()
        {
            InitGame();
        }

        private void InitGame()
        {
            //Init World
            World world = new World(3000, 3000);
            BuildGameObjects();
        }

        public void GameLoop()
        {
            audioSys.PlayMusic(objectSys.FindGameObject("music"));
            while (true)
            {
                if (backToMainLoop == true)
                {
                    break;
                }

                TimeInfo.Begin();
                ProcessInput();
                UpdateSimulation();
                RenderFrame();
                //Console.WriteLine("Camera: x= " + Camera.camera.x + " y= " + Camera.camera.y + " w= " + Camera.camera.w + " h= " + Camera.camera.h);

                //CleanGameObjectList();
                //TimeInfo.Sleep(16.6);
                TimeInfo.End();
                FrameInfo.CalculateFrameInfo(
                    TimeInfo.Time,
                    TimeInfo.DeltaTime
                );
            }
            CleanUp();
            //CleanUpInitializedSubsystems();
        }

        public GameObject LoadGameObjectsFromFile()
        {
            string filePath = "C://Users/Lennart/Documents/GitHub/gpp2019_SDL/gpp2019_haifischflosse/shootemup/savefiles/GameObjects.txt";//C:\savefiles\GameObjects.txt

            //Lennart-PC:
            //  C://Users/Lennart/Documents/GitHub/gpp2019_SDL/gpp2019_haifischflosse/shootemup/savefiles/GameObjects.txt
            //Niklas-PC:
            //  C://Users/Verwey/Documents/gpp2019_haifischflosse/gpp2019_haifischflosse/shootemup/savefiles/GameObjects.txt

            GameObject lastGm = null;
            Dictionary<string, GameObject> gameObjs = new Dictionary<string, GameObject>();

            List<string> lines = File.ReadAllLines(filePath).ToList();

            foreach (var line in lines)
            {
                string[] entries = line.Split(',');
                if (entries[0].Equals("G"))
                {
                    //interprete Line as Add GameObject
                    if(entries.Length != 9)
                    {
                        Console.WriteLine("Error: Wrong or Incomplete Data in LoadGameObejctsFromFile. Check Line: " + line);
                        continue;
                    }
                    try
                    {
                        string name = entries[1];
                        Tag tag = (Tag)Enum.Parse(typeof(Tag), entries[2]);
                        double xPos = Convert.ToDouble(entries[3]);
                        double yPos = Convert.ToDouble(entries[4]);
                        double xScale = Convert.ToDouble(entries[5]);
                        double yScale = Convert.ToDouble(entries[6]);
                        double rotation = Convert.ToDouble(entries[7]);
                        bool active;
                        if (entries[8].Equals("true")) active = true;
                        else active = false;

                        lastGm = objectSys.CreateGameObject(name, tag, xPos, yPos, xScale, yScale, rotation, active);
                        gameObjs.Add(name, lastGm);

                    } catch (FormatException e)
                    {
                        Console.WriteLine("Error: Wrong or Incomplete Data in LoadGameObejctsFromFile. Check Line: " + line);
                        Console.WriteLine(e.Message);
                    } catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                  

                }
                else if (entries[0].Equals("C"))
                {
                    //interprete Line as Add Component
                    switch (entries[1])
                    {
                        case "moveC":
                            if(entries.Length!= 5)
                            {
                                Console.WriteLine("Invalid Component Data in line: "+ line);
                            }
                            try
                            {
                                double velX = double.Parse(entries[2], CultureInfo.InvariantCulture);
                                double velY = double.Parse(entries[3], CultureInfo.InvariantCulture);
                                double factorM = double.Parse(entries[4],CultureInfo.InvariantCulture);

                                lastGm.AddComponent(true, physicSys.CreateMoveComponent(velX, velY, factorM));

                            } catch (FormatException e)
                            {
                                Console.WriteLine("Invalid Format in line: "+ line);
                                Console.WriteLine(e.Message);
                            } catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            break;

                        case "spriteC":
                            if(entries.Length!= 8)
                            {
                                Console.WriteLine("Invalid Component Data in line: " + line);
                                continue;
                            }
                            try
                            {
                                Animation ani = (Animation)Enum.Parse(typeof(Animation), entries[2]);
                                string path = entries[3];
                                uint row = Convert.ToUInt32(entries[4]);
                                uint column = Convert.ToUInt32(entries[5]);
                                uint total = Convert.ToUInt32(entries[6]);
                                int z = Convert.ToInt32(entries[7]);

                                lastGm.AddComponent(true, renderSys.CreateSpriteComponent(ani,path,row,column,total,z));
                            }
                            catch (FormatException e)
                            {
                                Console.WriteLine("Invalid Format in line: " + line);
                                Console.WriteLine(e.Message);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }

                            break;

                        case "constraintAreaC": 
                            if(entries.Length != 6)
                            {
                                Console.WriteLine("Invalid Component Data in line: " + line);
                                continue;
                            }
                            try
                            {
                                int minx = Convert.ToInt32(entries[2]);
                                int miny = Convert.ToInt32(entries[3]);
                                int maxx = Convert.ToInt32(entries[4]);
                                int maxy = Convert.ToInt32(entries[5]);

                                lastGm.AddComponent(true, new ConstraintAreaComponent(minx, miny, maxx, maxy));
                            }
                            catch (FormatException e)
                            {
                                Console.WriteLine("Invalid Format in line: " + line);
                                Console.WriteLine(e.Message);
                            }
                            
                            break;

                        case "mouseC":
                            lastGm.AddComponent(true, new MouseComponent());

                            break;

                        case "boundingBoxC":
                            SpriteComponent sprite = lastGm.GetComponent<SpriteComponent>() as SpriteComponent;
                            if(sprite != null)
                            {
                                lastGm.AddComponent(true, physicSys.CreateBoundingBoxComponent((int)lastGm.Position.X, (int)lastGm.Position.Y, sprite.DstRect.w, sprite.DstRect.h, true));
                            }
                            else
                            {
                                Console.WriteLine("You need a SpriteComponent to add bbc at line: " + line);
                            }
                            break;

                        case "soundFXC":
                            string audiopath = entries[2];
                            lastGm.AddComponent(true, audioSys.CreateSoundFXComponent(audiopath));
                            break;

                        case "circleColliderC":
                            if(entries.Length != 6)
                            {
                                Console.WriteLine("Invalid Component Data in line: " + line);
                                continue;
                            }
                            try
                            {
                                double forcex = double.Parse(entries[2], CultureInfo.InvariantCulture);
                                double forcey = double.Parse(entries[3], CultureInfo.InvariantCulture);
                                double mass = double.Parse(entries[4], CultureInfo.InvariantCulture);
                                int radius = Convert.ToInt32(entries[5]);

                                lastGm.AddComponent(true, physicSys.CreateCircleColliderComponent(forcex, forcey, mass, radius));
                            }
                            catch (FormatException e)
                            {
                                Console.WriteLine("Invalid Format in line: " + line);
                                Console.WriteLine(e.Message);
                            }
                            break;

                        case "physicsC":
                            if (entries.Length != 5)
                            {
                                Console.WriteLine("Invalid Component Data in line: " + line);
                                continue;
                            }
                            try
                            {
                                double forcex = double.Parse(entries[2], CultureInfo.InvariantCulture);
                                double forcey = double.Parse(entries[3], CultureInfo.InvariantCulture);
                                double mass = double.Parse(entries[4], CultureInfo.InvariantCulture);

                                lastGm.AddComponent(true, physicSys.CreatePhysicComponent(forcex, forcey, mass));
                            }
                            catch (FormatException e)
                            {
                                Console.WriteLine("Invalid Format in line: " + line);
                                Console.WriteLine(e.Message);
                            }
                            break;

                        case "playerBehaviorC":
                            lastGm.AddComponent(true, behaviorSys.CreateBehaviorComponent(new PlayerBehaviorComponent()));
                            break;

                        case "playerHealthC":
                            if (entries.Length != 8)
                            {
                                Console.WriteLine("Invalid Component Data in line: " + line);
                                continue;
                            }
                            GameObject oneF = gameObjs[entries[2]];
                            GameObject oneE = gameObjs[entries[3]];
                            GameObject twoF = gameObjs[entries[4]];
                            GameObject twoE = gameObjs[entries[5]];
                            GameObject threeF = gameObjs[entries[6]];
                            GameObject threeE = gameObjs[entries[7]];

                            if(oneE != null && oneF != null && twoE != null && twoF != null && threeE != null && threeF != null)
                            {
                                lastGm.AddComponent(true, behaviorSys.CreateBehaviorComponent(new PlayerHealthBehaviorComponent(oneF, oneE, twoF, twoE, threeF, threeE)));
                            } else
                            {
                                Console.WriteLine("Initzialize HearthUI Components bevore hearstbehavior! line: " + line);
                            }
                            break;

                        case "enemyC":
                            if (entries.Length != 5)
                            {
                                Console.WriteLine("Invalid Component Data in line: " + line);
                                continue;
                            }
                            try
                            {
                                GameObject target = gameObjs[entries[2]];
                                double turnspeed = double.Parse(entries[3], CultureInfo.InvariantCulture);
                                double shootfrequenzy = double.Parse(entries[4], CultureInfo.InvariantCulture);

                                lastGm.AddComponent(true, behaviorSys.CreateBehaviorComponent(new EnemyBehaviorComponent(target, turnspeed, shootfrequenzy)));
                            }
                            catch (FormatException e)
                            {
                                Console.WriteLine("Invalid Format in line: " + line);
                                Console.WriteLine(e.Message);
                            }
                            break;

                        case "enemySpeederC":
                            if (entries.Length != 4)
                            {
                                Console.WriteLine("Invalid Component Data in line: " + line);
                                continue;
                            }
                            try
                            {
                                GameObject target = gameObjs[entries[2]];
                                double turnspeed = double.Parse(entries[3], CultureInfo.InvariantCulture);

                                lastGm.AddComponent(true, behaviorSys.CreateBehaviorComponent(new EnemySpeederBehaviorComponent(target, turnspeed)));
                            }
                            catch (FormatException e)
                            {
                                Console.WriteLine("Invalid Format in line: " + line);
                                Console.WriteLine(e.Message);
                            }
                            break;

                        case "weaponC":
                            if (entries.Length != 4)
                            {
                                Console.WriteLine("Invalid Component Data in line: " + line);
                                continue;
                            }
                            try
                            {
                                GameObject target = gameObjs[entries[2]];
                                GameObject weapon = gameObjs[entries[3]];

                                if(target != null && weapon != null)
                                {
                                    target.AddComponent(true, weaponSys.CreateWeaponComponent(weapon));
                                } else
                                {
                                    Console.WriteLine("You need to declare the weapon owner and the weapon first. line: "+line);
                                }
                            }
                            catch (FormatException e)
                            {
                                Console.WriteLine("Invalid Format in line: " + line);
                                Console.WriteLine(e.Message);
                            }
                            break;

                        case "homingMissileC":
                            if (entries.Length != 5)
                            {
                                Console.WriteLine("Invalid Component Data in line: " + line);
                                continue;
                            }
                            try
                            {
                                GameObject target = gameObjs[entries[2]];
                                double turnspeed = double.Parse(entries[3], CultureInfo.InvariantCulture);
                                double lifetime = double.Parse(entries[4], CultureInfo.InvariantCulture);

                                lastGm.AddComponent(true, behaviorSys.CreateBehaviorComponent(new HomingMissileComponent(target,turnspeed,lifetime)));
                            }
                            catch (FormatException e)
                            {
                                Console.WriteLine("Invalid Format in line: " + line);
                                Console.WriteLine(e.Message);
                            }
                            break;

                        case "bossC":
                            if (entries.Length != 8)
                            {
                                Console.WriteLine("Invalid Component Data in line: " + line);
                                continue;
                            }
                            try
                            {
                                GameObject target = gameObjs[entries[2]];
                                GameObject laser = gameObjs[entries[3]];
                                GameObject missile = gameObjs[entries[4]];
                                int hp = Convert.ToInt32(entries[5]);
                                double shootfrequenzy = double.Parse(entries[6], CultureInfo.InvariantCulture);
                                double turnspeed = double.Parse(entries[7], CultureInfo.InvariantCulture);

                                if (target != null && laser != null && missile != null)
                                {
                                    lastGm.AddComponent(true, behaviorSys.CreateBehaviorComponent(new BossBehaviorComponent(target, laser, missile, hp, shootfrequenzy, turnspeed)));
                                } else
                                {
                                    Console.WriteLine("declare player and bossweappons before boss. line: " +line);
                                }
                            }
                            catch (FormatException e)
                            {
                                Console.WriteLine("Invalid Format in line: " + line);
                                Console.WriteLine(e.Message);
                            }
                            break;

                        case "textureC":
                            if (entries.Length != 4 && entries.Length != 6)
                            {
                                Console.WriteLine("Invalid Component Data in line: " + line);
                                continue;
                            }
                            try
                            {
                                string texturepath = entries[2];
                                int z = Convert.ToInt32(entries[3]);
                                if(entries.Length == 4)
                                {
                                    lastGm.AddComponent(true, renderSys.CreateTextureComponent(texturepath,z));
                                } else
                                {
                                    int w = Convert.ToInt32(entries[4]);
                                    int h = Convert.ToInt32(entries[5]);

                                    lastGm.AddComponent(true, renderSys.CreateTextureComponent(texturepath, z, w, h));
                                }
                                
                            }
                            catch (FormatException e)
                            {
                                Console.WriteLine("Invalid Format in line: " + line);
                                Console.WriteLine(e.Message);
                            }
                            break;

                        case "musicC":
                            if (entries.Length != 3)
                            {
                                Console.WriteLine("Invalid Component Data in line: " + line);
                                continue;
                            }
                            string musicpath = entries[2];
                            lastGm.AddComponent(true, audioSys.CreateMusicComponent(musicpath));

                            break;

                        case "textC":
                            if (entries.Length != 8)
                            {
                                Console.WriteLine("Invalid Component Data in line: " + line);
                                continue;
                            }
                            try
                            {
                                string text = entries[2];
                                int fontsize = Convert.ToInt32(entries[3]);
                                byte r = Convert.ToByte(entries[4]);
                                byte g = Convert.ToByte(entries[5]);
                                byte b = Convert.ToByte(entries[6]);
                                byte alpha = Convert.ToByte(entries[7]);

                                lastGm.AddComponent(true, uiSys.CreateTextComponent(text, fontsize, r, g, b, alpha));

                            }
                            catch (FormatException e)
                            {
                                Console.WriteLine("Invalid Format in line: " + line);
                                Console.WriteLine(e.Message);
                            }
                            break;

                        case "fpsDisplayC":
                            
                            lastGm.AddComponent(true, behaviorSys.CreateBehaviorComponent(new FpsDisplayerComponent(FrameInfo.GetFpsText())));
                            break;

                        case "explosionC":
                            if (entries.Length != 3)
                            {
                                Console.WriteLine("Invalid Component Data in line: " + line);
                                continue;
                            }
                            try
                            {
                                int lifetime = Convert.ToInt32(entries[2]);

                                lastGm.AddComponent(true, behaviorSys.CreateBehaviorComponent(new ExplosionBehavior(lifetime)));

                            }
                            catch (FormatException e)
                            {
                                Console.WriteLine("Invalid Format in line: " + line);
                                Console.WriteLine(e.Message);
                            }
                            break;

                        case "healthDropC":
                            lastGm.AddComponent(true, behaviorSys.CreateBehaviorComponent(new HealthDropBehavior()));
                            break;

                        case "spawnerC":
                            if (entries.Length < 9)
                            {
                                Console.WriteLine("Invalid Component Data in line: " + line);
                                continue;
                            }
                            try
                            {
                                GameObject boss = gameObjs[entries[2]];
                                int enemyCount = Convert.ToInt32(entries[3]);
                                GameObject[] enemies = new GameObject[enemyCount];
                                int index = 4;
                                for (int i = 0; i < enemyCount; i++)
                                {
                                    GameObject enemy = gameObjs[entries[4 + i]];
                                    enemies[i] = enemy;
                                    index++;
                                }
                                GameObject explosion = gameObjs[entries[index]];
                                GameObject healthDrop = gameObjs[entries[index + 1]];
                                GameObject screenflash = gameObjs[entries[index + 2]];
                                GameObject player = gameObjs[entries[index + 3]];;

                                if (boss != null && explosion != null && healthDrop != null && screenflash != null && player != null)
                                {
                                    lastGm.AddComponent(true, behaviorSys.CreateBehaviorComponent(new SpawnBehaviorComponent(boss, enemies, explosion, healthDrop, screenflash, player)));
                                }
                                else
                                {
                                    Console.WriteLine("declare boss, enemies,explosion,healthdrop,screenflash before spawner. line: " + line);
                                    Console.WriteLine("Format should be C,spawnerC,boss,2,enemy,enemySpeeder,explosion,healthDrop,screenflash,player");
                                }
                            }
                            catch (FormatException e)
                            {
                                Console.WriteLine("Invalid Format in line: " + line);
                                Console.WriteLine(e.Message);
                            }
                            break;

                        case "waveLevelC":
                            lastGm.AddComponent(true, behaviorSys.CreateBehaviorComponent(new WaveLevelComponent()));
                            break;

                        case "playerScoreC":
                            lastGm.AddComponent(true, behaviorSys.CreateBehaviorComponent(new PlayerScoreComponent()));
                            break;

                        case "backgroundC":
                            lastGm.AddComponent(true, behaviorSys.CreateBehaviorComponent(new BackgroundBehaviourComponent(lastGm)));
                            break;

                        default:
                            Console.WriteLine("Invalid Component Identifier in line: "+line);
                            break;

                    }
                }
            }

            return gameObjs["player"];
        }

        public void BuildGameObjects()
        {
            //Init of GameObjects
            bool isActive = true;
            /*
            //player
            GameObject player = objectSys.CreateGameObject("player", Tag.MainPlayer, World.WORLD_WIDTH/2, World.WORLD_HEIGHT / 2, 1, 1, 0, true);
            player.AddComponent(isActive, physicSys.CreateMoveComponent(0,0, 0.5));
            player.AddComponent(isActive, renderSys.CreateSpriteComponent(Animation.None, playerPath + "playerShip1_red.png", 1, 1, 1, 3));
            player.AddComponent(isActive, new ConstraintAreaComponent(0, 0, World.WORLD_WIDTH, World.WORLD_HEIGHT));
            player.AddComponent(isActive, new MouseComponent());
            SpriteComponent playerSC = player.GetComponent<SpriteComponent>() as SpriteComponent;
            if (playerSC != null)
            {
                player.AddComponent(isActive, physicSys.CreateBoundingBoxComponent((int)player.Position.X, (int)player.Position.Y, playerSC.DstRect.w, playerSC.DstRect.h, true));
            }
            player.AddComponent(isActive, audioSys.CreateSoundFXComponent(audios + "sfx_laser1.ogg"));
            player.AddComponent(isActive, physicSys.CreateCircleColliderComponent(0, 0, 1, 25));
            player.AddComponent(isActive, physicSys.CreatePhysicComponent(0, 0, 1));
            player.AddComponent(isActive, behaviorSys.CreateBehaviorComponent(new PlayerBehaviorComponent()));
            
            */
                GameObject player = LoadGameObjectsFromFile();
            // --- Player's HeartUI --- start ---
            /*
            GameObject playerHeartOneFull = objectSys.CreateGameObject("playerHeartOneFull", Tag.PlayerHeart, Window.CURRENT_SCREEN_WIDTH / 2 - 40, 10, 1, 1, 0, true);
            playerHeartOneFull.AddComponent(isActive, renderSys.CreateSpriteComponent(Animation.None, heartPath + "heart_full.png", 1, 1, 1, 3));

            GameObject playerHeartOneEmpty = objectSys.CreateGameObject("playerHeartOneEmpty", Tag.PlayerHeart, Window.CURRENT_SCREEN_WIDTH / 2 - 40, 10, 1, 1, 0, true);
            playerHeartOneEmpty.AddComponent(isActive, renderSys.CreateSpriteComponent(Animation.None, heartPath + "heart_empty.png", 1, 1, 1, 2));

            GameObject playerHeartTwoFull = objectSys.CreateGameObject("playerHeartTwoFull", Tag.PlayerHeart, Window.CURRENT_SCREEN_WIDTH / 2, 10, 1, 1, 0, true);
            playerHeartTwoFull.AddComponent(isActive, renderSys.CreateSpriteComponent(Animation.None, heartPath + "heart_full.png", 1, 1, 1, 3));

            GameObject playerHeartTwoEmpty = objectSys.CreateGameObject("playerHeartTwoEmpty", Tag.PlayerHeart, Window.CURRENT_SCREEN_WIDTH / 2, 10, 1, 1, 0, true);
            playerHeartTwoEmpty.AddComponent(isActive, renderSys.CreateSpriteComponent(Animation.None, heartPath + "heart_empty.png", 1, 1, 1, 2));

            GameObject playerHeartThreeFull = objectSys.CreateGameObject("playerHeartThreeFull", Tag.PlayerHeart, Window.CURRENT_SCREEN_WIDTH / 2 + 40, 10, 1, 1, 0, true);
            playerHeartThreeFull.AddComponent(isActive, renderSys.CreateSpriteComponent(Animation.None, heartPath + "heart_full.png", 1, 1, 1, 3));

            GameObject playerHeartThreeEmpty = objectSys.CreateGameObject("playerHeartThreeEmpty", Tag.PlayerHeart, Window.CURRENT_SCREEN_WIDTH / 2 + 40, 10, 1, 1, 0, true);
            playerHeartThreeEmpty.AddComponent(isActive, renderSys.CreateSpriteComponent(Animation.None, heartPath + "heart_empty.png", 1, 1, 1, 2));

            GameObject heartsUI = objectSys.CreateGameObject("heartsUI", Tag.PlayerHeartIndicator, Window.CURRENT_SCREEN_WIDTH / 2, 30, 1, 1, 0, true);
            heartsUI.AddComponent(isActive, audioSys.CreateSoundFXComponent(audios + "sfx_lose.ogg"));
            heartsUI.AddComponent(isActive, behaviorSys.CreateBehaviorComponent(new PlayerHealthBehaviorComponent(playerHeartOneFull, playerHeartOneEmpty, playerHeartTwoFull, playerHeartTwoEmpty, playerHeartThreeFull, playerHeartThreeEmpty)));
            // --- Player's HeartUI --- end ---
            */
            //enemy normal
            GameObject enemy = objectSys.CreateGameObject("enemy", Tag.EnemyNormal, 300, Window.CURRENT_SCREEN_HEIGHT / 2, 1, 1, 0, true);
            enemy.AddComponent(isActive, renderSys.CreateSpriteComponent(Animation.None, enemyPath + "enemyBlack1.png", 1, 1, 1, 1));
            SpriteComponent enemySC = enemy.GetComponent<SpriteComponent>() as SpriteComponent;
            if (enemySC != null)
            {
                enemy.AddComponent(isActive, physicSys.CreateBoundingBoxComponent((int)enemy.Position.X, (int)enemy.Position.Y, enemySC.DstRect.w, enemySC.DstRect.h, true));
            }
            enemy.AddComponent(isActive, physicSys.CreateMoveComponent(0, 0, 0.35));
            enemy.AddComponent(isActive, physicSys.CreateCircleColliderComponent(0, 0, 1, 35));
            enemy.AddComponent(isActive, physicSys.CreatePhysicComponent(0, 0, 1));
            enemy.AddComponent(isActive, audioSys.CreateSoundFXComponent(audios + "sfx_laser2.ogg"));
            enemy.AddComponent(isActive, behaviorSys.CreateBehaviorComponent(new EnemyBehaviorComponent(player, 0.1, 4000)));

            //enemy speeder
            GameObject enemySpeeder = objectSys.CreateGameObject("enemySpeeder", Tag.EnemyNormal, -1000, -1000, 1, 1, 0, true);
            enemySpeeder.AddComponent(isActive, renderSys.CreateSpriteComponent(Animation.None, enemyPath + "enemyBlack3.png", 1, 1, 1, 1));
            SpriteComponent enemySpeederSC = enemySpeeder.GetComponent<SpriteComponent>() as SpriteComponent;
            if (enemySpeederSC != null)
            {
                enemySpeeder.AddComponent(isActive, physicSys.CreateBoundingBoxComponent((int)enemySpeeder.Position.X, (int)enemySpeeder.Position.Y, enemySpeederSC.DstRect.w, enemySpeederSC.DstRect.h, true));
            }
            enemySpeeder.AddComponent(isActive, physicSys.CreateMoveComponent(0, 0, 0.5));
            enemySpeeder.AddComponent(isActive, physicSys.CreateCircleColliderComponent(0, 0, 1, 35));
            enemySpeeder.AddComponent(isActive, physicSys.CreatePhysicComponent(0, 0, 1));
            enemySpeeder.AddComponent(isActive, audioSys.CreateSoundFXComponent(audios + "sfx_laser2.ogg"));
            enemySpeeder.AddComponent(isActive, behaviorSys.CreateBehaviorComponent(new EnemySpeederBehaviorComponent(player, 0.15)));

            //player bullet

            GameObject playerBullet = objectSys.CreateGameObject("playerBullet", Tag.PlayerBullet, -100, -100, 1, 1, 0, true);
            playerBullet.AddComponent(isActive, renderSys.CreateSpriteComponent(Animation.None, laserPath + "laserBlue02.png", 1, 1, 1, 0));
            playerBullet.AddComponent(isActive, physicSys.CreateMoveComponent(0,0, 1.2));
            SpriteComponent playerBulletSC = playerBullet.GetComponent<SpriteComponent>() as SpriteComponent;
            if (playerBulletSC != null)
            {
                playerBullet.AddComponent(isActive, physicSys.CreateBoundingBoxComponent((int)playerBullet.Position.X, (int)playerBullet.Position.Y, playerBulletSC.DstRect.w, playerBulletSC.DstRect.h, true));
            }
            playerBullet.AddComponent(isActive, physicSys.CreatePhysicComponent(0, 0, 1));
            playerBullet.AddComponent(isActive, physicSys.CreateCircleColliderComponent(0, 0, 1, 25));
            playerBullet.AddComponent(isActive, audioSys.CreateSoundFXComponent(audios + "impact.wav"));
            player.AddComponent(isActive, weaponSys.CreateWeaponComponent(playerBullet));
            
            //enemy bullset

            GameObject enemyBullet = objectSys.CreateGameObject("enemyBullet", Tag.EnemyNormalBullet, -100, -100, 1, 1, 0, true);
            enemyBullet.AddComponent(isActive, renderSys.CreateSpriteComponent(Animation.None, laserPath + "laserRed02.png", 1, 1, 1, 0));
            enemyBullet.AddComponent(isActive, physicSys.CreateMoveComponent(0, 0, 0.9));
            SpriteComponent enemyBulletSC = enemyBullet.GetComponent<SpriteComponent>() as SpriteComponent;
            if (enemyBulletSC != null)
            {
                enemyBullet.AddComponent(isActive, physicSys.CreateBoundingBoxComponent((int)enemyBullet.Position.X, (int)enemyBullet.Position.Y, enemyBulletSC.DstRect.w, enemyBulletSC.DstRect.h, true));
            }
            enemyBullet.AddComponent(isActive, physicSys.CreatePhysicComponent(0, 0, 0.4));
            enemyBullet.AddComponent(isActive, physicSys.CreateCircleColliderComponent(0, 0, 1, 15));
            enemyBullet.AddComponent(isActive, audioSys.CreateSoundFXComponent(audios + "impact.wav"));
            enemy.AddComponent(isActive, weaponSys.CreateWeaponComponent(enemyBullet));

            //homing missile

            GameObject homingMissile = objectSys.CreateGameObject("homingMissile", Tag.EnemyNormalBullet, -1000, -1000, 1, 1, 0, true);
            homingMissile.AddComponent(isActive, renderSys.CreateSpriteComponent(Animation.None, laserPath + "Spacemines_Red_SpriteSheet.png", 1, 2, 2, 0));
            homingMissile.AddComponent(isActive, physicSys.CreateMoveComponent(0, 0, 0.6));
            SpriteComponent homingMissileSC = homingMissile.GetComponent<SpriteComponent>() as SpriteComponent;
            if (homingMissileSC != null)
            {
                homingMissile.AddComponent(isActive, physicSys.CreateBoundingBoxComponent((int)homingMissile.Position.X, (int)homingMissile.Position.Y, homingMissileSC.DstRect.w, homingMissileSC.DstRect.h, true));
            }
            homingMissile.AddComponent(isActive, physicSys.CreatePhysicComponent(0, 0, 1));
            homingMissile.AddComponent(isActive, physicSys.CreateCircleColliderComponent(0, 0, 1, 25));
            homingMissile.AddComponent(isActive, audioSys.CreateSoundFXComponent(audios + "impact.wav"));
            homingMissile.AddComponent(isActive, behaviorSys.CreateBehaviorComponent(new HomingMissileComponent(player, 0.07,5000)));

            //Boss
            GameObject boss = objectSys.CreateGameObject("boss", Tag.EnemyBoss, -2000, -2000, 1, 1, 0, true);
            boss.AddComponent(isActive, renderSys.CreateSpriteComponent(Animation.None, enemyPath + "Boss.png", 1, 1, 1, 1));
            SpriteComponent bossSC = boss.GetComponent<SpriteComponent>() as SpriteComponent;
            if (enemySC != null)
            {
                boss.AddComponent(isActive, physicSys.CreateBoundingBoxComponent((int)boss.Position.X, (int)boss.Position.Y, bossSC.DstRect.w, bossSC.DstRect.h, true));
            }
            boss.AddComponent(isActive, physicSys.CreateMoveComponent(0.1, 0.1, 0.3));
            boss.AddComponent(isActive, physicSys.CreateCircleColliderComponent(0, 0, 1, 150));
            boss.AddComponent(isActive, physicSys.CreatePhysicComponent(0, 0, 1));
            boss.AddComponent(isActive, audioSys.CreateSoundFXComponent(audios + "sfx_laser2.ogg"));
            boss.AddComponent(isActive, behaviorSys.CreateBehaviorComponent(new BossBehaviorComponent(player, enemyBullet, homingMissile, 10, 2000, 0.03)));

            //background
            GameObject background = objectSys.CreateGameObject("background", Tag.GameBackground, 0, 0, 1, 1, 0, true);
            background.AddComponent(isActive, renderSys.CreateTextureComponent(backgroundPath + "stars.png", 1));

            GameObject music = objectSys.CreateGameObject("music", Tag.BackgroundMusic, 0, 0, 1, 1, 0, true);
            music.AddComponent(isActive, audioSys.CreateMusicComponent(audios + "music.mp3"));

            GameObject fpsText = objectSys.CreateGameObject("fpsText", Tag.FpsIndicator, 0, 0, 1, 1, 0, true);
            fpsText.AddComponent(isActive, uiSys.CreateTextComponent("fps", 24, 100, 200, 240, 255));
            fpsText.AddComponent(isActive, behaviorSys.CreateBehaviorComponent(new FpsDisplayerComponent(FrameInfo.GetFpsText())));

            GameObject explosion = objectSys.CreateGameObject("explosion", Tag.GameBackground, -1000, -1000, 1, 1, 0, true);
            explosion.AddComponent(isActive, renderSys.CreateSpriteComponent(Animation.None, explosions + "explosionsheet.png", 9, 9, 74, 2));
            explosion.AddComponent(isActive, behaviorSys.CreateBehaviorComponent(new ExplosionBehavior(400)));

            GameObject healthDrop = objectSys.CreateGameObject("healthDrop", Tag.PickUp, -1200, -1200, 1, 1, 0, true);
            healthDrop.AddComponent(isActive, renderSys.CreateSpriteComponent(Animation.None, heartPath + "heart_full.png", 1, 1, 1, 4));
            healthDrop.AddComponent(isActive, behaviorSys.CreateBehaviorComponent(new HealthDropBehavior()));
            SpriteComponent healthSC = healthDrop.GetComponent<SpriteComponent>() as SpriteComponent;
            if (healthSC != null)
            {
                healthDrop.AddComponent(isActive, physicSys.CreateBoundingBoxComponent((int)healthDrop.Position.X, (int)healthDrop.Position.Y, healthSC.DstRect.w, healthSC.DstRect.h, true));
            }
            healthDrop.AddComponent(isActive, physicSys.CreatePhysicComponent(0, 0, 1));
            healthDrop.AddComponent(isActive, physicSys.CreateCircleColliderComponent(0, 0, 1, 50));
            healthDrop.AddComponent(isActive, audioSys.CreateSoundFXComponent(audios + "sfx_shieldUp.ogg"));

            GameObject screenFlash = objectSys.CreateGameObject("screenFlash", Tag.GameBackground, -10000, -10000, 1, 1, 0, true);
            screenFlash.AddComponent(isActive, renderSys.CreateSpriteComponent(Animation.None, explosions + "flash.png", 1, 1, 1, 10));
            screenFlash.AddComponent(isActive, behaviorSys.CreateBehaviorComponent(new ExplosionBehavior(15)));

            GameObject spawner = objectSys.CreateGameObject("spawner", Tag.EnemySpawner, 0, 0, 1, 1, 0, true);
            spawner.AddComponent(isActive, spawnSys.CreateSpawnComponent(objectSys.FindGameObject("enemy")));
            spawner.AddComponent(isActive, behaviorSys.CreateBehaviorComponent(new SpawnBehaviorComponent(boss, new GameObject[] { enemy, enemySpeeder}, explosion, healthDrop, screenFlash, player)));

            GameObject waveText = objectSys.CreateGameObject("waveText", Tag.WaveIndicator, Window.CURRENT_SCREEN_WIDTH * 0.85, 40* Window.SCALEFACTOR_Y, 1, 1, 0, true);
            waveText.AddComponent(isActive, uiSys.CreateTextComponent("waveLevel", 32, 235, 72, 72, 255));
            waveText.AddComponent(isActive, behaviorSys.CreateBehaviorComponent(new WaveLevelComponent()));

            GameObject playerScoreText = objectSys.CreateGameObject("playerScoreText", Tag.EnemysKilledIndicator, Window.CURRENT_SCREEN_WIDTH * 0.85, 0, 1, 1, 0, true);
            playerScoreText.AddComponent(isActive, uiSys.CreateTextComponent("playerScore", 32, 72, 235, 72, 255));
            playerScoreText.AddComponent(isActive, behaviorSys.CreateBehaviorComponent(new PlayerScoreComponent()));

            GameObject gameOverBackground = objectSys.CreateGameObject("gameOverBackground", Tag.GameOverText, Camera.camera.w / 2, Camera.camera.h / 4, 1, 1, 0, true);
            gameOverBackground.AddComponent(isActive, renderSys.CreateTextureComponent(backgroundPath + "game_over.png", 5));
            gameOverBackground.AddComponent(isActive, behaviorSys.CreateBehaviorComponent(new BackgroundBehaviourComponent(gameOverBackground)));

           
        }

        public void ProcessInput()
        {
            while (SDL_PollEvent(out events) != 0)
            {
                //Key Event for Window X Button to quit the game
                if (InputHandler.BackSpace(events))
                {
                    backToMainLoop = true;
                }
                else if (InputHandler.MouseMoved(events))
                {
                    //TODO: MOVE physicSys.Calc... into UpdateSimulation()
                    //physicSys.CalculateNewVelocityFromMousePos(objectSys.FindGameObject("player"), InputHandler.MousePosition(events));
                    physicSys.MousePosUpdate(objectSys.FindGameObject("player"), InputHandler.MousePosition());
                }
                else if (InputHandler.MouseButtonLeft(events))
                {
                    GameObject player = objectSys.FindGameObject("player");
                    EventSystem.Instance.AddEvent("PlayerShootEvent", player);
                }
                else if (InputHandler.MouseButtonRight(events))
                {
                    Console.WriteLine("MouseButtonRight Down");
                    EventSystem.Instance.AddEvent("PlayerSpeedUp");
                }
                else if (InputHandler.MouseButtonRightRe(events))
                {
                    Console.WriteLine("MouseButtonRight Released");
                    EventSystem.Instance.AddEvent("PlayerSpeedResume");
                }
                else if (InputHandler.Three(events))
                {
                    //EventSystem.Instance.AddEvent("DeactivateGameObject", objectSys.FindGameObject("enemy"));
                    //EventSystem.Instance.AddEvent("SpawnWave", objectSys.FindGameObject("spawner"), 3 , (double)100,(double)100);
                    EventSystem.Instance.AddEvent("SpawnBoss", objectSys.FindGameObject("boss"));
                }
                else if (InputHandler.Space(events))
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
                else if(InputHandler.Zero(events))
                {
                    if(Window.isDebug)
                    {
                        Window.isDebug = false;
                    }
                    else
                    {
                        Window.isDebug = true;
                    }
                }
                else if (InputHandler.One(events))
                {
                    if (godmode)
                    {
                        godmode = false;
                    }
                    else
                    {
                        godmode = true;
                    }
                    Console.WriteLine("Godemode: " + godmode);
                }
            }
        }

        private void Rescale()
        {
            objectSys.RescaleGameObjectPositions();
            physicSys.RescaleCollider();
            World.WORLD_HEIGHT = (int)(World.WORLD_HEIGHT * Window.WINDOW_SCALEFACTOR_X);
            World.WORLD_WIDTH = (int)(World.WORLD_WIDTH * Window.WINDOW_SCALEFACTOR_Y);
        }

        public void UpdateSimulation()
        {
            GameObject player = objectSys.FindGameObject("player");
            
            physicSys.CheckOverlapAABB();

            physicSys.Move(TimeInfo.DeltaTime);
            physicSys.MousePosUpdate(player, InputHandler.MousePosition());
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
            uiSys.Free();

        }

        private void CleanUp()
        {
            SDL_RenderClear(Renderer.renderer);
            SDL_RenderPresent(Renderer.renderer);
            //TODO: implement a .Free() method for each System and call their .Free() method
            audioSys.StopMusic();
            //uiSys.Free();
            //physicSys.Free();
        }
    }
}
