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

namespace Linergy
{
    class StaticGameObject :  GameObject
    {

        protected Vector2 position;     //Location of the Object
        protected int lifespan;         //Lifespan of the Object in milliseconds
        protected bool active;          //Whether or not this Object is active
        protected bool activateEffect;  //Lets the StaticGameObject do some special event
        protected bool affectsEnergons; //Whether or not this Object affects Energon movement

        public StaticGameObject()
        {
            position = Vector2.Zero;
            lifespan = 0;
            active = false;
            affectsEnergons = false;
        }

        public override void Update(GameTime gameTime)
        {
            if (active)
                lifespan -= gameTime.ElapsedGameTime.Milliseconds;
            if (lifespan <= 0)
                active = false;
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
        }

        
        /// <summary>
        /// A place for the StaticGameObject to change Energon's behavior
        /// </summary>
        /// <param name="e"></param>
        public virtual void AssertInfluence(Energon e)
        {
        }

        public Vector2 Position
        {
            get { return position; }
        }
        
        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        public bool ActivateEffect
        {
            get { return activateEffect; }
            set { activateEffect = value; }
        }
    }
}
