using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XnaGeometry;

namespace gpp2019_haifischflosse
{
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
                if (true) //godmode check
                {
                    CheckAllEvents();
                    PositionHeartsRelToParent();
                    ShowFullOrEmptyHearts();
                    CheckGameOver();
                }
                else
                {
                    //PositionHeartsRelToParent();
                    //indexofLastEvent = EventSystem.Instance.GetMyIndex();
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
                    Game.objectSys.DeleteGameObject(HeartOneEmpty.Name);
                    Game.objectSys.DeleteGameObject(HeartTwoEmpty.Name);
                    Game.objectSys.DeleteGameObject(HeartThreeEmpty.Name);
                    EventSystem.Instance.AddEvent("DeactivateGameObject", HeartOneEmpty);
                    EventSystem.Instance.AddEvent("DeactivateGameObject", HeartTwoEmpty);
                    EventSystem.Instance.AddEvent("DeactivateGameObject", HeartThreeEmpty);
                    Game.gameStateSys.ChangeState(new LevelOne());
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
            Vector2 cameraOffSet = new Vector2(Camera.camera.x, Camera.camera.y);
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
}
