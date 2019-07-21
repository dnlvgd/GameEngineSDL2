using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gpp2019_haifischflosse
{
    public class GameObjectLoader
    {
        private RenderSystem renderSys;
        private BehaviorSystem behaviorSys;
        private PhysicsSystem physicSys;
        private UISystem uiSys;
        private SpawnSystem spawnSys;
        private WeaponSystem weaponSys;
        private AudioSystem audioSys;
        private GameObjectSystem objectSys;


        public GameObjectLoader()
        {
            renderSys = Game.renderSys;
            behaviorSys = Game.behaviorSys;
            physicSys = Game.physicSys;
            uiSys = Game.uiSys;
            spawnSys = Game.spawnSys;
            weaponSys = Game.weaponSys;
            audioSys = Game.audioSys;
            objectSys = Game.objectSys;
        }

        public List<GameObject> LoadGameObjectsFromFile(string filePath, List<GameObject> gO)
        {
            GameObject lastGm = null;
            Dictionary<string, GameObject> gameObjs = new Dictionary<string, GameObject>();

            List<string> lines = File.ReadAllLines(filePath).ToList();

            foreach (var line in lines)
            {
                string[] entries = line.Split(',');
                if (entries[0].Equals("G"))
                {
                    //interprete Line as Add GameObject
                    if (entries.Length != 9)
                    {
                        Console.WriteLine("Error: Wrong or Incomplete Data in LoadGameObejctsFromFile. Check Line: " + line);
                        continue;
                    }
                    try
                    {
                        string name = entries[1];
                        Tag tag = (Tag)Enum.Parse(typeof(Tag), entries[2]);
                        double xPos = double.Parse(entries[3], CultureInfo.InvariantCulture);
                        double yPos = double.Parse(entries[4], CultureInfo.InvariantCulture);
                        double xScale = double.Parse(entries[5], CultureInfo.InvariantCulture);
                        double yScale = double.Parse(entries[6], CultureInfo.InvariantCulture);
                        double rotation = double.Parse(entries[7], CultureInfo.InvariantCulture);
                        bool active;
                        if (entries[8].Equals("true")) active = true;
                        else active = false;

                        lastGm = objectSys.CreateGameObject(name, tag, xPos, yPos, xScale, yScale, rotation, active);
                        gameObjs.Add(name, lastGm);

                    }
                    catch (FormatException e)
                    {
                        Console.WriteLine("Error: Wrong or Incomplete Data in LoadGameObejctsFromFile. Check Line: " + line);
                        Console.WriteLine(e.Message);
                    }
                    catch (Exception ex)
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
                            if (entries.Length != 5)
                            {
                                Console.WriteLine("Invalid Component Data in line: " + line);
                            }
                            try
                            {
                                double velX = double.Parse(entries[2], CultureInfo.InvariantCulture);
                                double velY = double.Parse(entries[3], CultureInfo.InvariantCulture);
                                double factorM = double.Parse(entries[4], CultureInfo.InvariantCulture);

                                lastGm.AddComponent(true, physicSys.CreateMoveComponent(velX, velY, factorM));

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

                        case "spriteC":
                            if (entries.Length != 8)
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

                                lastGm.AddComponent(true, renderSys.CreateSpriteComponent(ani, path, row, column, total, z));
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
                            if (entries.Length != 6)
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
                            if (sprite != null)
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
                            if (entries.Length != 6)
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
/*
                        case "playerBehaviorC":
                            lastGm.AddComponent(true, behaviorSys.CreateBehaviorComponent(new PlayerBehaviorComponent()));
                            break;
                            *//*
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

                            if (oneE != null && oneF != null && twoE != null && twoF != null && threeE != null && threeF != null)
                            {
                                lastGm.AddComponent(true, behaviorSys.CreateBehaviorComponent(new PlayerHealthBehaviorComponent(oneF, oneE, twoF, twoE, threeF, threeE)));
                            }
                            else
                            {
                                Console.WriteLine("Initzialize HearthUI Components bevore hearstbehavior! line: " + line);
                            }
                            break;
                            *//*
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
                            *//*
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
                            */
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

                                if (target != null && weapon != null)
                                {
                                    target.AddComponent(true, weaponSys.CreateWeaponComponent(weapon));
                                }
                                else
                                {
                                    Console.WriteLine("You need to declare the weapon owner and the weapon first. line: " + line);
                                }
                            }
                            catch (FormatException e)
                            {
                                Console.WriteLine("Invalid Format in line: " + line);
                                Console.WriteLine(e.Message);
                            }
                            break;
                            /*
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

                                lastGm.AddComponent(true, behaviorSys.CreateBehaviorComponent(new HomingMissileComponent(target, turnspeed, lifetime)));
                            }
                            catch (FormatException e)
                            {
                                Console.WriteLine("Invalid Format in line: " + line);
                                Console.WriteLine(e.Message);
                            }
                            break;
                            *//*
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
                                }
                                else
                                {
                                    Console.WriteLine("declare player and bossweappons before boss. line: " + line);
                                }
                            }
                            catch (FormatException e)
                            {
                                Console.WriteLine("Invalid Format in line: " + line);
                                Console.WriteLine(e.Message);
                            }
                            break;
                            */
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
                                if (entries.Length == 4)
                                {
                                    lastGm.AddComponent(true, renderSys.CreateTextureComponent(texturepath, z));
                                }
                                else
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
                        /*
                    case "fpsDisplayC":

                        lastGm.AddComponent(true, behaviorSys.CreateBehaviorComponent(new FpsDisplayerComponent(FrameInfo.GetFpsText())));
                        break;
                        *//*
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
                        *//*
                    case "healthDropC":
                        lastGm.AddComponent(true, behaviorSys.CreateBehaviorComponent(new HealthDropBehavior()));
                        break;
                        *//*
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
                            GameObject player = gameObjs[entries[index + 3]]; ;

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
                        *//*
                    case "waveLevelC":
                        lastGm.AddComponent(true, behaviorSys.CreateBehaviorComponent(new WaveLevelComponent()));
                        break;
                        *//*
                    case "playerScoreC":
                        lastGm.AddComponent(true, behaviorSys.CreateBehaviorComponent(new PlayerScoreComponent()));
                        break;
                        *//*
                    case "backgroundC":
                        lastGm.AddComponent(true, behaviorSys.CreateBehaviorComponent(new BackgroundBehaviourComponent(lastGm)));
                        break;
                        */

                        default:
                            Console.WriteLine("Invalid Component Identifier in line: " + line);
                            break;

                    }
                }
            }
            foreach (KeyValuePair<string, GameObject> entry in gameObjs)
            {
                gO.Add(entry.Value);
            }
            return gO;
        }
    }
}
