using System.Collections.Generic;
using System.Linq;

namespace gpp2019_haifischflosse
{
    public class GameStateSystem
    {
        public Stack<IGameState> states = new Stack<IGameState>();

        public void ChangeState(IGameState state)
        {
            /* Clear all current states an change to the new state */
            if (states.Any())
            {
                states.Last().CleanUp();
                states.Clear();
            }

            states.Push(state);
            states.Last().Init();
        }

        public void PushState(IGameState state)
        {
            /* Pause current state and push the new state */
            if (states.Any())
            {
                states.Last().Pause();
            }

            states.Push(state);
            states.First().Init();
        }

        public void PopState()
        {
            /* Pop the current state and resume to the last state */
            if (states.Any())
            {
                states.First().CleanUp();
                states.Pop();
            }

            if (states.Any())
            {
                states.Last().Resume();
            }
        }

        public void Update()
        {
            if(states.Any())
            {
                states.First().Update();
            }
        }

        public void ProcessInput()
        {
            if (states.Any())
            {
                states.First().ProcessInput();
            }
        }
    }
}
