using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XnaGeometry;
using static SDL2.SDL;
using static SDL2.SDL_ttf;
using static SDL2.SDL_mixer;

namespace gpp2019_haifischflosse
{
    public class Component
    {
        public GameObject Owner { get; set; }
        public bool IsActive { get; set; }

        public Component Clone()
        {

            Component newcp = (Component)this.MemberwiseClone();
            return newcp;
        }
    }

    public class BehaviorComponent : Component, IBehavior
    {
        public virtual void Update(double deltaTime) { }
        public virtual void HandleInput(Input input) { }
    }

    public class MoveComponent : Component
    {
        public Vector2 Velocity { get; set; }   //velocity
        public double Factor { get; set; }
        //public ConstraintArea constraintArea { get; set; }

        public MoveComponent(double velX, double velY, double factor/*, ConstraintArea constraintArea*/)
        {
            this.Velocity = new Vector2(velX, velY);
            this.Factor = factor;
            /*this.constraintArea = constraintArea;*/
        }
    };

    public class PhysicComponent : Component
    {
        public Vector2 Force { get; set; }  //force
        public double Mass { get; set; }    //mass

        public PhysicComponent(double forceX, double forceY, double mass)
        {
            this.Force = new Vector2(forceX, forceY);
            this.Mass = mass;
        }
    };

    public class RectangleColliderComponent : PhysicComponent
    {
        public int Width { get; set; }
        public int Height { get; set; }

        /*public int ORIGINAL_Width { get; set; }*/
        /*public int ORIGINAL_Height { get; set; }*/

        public RectangleColliderComponent(double forceX, double forceY, double mass, int width, int height) : base(forceX, forceY, mass)
        {
            this.Width = (int)(/*ORIGINAL_Width * */   width * Window.SCALEFACTOR_X);
            this.Height = (int)(/*ORIGINAL_Height * */  height * Window.SCALEFACTOR_Y);
        }
    }

    public class CircleColliderComponent : PhysicComponent
    {
        public int Radius { get; set; }
        public int Diameter { get; set; }

        public Vector2 offset = new Vector2(0,0);
        /*public int ORIGINAL_Radius { get; set; }*/

        public CircleColliderComponent(double forceX, double forceY, double mass, int radius) : base(forceX, forceY, mass)
        {
            /*ORIGINAL_Radius = radius;*/
            Radius = (int)(/*ORIGINAL_Radius * */  radius * Window.SCALEFACTOR_X);
            Diameter = Radius * 2;
        }
        public CircleColliderComponent(double forceX, double forceY, double mass, int radius, Vector2 offset) : base(forceX, forceY, mass)
        {
            Radius = (int)(/*ORIGINAL_Radius * */  radius * Window.SCALEFACTOR_X);
            Diameter = Radius * 2;
            this.offset = offset* Window.SCALEFACTOR_X;
        }
    }

    public class SpriteComponent : Component
    {
        public Animation Animation { get; set; }
        public Sprite S { get; set; }

        public int Z { get; set; }
        public SDL_Rect DstRect { get; set; }

        public SpriteComponent(Animation animation, string path, uint quantitySpriteRow, uint quantitySpriteColumn, uint totalQuantitySprites, int z, double animationDuration = 1000.0, SDL_RendererFlip rendererFlip = SDL_RendererFlip.SDL_FLIP_NONE)
        {
            this.Animation = animation;
            this.S = new Sprite(path, quantitySpriteRow, quantitySpriteColumn, totalQuantitySprites, animationDuration, rendererFlip);
            this.Z = z;
            this.DstRect = new SDL_Rect { x = (S.SpriteClips[0].w / 2), y = (S.SpriteClips[0].h / 2), w = S.Texture.Width, h = S.Texture.Height };
        }
    };

    public class TextureComponent : Component
    {
        public Texture T { get; set; }
        public SDL_Rect DstRect { get; set; }
        public int Z { get; set; }

        public TextureComponent(string path, int z)
        {
            this.T = new Texture(path);
            this.Z = z;
            this.DstRect = new SDL_Rect { x = 0, y = 0, w = T.Width, h = T.Height};
        }
        public TextureComponent(string path, int z, int w, int h)
        {
            this.T = new Texture(path, w, h);
            this.Z = z;
            this.DstRect = new SDL_Rect { x = 0, y = 0, w = T.Width, h = T.Height };
        }

        public TextureComponent(Texture tileTexture, SDL_Rect tileRect, int z)
        {
            this.T = tileTexture;
            this.DstRect = tileRect;
            this.Z = z;
        }
    }

