using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using static SDL2.SDL;
using static SDL2.SDL_ttf;
using static SDL2.SDL_image;
using static SDL2.SDL_mixer;
using XnaGeometry;

namespace gpp2019_haifischflosse
{
    public class AirPong
    { 
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello AirPong");
            Console.Read();
        }
        /*

        bool quit = false;
        bool gameOver = false;
        public static bool debug = false;
        SDL_Event e;

        //GameManager
        GameManager gm = GameManager.Gm;

        bool isFullscreen = false;

        //The surface contained by the window
        public static IntPtr surface;

        //Constraint aeras
        ConstraintArea completeWindow;
        ConstraintArea halfLeftWindow;
        ConstraintArea halfRightWindow;

        //Text
        Text fps;
        Text scoreP1;
        Text scoreP2;
        Text goal;
        Text winText;
        //ShowText bool
        bool goalShot = false;

        //Arena
        Sprite arena;

        //Player
        Player one;
        Player two;
        Ball ball;

        Vector2 oneStartPos;
        Vector2 twoStartPos;
        Vector2 ballStartPos;

        //Goal
        Goal goalOne;
        Goal goalTwo;

        Vector2 goalOnePos;
        Vector2 goalTwoPos;

        //Audio
        Music titleSong;
        SoundFX collisionSound;
        SoundFX goalSound;

        //Buttons
        Button playButton;             

        private bool showFps;

        public AirPong()
        {
            InitGame();
        }

        public static void Main()
        {

        }

        private void InitGame()
        {
            showFps = true;

            //Init SimpleDirectMediaLayer2 + SDL Audio
            if (SDL_Init(SDL_INIT_EVERYTHING | SDL_INIT_AUDIO) < 0)
            {
                Console.WriteLine(SDL_GetError());
                return;
            }

            //Initialize SDL_mixer
            if (Mix_OpenAudio(44100, MIX_DEFAULT_FORMAT, 2, 2048) < 0)
            {
                Console.WriteLine("SDL_mixer could not initialize!");
            }

            //Init SDL2_TrueTypeText 
            if (TTF_Init() < 0)
            {
                Console.WriteLine("Failed to load TTF!");
                return;
            }

            //Create window
            //window = new Window("AirPong", 960, 540, SDL_WindowFlags.SDL_WINDOW_SHOWN);

            //Get window surface
            surface = SDL_GetWindowSurface(Window.window);
            if (surface == System.IntPtr.Zero)
            {
                Console.WriteLine(SDL_GetError());
                return;
            }
            SDL_UpdateWindowSurface(surface);

            //Constraint Areas
            completeWindow = new ConstraintArea(
                new SDL_Rect
                {
                    x = 0,
                    y = 0,
                    w = Window.CURRENT_SCREEN_WIDTH,
                    h = Window.CURRENT_SCREEN_HEIGHT
                }
            );

            halfLeftWindow = new ConstraintArea(
                new SDL_Rect
                {
                    x = 0,
                    y = 0,
                    w = (int)(Window.CURRENT_SCREEN_WIDTH * 0.5d),
                    h = Window.CURRENT_SCREEN_HEIGHT
                }
            );

            halfRightWindow = new ConstraintArea(
                new SDL_Rect
                {
                    x = (int)(Window.CURRENT_SCREEN_WIDTH * 0.5d),
                    y = 0,
                    w = (Window.CURRENT_SCREEN_WIDTH),
                    h = (Window.CURRENT_SCREEN_HEIGHT)
                }
            );

            //Player Initialisierung
            oneStartPos = new Vector2(0 + Window.CURRENT_SCREEN_WIDTH * 0.08f, Window.CURRENT_SCREEN_HEIGHT / 2);
            twoStartPos = new Vector2(Window.CURRENT_SCREEN_WIDTH - Window.CURRENT_SCREEN_WIDTH * 0.08f, Window.CURRENT_SCREEN_HEIGHT / 2);
            one = new Player(PlayerID.One, oneStartPos, 128f, ControlType.DefaultWSADKeys, "assets/sprites/playerone.png", 1, 2, 2, halfLeftWindow);
            gm.AddGameObject(one);
            two = new Player(PlayerID.Two, twoStartPos, 128f, ControlType.ArrowKeys, "assets/sprites/playertwo.png", 1, 2, 2, halfRightWindow);
            gm.AddGameObject(two);

            //Arena Initialisierung
            arena = new Sprite("assets/sprites/arena.png", 1, 1, 1);
            gm.AddGameObject(arena);

            //Ball Initialisierung
            ballStartPos = new Vector2(Window.CURRENT_SCREEN_WIDTH / 2, Window.CURRENT_SCREEN_HEIGHT / 2);
            ball = new Ball(ballStartPos, 35f, 10, 10, 91, completeWindow);
            gm.AddGameObject(ball);

            //Goal Initialisierung
            goalOnePos = new Vector2(0, Window.CURRENT_SCREEN_HEIGHT / 2);
            goalOne = new Goal(goalOnePos, "assets/sprites/blue_goal.png", 1, 1, 1, one);
            gm.AddGameObject(goalOne);
            goalTwoPos = new Vector2(Window.CURRENT_SCREEN_WIDTH, Window.CURRENT_SCREEN_HEIGHT / 2);
            goalTwo = new Goal(goalTwoPos, "assets/sprites/red_goal.png", 1, 1, 1, two);
            gm.AddGameObject(goalTwo);

            //Text Initialisierung
            fps = new Text(FrameInfo.GetFpsText(), new Vector2(0, 0));
            gm.AddGameObject(fps);
            scoreP1 = new Text("Score P1: " + one.Score, new Vector2(500, 0));
            gm.AddGameObject(scoreP1);
            scoreP2 = new Text("Score P2: " + two.Score, new Vector2(700, 0));
            gm.AddGameObject(scoreP2);
            goal = new Text("GOAL!!!", new Vector2(Window.CURRENT_SCREEN_WIDTH / 2 - 50, Window.CURRENT_SCREEN_HEIGHT / 2), 35);
            goal.TextColor = new SDL_Color { r = 255, g = 0, b = 0, a = 255 };
            gm.AddGameObject(goal);
            winText = new Text("Player " + "" + " won!", new Vector2(Window.CURRENT_SCREEN_WIDTH / 2 - 120, Window.CURRENT_SCREEN_HEIGHT / 2 - 100), 40);
            winText.TextColor = new SDL_Color { r = 255, g = 165, b = 0, a = 255 };
            gm.AddGameObject(winText);

            //Audio Initialisierung
            titleSong = new Music("assets/audio/titlesong.mp3");
            collisionSound = new SoundFX("assets/audio/collision_sound.wav");
            goalSound = new SoundFX("assets/audio/goalsound.wav");

            //Play title music
            titleSong.PlayMusic();

            playButton = new Button(new SDL_Rect
                {
                    x = (int)(Window.CURRENT_SCREEN_WIDTH * 0.5d),
                    y = 0,
                    w = (Window.CURRENT_SCREEN_WIDTH),
                    h = (Window.CURRENT_SCREEN_HEIGHT)
                }
            );


        }

        public void GameLoop()
        {
            while (true)
            {
                TimeInfo.Begin();
                if (quit == true)
                {
                    break;
                }
                ProcessInput();
                UpdateSimulation();
                RenderFrame();
                TimeInfo.Sleep(16.6);
                TimeInfo.End();
                FrameInfo.CalculateFrameInfo(
                    TimeInfo.Time,
                    TimeInfo.DeltaTime
                );
            }
            CleanUpInitializedSubsystems();
        }

        void ProcessInput()
        {
            //Quit Input
            while (SDL_PollEvent(out e) != 0)
            {
                //Key Event for Window X Button to quit the game
                if (InputHandler.Quit(e))
                {
                    quit = true;
                }
                else if (InputHandler.Reset(e))
                {
                    winText.Msg = "Resetting";
                    RestartGame();
                }
                else if(InputHandler.Debug(e))
                {
                    if (!debug)
                    {
                        debug = true;
                    }
                    else
                    {
                        debug = false;
                    }

                }
                //Key Event for Space for Fullscreen or Window Mode
                else if (InputHandler.FullScreen(e))
                {
                    if (!isFullscreen)
                    {
                        isFullscreen = true;
                        Window.FullscreenMode();
                        Window.PrintWindowData();
                    }
                    else
                    {
                        isFullscreen = false;
                        Window.WindowMode(960, 540);
                        Window.PrintWindowData();
                    }

                    arena.Rescale();

                    //Loop over a list of all IUpdateable
                    for (int i = gm.GameObjects.Count; i > 0; i--)
                    {
                        if (gm.GameObjects[i - 1] is IRescaleable rescaleable)
                        {
                            rescaleable.Rescale();
                        }
                    }
                }
                //Mouse Event 
                else if (e.type == SDL_EventType.SDL_MOUSEBUTTONDOWN)
                {
                    //left button down
                    if (e.button.button == SDL_BUTTON_LEFT)
                    {
                        Vector2 clickPos = new Vector2(e.motion.x, e.motion.y);
                        playButton.IsClicked(clickPos);

                        //woanders registriere eigene methode die durch das event ausgelöst werden soll
                        //Button.buttonClicked += loadNextLevel;
                    }
                }

                else if (!gameOver && !goalShot)
                {
                    //Key Events for player movement
                    one.controls.ProcessKeyboardInput(e);
                    
                    two.controls.ProcessKeyboardInput(e);
                }
            }

        }


        void UpdateSimulation()
        {
            CheckCollisions(TimeInfo.DeltaTime);
            
            //Update Arena Sprite Position
            arena.Position = new Vector2(0 + (Window.CURRENT_SCREEN_WIDTH / 2), 0 + (Window.CURRENT_SCREEN_HEIGHT / 2));

            //Loop over a list of all IUpdateable
            for (int i = gm.GameObjects.Count; i > 0; i--)
            {
                if (gm.GameObjects[i - 1] is IUpdateable updateable)
                {
                    updateable.Update(TimeInfo.DeltaTime);
                }
            }

            //Loop over a list of all IMoveable
            for (int i = gm.GameObjects.Count; i > 0; i--)
            {
                if (gm.GameObjects[i - 1] is IMoveable moveable)
                {
                    moveable.Move(TimeInfo.DeltaTime);
                }
            }

            //Loop over a list of all IAnimating
            for (int i = gm.GameObjects.Count; i > 0; i--)
            {
                if (gm.GameObjects[i - 1] is IAnimating animating)
                {
                    animating.Animate();
                }
            }

            //Loop over a list of all DelayedActions
            for (int i = gm.DelayedActions.Count; i > 0; i--)
            {
                if (gm.DelayedActions[i - 1] is DelayedAction delayedAction)
                {
                    delayedAction.Update(TimeInfo.DeltaTimeSec);
                }
            }

            //Update Text
            scoreP2.Msg = "Score P2: " + two.Score.ToString();
            scoreP1.Msg = "Score P1: " + one.Score.ToString();
            UpdateFPS();
        }

        void RenderFrame()
        {
            SDL_RenderClear(Renderer.renderer);

            //Loop over a list of all IRenderables
            for (int i = gm.GameObjects.Count; i > 0; i--)
            {
                if(gm.GameObjects[i - 1] is IRenderable renderable)
                {
                    renderable.Render();
                }
            }

            SDL_RenderPresent(Renderer.renderer);
            CleanUp();
        }

        private void CleanUpInitializedSubsystems()
        {
            //Loop over a list of all IFreeables
            for (int i = gm.GameObjects.Count; i > 0; i--)
            {
                if (gm.GameObjects[i - 1] is IFreeable freeable)
                {
                    freeable.Free();
                }
            }

            IMG_Quit();
            TTF_Quit();
            Mix_Quit();
            SDL_Quit();
        }

        private void CheckCollisions(double deltaTime)
        {
            //Player one collisions with Ball?
            if (one.Collider.CheckCircleCircleCollision(one.Collider as CircleCollider, ball.Collider as CircleCollider))
            {
                //Verarbeitung
                Vector2[] result = two.Collider.CalculatCircleCircleCollisionReaction(
                    deltaTime,
                    one.Position,
                    ball.Position,
                    one.Velocity,
                    ball.Velocity,
                    one.Force,
                    ball.Force,
                    one.Mass,
                    ball.Mass,
                    (one.Collider as CircleCollider).Radius,
                    (ball.Collider as CircleCollider).Radius
                );

                one.Position = result[0]; // Player wird von Collision eigentlich nicht beeinträchtigt
                ball.Position = result[1];
                one.Velocity = result[2]; // Player wird von Collision eigentlich nicht beeinträchtigt
                ball.Velocity = result[3];
                collisionSound.PlaySoundFX();
            }

            //Player two collisions with Ball?
            if (two.Collider.CheckCircleCircleCollision(two.Collider as CircleCollider, ball.Collider as CircleCollider))
            {
                //Verarbeitung
                Vector2[] result = two.Collider.CalculatCircleCircleCollisionReaction(
                    deltaTime,
                    two.Position,
                    ball.Position,
                    two.Velocity,
                    ball.Velocity,
                    two.Force,
                    ball.Force,
                    two.Mass,
                    ball.Mass,
                    (two.Collider as CircleCollider).Radius,
                    (ball.Collider as CircleCollider).Radius
                );

                two.Position = result[0]; // Player wird von Collision eigentlich nicht beeinträchtigt
                ball.Position = result[1];
                two.Velocity = result[2]; // Player wird von Collision eigentlich nicht beeinträchtigt
                ball.Velocity = result[3];
                collisionSound.PlaySoundFX();
            }

            //Ball kollidiert mit Goal one von Spieler one -> Punkt für Spieler two
            if (!goalShot)
            {
                if (ball.Collider.CheckCircleRectangleCollision(ball.Collider as CircleCollider, goalOne.Collider as RectangleCollider) && goalOne.Owner.Equals(one))
                {
                    goalSound.PlaySoundFX();
                    two.Score += 1;
                    goalShot = true;
                    gm.AddDelyedAction(new DelayedAction(Goal, 1));
                    if (two.Score == 3)
                    {
                        GameWon(2);
                    }

                    //SDL_Delay(800);
                }
            }


            //Ball kollidiert mit Goal two von Spieler two -> Punkt für Spieler one
            if (!goalShot)
            {
                if (ball.Collider.CheckCircleRectangleCollision(ball.Collider as CircleCollider, goalTwo.Collider as RectangleCollider) && goalTwo.Owner.Equals(two))
                {
                    goalSound.PlaySoundFX();
                    one.Score += 1;
                    goalShot = true;
                    gm.AddDelyedAction(new DelayedAction(Goal, 1));
                    if (one.Score == 3)
                    {
                        GameWon(1);
                    }

                    //SDL_Delay(800);
                }
            }


        }

        private void UpdateFPS()
        {
            //Refresh text and color of FPS Object
            fps.Msg = FrameInfo.GetFpsText();
            fps.TextColor = FrameInfo.GetFpsColor();
        }

        private void CleanUp()
        {

            fps.Free();
            scoreP1.Free();
            scoreP2.Free();
            winText.Free();
            goal.Free();


            SDL_RenderClear(Renderer.renderer);
        }

        private void ResetPositions()
        {
            Vector2 zero = Vector2.Zero;

            one.Force = zero;
            one.Velocity = zero;
            one.Position = oneStartPos;

            two.Force = zero;
            two.Velocity = zero;
            two.Position = twoStartPos;

            ball.Force = zero;
            ball.Velocity = zero;
            ball.Position = ballStartPos;
        }

        private void RestartGame()
        {
            //delay = new DelayedAction(SetGameOverState, 3);
            gm.AddDelyedAction(new DelayedAction(SetGameOverState, 3));
            //gameObjects.Add(delay);

            gameOver = true;
            ResetPositions();

            one.Score = 0;
            two.Score = 0;

            //UpdateSimulation();
            //RenderFrame();


            //SDL_Delay(2000);
        }

        private void SetGameOverState()
        {

            if (gameOver)
            {
                gameOver = false;
            }
            else
            {
                gameOver = true;
            }
        }

        private void Goal()
        {
            goalShot = false;
            ResetPositions();
        }

        private void GameWon(int winner)
        {
            winText.Msg = "Player " + winner + " Won!";
            gameOver = true;
            RestartGame();
        } */
    } 
}
