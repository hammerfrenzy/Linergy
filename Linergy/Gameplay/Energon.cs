using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace Linergy
{
    class Energon : GameObject
    {
        protected Game1 game;
        protected Texture2D sprite;                     //Sprite representing this Energon
        protected Vector2 velocity;                     //This Energon's velocity
        protected Vector2 position;                     //This Energon's top left position
        protected Rectangle boundingRectangle;          //The rectangle used for collision
        protected bool collected;                       //Whether or not this Energon has just been collected
        protected bool active;                          //Whether or not this Energon is active in the game
        protected bool reflected;                       //Whether or not this Energon has been reflected
        protected bool setActiveTime;                   //Whether the activate time should be set
        protected int id;                               //An ID number for this Energon
        protected int bounceAllowance;                  //The number of times the Energon can bounce off the screen before leaving
        protected int powerLevel;                       //A number signifying this Energon's place in the power hierarchy
        protected int postCollectionTime;               //The length of time after this Energon has been collected
        protected float emitTimer;                      //Emit every ~second when reflected
        protected float energyValue;                    //Amount of energy gained when collecting this Energon
        protected double activatedTime;                  //The time of the first Update this Energon became Active

        public Energon() { }
        public Energon(Game1 game) 
        {
            this.sprite = game.DotSprite;
            this.game = game;
            
            powerLevel = 1;
            bounceAllowance = 1;
            emitTimer = 0;
            Initialize();
        }

        public override void Initialize()
        {
            //We'll set the position and velocity here, and let sub-classes modify these pre-sets
            active = true; //know to update/draw this Energon
            setActiveTime = true;
            reflected = false; //this energon hasn't been reflected by the GAET
            collected = false;
            id = Game1.GetID();
            bounceAllowance = 1;
            boundingRectangle = new Rectangle((int)position.X, (int)position.Y, sprite.Width, sprite.Height);
            float minVelocity = 1.75f;
            float maxVelocity = 3f;
            //Give a random Starting position and velocity
            #region RandomInitialization
            int whichWall = Game1.Random.Next(0, 4);
            switch (whichWall)
            {
                //Left wall
                case 0:
                    position.X = 1;
                    position.Y = Game1.RandomBetween(game.HUDHeight + 1, Game1.ScreenHeight - 1);
                    velocity.X = Game1.RandomBetween(minVelocity, maxVelocity);
                    velocity.Y = Game1.RandomBetween(-1.5f, 1.5f);
                    break;

                //Top Wall
                case 1:
                    position.X = Game1.RandomBetween(1, Game1.ScreenWidth - 1);
                    position.Y = game.HUDHeight + 1;
                    velocity.X = Game1.RandomBetween(-1.5f, 1.5f);
                    velocity.Y = Game1.RandomBetween(minVelocity, maxVelocity);
                    break;

                //Right Wall
                case 2:
                    position.X = Game1.ScreenWidth - 1;
                    position.Y = Game1.RandomBetween(game.HUDHeight + 1, Game1.ScreenHeight - 1);
                    velocity.X = Game1.RandomBetween(-minVelocity, -maxVelocity);
                    velocity.Y = Game1.RandomBetween(-1.5f, 1.5f);
                    break;

                //Bottom Wall
                case 3:
                    position.X = Game1.RandomBetween(1, Game1.ScreenWidth - 1);
                    position.Y = Game1.ScreenHeight - 1;
                    velocity.X = Game1.RandomBetween(-1.5f, 1.5f);
                    velocity.Y = Game1.RandomBetween(-minVelocity, -maxVelocity);
                    break;

                default:
                    position = Vector2.Zero;
                    break;
            }
            #endregion
        }

        public override void Update(GameTime gameTime)
        {
            position += velocity;
            boundingRectangle.X = (int)position.X;
            boundingRectangle.Y = (int)position.Y;
            if (setActiveTime)
                activatedTime = gameTime.TotalGameTime.TotalMilliseconds;
           
            //Deactivate any Energon that is no longer in the drawing area
            if (position.X < 0 || position.X > Game1.ScreenWidth ||
                position.Y < game.HUDHeight || position.Y > Game1.ScreenHeight && bounceAllowance <= 0)
                Deactivate();

            if (collected)
            {
                postCollectionTime += gameTime.ElapsedGameTime.Milliseconds;
                if (postCollectionTime > 1000)
                    Deactivate();
            }

            if (reflected) //emit every ~second
            {
                emitTimer += gameTime.ElapsedGameTime.Milliseconds;
                if (emitTimer > 5)
                {
                    Emit();
                    emitTimer = 0;
                }          
            }
        }

        public virtual void Draw()
        {
            if (!collected)
            {
                if (!reflected)
                    game.SpriteBatch.Draw(sprite, position, Color.White);
                else
                    game.SpriteBatch.Draw(sprite, position, Color.White);
            }
        }

        /// <summary>
        /// Override to emit particles from specific particle systems
        /// </summary>
        public virtual void Emit(){}
        public virtual void RemoveParticleSystem() { }

        public bool CollidesWith(Energon e)
        {
            if (boundingRectangle.Intersects(e.boundingRectangle) && this.id != e.ID)
                return true;
            else
                return false;
        }
        
        public virtual void Activate()
        {
            Initialize();
        }

        public virtual void Deactivate()
        {
            active = false;
            position = Vector2.Zero;
            velocity = Vector2.Zero;
        }  

        public void ChangeDirection(Vector2 normal)
        {
            //Vnew=V-2*N(V.N)  Perform the reflection against the normal
            Vector2 reflected = velocity - 2 * normal * (Vector2.Dot(normal, velocity));
            velocity = reflected;
        }

        public Vector2 Center()
        {
            return new Vector2(position.X + sprite.Width / 2, position.Y + sprite.Height / 2);
        }

        private Vector2 GetNormal(Vector2 start, Vector2 end)
        {
            //Calculate a normal vector for the vector between two points
            Vector2 normal = Vector2.Zero;
            normal.X = -(end.Y - start.Y);
            normal.Y = (end.X - start.X);
            double normalizedLength = Math.Sqrt(normal.X * normal.X + normal.Y * normal.Y);
            normal.X /= (float)normalizedLength;
            normal.Y /= (float)normalizedLength;
            return normal;
        }

        #region Accessors
        public Texture2D Texture
        {
            get { return sprite; }
            set { sprite = value; }
        }

        public Rectangle BoundingBox
        {
            get { return boundingRectangle; }
        }

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        public bool Reflected
        {
            get { return reflected; }
            set { reflected = value; }
        }

        public bool Collected { get { return collected; } set { collected = value; } }

        public float EnergyValue
        {
            get { return energyValue; }
        }

        public int Power
        {
            get { return powerLevel; }
            set { powerLevel = value; }
        }

        public int ID
        {
            get { return id; }
        }

        public int BounceAllowance
        {
            get { return bounceAllowance; }
        }
        #endregion
    }
}
