using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;

namespace gpp2019_haifischflosse
{

    public struct EventStruct
    {
        public string id;
        public Type type;
        public object[] data;
    }

    public sealed class EventSystem
    {
        public enum Eventtype
        {
            PlayerShoot,
            PlayerSpeedUp,
            PlayerSpeedResume,
            PlayerKilltEnemy,
            SpawnBoss,
            BulletHit,
            RenderOnce,
            DeactivateGameObject,
            

        }

        private static readonly EventSystem instance = new EventSystem();

        static EventSystem() { }
        private EventSystem() { }

        public static EventSystem Instance
        {
            get
            {
                return instance;
            }
        }
        private static int eventThisTick;
        private static int tail = 0;

        private static int MaxPending = 1000;
        private static EventStruct[] activeEvents = new EventStruct[MaxPending];

        public void Update()
        {
            if(eventThisTick > MaxPending)
            {
                Console.WriteLine("THIS TICK WHERE MORE EVENTS THEN MAXPENDING!!!! EVENT LOST POSSIBLE");
            }
            eventThisTick = 0;
        }

        public void AddEvent(string id, params object[] data)
        {
            activeEvents[tail].id = id;
            activeEvents[tail].data = new object[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                activeEvents[tail].data[i] = data[i];
            }
            tail = (tail + 1) % MaxPending;
            eventThisTick++;
        }

        public void AddEventWithType(Type type, params object[] data)
        {
            activeEvents[tail].type = type;
            activeEvents[tail].data = new object[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                activeEvents[tail].data[i] = data[i];
            }
            tail = (tail + 1) % MaxPending;
            eventThisTick++;
        }

        public int GetMyIndex()
        {
            return tail;
        }

        public List<EventStruct> GetEvents(string eventtype, int head)
        {
            List<EventStruct> myEvents = new List<EventStruct>();
            for (int i = head; i != tail; i = (i + 1) % MaxPending)
            {
                if (activeEvents[i].id.Equals(eventtype))
                {
                    myEvents.Add(activeEvents[i]);
                }

            }
            return myEvents;
        }

        public List<EventStruct> GetEventsOfType(Type type, int head)
        {
            List<EventStruct> myEvents = new List<EventStruct>();
            for (int i = head; i != tail; i = (i + 1) % MaxPending)
            {
                if (activeEvents[i].type.Equals(type))
                {
                    myEvents.Add(activeEvents[i]);
                }

            }
            return myEvents;
        }

        public bool CheckEvent(string eventtype, int head)
        {
            for (int i = head; i != tail; i = (i+1)%MaxPending)
            {
                if (activeEvents[i].id.Equals(eventtype))
                {
                    return true;
                }
            }
            return false;
        }
    }

}