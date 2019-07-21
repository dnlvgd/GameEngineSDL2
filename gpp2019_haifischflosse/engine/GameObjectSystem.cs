using System;
using System.Collections.Generic;
using XnaGeometry;

namespace gpp2019_haifischflosse
{
    public class GameObjectSystem
    {
        private Dictionary<string, GameObject> gameObjects = new Dictionary<string, GameObject>();
        private int indexofLastEvent = 0;

        public GameObject CreateGameObject(string name, Tag tag, double posX, double posY, double scaleX, double scaleY, double degAngle, bool isActive)
        {
            GameObject go = new GameObject(name, tag, posX, posY, scaleX, scaleY, degAngle, isActive);
            if(gameObjects.ContainsKey(name))
            {
                gameObjects[name] = go;
            } else
            {
                gameObjects.Add(name, go);
            }
            return go;
        }

        public void DeleteGameObject(string gameObjectKey)
        {
            gameObjects.TryGetValue(gameObjectKey, out GameObject go);
            if (go != null)
            {
                foreach (Component c in go.Components)
                {
                    RemoveComponentFromSystem(c);
                }
                go = null;
            }
            gameObjects.Remove(gameObjectKey);
        }

        public GameObject FindGameObject(string gameObjectKey)
        {
            gameObjects.TryGetValue(gameObjectKey, out GameObject go);
            return go;
        }

        public void ActivateGameObject(string gameObjectKey)
        {
            gameObjects.TryGetValue(gameObjectKey, out GameObject go);
            go.IsActive = true;
        }

        public void DeactivateGameObject(string gameObjectKey)
        {
            gameObjects.TryGetValue(gameObjectKey, out GameObject go);
            go.IsActive = false;
        }

        public bool IsGameObjectActive(string gameObjectKey)
        {
            gameObjects.TryGetValue(gameObjectKey, out GameObject go);
            return go.IsActive;
        }

        public void Update(Vector2 globalMousePos)
        {
            /* Event handling */
            List<EventStruct> myEvents;
            myEvents = EventSystem.Instance.GetEvents("CloneGameObject", indexofLastEvent);
            foreach (EventStruct e in myEvents)
            {
                CloneGameObject(e);
            }

            if (EventSystem.Instance.CheckEvent("LoadDeathScreen", indexofLastEvent))
            {
                if (gameObjects.TryGetValue("gameOverBackground", out GameObject background))
                {
                    TextureComponent backgroundTC = background.GetComponent<TextureComponent>() as TextureComponent;
                    if (backgroundTC != null)
                    {
                        Vector2 cameraOffSet = new Vector2(Camera.camera.x - backgroundTC.DstRect.w / 4, Camera.camera.y - backgroundTC.DstRect.y / 4);
                        background.Position = background.Position + cameraOffSet;
                        EventSystem.Instance.AddEvent("ActivateGameObject", background);
                    }
                }
            }

            myEvents = EventSystem.Instance.GetEvents("ActivateGameObject", indexofLastEvent);
            foreach (EventStruct e in myEvents)
            {
                GameObject gameObject = e.data[0] as GameObject;
                if (!gameObject.IsActive)
                {
                    Console.WriteLine("Activate GO: " + gameObject.Name);
                    gameObject.IsActive = true;
                    foreach (var gameObjectCP in gameObject.Components)
                    {
                        AddComponentToSystem(gameObjectCP);
                    }
                }
            }

            List<EventStruct> cbEvents = EventSystem.Instance.GetEvents("CloneBullet", indexofLastEvent);
            foreach (EventStruct e in cbEvents)
            {
                GameObject cloneObject = CloneGameObject(e);

                MoveComponent mcClone = cloneObject.GetComponent<MoveComponent>() as MoveComponent;
                Vector2 tempVelocity = new Vector2();

                double angleToRad;
                if (cloneObject.Tag == Tag.PlayerBullet)
                {
                    angleToRad = (double)e.data[6] - 90;
                }
                else
                {
                    angleToRad = (double)e.data[6] + 90;
                }


                angleToRad = angleToRad / 180.0 * Math.PI;


                tempVelocity.X = Math.Cos(angleToRad);

                tempVelocity.Y = Math.Sin(angleToRad);

                mcClone.Velocity = tempVelocity * (mcClone.Factor * Window.SCALEFACTOR_X);
            }

            /* Check if GO is out of World Bounds */
            /*
            foreach (var go in gameObjects.Values)
            {
                int goX = (int)go.Position.X;
                int goY = (int)go.Position.Y;

                if (((goX < -200 || goX > World.WORLD_WIDTH + 200) || (goY < -200 || goY > World.WORLD_HEIGHT + 200)) && go.IsActive)
                {
                    EventSystem.Instance.AddEvent("DeactivateGameObject", go);
                }
            }
            */

            if (EventSystem.Instance.CheckEvent("DeactivateMainPlayer", indexofLastEvent))
            {
                if (gameObjects.TryGetValue("player", out GameObject player))
                {
                    if (player.IsActive)
                    {
                        Console.WriteLine("Deactivate GO: " + player.Name);
                        player.IsActive = false;
                        foreach (var gameObjectCP in player.Components)
                        {
                            RemoveComponentFromSystem(gameObjectCP);
                        }
                    }
                }
            }

            if (EventSystem.Instance.CheckEvent("DeactivateAllGameObjects", indexofLastEvent))
            {
                Console.WriteLine("DeactivateAllGameObjects Fetched!");
                //EventSystem.Instance.AddEvent("RemoveAllCPs");
                foreach (var go in gameObjects.Values)
                {
                    EventSystem.Instance.AddEvent("DeactivateGameObject", go);
                }
            }

            myEvents = EventSystem.Instance.GetEvents("DeactivateGameObject", indexofLastEvent);
            foreach (EventStruct e in myEvents)
            {
                GameObject gameObject = e.data[0] as GameObject;
                if (gameObject.IsActive)
                {
                    Console.WriteLine("Deactivate GO: " + gameObject.Name);
                    gameObject.IsActive = false;
                    foreach (var gameObjectCP in gameObject.Components)
                    {
                        RemoveComponentFromSystem(gameObjectCP);
                    }
                }
            }

            indexofLastEvent = EventSystem.Instance.GetMyIndex();
        }

