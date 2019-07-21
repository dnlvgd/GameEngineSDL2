
using SDL2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XnaGeometry;
using static SDL2.SDL;

namespace gpp2019_haifischflosse
{ /*
    class Ball : PlaceableObject, IUpdateable, IRenderable, IMoveable, IAnimating, IFreeable
    {
        public double Radius { get; set; }
        public Collider Collider { get; set; }

        public Sprite sprite;

        public const String defaultSpritePath = "assets/sprites/17_felspell_spritesheet.png";

        public double Mass { get; set; } = 0.3;                 //mass
        public Vector2 Force { get; set; } = Vector2.Zero;      //acceleration
        public Vector2 Velocity { get; set; } = Vector2.Zero;   //velocity
        public ConstraintArea ConstraintArea { get; set; }

        public Ball(Vector2 pos, float radius, int quantitySpriteRow, int quantitySpriteColumn, int totalQuantitySprites, ConstraintArea constraintArea): base(pos)
        {

            this.sprite = new Sprite(defaultSpritePath, quantitySpriteRow, quantitySpriteColumn, totalQuantitySprites);
            this.Radius = radius;
            Collider = new CircleCollider(pos, (int)Radius, 1, 1, 1);

            ConstraintArea = constraintArea;
        }
        public void Update(double deltaTime)
        {
            //Update Sprite Location
            sprite.Position = Position;
        }

        public void Move(double deltaTime)
        {
            Vector2[] newMovement = Physics.BallConstraintMovement(deltaTime, Force, Velocity, Position, Mass, ConstraintArea, Collider);
            if (newMovement == null)
            {
                Console.WriteLine("Ball: newMovement Array is null");

            }
            else if (newMovement.Length != 3)
            {
                Console.WriteLine("Ball: newMovement Array does not have the required length of 3 (for newForce, newVelocity, newPostion).");
            }
            else
            {
                Force = newMovement[0];
                Velocity = newMovement[1];
                Position = newMovement[2];
            }

            Collider.Position = Position;
        }

        public void Animate()
        {
            sprite.Animate();
        }

        public void Render()
        {
            sprite.Render();

            if(AirPong.debug)
            {
                Collider.Render();
                SDL_RenderDrawPoint(Renderer.renderer, (int)Position.X, (int)Position.Y);
            }    
        }

        public void Free()
        {
            sprite.Free();
        }

        public override void Rescale()
        {
            base.Rescale();
            sprite.Rescale();
            Collider.Rescale();
            ConstraintArea.Rescale();
        }
    } */
}
