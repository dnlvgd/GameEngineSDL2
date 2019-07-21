using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using XnaGeometry;
using static SDL2.SDL;


namespace gpp2019_haifischflosse
{
    /* GameObject class */
    public class GameObject
    {
        public string Name { get; set; }
        public Tag Tag { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Scaling { get; set; }
        public double Angle { get; set; }
        public bool IsActive { get; set; }

        public List<Component> Components { get; set; } = new List<Component>();

        public GameObject(string name, Tag tag, double posX, double posY, double scaleX, double scaleY, double degAngle, bool isActive)
        {
            this.Name = name;
            this.Tag = tag;
            this.Position = new Vector2(posX, posY);
            this.Scaling = new Vector2(scaleX, scaleY);
            this.Angle = degAngle;
            this.IsActive = isActive;
        }

        public void AddComponent(bool isActive, Component component)
        {
            component.Owner = this;
            component.IsActive = isActive;
            Components.Add(component);
        }

        public void AddComponent(Component component, GameObject owner)
        {
            component.Owner = owner;
            Components.Add(component);
        }

        public Component GetComponent<T>()
        {
            for (int i = 0; i < Components.Count; i++)
            {
                Type t = Components[i].GetType();
                if (typeof(T).Equals(t))
                {
                    return Components[i];
                }
            }
            return null;
        }
        
        public BehaviorComponent GetCustomBehaviorCp<T>()
        {
            for (int i = 0; i < Components.Count; i++)
            {
                Type t = Components[i].GetType();
                if (Components[i] is IBehavior)
                {
                    return Components[i] as BehaviorComponent;
                }
            }
            return null;
        }

        public GameObject Clone()
        {
            GameObject newgo = (GameObject)this.MemberwiseClone();

            return newgo;
        }
    }

    /* Interfaces */
    public interface IUpdateable
    {
        void Update(double deltaTime); //delta time in ms
    }

    public interface IMoveable
    {
        void Move(double deltaTime);
    }

    public interface IRescaleable
    {
        void Rescale();
    }

    public interface IAnimating
    {
        void Animate();
    }

    public interface IFreeable
    {
        void Free();
    }

    public interface ICollidable
    {
        void Collide();
    }





    /*
        public class PlaceableObject : GameObject, IRescaleable
        {
        public Vector2 Position { get; set; }

        public PlaceableObject(Vector2 pos)
        {
            Position = pos;
        }

        ///TODO: Muss das weiterhin hier hin?
        public virtual bool InActive()
        {
            return false;
        }

        public virtual void Rescale()
        {
            Console.WriteLine(
                "OLD_POSITION_X: {0}" +
                " OLD_POSITION_Y: {1}",
                Position.X,
                Position.Y
                );

            double windowFactorWidth = (double)Window.CURRENT_SCREEN_WIDTH / Window.OLD_SCREEN_WIDTH;
            double windowFactorHeight = (double)Window.CURRENT_SCREEN_HEIGHT / Window.OLD_SCREEN_HEIGHT;

            Position = new Vector2(Position.X * windowFactorWidth,
                Position.Y * windowFactorHeight);

            Console.WriteLine(
                "NEW_POSITION_X: {0}" +
                " NEW_POSITION_Y: {1}",
                Position.X,
                Position.Y
                );
        }
    }
    */
}