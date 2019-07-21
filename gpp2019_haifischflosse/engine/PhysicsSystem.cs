using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XnaGeometry;
using static SDL2.SDL;

namespace gpp2019_haifischflosse
{
    public class PhysicsSystem
    {
        private List<MoveComponent> moveCps = new List<MoveComponent>();
        private List<PhysicComponent> physicCps = new List<PhysicComponent>();
        private List<BoundingBoxComponent> boundingBoxCps = new List<BoundingBoxComponent>();
        private List<CircleColliderComponent> circleColCps = new List<CircleColliderComponent>();
        private List<RectangleColliderComponent> rectangleColCps = new List<RectangleColliderComponent>();
        private int indexofLastEvent = 0;
        private Texture rectangleColliderTexture = new Texture("assets/sprites/collider/rectangleCollider.png");
        private Texture circleColliderTexture = new Texture("assets/sprites/collider/circleCollider.png");

        public MoveComponent CreateMoveComponent(double velX, double velY, double factor)
        {
            MoveComponent mc = new MoveComponent(velX, velY, factor);
            moveCps.Add(mc);
            return mc;
        }

        public PhysicComponent CreatePhysicComponent(double forceX, double forceY, double mass)
        {
            PhysicComponent pc = new PhysicComponent(forceX, forceY, mass);
            physicCps.Add(pc);
            return pc;
        }

        public CircleColliderComponent CreateCircleColliderComponent(double forceX, double forceY, double mass, int radius)
        {
            CircleColliderComponent ccp = new CircleColliderComponent(forceX, forceY, mass, radius);
            circleColCps.Add(ccp);
            return ccp;
        }

        public CircleColliderComponent CreateCircleColliderComponent(double forceX, double forceY, double mass, int radius, Vector2 offset)
        {
            CircleColliderComponent ccp = new CircleColliderComponent(forceX, forceY, mass, radius,offset);
            circleColCps.Add(ccp);
            return ccp;
        }

        public RectangleColliderComponent CreateRectangleColliderComponent(double forceX, double forceY, double mass, int width, int height)
        {
            RectangleColliderComponent rcp = new RectangleColliderComponent(forceX, forceY, mass, width, height);
            rectangleColCps.Add(rcp);
            return rcp;
        }

        public BoundingBoxComponent CreateBoundingBoxComponent(int ownerPosX, int ownerPosY, int w, int h, bool centerOrigin)
        {
            BoundingBoxComponent bbc = new BoundingBoxComponent(ownerPosX, ownerPosY, w, h, centerOrigin);
            boundingBoxCps.Add(bbc);
            return bbc;
        }

        public void RescaleCollider()
        {
            foreach(var bc in boundingBoxCps)
            {
                bc.Box = new SDL_Rect() { x = bc.Box.x, y = bc.Box.y, w = (int)(bc.Box.w * Window.WINDOW_SCALEFACTOR_X), h = (int)(bc.Box.h * Window.WINDOW_SCALEFACTOR_Y) };
            }
            foreach(var cc in circleColCps)
            {
                cc.Radius *= (int)(Window.WINDOW_SCALEFACTOR_X);
                cc.Diameter = cc.Radius * 2;
            }
            foreach(var rc in rectangleColCps)
            {
                rc.Width *= (int)Window.WINDOW_SCALEFACTOR_X;
                rc.Height *= (int)Window.WINDOW_SCALEFACTOR_Y;
            }
        }

        public void RenderColliderForDebug()
        {
            foreach (var bc in boundingBoxCps)
            {
                EventSystem.Instance.AddEvent("RenderOnce", rectangleColliderTexture, bc.Box.x, bc.Box.y, bc.Box.w, bc.Box.h);
            }
            foreach (var cc in circleColCps)
            {
                EventSystem.Instance.AddEvent("RenderOnce", circleColliderTexture, (int)cc.Owner.Position.X - cc.Radius, (int)cc.Owner.Position.Y - cc.Radius, cc.Diameter, cc.Diameter);
            }
            foreach (var rc in rectangleColCps)
            {
                EventSystem.Instance.AddEvent("RenderOnce", rectangleColliderTexture, (int)rc.Owner.Position.X, (int)rc.Owner.Position.Y, rc.Width, rc.Height);
            }
        }

