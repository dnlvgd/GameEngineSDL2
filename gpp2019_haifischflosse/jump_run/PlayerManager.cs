using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XnaGeometry;

namespace gpp2019_haifischflosse
{
    class PlayerManager : BehaviorComponent, IBehavior
    {
        private int indexofLastEvent = 0;

        public int Gold { get; set; }

        public override void Update(double deltaTime)
        {
            CheckForEvents();
            CheckHeight();

            indexofLastEvent = EventSystem.Instance.GetMyIndex();
        }

        private void CheckForEvents()
        {
            List<EventStruct> myGoldEvents = EventSystem.Instance.GetEvents("GoldPickUp", indexofLastEvent);
            foreach (var e in myGoldEvents)
            {
                // GameObject coin = (GameObject)e.data[0]; needed when different Gold objects possible...
                Gold += 10;
                EventSystem.Instance.AddEvent("GoldUpdate", Gold);
            }
            if(EventSystem.Instance.CheckEvent("PlayerReachedFlag", indexofLastEvent))
            {
                safeProgress();
                //EventSystem.Instance.AddEvent("LevelOver");
            }
           
        }

        private void CheckHeight()
        {
            if(Owner.Position.Y > 1200 * Window.SCALEFACTOR_Y)
            {
                EventSystem.Instance.AddEvent("AdjustPlayerHealth",1);
                Owner.Position = new Vector2(300 * Window.SCALEFACTOR_X, 800 * Window.SCALEFACTOR_Y);
            }
        }

        private void safeProgress()
        {
            int myGold;
            string[] lines = File.ReadAllLines(FilePath.playerSafe);
            string[] goldsafe = lines[0].Split(',');
            
            Int32.TryParse(goldsafe[1], out myGold);

            myGold += Gold;
            string[] safe = { "Gold," + myGold };

            File.WriteAllLines(FilePath.playerSafe,safe);
            Game.gameStateSys.ChangeState(new MainMenue());
        }

    }

    class GoldText: BehaviorComponent, IBehavior
    {
        private int indexofLastEvent = 0;

        public override void Update(double deltaTime)
        {
            CheckForGold();

            indexofLastEvent = EventSystem.Instance.GetMyIndex();
        }

        private void CheckForGold()
        {
            List<EventStruct> myEvents = EventSystem.Instance.GetEvents("GoldUpdate", indexofLastEvent);
            TextComponent tc = Owner.GetComponent<TextComponent>() as TextComponent;
            foreach (var e in myEvents)
            {
                tc.SetText("Gold: " + (int)e.data[0]);
            }
        }
    }
}
