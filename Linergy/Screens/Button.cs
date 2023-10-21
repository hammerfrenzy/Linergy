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
    class Button
    {
        protected Texture2D emptyButton, filledButton; //The textures used for when the Button is up / down
        protected Vector2 topLeftCorner, textAnchor;   //Top left coordinates for the Button and the text inside it
        protected Rectangle buttonFrame;               //The rectangle that defines the Button boundaries
        protected SpriteFont font;                     //The font used to display the Button's text
        protected SoundEffect buttonSound;             //The sound the button makes when touched
        protected string buttonText;                   //The text the button will display (centered)
        protected bool held, initialPress;             //Whether or not the Button is held down, if it's the first press of the hold

        public Button() { }

        public Button(Game1 game, string text, Vector2 topLeftCorner, Texture2D emptyTex, Texture2D filledTex, SpriteFont font)
        {
            buttonText = text;
            this.topLeftCorner = topLeftCorner;
            emptyButton = emptyTex;
            filledButton = filledTex;
            this.font = font;
            held = false;
            initialPress = true;

            if (buttonText == "back" || buttonText == "yes" || buttonText == "start")
                buttonSound = game.MenuBack;
            else
                buttonSound = game.MenuSelect;

            buttonFrame = new Rectangle((int)topLeftCorner.X, (int)topLeftCorner.Y, emptyButton.Width, emptyButton.Height);

            textAnchor = new Vector2(topLeftCorner.X + emptyButton.Width / 2 - font.MeasureString(buttonText).X / 2,
                                    topLeftCorner.Y + emptyButton.Height / 2 -  font.MeasureString(buttonText).Y / 2);
        }

        public virtual void Update(GameTime gameTime)
        {
            if (initialPress && held)
            {
                //Play sound
                if (Game1.ShouldPlaySound)
                    buttonSound.Play(1.0f, 0, 0);
                initialPress = false;
            }
            if (!held && !initialPress)
                initialPress = true;
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (held)
            {
                spriteBatch.Draw(filledButton, buttonFrame, Color.White);
                spriteBatch.DrawString(font, buttonText, textAnchor, Color.Black);
            }
            else
            {
                spriteBatch.Draw(emptyButton, buttonFrame, Color.White);
                spriteBatch.DrawString(font, buttonText, textAnchor, Color.White);
            }
        }

        public bool Held
        {
            get { return held; }
            set { held = value; }
        }

        public string Text
        {
            get { return buttonText; }
            set { buttonText = value; }
        }

        public Vector2 TopLeftCorner
        {
            get { return topLeftCorner; }
            set { topLeftCorner = value; }
        }

        public Rectangle ButtonFrame
        {
            get { return buttonFrame; }
        }
    }
}
