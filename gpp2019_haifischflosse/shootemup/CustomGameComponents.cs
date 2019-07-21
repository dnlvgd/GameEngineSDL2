using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XnaGeometry;
using static SDL2.SDL;

namespace gpp2019_haifischflosse
{
    public class PlayerBehaviorComponent : BehaviorComponent, IBehavior
    {
        private double mcFactorBeforeBoost = 0;

        private int indexofLastEvent = 0;

        public override void Update(double deltaTime)
        {
            CenterCameraOverPlayer();
            CalculateNewVelocityFromMousePos(Owner);
            CalculatePlayerRotation(Owner);
        }

        public void CenterCameraOverPlayer()
        {
            Camera.camera.x = (int)Owner.Position.X - Window.CURRENT_SCREEN_WIDTH / 2;
            Camera.camera.y = (int)Owner.Position.Y - Window.CURRENT_SCREEN_HEIGHT / 2;
            Camera.camera.w = (int)Window.CURRENT_SCREEN_WIDTH;
            Camera.camera.h = (int)Window.CURRENT_SCREEN_HEIGHT;

            
            //Console.WriteLine("Spieler Position x: {0} , y: {1}", player.Position.X, player.Position.Y);

            //Keep the camera in bounds
            if (Camera.camera.x < 0)
            {
                Camera.camera.x = 0;
            }
            if (Camera.camera.y < 0)
            {
                Camera.camera.y = 0;
            }
            if (Camera.camera.x > World.WORLD_WIDTH - Camera.camera.w)
            {
                Camera.camera.x = World.WORLD_WIDTH - Camera.camera.w;
            }
            if (Camera.camera.y > World.WORLD_HEIGHT - Camera.camera.h)
            {
                Camera.camera.y = World.WORLD_HEIGHT - Camera.camera.h;
            }
        }

        public void CalculateNewVelocityFromMousePos(GameObject player)
        {
            Vector2 tempVelocity = new Vector2();

            MoveComponent mc = player.GetComponent<MoveComponent>() as MoveComponent;
            MouseComponent mouseCom = player.GetComponent<MouseComponent>() as MouseComponent;

            if (EventSystem.Instance.CheckEvent("PlayerSpeedUp", indexofLastEvent))
            {
                mcFactorBeforeBoost = mc.Factor;
                mc.Factor = 0.8;
            }

            if (EventSystem.Instance.CheckEvent("PlayerSpeedResume", indexofLastEvent))
            {
                mc.Factor = mcFactorBeforeBoost;
            }
            indexofLastEvent = EventSystem.Instance.GetMyIndex();

            if (mc == null || mouseCom == null)
            {
                return;
            }
            Vector2 mP = mouseCom.PositionGlobal;

            mP -= player.Position;  //calulate Vector between player and mouse position

            if (-5 < mP.X && mP.X < 5 && -5 < mP.Y && mP.Y < 5)  //checks if the player is not allready at the mouse position +/- 2 Pixel
            {
                mc.Velocity = tempVelocity * 0;
            }
            else
            {
                //makes sure that the sum of the final velocity of x and y can not exceed 1
                double tempX = mP.X;
                double tempY = mP.Y;
                if (tempX < 0) tempX *= -1; //this is nessesary to make sure the sum is always calculated by positiv x and y ( tempX and Y do not influence the final velocity)
                if (tempY < 0) tempY *= -1;
                double vectorSum = tempX + tempY;

                tempVelocity.X = (mP.X / vectorSum); //calcualte the relativ portion the player should move on the x axis
                tempVelocity.Y = (mP.Y / vectorSum); //same for y axis
                mc.Velocity = tempVelocity * (mc.Factor * Window.SCALEFACTOR_X); //multiply by the factor and scalefactor of the window
                                                                                 //Console.WriteLine("Velocity: " + Velocity.X + " " + Velocity.Y);
            }
        }

        public void CalculatePlayerRotation(GameObject player)
        {
            MouseComponent mousePlayer = player.GetComponent<MouseComponent>() as MouseComponent;

            double angle_rad = Math.Atan2((player.Position.Y - mousePlayer.PositionGlobal.Y), (player.Position.X - mousePlayer.PositionGlobal.X));

            double angle_deg = ((angle_rad * 180) / Math.PI);
            player.Angle = angle_deg - 90;
            //Console.WriteLine("PlayerRoation " + angle_deg);// player.Angle);
        }
    }

