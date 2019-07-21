using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gpp2019_haifischflosse
{
    public class WeaponSystem
    {
        public int SpawnID = 0;
        private int indexofLastEvent = 0;

        private List<WeaponComponent> weaponCps = new List<WeaponComponent>();

        public WeaponComponent CreateWeaponComponent(GameObject weapon)
        {
            WeaponComponent mc = new WeaponComponent(weapon);
            weaponCps.Add(mc);
            return mc;
        }

        public void Update()
        {
            /* Event handling */
            if (EventSystem.Instance.CheckEvent("RemoveAllCPs", indexofLastEvent))
            {
                weaponCps.Clear();
            }

            List<EventStruct> awCpEvents;
            awCpEvents = EventSystem.Instance.GetEvents("AddWeaponCP", indexofLastEvent);
            foreach (EventStruct e in awCpEvents)
            {
                weaponCps.Add((WeaponComponent)e.data[0]);
            }

            List<EventStruct> rwCpEvents;
            rwCpEvents = EventSystem.Instance.GetEvents("RemoveWeaponCP", indexofLastEvent);
            foreach (EventStruct e in awCpEvents)
            {
                GameObject go = e.data[0] as GameObject;
                if (go != null)
                {
                    weaponCps.Remove((WeaponComponent)e.data[0]);
                }
            }

            List<EventStruct> psEvents = EventSystem.Instance.GetEvents("PlayerShootEvent", indexofLastEvent);
            foreach (EventStruct e in psEvents)
            {
                GameObject go = e.data[0] as GameObject;
                if(go != null)
                {
                    SpawnWeapon(go);
                }
            }

            List<EventStruct> esEvents = EventSystem.Instance.GetEvents("EnemyShootEvent", indexofLastEvent);
            foreach (EventStruct e in esEvents)
            {
                GameObject go = e.data[0] as GameObject;
                if (go != null)
                {
                    SpawnWeapon(go);
                }
            }
            indexofLastEvent = EventSystem.Instance.GetMyIndex();
        }

        private void AddWeaponCp(GameObject go)
        {
            WeaponComponent wc = go.GetComponent<WeaponComponent>() as WeaponComponent;
            if (wc != null)
            {
                weaponCps.Add(wc);
            }
        }

        private void SpawnWeapon(GameObject shooter)
        {
            WeaponComponent wc = shooter.GetComponent<WeaponComponent>() as WeaponComponent;
            if (wc != null)
            {
                EventSystem.Instance.AddEvent("CloneBullet", wc.Weapon, wc.Weapon.Tag, shooter.Position.X, shooter.Position.Y, shooter.Scaling.X, shooter.Scaling.Y, shooter.Angle, shooter.IsActive, SpawnID);
                SpawnID++;
            }
        }
    }
}