        private GameObject CloneGameObject(EventStruct e)
        {
            GameObject originObject = e.data[0] as GameObject;

            if (originObject != null)
            {
                GameObject cloneObject;
                Tag prefabTag = (Tag)e.data[1];
                double prefabPosX = (double)e.data[2];
                double prefabPosY = (double)e.data[3];
                double prefabScaleX = (double)e.data[4];
                double prefabScaleY = (double)e.data[5];
                double prefabAngle = (double)e.data[6];
                bool prefabIsActive = (bool)e.data[7];
                int prefabID = (int)e.data[8];

                cloneObject = new GameObject(originObject.Name + prefabID, prefabTag, prefabPosX, prefabPosY, prefabScaleX, prefabScaleY, prefabAngle, true);
                // if e.data[9] exits and is true add BulletStateComponent from e.data[10]
                try
                {
                    if (e.data[9] != null)
                    {
                        bool addBulletStateComponent = (bool)e.data[9];
                        if (addBulletStateComponent)
                        {
                            bool shootLeft = (bool)e.data[10];
                            cloneObject.AddComponent(true, Game.behaviorSys.CreateBehaviorComponent(new BulletStateComponent(shootLeft)));
                        }
                    }
                }
                catch (IndexOutOfRangeException ex)
                {
                   Console.WriteLine(ex.Message);
                }

                Console.WriteLine("cloneObject: " + cloneObject.Name + " at " + prefabPosX + " " + prefabPosY);
                foreach (var originCP in originObject.Components)
                {
                    var cloneCP = originCP.Clone();
                    cloneObject.AddComponent(cloneCP, cloneObject);
                    if (cloneCP.GetType().Name.Equals("BoundingBoxComponent"))
                    {
                        BoundingBoxComponent bc = (BoundingBoxComponent)cloneCP;
                        Console.WriteLine(bc.Box.x + " . " + bc.Box.y);

                    }
                    AddComponentToSystem(cloneCP);
                }



                gameObjects.Add(cloneObject.Name, cloneObject);
                return cloneObject;
            }

            return null;
        }

