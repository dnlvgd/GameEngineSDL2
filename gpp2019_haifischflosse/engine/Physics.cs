using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XnaGeometry;

namespace gpp2019_haifischflosse
{ /*
    public class Physics
    {
        public static Vector2[] BallConstraintMovement(
            double deltaTime,
            Vector2 force,
            Vector2 velocity,
            Vector2 position,
            double mass,
            ConstraintArea constArea,
            Collider collider)
        {
            Vector2[] oldMovement = new Vector2[] { force, velocity, position };
            
            if (constArea == null)
            { 
                Console.WriteLine("Physics: called BallConstraintMovement(...,...) without a ConstraintArea.");
                return oldMovement;
            }

            if (collider == null)
            {
                Console.WriteLine("Physics: called BallConstraintMovement(...,...) without a Collider.");
                return oldMovement;
            }

            //get CircleCollider from Collider
            if (!(collider is CircleCollider ballCollider))
            {
                Console.WriteLine("Physics: called BallConstraintMovement(...,...) without a Collider that can be cast to a CircleCollider.");
                return oldMovement;
            }

            force = CalculateNewDragForce(deltaTime, force, velocity);
            //check if the ball collides with one of the Walls/Lines and if yes,
            //calculate the reflection vector and use it as newVelocity
            foreach (var wall in constArea.Lines)
            {
                //circle & line Collision?
                if (collider.CheckCollisionCircleLine(ballCollider, wall))
                {
                    //reflect the ball
                    velocity = collider.StaticCollisionReflection(position, (position + wall.PerpendicularLeft), velocity);
                }
            }

            velocity = CalculateNewVelocity(deltaTime, force, velocity, mass);
            position = CalculateNewPosition(deltaTime, velocity, position);

            //array with new force, velocity, position,
            return new Vector2[] { force, velocity, position };
        }

        private static Vector2 CalculateNewDragForce(double deltaTime, Vector2 force, Vector2 velocity)
        {
            force = -velocity * 0.9 * deltaTime;   //simulate drag //TODO: maybe use deltaTime in seconds instead of ms

            //ignore to small forces
            if (force.X < 0.01)               
            {
                force = new Vector2(0, force.Y);
            }
            if (force.Y < 0.01)
            {
                force = new Vector2(force.X, 0);
            }

            return force;
        }

        public static Vector2[] PlayerConstraintMovement(
            double deltaTime,
            Vector2 force,
            Vector2 velocity,
            Vector2 position,
            double mass,
            ConstraintArea constArea,
            Collider collider)
        {
            Vector2[] oldMovement = new Vector2[] { force, velocity, position };

            if (constArea == null)
            {
                Console.WriteLine("Physics: called PlayerConstraintMovement(...,...) without a ConstraintArea.");
                return oldMovement;
            }

            if (collider == null)
            {
                Console.WriteLine("Physics: called PlayerConstraintMovement(...,...) without a Collider.");
                return oldMovement;
            }

            //get CircleCollider from Collider
            if (!(collider is CircleCollider ballCollider))
            {
                Console.WriteLine("Physics: called PlayerConstraintMovement(...,...) without a Collider that can be cast to a CircleCollider.");
                return oldMovement;
            }

            //check if the player collides with one of the Walls/Lines and if yes,
            //calculate the repel vector and use it as newVelocity
            foreach (var wall in constArea.Lines)
            {
                //circle & line Collision?
                if (collider.CheckCollisionCircleLine(ballCollider, wall))
                {
                    //repel the player
                    velocity = -wall.PerpendicularRight;
                }
            }

            velocity = CalculateNewVelocity(deltaTime, force, velocity, mass);
            position = CalculateNewPosition(deltaTime, velocity, position);

            //array with new force, velocity, position,
            return new Vector2[] { force, velocity, position };
        }

        public static Vector2 CalculateNewVelocity(double deltaTime, Vector2 force, Vector2 velocity, double mass)
        {
            return velocity += (force / mass) * deltaTime;
        }

        public static Vector2 CalculateNewPosition(double deltaTime, Vector2 velocity, Vector2 position)
        {
            return position += velocity * deltaTime;
        }

        public static Vector2[] CalculateNewMovement(double deltaTime, Vector2 force, Vector2 velocity, Vector2 position, double mass)
        {
            force = CalculateNewDragForce(deltaTime, force, velocity);
            velocity = CalculateNewVelocity(deltaTime, force, velocity, mass);
            position = CalculateNewPosition(deltaTime, velocity, position);
            return new Vector2[] { force, velocity, position };
        }

        public  static Vector2 CalculateNewVelocityFromControls(Vector2 velocity, double factor, Controls controls)
        {
            return controls.Velocity * (factor * Window.SCALEFACTOR_X);
        }
    }*/
}
