using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XnaGeometry;

namespace gpp2019_haifischflosse
{
    public class SpawnBehaviorComponent : BehaviorComponent, IBehavior
    {
        private int indexofLastEvent = 0;
        private Random rdm = new Random();
        private int spawnID;
        private GameObject explosionO;
        private GameObject healthDrop;
        private GameObject flash;
        private GameObject player;
        
        /*
        private int EnemiesKilled = 0;
        private int EnemiesSpawned = 1;
        private int waveSize = 3;
        private int waveLevel = 1;
        
        private bool spawnBoss = false;
        private GameObject myBoss;
        private GameObject[] myEnemies;
        
    */
        public SpawnBehaviorComponent(GameObject explosionO, GameObject flash, GameObject player)
        {
            spawnID = rdm.Next();
            this.explosionO = explosionO;
            this.flash = flash;
            this.player = player;
        }

        public override void Update(double deltaTime)
        {
            
            List<EventStruct> explosions = EventSystem.Instance.GetEvents("BulletHit", indexofLastEvent);
            foreach (var e in explosions)
            {
                GameObject where = e.data[0] as GameObject;
                SpawnExplosion(where);
            }
            List<EventStruct> screenFlashes = EventSystem.Instance.GetEvents("AdjustPlayerHealth", indexofLastEvent);
            foreach (var e in screenFlashes)
            {
                int x = (int)e.data[0];
                if (x > 0)
                {
                    SpawnFlash();
                }
            }

            indexofLastEvent = EventSystem.Instance.GetMyIndex();
            
        }
        private void SpawnExplosion(GameObject where)
        {
            EventSystem.Instance.AddEvent("CloneGameObject", explosionO, explosionO.Tag, where.Position.X, where.Position.Y, 1d, 1d, explosionO.Angle, true, spawnID);
            Console.WriteLine(where.Position.X + " , " +where.Position.Y);
            spawnID++;
        }

        private void SpawnFlash()
        {
            EventSystem.Instance.AddEvent("CloneGameObject", flash, flash.Tag, (double)Camera.camera.x, (double)Camera.camera.y, 2.0d, 2.0d, flash.Angle, true, spawnID);
            spawnID++;
        }

        private void SpawnHealth(Vector2 position)
        {
            EventSystem.Instance.AddEvent("CloneGameObject", healthDrop, healthDrop.Tag, position.X, position.Y, healthDrop.Scaling.X, healthDrop.Scaling.Y, healthDrop.Angle, true, spawnID);
            spawnID++;
        }

        private Vector2 GenerateSpawnPosition()
        {
            int offset = 30;

            int maxPosX = World.WORLD_WIDTH + offset;
            int maxPosY = World.WORLD_HEIGHT + offset;
            int minPosX = 0 - offset;
            int minPosY = 0 - offset;


            int xOrY = rdm.Next(0, 4);

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
            if (lifetime <= 0)
            {
                EventSystem.Instance.AddEvent("DeactivateGameObject", Owner);
            }
        }
    }
}
