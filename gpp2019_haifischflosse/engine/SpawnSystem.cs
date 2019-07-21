using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gpp2019_haifischflosse
{
    public class SpawnSystem
    {

        int[,] SpawnLocations = new int[,] { { 0, 0 }, { Window.CURRENT_SCREEN_WIDTH, 0 }, { Window.CURRENT_SCREEN_WIDTH, Window.CURRENT_SCREEN_HEIGHT }, { 0, Window.CURRENT_SCREEN_WIDTH } };
        public int SpawnID = 0;

        double spawnInterval = 1000;
        double timer = 0;

        int indexofLastEvent = 0;

        List<SpawnComponent> spawnCps = new List<SpawnComponent>();
        List<SpawnObject> activeSpawns = new List<SpawnObject>();

        public SpawnComponent CreateSpawnComponent(GameObject spawnObject)
        {
            SpawnComponent sc = new SpawnComponent(spawnObject);
            spawnCps.Add(sc);
            return sc;
        }

        public void Update(double dt)
        {
            /* Event handling */
            if (EventSystem.Instance.CheckEvent("RemoveAllCPs", indexofLastEvent))
            {
                spawnCps.Clear();
                activeSpawns.Clear();
            }

            List<EventStruct> myEvents;
            myEvents = EventSystem.Instance.GetEvents("AddSpawnCP", indexofLastEvent);
            foreach (EventStruct e in myEvents)
            {
                spawnCps.Add((SpawnComponent)e.data[0]);
            }

            myEvents = EventSystem.Instance.GetEvents("RemoveSpawnCP", indexofLastEvent);
            foreach (EventStruct e in myEvents)
            {
                spawnCps.Remove((SpawnComponent)e.data[0]);
            }

            myEvents = EventSystem.Instance.GetEvents("SpawnWave", indexofLastEvent);
            foreach (EventStruct e in myEvents)
            {
                Console.WriteLine("WaveSpawned");
                SpawnObject sO = new SpawnObject();
                sO.Timer = timer;
                sO.WaveSize = (int)e.data[1];
                sO.SpawnPosX = (double)e.data[2];
                sO.SpawnPosY = (double)e.data[3];

                GameObject gm = e.data[0] as GameObject;
                if (gm != null)
                {
                    SpawnComponent sc = gm.GetComponent<SpawnComponent>() as SpawnComponent;
                    if(sc != null)
                    {
                        sO.SpawnC = sc;
                        activeSpawns.Add(sO);
                    }
                } 
            }
            indexofLastEvent = EventSystem.Instance.GetMyIndex();
            Spawn();
        }
        
        void Spawn()
        {
            for (int i = activeSpawns.Count - 1; i>-1; i--)
            {
                activeSpawns[i].Timer += TimeInfo.DeltaTime;
                if (activeSpawns[i].Timer > spawnInterval)
                {
                    activeSpawns[i].Timer -= spawnInterval;
                    SpawnComponent sc = activeSpawns[i].SpawnC;
                    if (sc != null)
                    {
                        EventSystem.Instance.AddEvent("CloneGameObject", sc.SpawnObject, sc.SpawnObject.Tag, activeSpawns[i].SpawnPosX, activeSpawns[i].SpawnPosY, sc.Owner.Scaling.X, sc.Owner.Scaling.Y, sc.Owner.Angle, sc.Owner.IsActive, SpawnID);
                        SpawnID++;
                        activeSpawns[i].WaveSize--;
                        if( activeSpawns[i].WaveSize <= 0)
                        {
                            activeSpawns.RemoveAt(i);
                        }
                    }
                }
            }
        }
    }

    public class SpawnObject
    {
        public double Timer { get; set; }
        public double WaveSize { get; set; }
        public double SpawnPosX { get; set; }
        public double SpawnPosY { get; set; }
        public SpawnComponent SpawnC { get; set; }
    }
}
