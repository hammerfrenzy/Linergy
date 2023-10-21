using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace Linergy
{
    class CreditScreen : Screen
    {
        Texture2D background;
        Button exit;
        bool initialPress = true;
        bool screenHeld = false;

        public CreditScreen(string name, Game1 game)
        {
            this.game = game;
            this.name = name;
            this.musicName = "menumusic";
            background = game.Content.Load<Texture2D>("backgrounds/credits");
            buttonFont = game.Content.Load<SpriteFont>("fonts/MenuFont");

            exit = new Button(game, "exit", new Vector2(Game1.ScreenWidth / 2 - game.OptionsButtonEmpty.Width / 2,
                        Game1.ScreenHeight - game.OptionsButtonEmpty.Height), game.OptionsButtonEmpty, game.OptionsButtonFilled, buttonFont);
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
                    exit.Held = false;
                    Point p = new Point((int)touches[0].Position.X, (int)touches[0].Position.Y);
                    if (exit.ButtonFrame.Contains(p))
                        exit.Held = true;
                }
                if (t.State == TouchLocationState.Released)
                {
                    initialPress = true;
                    screenHeld = false;
                    if (!screenLock)
                    {
                        Point p = new Point((int)touches[0].Position.X, (int)touches[0].Position.Y);
                        if (exit.ButtonFrame.Contains(p))
                        {
                            game.player.Save(); //Save the game
                            game.Exit();        //Exit the game
                        }
                    }
                    exit.Held = false;
                }
            }
            exit.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Draw(background, new Rectangle(0, 0, Game1.ScreenWidth, Game1.ScreenHeight), Color.White);
            exit.Draw(gameTime, spriteBatch);
        }
    }
}
