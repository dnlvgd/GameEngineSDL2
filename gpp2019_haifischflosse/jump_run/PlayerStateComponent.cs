using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gpp2019_haifischflosse;
using SDL2;
using XnaGeometry;
using static SDL2.SDL;

namespace gpp2019_haifischflosse
{

    public class PlayerStateComponent : BehaviorComponent, IBehavior
    {
        private int indexofLastEvent = 0;

        public static bool PlayerIsGrounded { get; set; } = false;
        public static int SpawnIDArrowBullet { get; set; } = 1;

        public static double JumpStateDuration { get; set; } = 600.0D;
        public static double ShootBowStateDuration { get; set; } = 600.0D;

        public static double CrouchWalkFactor { get; set; } = 1D;
        public static double WalkFactor { get; set; } = 2D;
        public static double RunFactor { get; set; } = 4D;
        public static double JumpFactor { get; set; } = 0.008D;

        public static double forceX_duringFall { get; set; } = 1.0D;
        public static double forceY_duringFall { get; set; } = 1.0D;
        public static double factorX_duringFall { get; set; } = 0.01D; 
        public static double factorY_duringFall { get; set; } = 0.015D;

        //Caller
        public IState currentState;

        public PlayerStateComponent()
        {
            currentState = new StandIdleState();
        }

        public override void Update(double deltaTime)
        {
            PlayerIsGrounded = EventSystem.Instance.CheckEvent("PlayerIsGrounded", indexofLastEvent);
            indexofLastEvent = EventSystem.Instance.GetMyIndex();

            currentState.Update(deltaTime, Owner);
            CenterCameraOverPlayer();
        }

        public override void HandleInput(Input input)
        {
            IState nextState = currentState.HandleInputOnExit(input, Owner);
            if (nextState != currentState)
            {
                currentState = nextState;
                currentState.OnEnter(Owner);
            }

            if (Window.isDebug)
            {
                //Console.WriteLine("State: " + currentState.ToString());
            }

        }

        public void CenterCameraOverPlayer()
        {
            Camera.camera.x = (int)Owner.Position.X - Window.CURRENT_SCREEN_WIDTH / 2;
            Camera.camera.y = (int)Owner.Position.Y - Window.CURRENT_SCREEN_HEIGHT / 2;
            Camera.camera.w = (int)Window.CURRENT_SCREEN_WIDTH;
            Camera.camera.h = (int)Window.CURRENT_SCREEN_HEIGHT;

            //Keep the camera in bounds
            if (Camera.camera.x < 0)
            {
                Camera.camera.x = 0;
            }
            if (Camera.camera.y < 0)
            {
                Camera.camera.y = 0;
            }
            if (Camera.camera.x > World.WORLD_WIDTH - Camera.camera.w)
            {
                Camera.camera.x = World.WORLD_WIDTH - Camera.camera.w;
            }
            if (Camera.camera.y > 1080*Window.SCALEFACTOR_Y - Camera.camera.h)
            {
                Camera.camera.y = (int)(1080 * Window.SCALEFACTOR_Y) - Camera.camera.h;
            }
        }
    }

    class StandIdleState : IState
    {
        Vector2 currentVel = Vector2.Zero;

        //Implementation
        public void OnEnter(GameObject gameObject)
        {
            Game.renderSys.PlayAnimation(gameObject.Name, Animation.IdleRight);
            if (Window.isDebug)
            {
                Console.WriteLine("Enter IdleState");
            }
        }

        public void Update(double dt, GameObject gameObject)
        {
            MoveComponent mc = gameObject.GetComponent<MoveComponent>() as MoveComponent;
            if (mc != null)
            {
                mc.Velocity = currentVel;
            }
        }

        public IState HandleInputOnExit(Input input, GameObject gameObject)
        {
            if (!PlayerStateComponent.PlayerIsGrounded)
            {
                return new FallRightState(0.0D);
            }
            else if(input == Input.D_Press)
            {
                return new WalkRightState();
            }
            else if(input == Input.A_Press)
            {
                return new WalkLeftState();
            }
            else if(input == Input.Strg_Press)
            {
                return new CrouchIdleState();
            }
            else if(input == Input.Space_Press && PlayerStateComponent.PlayerIsGrounded)
            {
                return new JumpRightState();
            }
            else if (input == Input.MouseButtonLeft_Press)
            {
                return new ShootBowState();
            }
            else
            {
                return this;
            }
            
        }
    }
    