    public class PlayerHealthBehaviorComponent : BehaviorComponent, IBehavior
    {
        private int indexofLastEvent = 0;
        private int lifes { get; set; }
        private int MinLifes { get; set; }
        private int MaxLifes { get; set; }
        private bool playGameOverSound = true;
        private GameObject HeartOneFull { get; set; }
        private GameObject HeartOneEmpty { get; set; }
        private GameObject HeartTwoFull { get; set; }
        private GameObject HeartTwoEmpty { get; set; }
        private GameObject HeartThreeFull { get; set; }
        private GameObject HeartThreeEmpty { get; set; }
        private Vector2 HeartOffset { get; set; }

        public PlayerHealthBehaviorComponent(GameObject heartOneFull, GameObject heartOneEmpty, GameObject heartTwoFull, GameObject heartTwoEmpty, GameObject heartThreeFull, GameObject heartThreeEmpty, int lifes = 3)
        {
            this.HeartOneFull = heartOneFull;
            this.HeartOneEmpty = heartOneEmpty;
            this.HeartTwoFull = heartTwoFull;
            this.HeartTwoEmpty = heartTwoEmpty;
            this.HeartThreeFull = heartThreeFull;
            this.HeartThreeEmpty = heartThreeEmpty;
            this.MinLifes = 0;
            this.MaxLifes = lifes;
            this.Lifes = lifes;
            this.HeartOffset = new Vector2(100, 0);
        }

        public override void Update(double deltaTime)
        {
            if (playGameOverSound)
            {
                if(!ShootEmUp.godmode)
                {
                    CheckAllEvents();
                    PositionHeartsRelToParent();
                    ShowFullOrEmptyHearts();
                    CheckGameOver();
                } else
                {
                    PositionHeartsRelToParent();
                    indexofLastEvent = EventSystem.Instance.GetMyIndex();
                }
                

            }
        }

        private void CheckGameOver()
        {
            //player died:
            if (!PlayerIsAlive())
            {
                {
                    EventSystem.Instance.AddEvent("PlayGameOverSound", Owner);

                    //DeactivateAllGameObjects and LoadDeathScreen
                    //EventSystem.Instance.AddEvent("DeactivateAllGameObjects");
                    EventSystem.Instance.AddEvent("DeactivateMainPlayer");
                    EventSystem.Instance.AddEvent("LoadDeathScreen");

                    playGameOverSound = false;
                }  
            }
        }

        private void CheckAllEvents()
        {
            List<EventStruct> myEvents = EventSystem.Instance.GetEvents("AdjustPlayerHealth", indexofLastEvent);
            foreach (var e in myEvents)
            {
                Lifes -= (int)e.data[0]; // (int)e.data[0] = 1, player loses one life
            }
            indexofLastEvent = EventSystem.Instance.GetMyIndex();
        }

        private void PositionHeartsRelToParent()
        {
            Vector2 cameraOffSet = new Vector2(Camera.camera.x , Camera.camera.y );
            Vector2 placementOffset = new Vector2(HeartOffset.X * Window.SCALEFACTOR_X, HeartOffset.Y * Window.SCALEFACTOR_Y);
            // left heart
            HeartOneFull.Position = Owner.Position - placementOffset + cameraOffSet;
            HeartOneEmpty.Position = Owner.Position - placementOffset + cameraOffSet;
            // middle heart
            HeartTwoFull.Position = Owner.Position + cameraOffSet;
            HeartTwoEmpty.Position = Owner.Position + cameraOffSet;
            // right heart
            HeartThreeFull.Position = Owner.Position + placementOffset + cameraOffSet;
            HeartThreeEmpty.Position = Owner.Position + placementOffset + cameraOffSet;
        }