        private void AddComponentToSystem(Component c)
        {
            /*Add each component c to each the relevant system with events*/
            switch (c.GetType().Name)
            {
                case "SpriteComponent":
                    EventSystem.Instance.AddEvent("AddSpriteCP", c);
                    break;

                case "TextureComponent":
                    EventSystem.Instance.AddEvent("AddTextureCP", c);
                    break;

                case "MoveComponent":
                    EventSystem.Instance.AddEvent("AddMoveCP", c);
                    break;

                case "PhysicComponent":
                    EventSystem.Instance.AddEvent("AddPhysicCP", c);
                    break;

                case "BoundingBoxComponent":
                    EventSystem.Instance.AddEvent("AddBoundingBoxCP", c);
                    break;

                case "CircleColliderComponent":
                    EventSystem.Instance.AddEvent("AddCircleColliderCP", c);
                    break;

                case "RectangleColliderComponent":
                    EventSystem.Instance.AddEvent("AddRectangleColliderCP", c);
                    break;

                case "WeaponComponentComponent":
                    EventSystem.Instance.AddEvent("AddWeaponCP", c);
                    break;

                case "SpawnComponent":
                    EventSystem.Instance.AddEvent("AddSpawnCP", c);
                    break;
            }

            /*Special case for BehaviorComponent*/
            if (c is IBehavior)
            {
                EventSystem.Instance.AddEvent("AddBehaviorCP", c);
            }
        }

        private void AddComponentToSystemWithType(Component c)
        {
            /*Add each component c to each the relevant system with events*/
            EventSystem.Instance.AddEventWithType(c.GetType(), c);

            /*Special case for BehaviorComponent*/
            if (c is IBehavior)
            {
                EventSystem.Instance.AddEvent("AddBehaviorCP", c);
            }
        }

        private void RemoveComponentFromSystem(Component c)
        {
            /*Remove component c from the relevant system with events*/
            switch (c.GetType().Name)
            {
                case "SpriteComponent":
                    EventSystem.Instance.AddEvent("RemoveSpriteCP", c);
                    break;

                case "TextureComponent":
                    EventSystem.Instance.AddEvent("RemoveTextureCP", c);
                    break;

                case "MoveComponent":
                    EventSystem.Instance.AddEvent("RemoveMoveCP", c);
                    break;

                case "PhysicComponent":
                    EventSystem.Instance.AddEvent("RemovePhysicCP", c);
                    break;

                case "BoundingBoxComponent":
                    EventSystem.Instance.AddEvent("RemoveBoundingBoxCP", c);
                    break;

                case "CircleColliderComponent":
                    EventSystem.Instance.AddEvent("RemoveCircleColliderCP", c);
                    break;

                case "RectangleColliderComponent":
                    EventSystem.Instance.AddEvent("RemoveRectangleColliderCP", c);
                    break;

                case "WeaponComponentComponent":
                    EventSystem.Instance.AddEvent("RemoveWeaponCP", c);
                    break;

                case "SpawnComponent":
                    EventSystem.Instance.AddEvent("RemoveSpawnCP", c);
                    break;
            }

            /*Special case for BehaviorComponent*/
            if (c is IBehavior)
            {
                EventSystem.Instance.AddEvent("RemoveBehaviorCP", c);
            }
        }

        private double CalculateGameObjectRotation(Vector2 pos)
        {
            // mousePlayer = go.GetComponent<MouseComponent>() as MouseComponent;
            double defaultAngleOffset = -90;

            double angle_rad = Math.Atan2((pos.Y), (pos.X));
            double angle_deg = ((angle_rad * 180) / Math.PI);

            return angle_deg - defaultAngleOffset;
        }

        private Vector2 CalculateRotationBasedOnAngle(Vector2 currentPos, double angleInDegree)
        {
            angleInDegree -= 90;
            Vector2 result;
            result.X = currentPos.X * Math.Cos(DegToRad(angleInDegree)) - currentPos.Y * Math.Sin(DegToRad(angleInDegree)); // x′ = x * cos(θ) − y * sin(θ)
            result.Y = currentPos.X * Math.Sin(DegToRad(angleInDegree)) + currentPos.Y * Math.Cos(DegToRad(angleInDegree)); // y′ = x * sin(θ) + y * cos(θ)
            return result;
        }

        private double DegToRad(double alphaDegree)
        {
            //from Degree to Radiant
            return alphaDegree / 180.0 * Math.PI;   // x = a / 180° * π
        }

        private double RadToDeg(double radiant)
        {
            //from Degree to Radiant
            return radiant / Math.PI * 180.0;       // a = x / π * 180°
        }

