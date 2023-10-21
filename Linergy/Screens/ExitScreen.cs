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
    class ExitScreen : Screen
    {
        SpriteFont exitFont;

        Texture2D emptyFrame, filledFrame, background;
        Vector2 usureAnchor;
        string queryText;

        Button yes, no;

        //Activate menus buttons on touch release
        bool initialPress, screenHeld;

        public ExitScreen(string name, Game1 game)
        {
            this.name = name;
            this.nextScreen = "mainmenu";
            this.musicName = "menumusic";
            queryText = "are you sure you want to exit?";

            exitFont = game.Content.Load<SpriteFont>("fonts/MenuFont");
            emptyFrame = game.Content.Load<Texture2D>("backgrounds/optionsFrameEmpty");
            filledFrame = game.Content.Load<Texture2D>("backgrounds/optionsFrameFilled");
            background = game.Content.Load<Texture2D>("backgrounds/placeholder3");

            usureAnchor = new Vector2(Game1.ScreenWidth / 2 - exitFont.MeasureString(queryText).X / 2,
                                        Game1.ScreenHeight / 4);

            yes = new Button(game, "yes", new Vector2(Game1.ScreenWidth / 3 - emptyFrame.Width / 2,
                                    Game1.ScreenHeight - Game1.ScreenHeight / 3 - emptyFrame.Height / 2), emptyFrame, filledFrame, exitFont);

            no = new Button(game, "no", new Vector2(Game1.ScreenWidth - Game1.ScreenWidth / 3 - emptyFrame.Width / 2,
                                    Game1.ScreenHeight - Game1.ScreenHeight / 3 - emptyFrame.Height / 2), emptyFrame, filledFrame, exitFont);

            screenHeld = false;
            initialPress = true;

            this.game = game;
        }

        public override void Update(GameTime gameTime)
        {
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
                    yes.Held = no.Held = false;
                    Point p = new Point((int)touches[0].Position.X, (int)touches[0].Position.Y);
                    if (yes.ButtonFrame.Contains(p))
                        yes.Held = true;
                    if (no.ButtonFrame.Contains(p))
                        no.Held = true;
                }
                if (t.State == TouchLocationState.Released)
                {
                    initialPress = true;
                    screenHeld = false;

                    if (!screenLock)
                    {
                        Point p = new Point((int)touches[0].Position.X, (int)touches[0].Position.Y);
                        if (yes.ButtonFrame.Contains(p))
                        {
                            nextScreen = "credits";
                            changeScreen = true;
                        }
                        if (no.ButtonFrame.Contains(p))                
                        {
                            nextScreen = "mainmenu";
                            changeScreen = true;
                        }
                    }
                }
            }

            yes.Update(gameTime);
            no.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //draw background
            spriteBatch.Draw(background, new Rectangle(0, 0, Game1.ScreenWidth, Game1.ScreenHeight), Color.White * .4f);
            yes.Draw(gameTime, spriteBatch);
            no.Draw(gameTime, spriteBatch);
            spriteBatch.DrawString(exitFont, queryText, usureAnchor, Color.White); 
        }

        public override void Reset(GameTime gameTime)
        {
            yes.Held = no.Held = false;
            base.Reset(gameTime);
        }
    }
}