        private void ShowFullOrEmptyHearts()
        {
            switch (Lifes)
            {
                case 0:
                    //Draw zero hearts
                    EventSystem.Instance.AddEvent("DeactivateGameObject", HeartOneFull);
                    EventSystem.Instance.AddEvent("ActivateGameObject", HeartOneEmpty);

                    EventSystem.Instance.AddEvent("DeactivateGameObject", HeartTwoFull);
                    EventSystem.Instance.AddEvent("ActivateGameObject", HeartTwoEmpty);

                    EventSystem.Instance.AddEvent("DeactivateGameObject", HeartThreeFull);
                    EventSystem.Instance.AddEvent("ActivateGameObject", HeartThreeEmpty);
                    break;
                case 1:
                    //draw only 1 Heart
                    EventSystem.Instance.AddEvent("ActivateGameObject", HeartOneFull);
                    EventSystem.Instance.AddEvent("DeactivateGameObject", HeartOneEmpty);

                    EventSystem.Instance.AddEvent("DeactivateGameObject", HeartTwoFull);
                    EventSystem.Instance.AddEvent("ActivateGameObject", HeartTwoEmpty);

                    EventSystem.Instance.AddEvent("DeactivateGameObject", HeartThreeFull);
                    EventSystem.Instance.AddEvent("ActivateGameObject", HeartThreeEmpty);
                    break;
                case 2:
                    //draw 2 Hearts
                    EventSystem.Instance.AddEvent("ActivateGameObject", HeartOneFull);
                    EventSystem.Instance.AddEvent("DeactivateGameObject", HeartOneEmpty);

                    EventSystem.Instance.AddEvent("ActivateGameObject", HeartTwoFull);
                    EventSystem.Instance.AddEvent("DeactivateGameObject", HeartTwoEmpty);

                    EventSystem.Instance.AddEvent("DeactivateGameObject", HeartThreeFull);
                    EventSystem.Instance.AddEvent("ActivateGameObject", HeartThreeEmpty);
                    break;
                case 3:
                    //draw 3 Hearts
                    EventSystem.Instance.AddEvent("ActivateGameObject", HeartOneFull);
                    EventSystem.Instance.AddEvent("DeactivateGameObject", HeartOneEmpty);

                    EventSystem.Instance.AddEvent("ActivateGameObject", HeartTwoFull);
                    EventSystem.Instance.AddEvent("DeactivateGameObject", HeartTwoEmpty);

                    EventSystem.Instance.AddEvent("ActivateGameObject", HeartThreeFull);
                    EventSystem.Instance.AddEvent("DeactivateGameObject", HeartThreeEmpty);
                    break;
            }
        }

        private bool PlayerIsAlive()
        {
            if (Lifes > MinLifes)
            {
                return true;
            }
            return false;
        }

        public int Lifes
        {
            get
            {
                return lifes;
            }
            set
            {
                if (value > MaxLifes)
                {
                    lifes = MaxLifes;
                }
                else if (value < MinLifes)
                {
                    lifes = MinLifes;
                }
                else
                {
                    lifes = value;
                }
            }
        }
    }

    public class BackgroundBehaviourComponent : BehaviorComponent, IBehavior
    {
        private GameObject Background { get; set; }

        public BackgroundBehaviourComponent(GameObject background)
        {
            this.Background = background;
            EventSystem.Instance.AddEvent("DeactivateGameObject", background);
        }

        public override void Update(double deltaTime)
        {

        }
    }

    public class EnemyBehaviorComponent : BehaviorComponent, IBehavior
    {
        private GameObject target;
        //private MoveComponent mc;

        private double shootIntervall;
        private double timeSinceLastShoot;
        private double turnspeed;
        private double viewdistance = 900;
        
        public GameObject Target { get { return target; } set { target = value; } }

        public EnemyBehaviorComponent(GameObject target, double turnspeed, double shootfrequenzy)
        {
            this.target = target;
            this.turnspeed = turnspeed;
            shootIntervall = shootfrequenzy;
            timeSinceLastShoot = 0;
        }
    
        public override void Update(double deltaTime)
        {
           
            RotateMe(deltaTime);
            MoveMe();
            Shoot();
        }

        private void MoveMe()
        {
            Vector2 tempVelocity = new Vector2();

            MoveComponent mc = Owner.GetComponent<MoveComponent>() as MoveComponent;

            double angleToRad = Owner.Angle + 90;
            angleToRad = angleToRad / 180.0 * Math.PI;


            tempVelocity.X = Math.Cos(angleToRad);

            tempVelocity.Y = Math.Sin(angleToRad);

            mc.Velocity = tempVelocity * (mc.Factor * Window.SCALEFACTOR_X);
        }