        public Vector2 GlobalMousePos(Vector2 localMousePos)
        {
            Vector2 cameraPosGlobal;
            cameraPosGlobal.X = Camera.camera.x;
            cameraPosGlobal.Y = Camera.camera.y;
            return localMousePos + cameraPosGlobal; // return globalMousePos 
        }

        public void RescaleGameObjectPositions()
        {
            foreach (var go in gameObjects.Values)
            {
                /*
                Console.WriteLine(
                   "OLD_POSITION_X: {0}" +
                   " OLD_POSITION_Y: {1}",
                   go.Position.X,
                   go.Position.Y
                   );
                */
                double windowFactorWidth = (double)Window.CURRENT_SCREEN_WIDTH / Window.OLD_SCREEN_WIDTH;
                double windowFactorHeight = (double)Window.CURRENT_SCREEN_HEIGHT / Window.OLD_SCREEN_HEIGHT;

                go.Position = new Vector2(go.Position.X * windowFactorWidth,
                go.Position.Y * windowFactorHeight);

                Camera.camera.x = (int)(Camera.camera.x * windowFactorWidth);
                Camera.camera.y = (int)(Camera.camera.y * windowFactorHeight);
                /*
                Console.WriteLine(
                    "NEW_POSITION_X: {0}" +
                    " NEW_POSITION_Y: {1}",
                    go.Position.X,
                    go.Position.Y
                    );
                */
            }
        }
    }

    public enum Tag
    {
        // used to check Tag of GameObject's for collision reaction
        MainPlayer,
        PlayerBullet,
        EnemyNormal,
        EnemyFast,
        EnemyBoss,
        EnemyNormalBullet,
        EnemySpawner,
        GameBackground,
        MenuBackground,
        LoadGameButton,
        FpsIndicator,
        WaveIndicator,
        EnemysKilledIndicator,
        BackgroundMusic,
        HealthPackage,
        PickUp,
        PlayerHeart,
        PlayerHeartIndicator,
        GameOverText,
        Text,
        Coin,
        Tile,
        Button,
        Flag,
        SoundFX
    }

    public enum Input
    {
        None,
        D_Press,                    // walk right
        D_Release,                  // 
        A_Press,                    // walk left
        A_Release,                  // 
        S_Press,                    // slide/crouch
        S_Release,                  // 
        Strg_Press,                 // crouch
        Strg_Release,               //
        Shift_Press,                // Run
        Shift_Release,              //
        Space_Press,                // jump
        Space_Release,              //
        MouseButtonLeft_Press,      // shoot bow
        MouseButtonLeft_Release,    //
        MouseButtonRight_Press,     // attack melee
        MouseButtonRight_Release    //

        /*
        QuitWindowsRedX,
        Escape,
        BackSpace,
        Zero,
        One,
        Two,
        Three,
        MouseMoved,
        MousePosition,
        MouseButtonLeft,
        MouseButtonLeftRe,
        MouseButtonRight,
        MouseButtonRightRe,
        w,
        Wre,
        s,
        SRe,
        a,
        ARe,
        d,
        DRe,
        ArrowUp,
        ArrowUpRe,
        ArrowDown,
        ArrowDownRe,
        ArrowLeft,
        ArrowLeftRe,
        ArrowRight,
        ArrowRightRe,
        r,
        rRe    
         */
    }

    public enum Animation
    {
        None,
        IdleRight,
        CrouchRight,
        CrouchWalkRight,
        CrouchWalkLeft,
        WalkRight,
        WalkLeft,
        RunRight,
        RunLeft,
        SlideRight,
        SlideLeft,
        ShootBowRight,
        ShootBowLeft,
        JumpRight,
        JumpLeft,
        AirShootBowRight,
        AirShootBowLeft,
        AirMeleeAttackStartRight,
        AirMeleeAttackStartLeft,
        AirMeleeAttackLoopRight,
        AirMeleeAttackLoopLeft,
        AirMeleeAttackEndRight,
        AirMeleeAttackEndLeft,
        AirFallRight,
        AirFallLeft,
        RollRight,
        RollLeft,
        StandUpRight,
        StandUpLeft,
        DieRight,
        DieLeft,
        CoinRotation,
        FlagWaving,
        FlyingArrowRight,
        FlyingArrowLeft,
        FlyingArrowHit,
        FlyingArrowPuff
    }
}