    class WalkRightState : IState
    {
        //Implementation
        public void OnEnter(GameObject gameObject)
        {
            Game.renderSys.PlayAnimation(gameObject.Name, Animation.WalkRight);
            if (Window.isDebug)
            {
                Console.WriteLine("Enter WalkRightState");
            }
        }

        public void Update(double dt, GameObject gameObject)
        {
            WalkRight(gameObject, PlayerStateComponent.WalkFactor);
        }

        private void WalkRight(GameObject gameObject, double factor)
        {
            MoveComponent mc = gameObject.GetComponent<MoveComponent>() as MoveComponent;
            if (mc != null)
            {
                mc.Velocity = Vector2.UnitX * factor;
            }
        }

        public IState HandleInputOnExit(Input input, GameObject gameObject)
        {
            if (!PlayerStateComponent.PlayerIsGrounded)
            {
                return new FallRightState();
            }
            else if (input == Input.D_Release || input == Input.Shift_Release)
            {
                return new StandIdleState();
            }
            else if (input == Input.Shift_Press)
            {
                return new RunRightState();
            }
            else if (input == Input.A_Press)
            {
                return new WalkLeftState();
            }
            else if (input == Input.Space_Press && PlayerStateComponent.PlayerIsGrounded)
            {
                return new JumpRightState();
            }
            else if (input == Input.MouseButtonLeft_Press)
            {
                return new ShootBowState();
            }
            else
            {
                return this;
            }
        }
    }

    class WalkLeftState : IState
    {
        //Implementation
        public void OnEnter(GameObject gameObject)
        {
            Game.renderSys.PlayAnimation(gameObject.Name, Animation.WalkLeft);
            if (Window.isDebug)
            {
                Console.WriteLine("Enter WalkLeftState");
            }
        }

        public void Update(double dt, GameObject gameObject)
        {
            WalkLeft(gameObject, PlayerStateComponent.WalkFactor);
        }

        private void WalkLeft(GameObject gameObject, double factor)
        {
            MoveComponent mc = gameObject.GetComponent<MoveComponent>() as MoveComponent;
            if (mc != null)
            {
                mc.Velocity = -Vector2.UnitX * factor;
            }
        }

        public IState HandleInputOnExit(Input input, GameObject gameObject)
        {
            if (!PlayerStateComponent.PlayerIsGrounded)
            {
                return new FallLeftState();
            }
            else if (input == Input.A_Release || input == Input.Shift_Release)
            {
                return new StandIdleState();
            }
            else if (input == Input.Shift_Press)
            {
                return new RunLeftState();
            }
            else if (input == Input.D_Press)
            {
                return new WalkRightState();
            }
            else if (input == Input.Space_Press && PlayerStateComponent.PlayerIsGrounded)
            {
                return new JumpLeftState();
            }
            else if (input == Input.MouseButtonLeft_Press)
            {
                return new ShootBowState();
            }
            else
            {
                return this;
            }
        }
    }

    class RunRightState : IState
    {
        private double maxSlideCooldown;
        private double remainingSlideCooldown;

        public RunRightState()
        {
            this.maxSlideCooldown = 0;
            this.remainingSlideCooldown = maxSlideCooldown;
        }

        public RunRightState(double slideCooldown)
        {
            this.maxSlideCooldown = slideCooldown;
            this.remainingSlideCooldown = maxSlideCooldown;
        }

        public void OnEnter(GameObject gameObject)
        {
            Game.renderSys.PlayAnimation(gameObject.Name, Animation.RunRight);
            if (Window.isDebug)
            {
                Console.WriteLine("Enter RunRightState");
            }
        }

        public void Update(double dt, GameObject gameObject)
        {
            remainingSlideCooldown -= dt;
            RunRight(gameObject, PlayerStateComponent.RunFactor);
        }

        private void RunRight(GameObject gameObject, double factor)
        {
            MoveComponent mc = gameObject.GetComponent<MoveComponent>() as MoveComponent;
            if (mc != null)
            {
                mc.Velocity = Vector2.UnitX * factor;
            }
        }