        private void RotateMe(double deltaTime)
        {
            double oldAngle = Owner.Angle;
            double angle_rad = Math.Atan2((Owner.Position.Y - target.Position.Y), (Owner.Position.X - target.Position.X));
            double angle_deg = ((angle_rad * 180) / Math.PI);
            double newAngle = angle_deg + 90;

            newAngle -= oldAngle;
            if (newAngle < -180)
            {
                newAngle += 360;
            }else if(newAngle > 180)
            {
                newAngle -= 360;
            }

            if(newAngle > 0) {
                Owner.Angle = Owner.Angle + ((turnspeed) * deltaTime);
                if(Owner.Angle > 359)
                {
                    Owner.Angle -= 360;
                }
            }else if(newAngle < 0) {
                Owner.Angle = Owner.Angle - ((turnspeed) * deltaTime);
                if (Owner.Angle < 0)
                {
                    Owner.Angle += 360;
                }
            }

        }

        private void Shoot()
        {
            Vector2 vecdist = Owner.Position - target.Position;
            double dist= vecdist.Length();

            if(dist <= (viewdistance * Window.SCALEFACTOR_X))
            {
                timeSinceLastShoot += TimeInfo.DeltaTime;
                if (timeSinceLastShoot >= shootIntervall)
                {
                    Console.WriteLine("EnemyShootEvent");
                    EventSystem.Instance.AddEvent("EnemyShootEvent", Owner);
                    timeSinceLastShoot = 0;
                }
            }
            
        }

    }

    public class EnemySpeederBehaviorComponent : BehaviorComponent, IBehavior
    {
        private GameObject target;
        //private MoveComponent mc;
        
        private double turnspeed;
        
        public GameObject Target { get { return target; } set { target = value; } }

        public EnemySpeederBehaviorComponent(GameObject target, double turnspeed)
        {
            this.target = target;
            this.turnspeed = turnspeed;
        }

        public override void Update(double deltaTime)
        {
            RotateMe(deltaTime);
            MoveMe();
        }

        private void MoveMe()
        {
            Vector2 tempVelocity = new Vector2();

            MoveComponent mc = Owner.GetComponent<MoveComponent>() as MoveComponent;

            double angleToRad = Owner.Angle + 90;
            angleToRad = angleToRad / 180.0 * Math.PI;


            tempVelocity.X = Math.Cos(angleToRad);

            tempVelocity.Y = Math.Sin(angleToRad);

            mc.Velocity = tempVelocity * (mc.Factor * Window.SCALEFACTOR_X);
        }

        private void RotateMe(double deltaTime)
        {
            double oldAngle = Owner.Angle;
            double angle_rad = Math.Atan2((Owner.Position.Y - target.Position.Y), (Owner.Position.X - target.Position.X));
            double angle_deg = ((angle_rad * 180) / Math.PI);
            double newAngle = angle_deg + 90;

            newAngle -= oldAngle;
            if (newAngle < -180)
            {
                newAngle += 360;
            }
            else if (newAngle > 180)
            {
                newAngle -= 360;
            }

            if (newAngle > 0)
            {
                Owner.Angle = Owner.Angle + ((turnspeed) * deltaTime);
                if (Owner.Angle > 359)
                {
                    Owner.Angle -= 360;
                }
            }
            else if (newAngle < 0)
            {
                Owner.Angle = Owner.Angle - ((turnspeed) * deltaTime);
                if (Owner.Angle < 0)
                {
                    Owner.Angle += 360;
                }
            }

        }
    }

    public class BossBehaviorComponent : BehaviorComponent, IBehavior
    {
        private int health;
        private GameObject target;
        //private MoveComponent mc;

        private int indexofLastEvent = 0;
        private double shootIntervall;
        private double timeSinceLastLaser;
        private double timeSinceLastMissile;
        private double turnspeed;
        private double viewdistance = 1200;
        private static int spawnID = 9000000;

        public int Health { get { return health; } set { health = value; } }
        public GameObject Target { get { return target; } set { target = value; } }
        public GameObject LaserProjectile { get; set; }
        public GameObject HomingMissile { get; set; }

        public BossBehaviorComponent(GameObject target, GameObject laser, GameObject missile, int hp, double shootfrequenzy, double turnspeed)
        {
            this.target = target;
            this.LaserProjectile = laser;
            this.HomingMissile = missile;
            this.turnspeed = turnspeed;
            health = hp;
            shootIntervall = shootfrequenzy;
            timeSinceLastLaser = 0;
            timeSinceLastMissile = 0;
        }

