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
    class Screen
    {
        protected Game1 game;
        protected SpriteFont buttonFont; //Font for buttons on any screen
        protected string name;           //The name of this screen
        protected string nextScreen;     //The name of the screen that this screen will change to
        protected int activateTime;      //The time that this screen was activated
        protected bool changeScreen;     //Whether or not this screen should change to the next screen
        protected bool screenLock;       //Whether or not this screen is locked from changing
        protected Song bgm;              //This screen's background music
        protected string musicName;      //A title for this screen's music (for continuing a song shared between screens)        

        public Screen() { }
        public Screen(Game1 game, string name)
        {
            this.game = game;
            this.name = name;
            screenLock = true;
        }

        public virtual void Update(GameTime gameTime)
        {
            //unlock the screen after one second
            if (screenLock && gameTime.TotalGameTime.TotalMilliseconds - activateTime >= 200)
                screenLock = false;
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            
        }

        /// <summary>
        /// Resets relevant screen information to its initial state
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Reset(GameTime gameTime)
        {
            changeScreen = false;
            screenLock = true;
            activateTime = (int)gameTime.TotalGameTime.TotalMilliseconds;
        }

        /// <summary>
        /// A place to perform actions when the screen becomes active.
        /// </summary>
        public virtual void OnScreenActivate()
        {
        }


        /// <summary>
        /// Lets the screen do World-specific setup
        /// </summary>
        /// <param name="world">the number of the world</param>
        public virtual void TellWorld(int world)
        {
        }
        
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Song Music
        {
            get { return bgm; }
        }

        public string NextScreen
        {
            get { return nextScreen; }
            set { nextScreen = value; }
        }

        public string MusicName
        {
            get { return musicName; }
        }

        public bool ChangeScreen
        {
            get { return changeScreen; }
            set { changeScreen = value; }
        }

        public bool ScreenLock
        {
            get { return screenLock; }
            set { screenLock = value; }
        }

        public int ActivateTime
        {
            get { return activateTime; }
            set { activateTime = value; }
        }
    }
}