        public void Move(double deltaTime)
        {
            SDL_Rect bb = new SDL_Rect { x = 0, y = 0, w = 0, h = 0 };

            //refresh move component values
            for (int i = moveCps.Count - 1; i >= 0; i--)
            {
                if (moveCps[i].Owner == null)
                {
                    moveCps.RemoveAt(i);
                    Console.WriteLine("MoveComponent == null");
                    continue;
                }

                //ConstraintAreaComponent ca = moveCps[i].Owner.GetComponent<ConstraintAreaComponent>() as ConstraintAreaComponent;
                if (moveCps[i].Owner.Name == "player")
                {
                    MouseComponent mc = moveCps[i].Owner.GetComponent<MouseComponent>() as MouseComponent;
                    if (mc != null)
                    {
                        Vector2 mP = mc.PositionGlobal - moveCps[i].Owner.Position;
                        if (-5 < mP.X && mP.X < 5 && -5 < mP.Y && mP.Y < 5)  //checks if the player is not allready at the mouse position +/- 2 Pixel
                        {
                            moveCps[i].Velocity = Vector2.Zero;
                        }
                    }
                }

                //1.
                PhysicComponent physicCp = moveCps[i].Owner.GetComponent<PhysicComponent>() as PhysicComponent;
                if (physicCp != null)
                {
                    moveCps[i].Velocity = CalculateNewVelocity(deltaTime, moveCps[i].Velocity, physicCp.Force, physicCp.Mass);
                }
                //2.
                MoveComponent moveCp = moveCps[i];
                if (moveCp != null)
                {
                    moveCps[i].Owner.Position = CalculateNewPosition(deltaTime, moveCps[i].Velocity, moveCps[i].Factor, moveCps[i].Owner.Position);
                }

                BoundingBoxComponent bbc = moveCps[i].Owner.GetComponent<BoundingBoxComponent>() as BoundingBoxComponent;
                if (bbc != null)
                {
                    //Console.WriteLine("GameObject '" + moveCps[i].Owner.Name + "' BoundingBoxComponent == null");
                   
                    if (bbc.centerOrigin)
                    {
                        int offSetX = bbc.Box.w / 2;
                        int offSetY = bbc.Box.h / 2;
                        bb.x = (int)moveCps[i].Owner.Position.X - offSetX;
                        bb.y = (int)moveCps[i].Owner.Position.Y - offSetY;
                    }
                    else
                    {
                        bb.x = (int)moveCps[i].Owner.Position.X;
                        bb.y = (int)moveCps[i].Owner.Position.Y;
                    }
                    bb.w = bbc.Box.w;
                    bb.h = bbc.Box.h;
                    bbc.Box = bb;
                }
            }
        }

        public void Update(double deltaTime)
        {
            /* Event handling */
            if (EventSystem.Instance.CheckEvent("RemoveAllCPs", indexofLastEvent))
            {
                moveCps.Clear();
                physicCps.Clear();
                boundingBoxCps.Clear();
                circleColCps.Clear();
                rectangleColCps.Clear();
            }

            List<EventStruct> myEvents;
            myEvents = EventSystem.Instance.GetEvents("AddMoveCP", indexofLastEvent);
            foreach (EventStruct e in myEvents)
            {
                moveCps.Add((MoveComponent)e.data[0]);
            }

            myEvents = EventSystem.Instance.GetEvents("AddPhysicsCP", indexofLastEvent);
            foreach (EventStruct e in myEvents)
            {
                physicCps.Add((PhysicComponent)e.data[0]);
            }

            myEvents = EventSystem.Instance.GetEvents("AddBoundingBoxCP", indexofLastEvent);
            foreach (EventStruct e in myEvents)
            {
                boundingBoxCps.Add((BoundingBoxComponent)e.data[0]);
            }

            myEvents = EventSystem.Instance.GetEvents("AddCircleColliderCP", indexofLastEvent);
            foreach (EventStruct e in myEvents)
            {
                circleColCps.Add((CircleColliderComponent)e.data[0]);
            }

            myEvents = EventSystem.Instance.GetEvents("AddRectangleColliderCP", indexofLastEvent);
            foreach (EventStruct e in myEvents)
            {
                rectangleColCps.Add((RectangleColliderComponent)e.data[0]);
            }

            myEvents = EventSystem.Instance.GetEvents("RemoveMoveCP", indexofLastEvent);
            foreach (EventStruct e in myEvents)
            {
                moveCps.Remove((MoveComponent)e.data[0]);
            }

            myEvents = EventSystem.Instance.GetEvents("RemovePhysicsCP", indexofLastEvent);
            foreach (EventStruct e in myEvents)
            {
                physicCps.Remove((PhysicComponent)e.data[0]);
            }

            myEvents = EventSystem.Instance.GetEvents("RemoveBoundingBoxCP", indexofLastEvent);
            foreach (EventStruct e in myEvents)
            {
                boundingBoxCps.Remove((BoundingBoxComponent)e.data[0]);
            }

            myEvents = EventSystem.Instance.GetEvents("RemoveCircleColliderCP", indexofLastEvent);
            foreach (EventStruct e in myEvents)
            {
                circleColCps.Remove((CircleColliderComponent)e.data[0]);
            }

            myEvents = EventSystem.Instance.GetEvents("RemoveRectangleColliderCP", indexofLastEvent);
            foreach (EventStruct e in myEvents)
            {
                rectangleColCps.Remove((RectangleColliderComponent)e.data[0]);
            }

            indexofLastEvent = EventSystem.Instance.GetMyIndex();
        }