        public override void Update(double deltaTime)
        {
            List<EventStruct> damageEvents = EventSystem.Instance.GetEvents("BossHitByPlayer",indexofLastEvent);
            foreach(var e in damageEvents)
            {
                GameObject who = (GameObject)e.data[0];

                if (who.Name.Equals(Owner.Name))
                {
                    GetDamage(1);
                }
            }

            RotateMe(deltaTime);
            MoveMe();
            ShootLaser();
            ShootMissile();

            indexofLastEvent = EventSystem.Instance.GetMyIndex();
        }

        private void MoveMe()
        {
            Vector2 tempVelocity = new Vector2();

            MoveComponent mc = Owner.GetComponent<MoveComponent>() as MoveComponent;

            double angleToRad = Owner.Angle + 90;
            angleToRad = angleToRad / 180.0 * Math.PI;


            tempVelocity.X = Math.Cos(angleToRad);

            tempVelocity.Y = Math.Sin(angleToRad);

            mc.Velocity = tempVelocity * (mc.Factor * Window.SCALEFACTOR_X);
        }

        private void RotateMe(double deltaTime)
        {
            double oldAngle = Owner.Angle;
            double angle_rad = Math.Atan2((Owner.Position.Y - target.Position.Y), (Owner.Position.X - target.Position.X));
            double angle_deg = ((angle_rad * 180) / Math.PI);
            double newAngle = angle_deg + 90;

            newAngle -= oldAngle;
            if (newAngle < -180)
            {
                newAngle += 360;
            }
            else if (newAngle > 180)
            {
                newAngle -= 360;
            }

            if (newAngle > 0)
            {
                Owner.Angle = Owner.Angle + ((turnspeed) * deltaTime);
                if (Owner.Angle > 359)
                {
                    Owner.Angle -= 360;
                }
            }
            else if (newAngle < 0)
            {
                Owner.Angle = Owner.Angle - ((turnspeed) * deltaTime);
                if (Owner.Angle < 0)
                {
                    Owner.Angle += 360;
                }
            }

        }
        private void ShootLaser()
        {
            Vector2 vecdist = Owner.Position - target.Position;
            double dist = vecdist.Length();

            if (dist <= (viewdistance * Window.SCALEFACTOR_X))
            {
                timeSinceLastLaser += TimeInfo.DeltaTime;
                if (timeSinceLastLaser >= shootIntervall)
                {
                    EventSystem.Instance.AddEvent("CloneBullet", LaserProjectile, LaserProjectile.Tag, Owner.Position.X, Owner.Position.Y, Owner.Scaling.X, Owner.Scaling.Y, Owner.Angle + 15, true, spawnID++);
                    EventSystem.Instance.AddEvent("CloneBullet", LaserProjectile, LaserProjectile.Tag, Owner.Position.X, Owner.Position.Y, Owner.Scaling.X, Owner.Scaling.Y, Owner.Angle, true, spawnID++);
                    EventSystem.Instance.AddEvent("CloneBullet", LaserProjectile, LaserProjectile.Tag, Owner.Position.X, Owner.Position.Y, Owner.Scaling.X, Owner.Scaling.Y, Owner.Angle - 15, true, spawnID++);
                    timeSinceLastLaser = 0;
                }
            }
        }

        private void ShootMissile()
        {
            Vector2 vecdist = Owner.Position - target.Position;
            double dist = vecdist.Length();

            if (dist <= (viewdistance * Window.SCALEFACTOR_X))
            {
                timeSinceLastMissile += TimeInfo.DeltaTime;
                if (timeSinceLastMissile >= shootIntervall * 2)
                {
                    EventSystem.Instance.AddEvent("CloneBullet", HomingMissile, HomingMissile.Tag, Owner.Position.X, Owner.Position.Y, Owner.Scaling.X, Owner.Scaling.Y, Owner.Angle, true, spawnID++);
                    timeSinceLastMissile = 0;
                }
            }

        }

        private void Die()
        {
            EventSystem.Instance.AddEvent("PlayerKilltBoss", Owner);
            EventSystem.Instance.AddEvent("DeactivateGameObject", Owner);
        }

        private void GetDamage(int dmg)
        {
            health -= dmg;
            if(health<= 0)
            {
                Die();
            }
        }
    }