        public IState HandleInputOnExit(Input input, GameObject gameObject)
        {
            bool canSlide = remainingSlideCooldown <= 0;
            if (!PlayerStateComponent.PlayerIsGrounded)
            {
                return new FallRightState();
            }
            else if (input == Input.S_Press && canSlide)
            {
                return new SlideRightState();
            }
            else if (input == Input.Shift_Release || input == Input.D_Release)
            {
                return new WalkRightState();
            }
            else if (input == Input.Space_Press && PlayerStateComponent.PlayerIsGrounded)
            {
                return new JumpRightState();
            }
            else
            {
                return this;
            }
        }
    }

    class RunLeftState : IState
    {
        private double maxSlideCooldown;
        private double remainingSlideCooldown;

        public RunLeftState()
        {
            this.maxSlideCooldown = 0;
            this.remainingSlideCooldown = maxSlideCooldown;
        }

        public RunLeftState(double slideCooldown)
        {
            this.maxSlideCooldown = slideCooldown;
            this.remainingSlideCooldown = maxSlideCooldown;
        }

        public void OnEnter(GameObject gameObject)
        {
            Game.renderSys.PlayAnimation(gameObject.Name, Animation.RunLeft);
            if (Window.isDebug)
            {
                Console.WriteLine("Enter RunLeftState");
            }
        }

        public void Update(double dt, GameObject gameObject)
        {
            remainingSlideCooldown -= dt;
            RunLeft(gameObject, PlayerStateComponent.RunFactor);
        }

        private void RunLeft(GameObject gameObject, double factor)
        {
            MoveComponent mc = gameObject.GetComponent<MoveComponent>() as MoveComponent;
            if (mc != null)
            {
                mc.Velocity = -Vector2.UnitX * factor;
            }
        }

        public IState HandleInputOnExit(Input input, GameObject gameObject)
        {
            bool canSlide = remainingSlideCooldown <=  0;
            if (!PlayerStateComponent.PlayerIsGrounded)
            {
                return new FallLeftState();
            }
            else if (input == Input.S_Press && canSlide)
            {
                return new SlideLeftState();
            }
            else if (input == Input.Shift_Release || input == Input.A_Release)
            {
                return new WalkLeftState();
            }
            else if (input == Input.Space_Press && PlayerStateComponent.PlayerIsGrounded)
            {
                return new JumpLeftState();
            }
            else
            {
                return this;
            }
        }
    }

    class SlideRightState : IState
    {
        private double maxSlideDuration;
        private double remainingSlideDuration;

        public SlideRightState()
        {
            maxSlideDuration = 1000.0D;
            remainingSlideDuration = maxSlideDuration;
        }

        public void OnEnter(GameObject gameObject)
        {
            Game.renderSys.PlayAnimation(gameObject.Name, Animation.SlideRight);
            if (Window.isDebug)
            {
                Console.WriteLine("Enter SlideRightState");
            }
        }

        public void Update(double dt, GameObject gameObject)
        {
            remainingSlideDuration -= dt;
        }

        public IState HandleInputOnExit(Input input, GameObject gameObject)
        {
            bool shouldEnd = remainingSlideDuration <= 0;
            if (!PlayerStateComponent.PlayerIsGrounded)
            {
                return new FallRightState();
            }
            else if (input == Input.S_Release || shouldEnd || input == Input.Shift_Release)
            {
                return new StandUpRightState();
            }
            else
            {
                return this;
            }
        }
    }

    class SlideLeftState : IState
    {
        private double maxSlideDuration;
        private double remainingSlideDuration;

        public SlideLeftState()
        {
            maxSlideDuration = 1000.0D;
            remainingSlideDuration = maxSlideDuration;
        }

        public void OnEnter(GameObject gameObject)
        {
            Game.renderSys.PlayAnimation(gameObject.Name, Animation.SlideLeft);
            if (Window.isDebug)
            {
                Console.WriteLine("Enter SlideLeftState");
            }
        }

        public void Update(double dt, GameObject gameObject)
        {
            remainingSlideDuration -= dt;
        }

        public IState HandleInputOnExit(Input input, GameObject gameObject)
        {
            bool shouldEnd = remainingSlideDuration <= 0;
            if (!PlayerStateComponent.PlayerIsGrounded)
            {
                return new FallLeftState();
            }
            else if (input == Input.S_Release || shouldEnd || input == Input.Shift_Release)
            {
                return new StandUpLeftState();
            }
            else
            {
                return this;
            }
        }
    }

