using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL_mixer;

namespace gpp2019_haifischflosse
{
    public class AudioSystem
    {
        private List<SoundFXComponent> soundFXCps = new List<SoundFXComponent>();
        private List<MusicComponent> musicCps = new List<MusicComponent>();
        private int indexofLastEvent = 0;

        public SoundFXComponent CreateSoundFXComponent(string path)
        {
            SoundFXComponent sc = new SoundFXComponent(path);
            soundFXCps.Add(sc);
            return sc;
        }

        public MusicComponent CreateMusicComponent(string path)
        {
            MusicComponent mc = new MusicComponent(path);
            musicCps.Add(mc);
            return mc;
        }

        public void Update()
        {
            /* Event handling */
            if (EventSystem.Instance.CheckEvent("RemoveAllCPs", indexofLastEvent))
            {
                soundFXCps.Clear();
                musicCps.Clear();
            }

            List<EventStruct> myEvents = EventSystem.Instance.GetEvents("PlayerShootEvent", indexofLastEvent);
            foreach (EventStruct e in myEvents)
            {
                GameObject go = e.data[0] as GameObject;
                if (go != null)
                {
                    PlaySoundFX(go);
                }
            }
            List<EventStruct> enemyShootEvents = EventSystem.Instance.GetEvents("EnemyShootEvent", indexofLastEvent);
            foreach (EventStruct e in enemyShootEvents)
            {
                PlaySoundFX((GameObject)e.data[0]);
            }
            List<EventStruct> healthPickUps = EventSystem.Instance.GetEvents("AdjustPlayerHealth", indexofLastEvent);
            foreach(EventStruct e in healthPickUps)
            {
                int x = (int)e.data[0];
                if(x < 0)
                {
                    PlaySoundFX((GameObject)e.data[1]);
                }
            }

            List<EventStruct> impacts = EventSystem.Instance.GetEvents("BulletHit", indexofLastEvent);
            foreach (EventStruct e in impacts)
            {
                PlaySoundFX((GameObject)e.data[0]);
            }

            List<EventStruct> buttonClicks = EventSystem.Instance.GetEvents("ButtonClicked", indexofLastEvent);
            foreach (EventStruct e in buttonClicks)
            {
                PlaySoundFX((GameObject)e.data[0]);
            }

            List<EventStruct> coinCollect = EventSystem.Instance.GetEvents("CoinCollect", indexofLastEvent);
            foreach (EventStruct e in coinCollect)
            {
                PlaySoundFX((GameObject)e.data[0]);
            }

            List<EventStruct> shootArrow = EventSystem.Instance.GetEvents("ShootArrow", indexofLastEvent);
            foreach (EventStruct e in shootArrow)
            {
                PlaySoundFX((GameObject)e.data[0]);
            }
            
            List<EventStruct> arrowHitExplosion = EventSystem.Instance.GetEvents("ArrowHitExplosion", indexofLastEvent);
            foreach (EventStruct e in arrowHitExplosion)
            {
                PlaySoundFX((GameObject)e.data[0]);
            }

            List<EventStruct> gameOverEvents = EventSystem.Instance.GetEvents("PlayGameOverSound", indexofLastEvent);
            foreach(EventStruct e in gameOverEvents)
            {
                GameObject go = e.data[0] as GameObject;
                if (go != null)
                {
                    PlaySoundFX(go);
                }
            }
            indexofLastEvent = EventSystem.Instance.GetMyIndex();
        }

        public void PlaySoundFX(GameObject go)
        {
            SoundFXComponent sc = go.GetComponent<SoundFXComponent>() as SoundFXComponent;
            if (sc != null)
            {
                Console.WriteLine("PlaySoundFX for Gameobject: " + go.Name);
                Mix_PlayChannel(-1, sc.SoundFX, 0);
            }
            else
            {
                Console.WriteLine(go.Name + " has no SoundeFXComponent to play any soundfx from");
            }
        }

        public void PlayMusic(GameObject go)
        {
            MusicComponent mc = go.GetComponent<MusicComponent>() as MusicComponent;
            if (mc != null)
            {
                Mix_PlayMusic(mc.Music, -1);
            }
        }

        public void StopSoundFX()
        {
            Mix_HaltChannel(-1);
        }

        public void StopMusic()
        {
            Mix_HaltMusic();
        }
    }
}