    public class PlayerScoreComponent : BehaviorComponent, IBehavior
    {
        private int indexofLastEvent = 0;
        private int Start { get; set; }
        private int End { get; set; }
        private int EasingResult { get; set; }
        private double StartTime { get; set; }
        private double IncreaseDuration { get; set; } = 2; // in seconds
        private float Percentage { get; set; }
        private bool UpdateScore { get; set; } = false;

        public override void Update(double deltaTime)
        {
            /* Handle Events */
            List<EventStruct> myEvents = EventSystem.Instance.GetEvents("PlayerKilltEnemy", indexofLastEvent);
            foreach (var e in myEvents)
            {
                int scoreToAdd = 100; // (int)e.data[0] = 1 enemy killed by player multiplied with score multiplier
                if (UpdateScore)
                {
                    // if ease is already getting updated only increase end value by scoreToAdd value
                    End += scoreToAdd;
                }
                else
                {
                    End = Start + scoreToAdd;
                    StartTime = TimeInfo.TimeSec;
                    Percentage = 0;
                    EasingResult = 0;
                }
                UpdateScore = true;
            }
            List<EventStruct> bossEvents = EventSystem.Instance.GetEvents("PlayerKilltBoss", indexofLastEvent);
            foreach( var e in bossEvents)
            {
                int scoreToAdd = 1000;
                if (UpdateScore)
                {
                    End += scoreToAdd;
                }
                else
                {
                    End = Start + scoreToAdd;
                    StartTime = TimeInfo.TimeSec;
                    Percentage = 0;
                    EasingResult = 0;
                }
                UpdateScore = true;
            }

            // update ease of player score as long as it didnt reach the end value
            if (UpdateScore)
            {
                Percentage = (float)((TimeInfo.TimeSec - StartTime) / IncreaseDuration);
                Percentage = MathHelper.Clamp(Percentage, 0, 1);
                EasingResult = (int)(Start + Ease.SineInOut(Percentage) * (End - Start));
                //Console.WriteLine("Start: " + Start + " EasingValue: " + EasingValue + " t: " + (percentage) + "%" + " End: " + End);
                if (Percentage == 1)
                {
                    Start = End;
                    UpdateScore = false;
                }
            }

            TextComponent tc = Owner.GetComponent<TextComponent>() as TextComponent;
            if (tc != null)
            {
                tc.SetText("Score: " + EasingResult); //Enemys killed
            }
            indexofLastEvent = EventSystem.Instance.GetMyIndex();
        }
    }

    public class WaveLevelComponent : BehaviorComponent, IBehavior
    {
        private int EnemysKilledCount { get; set; }
        private int WaveLevel { get; set; }

        private int indexofLastEvent = 0;

        public WaveLevelComponent()
        {
            WaveLevel = 1;
        }

        public override void Update(double deltaTime)
        {
            TextComponent tc = Owner.GetComponent<TextComponent>() as TextComponent;

            List<EventStruct> myEvents = EventSystem.Instance.GetEvents("WaveLevelIncrease", indexofLastEvent);
            foreach (var e in myEvents)
            {
                WaveLevel = (int)e.data[0];

            }

            if (tc != null)
            {
                tc.SetText("Wave: " + WaveLevel);
            }
            indexofLastEvent = EventSystem.Instance.GetMyIndex();
        }
    }

    public class FpsDisplayerComponent : BehaviorComponent, IBehavior
    {
        public string FpsText { get; set; }

        public FpsDisplayerComponent(string fpsText)
        {
            this.FpsText = fpsText;
        }

        public override void Update(double deltaTime)
        {
            TextComponent tc = Owner.GetComponent<TextComponent>() as TextComponent;

            if (tc != null)
            {
                FpsText = FrameInfo.GetFpsText();
                tc.SetText(FpsText);
            }
        }
    }

    public class SpawnBehaviorComponent : BehaviorComponent, IBehavior
    {
        private int EnemiesKilled = 0;
        private int EnemiesSpawned = 1;
        private int indexofLastEvent = 0;
        private int waveSize = 3;
        private int waveLevel = 1;
        private int spawnID = 1;
        private bool spawnBoss = false;
        private GameObject myBoss;
        private GameObject[] myEnemies;
        private GameObject explosionO;
        private GameObject healthDrop;
        private GameObject flash;
        private GameObject player;
        private Random rdm = new Random();