    class StandUpRightState : IState
    {
        private double maxStandUpAnimationDuration;
        private double remainingStandUpAnimationDuration;

        public StandUpRightState()
        {
            maxStandUpAnimationDuration = 250.0D;
            remainingStandUpAnimationDuration = maxStandUpAnimationDuration;
        }

        public void OnEnter(GameObject gameObject)
        {
            Game.renderSys.PlayAnimation(gameObject.Name, Animation.StandUpRight);
            if (Window.isDebug)
            {
                Console.WriteLine("Enter StandUpRightState");
            }
        }

        public void Update(double dt, GameObject gameObject)
        {
            remainingStandUpAnimationDuration -= dt;
        }

        public IState HandleInputOnExit(Input input, GameObject gameObject)
        {
            bool animationFinished = remainingStandUpAnimationDuration <= 0;
            if (!PlayerStateComponent.PlayerIsGrounded)
            {
                return new FallRightState();
            }
            else if (animationFinished)
            {
                return new RunRightState(750);
            }
            else
            {
                return this;
            }
        }
    }

    class StandUpLeftState : IState
    {
        private double maxStandUpAnimationDuration;
        private double remainingStandUpAnimationDuration;

        public StandUpLeftState()
        {
            maxStandUpAnimationDuration = 250.0D;
            remainingStandUpAnimationDuration = maxStandUpAnimationDuration;
        }

        public void OnEnter(GameObject gameObject)
        {
            Game.renderSys.PlayAnimation(gameObject.Name, Animation.StandUpLeft);
            if (Window.isDebug)
            {
                Console.WriteLine("Enter StandUpLeftState");
            }
        }

        public void Update(double dt, GameObject gameObject)
        {
            remainingStandUpAnimationDuration -= dt;
        }

        public IState HandleInputOnExit(Input input, GameObject gameObject)
        {
            bool animationFinished = remainingStandUpAnimationDuration <= 0;
            if (!PlayerStateComponent.PlayerIsGrounded)
            {
                return new FallLeftState();
            }
            else if (animationFinished)
            {
                return new RunLeftState(750);
            }
            else
            {
                return this;
            }
        }
    }

    class JumpRightState : IState
    {
        private double jumpStateAnimationDuration;
        private double remainingJumpStateAnimationDuration;

        public JumpRightState()
        {
            jumpStateAnimationDuration = PlayerStateComponent.JumpStateDuration; // in ms
            remainingJumpStateAnimationDuration = jumpStateAnimationDuration;
        }

        //Implementation
        public void OnEnter(GameObject gameObject)
        {
            Game.renderSys.PlayAnimation(gameObject.Name, Animation.JumpRight);
            if (Window.isDebug)
            {
                Console.WriteLine("Enter JumpRightState");
            }
        }

        public void Update(double dt, GameObject gameObject)
        {
            remainingJumpStateAnimationDuration -= dt;

            PhysicComponent physicCp = gameObject.GetComponent<PhysicComponent>() as PhysicComponent;
            if(physicCp != null)
            {
                physicCp.Force = -Vector2.UnitY * PlayerStateComponent.JumpFactor;
            }
        }

        private void ResetForce(GameObject gameObject)
        {
            PhysicComponent physicCp = gameObject.GetComponent<PhysicComponent>() as PhysicComponent;
            if (physicCp != null)
            {
                physicCp.Force = Vector2.Zero;
            }
        }

        public IState HandleInputOnExit(Input input, GameObject gameObject)
        {
            if (remainingJumpStateAnimationDuration < 0)
            {
                ResetForce(gameObject);
                if (PlayerStateComponent.PlayerIsGrounded)
                {

                    return new StandIdleState();
                }
                else
                {
                    return new FallRightState();
                }
            }
            else
            {
                return this;
            }
        }
    }

    class JumpLeftState : IState
    {
        private double jumpStateAnimationDuration;
        private double remainingJumpStateAnimationDuration;

        public JumpLeftState()
        {
            jumpStateAnimationDuration = PlayerStateComponent.JumpStateDuration; // in ms
            remainingJumpStateAnimationDuration = jumpStateAnimationDuration;
        }

        //Implementation
        public void OnEnter(GameObject gameObject)
        {
            Game.renderSys.PlayAnimation(gameObject.Name, Animation.JumpLeft);
            if (Window.isDebug)
            {
                Console.WriteLine("Enter JumpLeftState");
            }
        }