        public void Free()
        {
            moveCps.Clear(); 
            boundingBoxCps.Clear();
            circleColCps.Clear();
            rectangleColCps.Clear();
        }

        public void CheckCollisions()
        {
            CheckCollisionCirclesWithAllOtherCircles();
            CheckCollisionCirclesWithAllOtherRectangles();
        }

        public void CheckOverlapAABB()
        {
            // Axis-Aligned Bounding Box examination
            Stack<Tuple<GameObject, GameObject>> collisionPairCandidates = new Stack<Tuple<GameObject, GameObject>>();

            // find collision candidates
            for (int i = boundingBoxCps.Count - 1; i >= 0; i--)
            {
                BoundingBoxComponent bbc = boundingBoxCps[i] as BoundingBoxComponent;
                if (bbc == null)
                {
                    continue;
                }

                int startX = bbc.Box.x;
                int endX = bbc.Box.x + bbc.Box.w;
                if (!ValidStartToEnd(startX, endX))
                {
                    continue;
                }

                for (int j = boundingBoxCps.Count - 1; j >= 0; j--)
                {
                    // avoid intersection axis checks of components of the same GameObject
                    if (!(boundingBoxCps[i].Owner.Name.Equals(boundingBoxCps[j].Owner.Name)))
                    {
                        BoundingBoxComponent bbcOther = boundingBoxCps[j] as BoundingBoxComponent;
                        if (bbcOther == null)
                        {
                            continue;
                        }

                        int otherStartX = bbcOther.Box.x;
                        int otherEndX = bbcOther.Box.x + bbcOther.Box.w;
                        if (!ValidStartToEnd(otherStartX, otherEndX))
                        {
                            continue;
                        }

                        // X-Axis intersection check
                        if (CheckIntersectionInAxis(startX, endX, otherStartX, otherEndX))
                        {
                            int startY = bbc.Box.y;
                            int endY = bbc.Box.y + bbc.Box.h;
                            if (!ValidStartToEnd(startY, endY))
                            {
                                continue;
                            }

                            int otherStartY = bbcOther.Box.y;
                            int otherEndY = bbcOther.Box.y + bbcOther.Box.h;
                            if (!ValidStartToEnd(otherStartY, otherEndY))
                            {
                                continue;
                            }

                            // Y-Axis intersection check
                            if (CheckIntersectionInAxis(startY, endY, otherStartY, otherEndY))
                            {
                                // found collision candidates! 
                                var collsionPairVarOne = Tuple.Create(bbc.Owner, bbcOther.Owner);
                                var collsionPairVarTwo = Tuple.Create(bbcOther.Owner, bbc.Owner);

                                // only push them onto the stack if the stack does not already contain them
                                if (!collisionPairCandidates.Contains(collsionPairVarOne) && !collisionPairCandidates.Contains(collsionPairVarTwo))
                                {
                                    // added collision candidate to stack for later handling
                                    collisionPairCandidates.Push(collsionPairVarOne);
                                }
                            }
                        }
                    }
                }
            }

            // handle collision candidates
            while (collisionPairCandidates.Count > 0)
            {
                var colPairCandi = collisionPairCandidates.Pop();

                CircleColliderComponent firstCCC = colPairCandi.Item1.GetComponent<CircleColliderComponent>() as CircleColliderComponent;
                RectangleColliderComponent firstRCC = colPairCandi.Item1.GetComponent<RectangleColliderComponent>() as RectangleColliderComponent;

                CircleColliderComponent secondCCC = colPairCandi.Item2.GetComponent<CircleColliderComponent>() as CircleColliderComponent;
                RectangleColliderComponent secondRCC = colPairCandi.Item2.GetComponent<RectangleColliderComponent>() as RectangleColliderComponent;

                // CheckCircleCircleCollision
                if (firstCCC != null && secondCCC != null)
                {
                    if (firstCCC.Owner.Tag == Tag.PlayerBullet && secondCCC.Owner.Tag == Tag.EnemyNormal ||
                        firstCCC.Owner.Tag == Tag.EnemyNormal && secondCCC.Owner.Tag == Tag.PlayerBullet )
                    {
                        if(CheckCircleCircleCollision(firstCCC, secondCCC))
                        {
                            /* Delete PlayerBullet and EnemyNormal on Collision */
                            Console.WriteLine("PlayerShotEnemy");
                            EventSystem.Instance.AddEvent("DeactivateGameObject", firstCCC.Owner);
                            EventSystem.Instance.AddEvent("DeactivateGameObject", secondCCC.Owner);
                            EventSystem.Instance.AddEvent("PlayerKilltEnemy");
                            if (firstCCC.Owner.Tag == Tag.PlayerBullet)
                            {
                                //EventSystem.Instance.AddEvent("DeactivateGameObject", secondCCC.Owner);
                                //EventSystem.Instance.AddEvent("PlayerKilltEnemy");
                                EventSystem.Instance.AddEvent("BulletHit", firstCCC.Owner);
                            }
                            if (secondCCC.Owner.Tag == Tag.PlayerBullet)
                            {
                                //EventSystem.Instance.AddEvent("DeactivateGameObject", firstCCC.Owner);
                                //EventSystem.Instance.AddEvent("PlayerKilltEnemy");
                                EventSystem.Instance.AddEvent("BulletHit", secondCCC.Owner);
                            }
                        }
                    }

                    else if (firstCCC.Owner.Tag == Tag.EnemyNormalBullet && secondCCC.Owner.Tag == Tag.MainPlayer)
                    {
                        if (CheckCircleCircleCollision(firstCCC, secondCCC))
                        {
                            Console.WriteLine("EnemyShotPlayer");
                            EventSystem.Instance.AddEvent("DeactivateGameObject", firstCCC.Owner);
                            EventSystem.Instance.AddEvent("BulletHit", firstCCC.Owner);
                            EventSystem.Instance.AddEvent("AdjustPlayerHealth", 1);
                        }
                    }

                    else if (firstCCC.Owner.Tag == Tag.MainPlayer && secondCCC.Owner.Tag == Tag.EnemyNormalBullet)
                    {
                        if (CheckCircleCircleCollision(firstCCC, secondCCC))
                        {
                            Console.WriteLine("EnemyShotPlayer");
                            EventSystem.Instance.AddEvent("DeactivateGameObject", secondCCC.Owner);
                            EventSystem.Instance.AddEvent("AdjustPlayerHealth", 1);
                            EventSystem.Instance.AddEvent("BulletHit", secondCCC.Owner);
                        }
                    }

                    else if (firstCCC.Owner.Tag == Tag.MainPlayer && secondCCC.Owner.Tag == Tag.EnemyNormal )
                    {
                        if (CheckCircleCircleCollision(firstCCC, secondCCC))
                        {
                            Console.WriteLine("PlayerCollidedEnemy");
                            EventSystem.Instance.AddEvent("DeactivateGameObject", secondCCC.Owner);
                            EventSystem.Instance.AddEvent("PlayerKilltEnemy");
                            EventSystem.Instance.AddEvent("AdjustPlayerHealth", 1);
                        }
                    }
                    else if (firstCCC.Owner.Tag == Tag.EnemyNormal && secondCCC.Owner.Tag == Tag.MainPlayer)
                    {
                        if (CheckCircleCircleCollision(firstCCC, secondCCC))
                        {
                            Console.WriteLine("PlayerKilltEnemy");
                            EventSystem.Instance.AddEvent("DeactivateGameObject", firstCCC.Owner);
                            EventSystem.Instance.AddEvent("PlayerKilltEnemy");
                            EventSystem.Instance.AddEvent("AdjustPlayerHealth", 1);
                        }
                    }
                    else if (firstCCC.Owner.Tag == Tag.PlayerBullet && secondCCC.Owner.Tag == Tag.EnemyBoss)
                    {
                        if (CheckCircleCircleCollision(firstCCC, secondCCC))
                        {
                            Console.WriteLine("PlayerHitBoss!");
                            EventSystem.Instance.AddEvent("DeactivateGameObject", firstCCC.Owner);
                            EventSystem.Instance.AddEvent("BossHitByPlayer", secondCCC.Owner);
                            EventSystem.Instance.AddEvent("BulletHit", firstCCC.Owner);
                        }
                    }
                    else if (firstCCC.Owner.Tag == Tag.EnemyBoss && secondCCC.Owner.Tag == Tag.PlayerBullet)// || firstCCC.Owner.Tag == Tag.PlayerBullet && secondCCC.Owner.Tag == Tag.EnemyBoss)
                    {
                        if(CheckCircleCircleCollision(firstCCC, secondCCC))
                        {
                            Console.WriteLine("PlayerHitBoss!");
                            EventSystem.Instance.AddEvent("DeactivateGameObject", secondCCC.Owner);
                            EventSystem.Instance.AddEvent("BossHitByPlayer", firstCCC.Owner);
                            EventSystem.Instance.AddEvent("BulletHit", secondCCC.Owner);
                        }
                    }
                    else if(firstCCC.Owner.Tag == Tag.PickUp && secondCCC.Owner.Tag == Tag.MainPlayer)
                    {
                        if(CheckCircleCircleCollision(firstCCC, secondCCC))
                        {
                            EventSystem.Instance.AddEvent("AdjustPlayerHealth", -1, firstCCC.Owner);
                            EventSystem.Instance.AddEvent("DeactivateGameObject", firstCCC.Owner);
                        }
                    }
                    else if (firstCCC.Owner.Tag == Tag.MainPlayer && secondCCC.Owner.Tag == Tag.PickUp)
                    {
                        if (CheckCircleCircleCollision(firstCCC, secondCCC))
                        {
                            EventSystem.Instance.AddEvent("AdjustPlayerHealth", -1, secondCCC.Owner);
                            EventSystem.Instance.AddEvent("DeactivateGameObject", secondCCC.Owner);
                        }
                    }
                    else if (firstCCC.Owner.Tag == Tag.Coin && secondCCC.Owner.Tag == Tag.MainPlayer)
                    {
                        if (CheckCircleCircleCollision(firstCCC, secondCCC))
                        {
                            EventSystem.Instance.AddEvent("GoldPickUp", firstCCC.Owner);
                            EventSystem.Instance.AddEvent("DeactivateGameObject", firstCCC.Owner);
                            EventSystem.Instance.AddEvent("CoinCollect", firstCCC.Owner);
                        }
                    }
                    else if (firstCCC.Owner.Tag == Tag.MainPlayer && secondCCC.Owner.Tag == Tag.Coin)
                    {
                        if (CheckCircleCircleCollision(firstCCC, secondCCC))
                        {
                            EventSystem.Instance.AddEvent("GoldPickUp", secondCCC.Owner);
                            EventSystem.Instance.AddEvent("DeactivateGameObject", secondCCC.Owner);
                            EventSystem.Instance.AddEvent("CoinCollect", secondCCC.Owner);
                        }
                    }
                    else if (firstCCC.Owner.Tag == Tag.Flag && secondCCC.Owner.Tag == Tag.MainPlayer)
                    {
                        if (CheckCircleCircleCollision(firstCCC, secondCCC))
                        {
                            EventSystem.Instance.AddEvent("PlayerReachedFlag");
                            Console.WriteLine("Yes");
                        }
                    }
                    else if (firstCCC.Owner.Tag == Tag.MainPlayer && secondCCC.Owner.Tag == Tag.Flag)
                    {
                        EventSystem.Instance.AddEvent("PlayerReachedFlag");
                        Console.WriteLine("Yes");
                    }
                    else if (firstCCC.Owner.Tag == Tag.PlayerBullet && secondCCC.Owner.Tag == Tag.EnemyNormal)
                    {
                        if (CheckCircleCircleCollision(firstCCC, secondCCC))
                        {
                            EventSystem.Instance.AddEvent("ArrowHitEnemy");
                            //Console.WriteLine("1. ArrowHitTile!");
                        }
                    }
                    else if (firstCCC.Owner.Tag == Tag.EnemyNormal && secondCCC.Owner.Tag == Tag.PlayerBullet)
                    {
                        if (CheckCircleCircleCollision(firstCCC, secondCCC))
                        {
                            EventSystem.Instance.AddEvent("ArrowHitEnemy");
                            //Console.WriteLine("1. ArrowHitTile!");
                        }
                    }
                }

                // CheckCircleRectangleCollision and swapped
                if (firstCCC != null && secondRCC != null)
                {
                    if (firstCCC.Owner.Tag == Tag.MainPlayer && secondRCC.Owner.Tag == Tag.Tile)
                    {
                        if (CheckCircleRectangleCollision(firstCCC, secondRCC))
                        {
                            EventSystem.Instance.AddEvent("PlayerIsGrounded");
                            //Console.WriteLine("1. PlayerIsGrounded!");
                        }
                    }
                    else if(firstCCC.Owner.Tag == Tag.PlayerBullet && secondRCC.Owner.Tag == Tag.Tile)
                    {
                        if (CheckCircleRectangleCollision(firstCCC, secondRCC))
                        {
                            EventSystem.Instance.AddEvent("ArrowHitTile");
                            //Console.WriteLine("1. ArrowHitTile!");
                        }      
                    }
                }

                if (firstRCC != null && secondCCC != null)
                {
                    if (firstRCC.Owner.Tag == Tag.Tile && secondCCC.Owner.Tag == Tag.MainPlayer)
                    {
                        if (CheckCircleRectangleCollision(secondCCC, firstRCC))
                        {
                            EventSystem.Instance.AddEvent("PlayerIsGrounded");
                            //Console.WriteLine("2. PlayerIsGrounded!");
                        }
                    }
                    else if (firstRCC != null && secondCCC != null)
                    {
                        if (firstRCC.Owner.Tag == Tag.Tile && secondCCC.Owner.Tag == Tag.PlayerBullet)
                        {
                            if (CheckCircleRectangleCollision(secondCCC, firstRCC))
                            {
                                EventSystem.Instance.AddEvent("ArrowHitTile");
                                //Console.WriteLine("2. ArrowHitTile!");
                            }
                        }
                    }

                }
            }
        }

