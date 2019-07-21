using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gpp2019_haifischflosse
{
    public class BehaviorSystem
    {
        private List<IBehavior> behaviorCps = new List<IBehavior>();
        private int indexofLastEvent = 0;

        public BehaviorComponent CreateBehaviorComponent(BehaviorComponent bc)
        {
            if(bc is IBehavior)
            {
                behaviorCps.Add(bc);
            }
            return bc;
        }

        public void Update(double deltaTime, Input input)
        {
            /* Event handling */
            if (EventSystem.Instance.CheckEvent("RemoveAllCPs", indexofLastEvent))
            {
                behaviorCps.Clear();
            }

            List<EventStruct> myEvents;
            myEvents = EventSystem.Instance.GetEvents("AddBehaviorCP", indexofLastEvent);
            foreach (EventStruct e in myEvents)
            {
                behaviorCps.Add((BehaviorComponent)e.data[0]);
            }

            myEvents = EventSystem.Instance.GetEvents("RemoveBehaviorCP", indexofLastEvent);
            foreach (EventStruct e in myEvents)
            {
                behaviorCps.Remove((BehaviorComponent)e.data[0]);
            }

            /* Call each Update and HandleInput in behaviorCps */
            for (int i = behaviorCps.Count - 1; i >= 0 ; i--)
            {
                if (behaviorCps[i] is Component bcCP)
                {
                    if(bcCP != null)
                    {
                        if(bcCP.Owner.IsActive && bcCP.IsActive)
                        {
                            behaviorCps[i].Update(deltaTime);
                            behaviorCps[i].HandleInput(input);
                        }
                    }
                }
            }

            indexofLastEvent = EventSystem.Instance.GetMyIndex();
        }
    }
}