        public void Update(double dt, GameObject gameObject)
        {
            remainingJumpStateAnimationDuration -= dt;

            PhysicComponent physicCp = gameObject.GetComponent<PhysicComponent>() as PhysicComponent;
            if (physicCp != null)
            {
                physicCp.Force = -Vector2.UnitY * PlayerStateComponent.JumpFactor;
            }
        }

        private void ResetForce(GameObject gameObject)
        {
            PhysicComponent physicCp = gameObject.GetComponent<PhysicComponent>() as PhysicComponent;
            if (physicCp != null)
            {
                physicCp.Force = Vector2.Zero;
            }
        }

        public IState HandleInputOnExit(Input input, GameObject gameObject)
        {
            if (remainingJumpStateAnimationDuration < 0)
            {
                ResetForce(gameObject);
                if (PlayerStateComponent.PlayerIsGrounded)
                {
                    return new StandIdleState();
                }
                else
                {
                    return new FallLeftState();
                }
            }
            else
            {
                return this;
            }
        }
    }

    class FallRightState : IState
    {
        private double forceX_duringFall;
        private double forceY_duringFall;

        public FallRightState()
        {
            this.forceX_duringFall = PlayerStateComponent.forceX_duringFall;
            this.forceY_duringFall = PlayerStateComponent.forceY_duringFall;
        }

        public FallRightState(double forceX_duringJump)
        {
            this.forceX_duringFall = forceX_duringJump;
            this.forceY_duringFall = PlayerStateComponent.forceY_duringFall;
        }

        //Implementation
        public void OnEnter(GameObject gameObject)
        {
            Game.renderSys.PlayAnimation(gameObject.Name, Animation.AirFallRight);
            if (Window.isDebug)
            {
                Console.WriteLine("Enter FallRightState");
            }
        }

        public void Update(double dt, GameObject gameObject)
        {
            ApplyDownForce(gameObject, new Vector2(forceX_duringFall * PlayerStateComponent.factorX_duringFall, forceY_duringFall * PlayerStateComponent.factorY_duringFall));
        }

        private void ApplyDownForce(GameObject gameObject, Vector2 force)
        {
            PhysicComponent physicCp = gameObject.GetComponent<PhysicComponent>() as PhysicComponent;
            if (physicCp != null)
            {
                physicCp.Force = force;
            }
        }

        private void ResetForce(GameObject gameObject)
        {
            PhysicComponent physicCp = gameObject.GetComponent<PhysicComponent>() as PhysicComponent;
            if (physicCp != null)
            {
                physicCp.Force = Vector2.Zero;
            }
        }

        public IState HandleInputOnExit(Input input, GameObject gameObject)
        {
            if (PlayerStateComponent.PlayerIsGrounded)
            {
                ResetForce(gameObject);
                return new StandIdleState();
            }
            else if (input == Input.A_Press)
            {
                return new FallLeftState();
            }
            else
            {
                return this;
            }
        }
    }

    class FallLeftState : IState
    {
        private double forceX_duringFall;
        private double forceY_duringFall;

        public FallLeftState()
        {
            this.forceX_duringFall = PlayerStateComponent.forceX_duringFall;
            this.forceY_duringFall = PlayerStateComponent.forceY_duringFall;
        }

        public FallLeftState(double forceX_duringJump)
        {
            this.forceX_duringFall = forceX_duringJump;
            this.forceY_duringFall = PlayerStateComponent.forceY_duringFall;
        }

        //Implementation
        public void OnEnter(GameObject gameObject)
        {
            Game.renderSys.PlayAnimation(gameObject.Name, Animation.AirFallLeft);
            if (Window.isDebug)
            {
                Console.WriteLine("Enter FallLeftState");
            }
        }

        public void Update(double dt, GameObject gameObject)
        {
            ApplyDownForce(gameObject, new Vector2(-forceX_duringFall * PlayerStateComponent.factorX_duringFall, forceY_duringFall * PlayerStateComponent.factorY_duringFall));
        }

        private void ApplyDownForce(GameObject gameObject, Vector2 force)
        {
            PhysicComponent physicCp = gameObject.GetComponent<PhysicComponent>() as PhysicComponent;
            if (physicCp != null)
            {
                physicCp.Force = force;
            }
        }

