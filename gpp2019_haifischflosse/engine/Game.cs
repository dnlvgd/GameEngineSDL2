using System;
using static SDL2.SDL;


namespace gpp2019_haifischflosse
{
    public class Game
    {
        /* Systems */
        public static RenderSystem renderSys = new RenderSystem();
        public static PhysicsSystem physicSys = new PhysicsSystem();
        public static UISystem uiSys = new UISystem();
        public static SpawnSystem spawnSys = new SpawnSystem();
        public static WeaponSystem weaponSys = new WeaponSystem();
        public static BehaviorSystem behaviorSys = new BehaviorSystem();
        public static AudioSystem audioSys = new AudioSystem();
        public static GameObjectSystem objectSys = new GameObjectSystem();
        public static GameStateSystem gameStateSys = new GameStateSystem();

        /* SDL Events */
        protected SDL_Event e;

        /* Input for Animation States */
        public static Input input = Input.None;

        /* Used to close the window if wished by the user*/
        public static bool closeWindow = false;

        public static void FullscreenMode()
        {
            Window.isFullscreen = true;
            Window.FullscreenMode();
            Rescale();
        }

        public static void WindowMode()
        {
            Window.isFullscreen = false;
            Window.WindowMode(960, 540);
            Rescale();
        }

        public static void ResetSystems()
        {
            renderSys = new RenderSystem();
            behaviorSys = new BehaviorSystem();
            uiSys = new UISystem();
            audioSys = new AudioSystem();
            objectSys = new GameObjectSystem();
            physicSys = new PhysicsSystem();
            spawnSys = new SpawnSystem();
            weaponSys = new WeaponSystem();
            gameStateSys = new GameStateSystem();
        }

        private static void Rescale()
        {
            objectSys.RescaleGameObjectPositions();
            physicSys.RescaleCollider();
            World.WORLD_HEIGHT = (int)(World.WORLD_HEIGHT * Window.WINDOW_SCALEFACTOR_X);
            World.WORLD_WIDTH = (int)(World.WORLD_WIDTH * Window.WINDOW_SCALEFACTOR_Y);
        }


    }
}
