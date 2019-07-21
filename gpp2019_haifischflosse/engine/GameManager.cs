using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gpp2019_haifischflosse
{
    public sealed class GameManager
    {
        /* List for all GameObjects */
        public IList<GameObject> GameObjects { get; set; } = new List<GameObject>();
        public IList<DelayedAction> DelayedActions { get; set; } = new List<DelayedAction>();

        //SINGELTON
        private static GameManager gm = null;
        private static readonly object padlock = new object();

        GameManager()
        {

        }

        public static GameManager Gm
        {
            get
            {
                lock (padlock)
                {
                    if (gm == null)
                    {
                        gm = new GameManager();
                    }
                    return gm;
                }
            }
        }

 
        /* Add a object depending of the usage of classes and interfaces to each list /*/
        public void AddGameObject(GameObject go)
        {
            if (go == null)
            {
                return;
            } else
            {
                GameObjects.Add(go);
            }
        }

        public void AddDelyedAction(DelayedAction d)
        {
            if (d == null)
            {
                return;
            }
            else
            {
                DelayedActions.Add(d);
            }
        }
    }
}
 