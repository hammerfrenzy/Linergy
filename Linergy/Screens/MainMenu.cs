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
    class MainMenu : Screen
    {
        Texture2D background, frameEmpty, frameFilled;
        SpriteFont timerFont;

        Button playButton, chaptersButton, optionsButton, exitButton;

        //Activate menus buttons on touch release
        bool initialPress, screenHeld;

        public MainMenu(string name, Game1 g)
        {
            this.name = name;
            nextScreen = "start";

            frameEmpty = g.MenuButtonEmpty;
            frameFilled = g.MenuButtonFilled;
            background = g.Content.Load<Texture2D>("backgrounds/placeholder3");
            bgm = g.Content.Load<Song>("audio/title");
            timerFont = g.Content.Load<SpriteFont>("fonts/MenuFont");
            this.musicName = "menumusic";
            this.game = g;

            int spacer = Game1.ScreenWidth / 32;

            playButton = new Button(g, "play", new Vector2(Game1.ScreenWidth / 2 - frameEmpty.Width - spacer,
                                     Game1.ScreenHeight / 2 - frameEmpty.Height - spacer), frameEmpty, frameFilled, timerFont);

            chaptersButton = new Button(g, "chapters", new Vector2(Game1.ScreenWidth / 2 + spacer,
                                     Game1.ScreenHeight / 2 - frameEmpty.Height - spacer), frameEmpty, frameFilled, timerFont);

            optionsButton = new Button(g, "options", new Vector2(Game1.ScreenWidth / 2 - frameEmpty.Width - spacer,
                                     Game1.ScreenHeight / 2 + spacer), frameEmpty, frameFilled, timerFont);

            exitButton = new Button(g, "exit", new Vector2(Game1.ScreenWidth / 2 + spacer,
                                     Game1.ScreenHeight / 2 + spacer), frameEmpty, frameFilled, timerFont);

            screenHeld = false;
            initialPress = true;
        }

        public override void Update(GameTime gameTime)
        {
            //Button Touch Logic
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
                    playButton.Held = chaptersButton.Held = optionsButton.Held = exitButton.Held = false;
                    Point p = new Point((int)touches[0].Position.X, (int)touches[0].Position.Y);
                    if (playButton.ButtonFrame.Contains(p))
                        playButton.Held = true;
                    if (chaptersButton.ButtonFrame.Contains(p))
                        chaptersButton.Held = true;
                    if (optionsButton.ButtonFrame.Contains(p))
                        optionsButton.Held = true;
                    if (exitButton.ButtonFrame.Contains(p))
                        exitButton.Held = true;
                }
                if (t.State == TouchLocationState.Released)
                {
                    initialPress = true;
                    screenHeld = false;

                    if (!screenLock)
                    {
                        Point p = new Point((int)touches[0].Position.X, (int)touches[0].Position.Y);
                        if (playButton.ButtonFrame.Contains(p))
                        {
                            nextScreen = "worldselect";
                            changeScreen = true;
                        }
                        if (chaptersButton.ButtonFrame.Contains(p))
                        {
                            nextScreen = "chapters";
                            changeScreen = true;
                        }
                        if (optionsButton.ButtonFrame.Contains(p))
                        {
                            nextScreen = "options";
                            changeScreen = true;
                        }
                        if (exitButton.ButtonFrame.Contains(p))
                        {
                            nextScreen = "exit";
                            changeScreen = true;
                        }
                        playButton.Held = chaptersButton.Held = optionsButton.Held = exitButton.Held = false;
                    }
                }
            }

            playButton.Update(gameTime);
            chaptersButton.Update(gameTime);
            optionsButton.Update(gameTime);
            exitButton.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.GraphicsDevice.Clear(Color.Black);
            //draw background
            spriteBatch.Draw(background, new Rectangle(0, 0, Game1.ScreenWidth, Game1.ScreenHeight), Color.White * .4f);
            //draw buttons
            playButton.Draw(gameTime, spriteBatch);
            chaptersButton.Draw(gameTime, spriteBatch);
            optionsButton.Draw(gameTime, spriteBatch);
            exitButton.Draw(gameTime, spriteBatch);
        }

        public override void Reset(GameTime gameTime)
        {
            //Ask player to buy if in trial mode
            if (Guide.IsTrialMode && game.player.UnlockedLevels >= 4)
            {
                List<String> mbList = new List<string>();
                mbList.Add("OK");
                mbList.Add("Cancel");
                // BeginShowMessageBox is asynchronous. We define the method PromptPurchase as the callback.

                Guide.BeginShowMessageBox("Trial Mode Complete!", "Tap OK to buy the full game!", mbList, 0,
                                                MessageBoxIcon.None, PromptPurchase, null);
            }
            playButton.Held = chaptersButton.Held = optionsButton.Held = exitButton.Held = false;
            base.Reset(gameTime);
        }

        /// <summary>
        /// Prompt the user to buy the Game!
        /// </summary>
        /// <param name="ar"></param>
        private void PromptPurchase(IAsyncResult ar)
        {
            // Complete the ShowMessageBox operation and get the index of the button that was clicked.
            int? result = Guide.EndShowMessageBox(ar);

            // Clicked "OK", so bring the user to the application's Marketplace page to buy the application.
            if (result.HasValue && result == 0)
            {
                Guide.ShowMarketplace(PlayerIndex.One);
            }
        }
    }
}