        private bool ValidStartToEnd(int start, int end)
        {
            // checks if start location is greater than end location
            // (search region size less than one pixel means no possible intersection)
            if (!(start > end))
            {
                return true;
            }
            return false;
        }

        private bool CheckIntersectionInAxis(int searchRegionStart, int searchRegionEnd, int otherStart, int otherEnd)
        {
            if( (otherStart >= searchRegionStart && otherStart <= searchRegionEnd) ||   // other start point in search region?
                (otherEnd >= searchRegionStart && otherEnd <= searchRegionEnd)          // other end point in search region?  
              )
            {
                return true;                                    // yes, collision in an axis
            }
            return false;
        }

        private void CheckCollisionCirclesWithAllOtherCircles()
        {
            // used to avoid raising more than one event for the same collision
            List<int> previousCollisions = new List<int>();

            // checks all existing circle collider components for collisions
            // with all other circle colliders components (except themselves)
            for (int i = circleColCps.Count - 1; i >= 0; i--)
            {
                if (circleColCps[i] == null)
                {
                    circleColCps.RemoveAt(i);
                    Console.WriteLine("CircleColliderComponent owner == null");
                    continue;
                }

                for (int j = circleColCps.Count - 1; j >= 0; j--)
                {
                    if (circleColCps[j] == null)
                    {
                        circleColCps.RemoveAt(j);
                        Console.WriteLine("CircleColliderComponent owner == null");
                        continue;
                    }

                    // avoid collision checks of current circle collider component and other circle collider component of the same GameObject
                    if (!circleColCps[i].Owner.Name.Equals(circleColCps[j].Owner.Name))
                    {
                        if (CheckCircleCircleCollision(circleColCps[i], circleColCps[j]))
                        {
                            int hashColA = GenerateHash(circleColCps[i].Owner.Name, circleColCps[j].Owner.Name);
                            int hashColB = GenerateHash(circleColCps[j].Owner.Name, circleColCps[i].Owner.Name);
                            if (!(previousCollisions.Contains(hashColA) || previousCollisions.Contains(hashColB)))
                            {
                                // add collision to list to avoid raising an event for the same collision twice
                                previousCollisions.Add(hashColA);
                                //TODO: raise event (notify the event manager)
                                Console.WriteLine("Circle & Circle Collsion!");
                            }
                        }
                    }
                }
            }
        }