        public SpawnBehaviorComponent(GameObject myBoss, GameObject[] myEnemies, GameObject explosionO, GameObject healthDrop, GameObject flash, GameObject player)
        {
            this.myBoss = myBoss;
            this.myEnemies = myEnemies;
            this.explosionO = explosionO;
            this.healthDrop = healthDrop;
            this.flash = flash;
            this.player = player;
        }

        public override void Update(double deltaTime)
        {
            List<EventStruct> myEvents = EventSystem.Instance.GetEvents("PlayerKilltEnemy", indexofLastEvent);
            foreach (var e in myEvents)
            {
                EnemiesKilled++;
            }
            List<EventStruct> bossDied = EventSystem.Instance.GetEvents("PlayerKilltBoss", indexofLastEvent);
            foreach ( var e in bossDied)
            {
                GameObject boss = e.data[0] as GameObject;

                SpawnHealth(boss.Position);
            }
            List<EventStruct> bossSpawns = EventSystem.Instance.GetEvents("SpawnBoss", indexofLastEvent);
            foreach(var e in bossSpawns)
            {
                GameObject boss = e.data[0] as GameObject;
                SpawnBoss(boss);
            }
            List<EventStruct> explosions = EventSystem.Instance.GetEvents("BulletHit", indexofLastEvent);
            foreach(var e in explosions)
            {
                GameObject where = e.data[0] as GameObject;
                SpawnExplosion(where);
            }
            List<EventStruct> screenFlashes = EventSystem.Instance.GetEvents("AdjustPlayerHealth", indexofLastEvent);
            foreach(var e in screenFlashes)
            {
                int x = (int)e.data[0];
                if(x > 0)
                {
                    SpawnFlash();
                }
            }

            indexofLastEvent = EventSystem.Instance.GetMyIndex();
            int spawnBosCon = 10 + waveSize; 
            if(EnemiesKilled % spawnBosCon == 0 && spawnBoss)
            {
                spawnBoss = false;
                SpawnBoss(myBoss);
            }
            if(EnemiesKilled >= EnemiesSpawned)
            {
                EnemiesSpawned += waveSize;
                for (int i = 0; i < waveSize; i++)
                {
                    Vector2 spawnPos = GenerateSpawnPosition();
                    //EventSystem.Instance.AddEvent("SpawnWave", Owner, 1, spawnPos.X, spawnPos.Y);
                    
                    int whichEnemy = rdm.Next(0, myEnemies.Length);
                    GameObject enemy = myEnemies[whichEnemy];
                    Console.WriteLine(enemy.Name);
                    EventSystem.Instance.AddEvent("CloneGameObject", enemy, enemy.Tag, spawnPos.X, spawnPos.Y, enemy.Scaling.X, enemy.Scaling.Y, enemy.Angle, true, spawnID);
                    spawnID++;
                }
                waveLevel++;
                waveSize += (waveLevel / 2);
                EventSystem.Instance.AddEvent("WaveLevelIncrease", waveLevel);
                spawnBoss = true;
            }
        }
        private void SpawnExplosion(GameObject where)
        {
            EventSystem.Instance.AddEvent("CloneGameObject", explosionO, explosionO.Tag, where.Position.X, where.Position.Y, explosionO.Scaling.X, explosionO.Scaling.Y, explosionO.Angle, true, spawnID);
            spawnID++;
        }

        private void SpawnFlash()
        {
            EventSystem.Instance.AddEvent("CloneGameObject", flash, flash.Tag, player.Position.X,player.Position.Y , flash.Scaling.X, flash.Scaling.Y, flash.Angle, true, spawnID);
            spawnID++;
        }

        private void SpawnHealth(Vector2 position)
        {
            EventSystem.Instance.AddEvent("CloneGameObject", healthDrop, healthDrop.Tag, position.X, position.Y, healthDrop.Scaling.X, healthDrop.Scaling.Y, healthDrop.Angle, true, spawnID);
            spawnID++;
        }

        private void SpawnBoss(GameObject boss)
        {
            Vector2 spawnPos = GenerateSpawnPosition();
            EventSystem.Instance.AddEvent("CloneGameObject", boss, boss.Tag, spawnPos.X, spawnPos.Y, boss.Scaling.X, boss.Scaling.Y, boss.Angle, true, spawnID);
            spawnID++;
        }

