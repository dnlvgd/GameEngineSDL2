using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gpp2019_haifischflosse
{
    public interface IGameState
    {
        List<GameObject> GameObjects { get; set; }

        void Init();
        void Pause();
        void Resume();
        void CleanUp();
        void Update();
        void ProcessInput();
    }
}