        private void CheckCollisionCirclesWithAllOtherRectangles()
        {
            // checks all existing rectangle collider components for collisions
            // with all other circle colliders components (except themselves)
            for (int i = circleColCps.Count - 1; i >= 0; i--)
            {
                if (circleColCps[i] == null)
                {
                    circleColCps.RemoveAt(i);
                    Console.WriteLine("CircleColliderComponent owner == null");
                    continue;
                }

                for (int j = rectangleColCps.Count - 1; j >= 0; j--)
                {
                    if (rectangleColCps[j] == null)
                    {
                        rectangleColCps.RemoveAt(j);
                        Console.WriteLine("RectangleColliderComponent owner == null");
                        continue;
                    }

                    // avoid collision checks of current circle collider component and other rectangle collider component of the same GameObject
                    if (!circleColCps[i].Owner.Name.Equals(rectangleColCps[j].Owner.Name))
                    {
                        if (CheckCircleRectangleCollision(circleColCps[i], rectangleColCps[j]))
                        {
                            //TODO: raise event (notify the event manager)
                            Console.WriteLine("Circle & Rectangle Collsion!");
                        }
                    }
                }
            }
        }

        private static Vector2 CalculateNewDragForce(double deltaTime, Vector2 force, Vector2 velocity)
        {
            force = -velocity * 0.9 * deltaTime;   //simulate drag (maybe use deltaTime in seconds instead of ms)

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

        public static Vector2 CalculateNewVelocity(double deltaTime, Vector2 velocity, Vector2 force, double mass)
        {
            return velocity += force / mass * deltaTime;
        }

        public static Vector2 CalculateNewPosition(double deltaTime, Vector2 velocity, double factor, Vector2 position)
        {
            velocity *= factor;
            return position += velocity * deltaTime;
        }

        public static Vector2[] CalculateNewMovement(double deltaTime, Vector2 position, Vector2 velocity, double factor, Vector2 force, double mass)
        {
            force = CalculateNewDragForce(deltaTime, force, velocity);
            velocity = CalculateNewVelocity(deltaTime, velocity, force, mass);
            position = CalculateNewPosition(deltaTime, velocity, factor, position);
            return new Vector2[] { force, velocity, position };
        }

        public void MousePosUpdate(GameObject player, Vector2 mousePosLocal)
        {
            MouseComponent mc = player.GetComponent<MouseComponent>() as MouseComponent;
            if(mc == null)
            {
                return;
            }
            SDL_Rect camera = Camera.camera;
            Vector2 cameraPosGlobal;
            cameraPosGlobal.X = camera.x;
            cameraPosGlobal.Y = camera.y;

            Vector2 mousePosGlobal = mousePosLocal + cameraPosGlobal;
            mc.PositionGlobal = mousePosGlobal;
            mc.Position = mousePosLocal;
        }

        public bool CheckCircleCircleCollision(CircleColliderComponent circleColA, CircleColliderComponent circleColB)
        {
            if (circleColA == null || circleColB == null)
            {
                return false;
            }

            //Calculate total radius squared
            int totalRadiusSquared = circleColA.Radius + circleColB.Radius;
            totalRadiusSquared = totalRadiusSquared * totalRadiusSquared;

            //If the distance between the centers of the circles is less than the sum of their radii
            if (DistanceSquared((int)circleColA.Owner.Position.X, (int)circleColA.Owner.Position.Y, (int)circleColB.Owner.Position.X, (int)circleColB.Owner.Position.Y) < (totalRadiusSquared))
            {
                //the circles have collided
                return true;
            }
            return false;
        }

        public bool CheckCircleRectangleCollision(CircleColliderComponent circleColA, RectangleColliderComponent rectangleColB)
        {
            if (circleColA == null || rectangleColB == null)
            {
                return false;
            }

            //Closest point on collision box
            int closePointX;
            int closePointY;

            double posX = circleColA.Owner.Position.X - circleColA.offset.X;
            double posY = circleColA.Owner.Position.Y - circleColA.offset.Y;

            //Find closest x offset
            if (posX < rectangleColB.Owner.Position.X)
            {
                closePointX = (int)rectangleColB.Owner.Position.X;
            }
            else if (posX > rectangleColB.Owner.Position.X + rectangleColB.Width)
            {
                closePointX = (int)rectangleColB.Owner.Position.X + rectangleColB.Width;
            }
            else
            {
                closePointX = (int)posX;
            }

            //Find closest y offset
            if (posY < rectangleColB.Owner.Position.Y)
            {
                closePointY = (int)rectangleColB.Owner.Position.Y;
            }
            else if (posY > rectangleColB.Owner.Position.Y + rectangleColB.Height)
            {
                closePointY = (int)rectangleColB.Owner.Position.Y + rectangleColB.Height;
            }
            else
            {
                closePointY = (int)posY;
            }

            //If the closest point is inside the circle
            if (DistanceSquared((int)posX, (int)posY, closePointX, closePointY) < circleColA.Radius * circleColA.Radius)
            {
                //This box and the circle have collided
                return true;
            }

            return false;
        }

        public Vector2 StaticCollisionReflection(Vector2 positionBeforeReflectionA, Vector2 positionOfCollisionPointB, Vector2 velocityBefore)
        {
            Vector2 normal = Vector2.Normalize(positionOfCollisionPointB - positionBeforeReflectionA);  // distance from a->b = b-a and normalized (vector length = 1)
            return velocityBefore - (2 * Vector2.Dot(velocityBefore, normal)) * normal;                 // v-2*(v*n)*n      //returns the new reflection velocity
        }

        private double DistanceSquared(int aX, int aY, int bX, int bY)
        {
            //Distance between two points/positions split up into X- and Y-Axis part
            int deltaX = bX - aX;
            int deltaY = bY - aY;
            return deltaX * deltaX + deltaY * deltaY;   //( x^2 + y^2 )
        }

        private int GenerateHash(string a, string b)
        {
            return new { a, b }.GetHashCode();
        }

        public void CalculateShootVelocity(GameObject shooter, GameObject laser)
        {
            //Calculate velX and velY depending on angle
        }

        /*
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
        */

        /*
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
        */

        /*
        public static Vector2 CalculateNewVelocityFromControls(Vector2 velocity, double factor, Controls controls)
        {
            return controls.Velocity * (factor * Window.SCALEFACTOR_X);
        }
        */

        /*
        public Vector2[] CalculatCircleCircleCollisionReaction
            (
            double deltaTime,
            Vector2 posi1,  //position first
            Vector2 posi2,  //position second
            Vector2 velo1,  //velocity first
            Vector2 velo2,  //velocity second
            Vector2 force1, //force first
            Vector2 force2, //force second
            double mass1,   //mass first
            double mass2,   //mass second
            double radius1, //radius first
            double radius2  //radius second
            )
        {
            //collision normal (unit vector between middle points)
            Vector2 normal = Vector2.Normalize(posi2 - posi1);
            //Proportion of speed along the contact normal
            Vector2 velo1Normal = normal * Vector2.Dot(normal, velo1);
            Vector2 velo2Normal = normal * Vector2.Dot(normal, velo2);
            //(elastic collision) new speed calculation is based on previous speed and mass
            Vector2 velo1New = (velo1Normal * (mass1 - mass2) + 2 * mass2 * velo2Normal) / (mass1 + mass2);
            Vector2 velo2New = (velo2Normal * (mass2 - mass1) + 2 * mass1 * velo1Normal) / (mass1 + mass2);
            //Speed of the circles after the collision
            Vector2 v1 = (velo1 - velo1Normal) + velo1New;
            Vector2 v2 = (velo2 - velo2Normal) + velo2New;

            //<-- Start: calculate overlap of the two circles and move back -->
            double distance = Vector2.Distance(posi2, posi1);                               //length between the two positions
            double halfOverlapDistance = 0.5 * (distance - radius1 - radius2);              //half of the distance that both balls overlap with each other, needed to push back / dismantle both balls in a collision
            Vector2 halfOverlap = new Vector2(halfOverlapDistance, halfOverlapDistance);    //save halfOverlapDistance as vector
            Vector2 displace1 = -(halfOverlap * (posi1 - posi2) / distance);                //put the first ball to its position before the collision (back: minus)
            Vector2 displays2 = (halfOverlap * (posi1 - posi2) / distance);                //put the second ball to its position before the collision (forward: plus)
            //<-- End: calculate the overlap of the two circles and move back -->

            return new Vector2[] {
                posi1 + displace1 + v1 * deltaTime,
                posi2 + displays2 + v2 * deltaTime,
                v1 + (force1 / mass1) * deltaTime,
                v2 + (force2 / mass2) * deltaTime
            };
        }
        */

        /*
        public bool CheckCollisionCircleLine(CircleCollider a, LineCollider b)
        {
            //calculate perpendicular vector length to the circle position
            Vector2 wallstartposToCircleendposDistance = a.Position - b.StartPos;
            Vector2 wallPerpendicularLeftNormalized = Vector2.Normalize(b.PerpendicularLeft);
            double perpendicularLengthBetweenCircleAndWall = Vector2.Dot(wallPerpendicularLeftNormalized, wallstartposToCircleendposDistance);

            //check collision between circle and line:
            //If the perpendicular distance from the line to the circle is 
            //less then (or equal to) the radius of the circle -> circle touches line!
            if (Math.Abs(perpendicularLengthBetweenCircleAndWall) <= a.Radius)
            {
                //This circle and line have collided
                return true;
            }

            return false;
        }
        */

        /*
        public Vector2 StaticCollisionReflection(Vector2 positionBeforeReflectionA, Vector2 positionOfCollisionPointB, Vector2 velocityBefore)
        {
            Vector2 normal = Vector2.Normalize(positionOfCollisionPointB - positionBeforeReflectionA);  // distance from a->b = b-a and normalized (vector length = 1)
            return velocityBefore - (2 * Vector2.Dot(velocityBefore, normal)) * normal;                 // v-2*(v*n)*n      //returns the new reflection velocity
        } 
        */

        /*
        private double DistanceSquared(int aX, int aY, int bX, int bY)
        {
            //Distance between two points/positions split up into X- and Y-Axis part
            int deltaX = bX - aX;
            int deltaY = bY - aY;
            return deltaX * deltaX + deltaY * deltaY;   //( x^2 + y^2 )
        }
        */
    }
}
