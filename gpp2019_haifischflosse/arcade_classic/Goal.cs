using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XnaGeometry;

namespace gpp2019_haifischflosse
{ /*
    class Goal : PlaceableObject, IRenderable, IRescaleable, IUpdateable
    {
        private Sprite sprite;
        public Collider Collider { get; set; }
        public Player Owner { get; set; }

        public Goal(Vector2 pos, string spritePath, int quantitySpriteRow, int quantitySpriteColumn, int totalQuantitySprites, Player owner) : base(pos)
        {
            this.sprite = new Sprite(spritePath, quantitySpriteRow, quantitySpriteColumn, totalQuantitySprites);

            Collider = new RectangleCollider(pos, 50, 200);
            Owner = owner;
        }

        public void Render()
        {
            sprite.Render();

            if(AirPong.debug)
            {
                Collider.Render();
                SDL2.SDL.SDL_RenderDrawPoint(Renderer.renderer, (int)Position.X, (int)Position.Y);
            }
        }

        public override void Rescale()
        {
            base.Rescale();
            sprite.Rescale();
            Collider.Rescale();
        }

        public void Update(double deltaTime)
        {
            //Update Sprite Location
            sprite.Position = Position;
        }
    } */
}