        private void ResetForce(GameObject gameObject)
        {
            PhysicComponent physicCp = gameObject.GetComponent<PhysicComponent>() as PhysicComponent;
            if (physicCp != null)
            {
                physicCp.Force = Vector2.Zero;
            }
        }

        public IState HandleInputOnExit(Input input, GameObject gameObject)
        {
            if (PlayerStateComponent.PlayerIsGrounded)
            {
                ResetForce(gameObject);
                return new StandIdleState();
            }
            else if (input == Input.D_Press)
            {
                return new FallRightState();
            }
            else
            {
                return this;
            }
        }
    }

    class CrouchIdleState : IState
    {
        Vector2 currentVel = Vector2.Zero;

        //Implementation
        public void OnEnter(GameObject gameObject)
        {
            Game.renderSys.PlayAnimation(gameObject.Name, Animation.CrouchRight);
            if (Window.isDebug)
            {
                Console.WriteLine("Enter CrouchState");
            }
        }

        public void Update(double dt, GameObject gameObject)
        {
            MoveComponent mc = gameObject.GetComponent<MoveComponent>() as MoveComponent;
            if (mc != null)
            {
                mc.Velocity = currentVel;
            }
        }

        public IState HandleInputOnExit(Input input, GameObject gameObject)
        {
            if (!PlayerStateComponent.PlayerIsGrounded)
            {
                return new FallRightState(0);
            }
            else if (input == Input.Strg_Release)
            {
                return new StandIdleState();
            }
            else if (input == Input.D_Press)
            {
                return new CrouchWalkRightState();
            }
            else if (input == Input.A_Press)
            {
                return new CrouchWalkLeftState();
            }
            else if (input == Input.Space_Press && PlayerStateComponent.PlayerIsGrounded)
            {
                return new JumpRightState(); 
            }
            else
            {
                return this;
            }
        }
    }

    class CrouchWalkRightState : IState
    {
        //Implementation
        public void OnEnter(GameObject gameObject)
        {
            Game.renderSys.PlayAnimation(gameObject.Name, Animation.CrouchWalkRight);
            if (Window.isDebug)
            {
                Console.WriteLine("Enter CrouchWalkRightState");
            }
        }

        public void Update(double dt, GameObject gameObject)
        {
            CrouchWalkRight(gameObject, PlayerStateComponent.CrouchWalkFactor);
        }

        private void CrouchWalkRight(GameObject gameObject, double factor)
        {
            MoveComponent mc = gameObject.GetComponent<MoveComponent>() as MoveComponent;
            if (mc != null)
            {
                mc.Velocity = Vector2.UnitX * factor;
            }
        }

        public IState HandleInputOnExit(Input input, GameObject gameObject)
        {
            if (!PlayerStateComponent.PlayerIsGrounded)
            {
                return new FallRightState();
            }
            else if (input == Input.D_Release)
            {
                return new CrouchIdleState();
            }
            else if (input == Input.Strg_Release)
            {
                return new StandIdleState();
            }
            else if (input == Input.Space_Press && PlayerStateComponent.PlayerIsGrounded)
            {
                return new JumpRightState();
            }
            else
            {
                return this;
            }
        }
    }

    class CrouchWalkLeftState : IState
    {
        Vector2 currentVel = Vector2.Zero;

        //Implementation
        public void OnEnter(GameObject gameObject)
        {
            Game.renderSys.PlayAnimation(gameObject.Name, Animation.CrouchWalkLeft);
            if (Window.isDebug)
            {
                Console.WriteLine("Enter CrouchWalkLeftState");
            }
        }

        public void Update(double dt, GameObject gameObject)
        {
            CrouchWalkLeft(gameObject, PlayerStateComponent.CrouchWalkFactor);
        }

        private void CrouchWalkLeft(GameObject gameObject, double factor)
        {
            MoveComponent mc = gameObject.GetComponent<MoveComponent>() as MoveComponent;
            if (mc != null)
            {
                mc.Velocity = -Vector2.UnitX * factor;
            }
        }

        public IState HandleInputOnExit(Input input, GameObject gameObject)
        {
            if (!PlayerStateComponent.PlayerIsGrounded)
            {
                return new FallLeftState();
            }
            else if (input == Input.A_Release)
            {
                return new CrouchIdleState();
            }
            else if (input == Input.Strg_Release)
            {
                return new StandIdleState();
            }
            else if (input == Input.Space_Press && PlayerStateComponent.PlayerIsGrounded)
            {
                return new JumpLeftState();
            }
            else
            {
                return this;
            }
        }
    }

