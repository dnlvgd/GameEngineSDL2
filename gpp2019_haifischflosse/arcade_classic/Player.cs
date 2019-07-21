using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XnaGeometry;
using static SDL2.SDL;

namespace gpp2019_haifischflosse
{ /*
    class Player : PlaceableObject, IUpdateable, IRenderable, IRescaleable, IFreeable, IMoveable
    {
        public double Radius { get; set; }
        public double Mass { get; set; } = 1;                   //mass
        public Vector2 Force { get; set; } = Vector2.Zero;      //acceleration
        public Vector2 Velocity { get; set; } = Vector2.Zero;   //velocity
        public PlayerID Id { get; }

        public int Score { get; set; } = 0;

        private const double factor = 1;

        public Controls controls;

        private ControlType myControlType;

        private Sprite sprite;

        public Collider Collider { get; set; }

        public ConstraintArea ConstraintArea { get; set;}

        public Player(
            PlayerID Id,
            Vector2 pos,
            float radius,
            ControlType controlType,
            string spritePath,
            int quantitySpriteRow,
            int quantitySpriteColumn,
            int totalQuantitySprites,
            ConstraintArea constraintArea): base(pos)
        {
            this.controls = new Controls(controlType);
            myControlType = controlType;

            this.sprite = new Sprite(spritePath, quantitySpriteRow, quantitySpriteColumn, totalQuantitySprites);
            this.Radius = radius;

            this.Id = Id;

            Collider = new CircleCollider(pos, (int)Radius, 1, 1, 1);
            ConstraintArea = constraintArea;
        }

        public void Render()
        {
            //change sprite when moving
            sprite.CurrentSpriteClipIndex = controls.Velocity.X != 0 || controls.Velocity.Y != 0 ? 1 : 0;
            sprite.Render();
            
            if(AirPong.debug)
            {
                //draw collsion-area and position point
                Collider.Render();
                SDL_RenderDrawPoint(Renderer.renderer, (int)Position.X, (int)Position.Y);
            }
        }

        public void Update(double deltaTime)
        {
            //Update Sprite Location
            sprite.Position = Position;

            //Update Player Informations
            if (myControlType != ControlType.Mouse)
            {
                Velocity = Physics.CalculateNewVelocityFromControls(Velocity, factor, controls);
            }
            else
            {
                Vector2 mP = controls.MousePos;  //gets Mouse Position
                Vector2 tempVelocity = new Vector2();

                mP.X = mP.X - Position.X;  //calulate Vector between player and mouse position
                mP.Y = mP.Y - Position.Y;

                if (-2 < mP.X && mP.X < 2 && -2 < mP.Y && mP.Y < 2)  //checks if the player is not allready at the mouse position +/- 2 Pixel
                {
                    Velocity = tempVelocity * 0;
                }
                else
                {
                    //makes sure that the sum of the final velocity of x and y can not exceed 1
                    double tempX = mP.X;
                    double tempY = mP.Y;
                    if (tempX < 0) tempX *= -1; //this is nessesary to make sure the sum is always calculated by positiv x and y ( tempX and Y do not influence the final velocity)
                    if (tempY < 0) tempY *= -1;
                    double vectorSum = tempX + tempY;

                    tempVelocity.X = (mP.X / vectorSum); //calcualte the relativ portion the player should move on the x axis
                    tempVelocity.Y = (mP.Y / vectorSum); //same for y axis
                    Velocity = tempVelocity * (factor * Window.SCALEFACTOR_X); //multiply by the factor and scalefactor of the window
                    //Console.WriteLine("Velocity: " + Velocity.X + " " + Velocity.Y);
                }

            }

            Vector2[] newMovement = Physics.PlayerConstraintMovement(deltaTime, Force, Velocity, Position, Mass, ConstraintArea, Collider);
            if (newMovement == null)
            {
                Console.WriteLine("Player: newMovement Array is null");

            }
            else if (newMovement.Length != 3)
            {
                Console.WriteLine("Player: newMovement Array does not have the required length of 3 (for newForce, newVelocity, newPostion).");
            }
            else
            {
                Force = newMovement[0];
                Velocity = newMovement[1];
                Position = newMovement[2];
            }
        }

        public override void Rescale()
        {
            base.Rescale();
            sprite.Rescale();
            Collider.Rescale();
            ConstraintArea.Rescale();
        }

        public void Move(double deltaTime)
        {
            Collider.Position = Position;
        }

        public void Free()
        {
            sprite.Free();
        }
    }
    enum PlayerID {One, Two}; */
}
