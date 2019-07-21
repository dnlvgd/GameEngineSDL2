using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XnaGeometry;

namespace gpp2019_haifischflosse
{
    public class BulletStateComponent : BehaviorComponent, IBehavior
    {
        private int indexofLastEvent = 0;

        public bool ArrowHit { get; set; } = false;

        public double MaxFlyingArrowHitAnimationDuration { get; set; } = 2000.0D;
        public double MaxFlyingArrowPuffAnimationDuration { get; set; } = 1000.0D;

        public double FlyingArrowFactor { get; set; } = 0.5D;
        public double ArrowLifeTime { get; set; } = 2000.0D;

        //Caller
        public IState currentState;

        public BulletStateComponent(bool shootLeft)
        {
            currentState = new FlyingArrowState(this, shootLeft);
        }

        public override void Update(double deltaTime)
        {
            ArrowHit = false;
            ArrowHit =/* EventSystem.Instance.CheckEvent("ArrowHitTile", indexofLastEvent) ||*/ EventSystem.Instance.CheckEvent("ArrowHitEnemy", indexofLastEvent);
            indexofLastEvent = EventSystem.Instance.GetMyIndex();

            if (currentState != null)
            {
                currentState.Update(deltaTime, Owner);
            }
        }

        public override void HandleInput(Input input)
        {
            if (currentState != null)
            {
                IState nextState = currentState.HandleInputOnExit(input, Owner);
                if (nextState != currentState)
                {
                    currentState = nextState;
                    if (currentState != null)
                    {
                        currentState.OnEnter(Owner);
                    }
                }
            }

            if (Window.isDebug)
            {
                //Console.WriteLine("State: " + currentState.ToString());
            }

        }
    }

    class FlyingArrowState : IState
    {
        private double maxFlyingArrowLifetime;
        private double remainingFlyingArrowLifetime;
        private BulletStateComponent bsc;
        bool shootLeft;
        bool done = false;

        public FlyingArrowState(BulletStateComponent bsc, bool shootLeft)
        {
            this.bsc = bsc;
            this.shootLeft = shootLeft;
            this.maxFlyingArrowLifetime = bsc.ArrowLifeTime;
            this.remainingFlyingArrowLifetime = maxFlyingArrowLifetime;
        }

        //Implementation
        public void OnEnter(GameObject gameObject)
        {
            if (shootLeft)
            {
                Game.renderSys.PlayAnimation(gameObject.Name, Animation.FlyingArrowLeft);
            }
            else
            {
                Game.renderSys.PlayAnimation(gameObject.Name, Animation.FlyingArrowRight);
            }

            if (Window.isDebug)
            {
                Console.WriteLine("Enter FlyingArrowState");
            }
        }

        public void Update(double dt, GameObject gameObject)
        {
            remainingFlyingArrowLifetime -= dt;

            if (shootLeft)
            {
                MoveArrow(gameObject, -bsc.FlyingArrowFactor);
            }
            else
            {
                MoveArrow(gameObject, bsc.FlyingArrowFactor);
            }

            if (!done)
            {
                if (shootLeft)
                {
                    Game.renderSys.PlayAnimation(gameObject.Name, Animation.FlyingArrowLeft);
                }
                else
                {
                    Game.renderSys.PlayAnimation(gameObject.Name, Animation.FlyingArrowRight);
                }
                done = true;
            }
        }

        private void MoveArrow(GameObject gameObject, double factor)
        {
            MoveComponent mc = gameObject.GetComponent<MoveComponent>() as MoveComponent;
            if (mc != null)
            {
                mc.Velocity = Vector2.UnitX * factor;
            }
        }

        public IState HandleInputOnExit(Input input, GameObject gameObject)
        {
            bool lifetimeOver = remainingFlyingArrowLifetime <= 0;
            if (bsc.ArrowHit)
            {
                return new FlyingArrowHitState(bsc);
            }
            else if (lifetimeOver)
            {
                return new FlyingArrowPuffState(bsc);
            }
            else
            {
                return this;
            }
        }
    }