    class ShootBowState : IState
    {
        private double shootAnimationDuration;    // in ms
        private double remainingShootAnimationDuration;
        private bool IsAnimationShootBowLeft;
        bool createdGO;
        bool addedCP;
        bool doneCreatingGOandAddingCP;

        public ShootBowState()
        {
            this.createdGO = false;
            this.addedCP = false;
            this.doneCreatingGOandAddingCP = false;
            this.shootAnimationDuration = PlayerStateComponent.ShootBowStateDuration;
            remainingShootAnimationDuration = shootAnimationDuration;
        }

        //Implementation
        public void OnEnter(GameObject gameObject)
        {
            //use animation ShootLeft or ShootRight based on mouse pos
            if ((InputHandler.MousePosition().X + Camera.camera.x) <= gameObject.Position.X)
            {
                Game.renderSys.PlayAnimation(gameObject.Name, Animation.ShootBowLeft);
                IsAnimationShootBowLeft = true;
            }
            else
            {
                Game.renderSys.PlayAnimation(gameObject.Name, Animation.ShootBowRight);
                IsAnimationShootBowLeft = false;
            }

            if (Window.isDebug)
            {
                Console.WriteLine("Enter ShootBowState");
            }
        }

        private void SpawnArrow(GameObject playerGO, bool shootLeft)
        {
            if (!doneCreatingGOandAddingCP)
            {
                if (!createdGO)
                {
                    GameObject arrowBullet = Game.objectSys.FindGameObject("arrowBullet");

                    if (shootLeft)
                    {
                        Game.renderSys.PlayAnimation(arrowBullet.Name, Animation.FlyingArrowLeft);
                    }
                    else
                    {
                        Game.renderSys.PlayAnimation(arrowBullet.Name, Animation.FlyingArrowRight);

                    }

                    if (arrowBullet != null)
                    {
                        double xOffset = (15 * Window.SCALEFACTOR_X);
                        double yOffset = (5 * Window.SCALEFACTOR_Y);
                        EventSystem.Instance.AddEvent("CloneGameObject", arrowBullet, arrowBullet.Tag, playerGO.Position.X + xOffset, playerGO.Position.Y + yOffset, arrowBullet.Scaling.X, arrowBullet.Scaling.Y, arrowBullet.Angle, true, PlayerStateComponent.SpawnIDArrowBullet++, true, shootLeft);
                        Console.WriteLine("1. PlayerStateComponent.SpawnIDArrowBullet: "+ PlayerStateComponent.SpawnIDArrowBullet);
                        createdGO = true;
                    }
                }
                else
                {
                    //Console.WriteLine("2. PlayerStateComponent.SpawnIDArrowBullet: " + PlayerStateComponent.SpawnIDArrowBullet);
                    //GameObject newGO = Game.objectSys.FindGameObject("arrowBullet" + (PlayerStateComponent.SpawnIDArrowBullet - 1));
                    //newGO.AddComponent(true, Game.behaviorSys.CreateBehaviorComponent(new BulletStateComponent(shootLeft)));
                    addedCP = true;
                }

                if (createdGO && addedCP)
                {
                    doneCreatingGOandAddingCP = true;
                    EventSystem.Instance.AddEvent("ShootArrow", playerGO);
                }
            }
        }

        public void Update(double dt, GameObject gameObject)
        {
            remainingShootAnimationDuration -= dt;

            //spawn arrow after 80% of the animation is done
            if (remainingShootAnimationDuration <= shootAnimationDuration * (1 - 0.8))
            {
                SpawnArrow(gameObject, IsAnimationShootBowLeft);
            }
        }

        public IState HandleInputOnExit(Input input, GameObject gameObject)
        {
            if (!PlayerStateComponent.PlayerIsGrounded)
            {
                if (IsAnimationShootBowLeft)
                {
                    return new FallLeftState();
                }
                else
                {
                    return new FallRightState();
                }
            }
            else if (remainingShootAnimationDuration < 0)
            {
                return new StandIdleState();
            }
            else
            {
                return this;
            }
        }
    }


}
