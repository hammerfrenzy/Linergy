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
    class TitleScreen : Screen
    {
        Texture2D presentationScreen;
        SpriteFont titleFont;
        SpriteFont smallTitleFont;
        string staticText;
        string flickerText;
        Vector2 staticTextPos;
        Vector2 flickerTextPos;
        float flickerOpacity = 0.0f;
        float titleOpacity = 0.0f;
        float presentationOpacity = 0.01f;
        bool flickering, initialPress, intro, presentationFading = true;
        bool screenHeld = false;

        public TitleScreen(string name, Game1 game)
        {
            this.name = name;
            this.nextScreen = "mainmenu";
            this.musicName = "menumusic";
            this.game = game;

            bgm = game.Content.Load<Song>("audio/title");
            titleFont = game.Content.Load<SpriteFont>("fonts/TitleFont");
            smallTitleFont = game.Content.Load<SpriteFont>("fonts/SmallTitleFont");
            presentationScreen = game.Content.Load<Texture2D>("backgrounds/presents");

            staticText = "linergy";
            flickerText = "tap to start";

            staticTextPos.X = Game1.ScreenWidth / 2 - (titleFont.MeasureString(staticText).X / 2);
            staticTextPos.Y = Game1.ScreenHeight / 4;
            flickerTextPos.X = Game1.ScreenWidth / 2 - (smallTitleFont.MeasureString(flickerText).X / 2);
            flickerTextPos.Y = Game1.ScreenHeight / 2 + Game1.ScreenHeight / 4;
        }

        public override void Update(GameTime gameTime)
        {
            //Flash the "presents" image in, then out
            intro = true;
            if (presentationFading)
                presentationOpacity += .02f;
            else
                presentationOpacity -= .02f;
            if (presentationOpacity >= 1.3)
                presentationFading = false;
            if (!presentationFading && presentationOpacity < 0)
                intro = false;

            //Cause "tap to start" to fade in and out
            if (!intro && flickering)
            {
                flickerOpacity -= .02f;
                if (flickerOpacity <= 0)
                    flickering = false;
            }
            else if (!intro)
            {
                flickerOpacity += .02f;
                titleOpacity += .02f;
                if (titleOpacity > 1f)
                    titleOpacity = 1f;
                if (flickerOpacity >= 1)
                    flickering = true;
            }

            TouchCollection touches = TouchPanel.GetState();
            foreach (TouchLocation t in touches)
            {
                if (t.State == TouchLocationState.Pressed && initialPress)
                {
                    initialPress = false;
                    screenHeld = true;
                }
                if (t.State == TouchLocationState.Moved && screenHeld)
                {
                }
                if (t.State == TouchLocationState.Released)
                {
                    initialPress = true;
                    screenHeld = false;
                    if (!screenLock && !intro)
                    {
                        nextScreen = "mainmenu";
                        if (Game1.ShouldPlaySound)
                            game.MenuBack.Play();
                        changeScreen = true;
                    }
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.GraphicsDevice.Clear(Color.Black);

            if (intro)
                spriteBatch.Draw(presentationScreen, Vector2.Zero, Color.White * presentationOpacity);
            else
            {
                spriteBatch.DrawString(titleFont, staticText, staticTextPos, Color.White * titleOpacity);
                spriteBatch.DrawString(smallTitleFont, flickerText, flickerTextPos, Color.White * flickerOpacity);
            }
            base.Update(gameTime);
        }
    }
}