    class FlyingArrowHitState : IState
    {
        private double maxFlyingArrowHitAnimationDuration;
        private double remainingFlyingArrowHitAnimationDuration;
        private BulletStateComponent bsc;

        public FlyingArrowHitState(BulletStateComponent bsc)
        {
            this.bsc = bsc;
            this.maxFlyingArrowHitAnimationDuration = bsc.MaxFlyingArrowHitAnimationDuration;
            this.remainingFlyingArrowHitAnimationDuration = maxFlyingArrowHitAnimationDuration;
        }

        //Implementation
        public void OnEnter(GameObject gameObject)
        {
            Game.renderSys.PlayAnimation(gameObject.Name, Animation.FlyingArrowHit);
            EventSystem.Instance.AddEvent("ArrowHitExplosion", gameObject);

            if (Window.isDebug)
            {
                Console.WriteLine("Enter FlyingArrowHitState");
            }
        }

        public void Update(double dt, GameObject gameObject)
        {
            this.OnEnter(gameObject);

            remainingFlyingArrowHitAnimationDuration -= dt;
            StopArrow(gameObject, 0);

        }

        private void StopArrow(GameObject gameObject, double factor)
        {
            MoveComponent mc = gameObject.GetComponent<MoveComponent>() as MoveComponent;
            if (mc != null)
            {
                mc.Velocity = Vector2.UnitX * factor;
            }
        }

        public IState HandleInputOnExit(Input input, GameObject gameObject)
        {
            bool endAnimation = remainingFlyingArrowHitAnimationDuration <= 0;
            if (endAnimation)
            {
                return new EndState(bsc);          
            }
            else
            {
                return this;
            }
        }
    }

    class FlyingArrowPuffState : IState
    {
        private double maxFlyingArrowPuffAnimationDuration;
        private double remainingFlyingArrowPuffAnimationDuration;
        private BulletStateComponent bsc;

        public FlyingArrowPuffState(BulletStateComponent bsc)
        {
            this.bsc = bsc;
            this.maxFlyingArrowPuffAnimationDuration = bsc.MaxFlyingArrowPuffAnimationDuration;
            this.remainingFlyingArrowPuffAnimationDuration = maxFlyingArrowPuffAnimationDuration;
        }

        //Implementation
        public void OnEnter(GameObject gameObject)
        {
            Game.renderSys.PlayAnimation(gameObject.Name, Animation.FlyingArrowPuff);
            EventSystem.Instance.AddEvent("ArrowHitExplosion", gameObject);

            if (Window.isDebug)
            {
                Console.WriteLine("Enter FlyingArrowPuffState");
            }
        }

        public void Update(double dt, GameObject gameObject)
        {
            remainingFlyingArrowPuffAnimationDuration -= dt;
            StopArrow(gameObject, 0);
        }

        private void StopArrow(GameObject gameObject, double factor)
        {
            MoveComponent mc = gameObject.GetComponent<MoveComponent>() as MoveComponent;
            if (mc != null)
            {
                mc.Velocity = Vector2.UnitX * factor;
            }
        }

        public IState HandleInputOnExit(Input input, GameObject gameObject)
        {
            bool endAnimation = remainingFlyingArrowPuffAnimationDuration <= 0;
            if (endAnimation)
            {
                return new EndState(bsc);
            }
            else
            {
                return this;
            }
        }
    }

    class EndState : IState
    {
        public EndState(BulletStateComponent bsc)
        {
            bsc = null;
        }

        //Implementation
        public void OnEnter(GameObject gameObject)
        {
            Game.renderSys.PlayAnimation(gameObject.Name, Animation.None);

            if (Window.isDebug)
            {
                Console.WriteLine("Enter EndState");
            }
        }

        public void Update(double dt, GameObject gameObject)
        {

        }

        public IState HandleInputOnExit(Input input, GameObject gameObject)
        {
            return null;
        }
    }
    public interface IState
    {
        //Declaration
        void OnEnter(GameObject gameObject);
        void Update(double dt, GameObject gameObject);
        IState HandleInputOnExit(Input input, GameObject gameObject);
    }
}