        private Vector2 GenerateSpawnPosition()
        {
            int offset = 30;

            int maxPosX = World.WORLD_WIDTH + offset;
            int maxPosY = World.WORLD_HEIGHT + offset;
            int minPosX = 0 - offset;
            int minPosY = 0 - offset;

            
            int xOrY =  rdm.Next(0, 4);

            Vector2 spawnPos = new Vector2();
            Console.WriteLine("Max X: {0} , Max Y: {1}", maxPosX, maxPosY);
            switch (xOrY)
            {
                case 0:
                    Console.WriteLine("Case 0 called");
                    spawnPos.X = minPosX;
                    spawnPos.Y = rdm.Next(minPosY, maxPosY);
                    break;

                case 1:
                    Console.WriteLine("Case 1 called");
                    spawnPos.X = rdm.Next(minPosX, maxPosX);
                    spawnPos.Y = minPosY;
                    break;

                case 2:
                    Console.WriteLine("Case 2 called");
                    spawnPos.X = maxPosX;
                    spawnPos.Y = rdm.Next(minPosY, maxPosY);
                    break;

               case 3:
                    Console.WriteLine("Case 3 called");
                    spawnPos.X = rdm.Next(minPosX, maxPosX);
                    spawnPos.Y = maxPosY;
                    break;
            }
            Console.WriteLine(spawnPos);
            return spawnPos;
      
        }
    }

    public class HomingMissileComponent : BehaviorComponent, IBehavior
    {
        private double turnspeed;
        private double lifetime;

        public GameObject Target { get; set; }

        public HomingMissileComponent(GameObject target, double turnspeed, double lifetime)
        {
            this.Target = target;
            this.turnspeed = turnspeed;
            this.lifetime = lifetime;
        }

        public override void Update(double deltaTime)
        {
            lifetime -= deltaTime;
            if(lifetime<= 0)
            {
                EventSystem.Instance.AddEvent("DeactivateGameObject", Owner);
            }
            RotateMe(deltaTime);
            MoveMe();
        }

        private void MoveMe()
        {
            Vector2 tempVelocity = new Vector2();

            MoveComponent mc = Owner.GetComponent<MoveComponent>() as MoveComponent;

            double angleToRad = Owner.Angle + 90;
            angleToRad = angleToRad / 180.0 * Math.PI;


            tempVelocity.X = Math.Cos(angleToRad);

            tempVelocity.Y = Math.Sin(angleToRad);

            mc.Velocity = tempVelocity * (mc.Factor * Window.SCALEFACTOR_X);
        }

        private void RotateMe(double deltaTime)
        {
            double oldAngle = Owner.Angle;
            double angle_rad = Math.Atan2((Owner.Position.Y - Target.Position.Y), (Owner.Position.X - Target.Position.X));
            double angle_deg = ((angle_rad * 180) / Math.PI);
            double newAngle = angle_deg + 90;

            newAngle -= oldAngle;
            if (newAngle < -180)
            {
                newAngle += 360;
            }
            else if (newAngle > 180)
            {
                newAngle -= 360;
            }

            if (newAngle > 0)
            {
                Owner.Angle = Owner.Angle + ((turnspeed) * deltaTime);
                if (Owner.Angle > 359)
                {
                    Owner.Angle -= 360;
                }
            }
            else if (newAngle < 0)
            {
                Owner.Angle = Owner.Angle - ((turnspeed) * deltaTime);
                if (Owner.Angle < 0)
                {
                    Owner.Angle += 360;
                }
            }

        }
    }

    public class ExplosionBehavior : BehaviorComponent, IBehavior
    {
        private double lifetime;

        public ExplosionBehavior(int lifetime)
        {
            this.lifetime = lifetime;
        }
        public override void Update(double deltaTime)
        {
            lifetime -= deltaTime;
            if(lifetime<= 0)
            {
                EventSystem.Instance.AddEvent("DeactivateGameObject", Owner);
            }
        }
    }

    public class HealthDropBehavior : BehaviorComponent, IBehavior
    {
        public override void Update(double deltaTime)
        {
            BoundingBoxComponent bc = Owner.GetComponent<BoundingBoxComponent>() as BoundingBoxComponent;
            bc.Box = new SDL_Rect{x = (int)Owner.Position.X, y = (int)Owner.Position.Y , w= bc.Box.w,h=bc.Box.h};
        }
    }

}