    public class TextComponent : Component
    {
        public string Text { get; set; }
        public IntPtr TextSurface { get { return textSurface; } set { textSurface = value; } }
        public IntPtr TextTexture { get { return textTexture; } }
        //public SDL_Rect TextRectangle { get; set; }  Brauchen wir hier nicht.... oder? :D //genau: habe dafür eine SizeComponent erstellt und PositionComponent haben wie ja schon :D
        IntPtr textSurface;
        IntPtr textTexture;
        IntPtr textFontFamily;
        SDL_Color textColor;
        SDL_Color textBackgroundColor;

        public TextComponent(string text, int fontSize, byte r, byte g, byte b, byte alpha)
        {
            this.Text = text;
            this.textColor = new SDL_Color { r = r, g = g, b = b, a = alpha };
            this.textBackgroundColor = new SDL_Color { r = 0, g = 0, b = 0, a = 0 };
            this.textFontFamily = TTF_OpenFont("assets/fonts/kenvector_future.ttf", fontSize);
            this.textSurface = TTF_RenderText_Blended(textFontFamily, text, textColor);
            this.textTexture = SDL_CreateTextureFromSurface(Renderer.renderer, textSurface);
        }

        public void SetText(string msg)
        {
            textSurface = TTF_RenderText_Blended(textFontFamily, msg, textColor);
            textTexture = SDL_CreateTextureFromSurface(Renderer.renderer, textSurface);
        }
    }

    public class SoundFXComponent : Component
    {
        public IntPtr SoundFX { get; set; }

        public SoundFXComponent(string path)
        {
            this.SoundFX = Mix_LoadWAV(path);

            if (SoundFX == IntPtr.Zero)
            {
                Console.WriteLine("Audiodatei konnte nicht geladen werden!");
            }
        }
    }

    public class MusicComponent : Component
    {
        public IntPtr Music { get; set; }

        public MusicComponent(string path)
        {
            this.Music = Mix_LoadMUS(path);

            if (Music == IntPtr.Zero)
            {
                Console.WriteLine("Musikdatei konnte nicht geladen werden!");
            }

        }
    }

    public class ButtonClickAreaComponent : Component
    {
        public SDL_Rect ClickableBox { get; set; }

        public ButtonClickAreaComponent(int ownerTopLeftPosX, int ownerTopLeftPosY, int w, int h)
        {
            ClickableBox = new SDL_Rect
            {
                x = (int)(ownerTopLeftPosX),
                y = (int)(ownerTopLeftPosY),
                w = (int)(w * Window.SCALEFACTOR_X),
                h = (int)(h * Window.SCALEFACTOR_Y)
            };
            
        }
    }

    public class ConstraintAreaComponent : Component
    {
        public SDL_Rect Area { get; set; }
        
        public ConstraintAreaComponent(int x, int y, int w, int h)
        {
            Area = new SDL_Rect { x = x, y = y, w = w, h = h };
        }
    }

    public class BoundingBoxComponent : Component
    {
        public SDL_Rect Box { get; set; }
        public bool centerOrigin;

        public BoundingBoxComponent(int ownerPosX, int ownerPosY, int w, int h, bool centerOrigin)
        {
            this.centerOrigin = centerOrigin;
            if (centerOrigin)
            {
                // used to relocate the rectangle's origin from the top left corner point to the center/middle point
                int offSetX = w / 2;
                int offSetY = h / 2;

                Box = new SDL_Rect
                {
                    x = (int)(ownerPosX - offSetX),
                    y = (int)(ownerPosY - offSetY),
                    w = (int)(w * Window.SCALEFACTOR_X),
                    h = (int)(h * Window.SCALEFACTOR_Y)
                };
            }
            else
            {
                Box = new SDL_Rect {
                    x = (int)(ownerPosX),
                    y = (int)(ownerPosY),
                    w = (int)(w * Window.SCALEFACTOR_X),
                    h = (int)(h * Window.SCALEFACTOR_Y)
                };
            }
        }
    }

    public class MouseComponent : Component
    {
        public Vector2 Position { get; set; }
        public Vector2 PositionGlobal { get; set; }
        public MouseComponent()
        {
            Position = Vector2.Zero;
        }
    }

    public class SpawnComponent : Component
    {
        public GameObject SpawnObject { get; set; }

        public SpawnComponent(GameObject spawnObject)
        {
            this.SpawnObject = spawnObject;
        }

        public void Update()
        {

        }
    }

    public class WeaponComponent : Component
    {
        public GameObject Weapon { get; set; }

        public WeaponComponent(GameObject weapon)
        {
            Weapon = weapon;
        }
    }
    
    public interface IBehavior
    {
        void Update(double deltaTime);
        void HandleInput(Input input);
    }
}
