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
    class OptionsButton : Button
    {
        string buttonText2, currentText;
        bool toggled;

        public OptionsButton(Game1 game, string text1, string text2, Vector2 topLeftCorner, Texture2D emptyTex, Texture2D filledTex, SpriteFont font)
        {
            buttonText = text1;
            buttonText2 = text2;
            currentText = buttonText;
            this.topLeftCorner = topLeftCorner;
            emptyButton = emptyTex;
            filledButton = filledTex;
            this.font = font;
            held = false;
            buttonSound = game.MenuSelect;
            buttonFrame = new Rectangle((int)topLeftCorner.X, (int)topLeftCorner.Y, emptyButton.Width, emptyButton.Height);

            textAnchor = new Vector2(topLeftCorner.X + emptyButton.Width / 2 - font.MeasureString(buttonText).X / 2,
                                    topLeftCorner.Y + emptyButton.Height / 2 - font.MeasureString(buttonText).Y / 2);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (held)
            {
                spriteBatch.Draw(filledButton, buttonFrame, Color.White);
                spriteBatch.DrawString(font, currentText, textAnchor, Color.Black);
            }
            else
            {
                spriteBatch.Draw(emptyButton, buttonFrame, Color.White);
                spriteBatch.DrawString(font, currentText, textAnchor, Color.White);
            }
        }

        public void Toggle()
        {
            //Toggle the text on the Button
            if (!toggled)
            {
                toggled = true;
                currentText = buttonText2;
            }
            else
            {
                toggled = false;
                currentText = buttonText;
            }
            textAnchor.X = topLeftCorner.X + emptyButton.Width / 2 - font.MeasureString(buttonText).X / 2;
            textAnchor.Y = topLeftCorner.Y + emptyButton.Height / 2 - font.MeasureString(buttonText).Y / 2;
        }
    }
}
