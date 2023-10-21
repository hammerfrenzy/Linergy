using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Phone;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace Linergy
{
    class TutorialScreen : Screen
    {
        List<Texture2D> tutorialImages;
        Button next, prev;
        Rectangle screen;
        bool hasNext, initialPress = true;
        bool hasPrev, screenHeld = false;
        int currentTutorial = 0;

        public TutorialScreen(string name, Game1 game)
        {
            this.game = game;
            this.name = name;
            this.nextScreen = "prelude";
            buttonFont = game.Content.Load<SpriteFont>("fonts/MenuFont");
            //this.musicName = "tutorialmusic"
            tutorialImages = new List<Texture2D>();
            if (!LowMemoryHelper.IsLowMemDevice)
            {
                #region Load Tutorial Images 512MB Device
                tutorialImages.Add(game.Content.Load<Texture2D>("tutorial/tutorial0"));
                tutorialImages.Add(game.Content.Load<Texture2D>("tutorial/tutorial1a"));
                tutorialImages.Add(game.Content.Load<Texture2D>("tutorial/tutorial1b"));
                tutorialImages.Add(game.Content.Load<Texture2D>("tutorial/tutorial1c"));
                tutorialImages.Add(game.Content.Load<Texture2D>("tutorial/tutorial1d"));
                tutorialImages.Add(game.Content.Load<Texture2D>("tutorial/tutorial2a"));
                tutorialImages.Add(game.Content.Load<Texture2D>("tutorial/tutorial2b"));
                tutorialImages.Add(game.Content.Load<Texture2D>("tutorial/tutorial3a"));
                tutorialImages.Add(game.Content.Load<Texture2D>("tutorial/tutorial3b"));
                tutorialImages.Add(game.Content.Load<Texture2D>("tutorial/tutorial3c"));
                tutorialImages.Add(game.Content.Load<Texture2D>("tutorial/tutorial4a"));
                tutorialImages.Add(game.Content.Load<Texture2D>("tutorial/tutorial5a"));
                tutorialImages.Add(game.Content.Load<Texture2D>("tutorial/tutorial5b"));
                tutorialImages.Add(game.Content.Load<Texture2D>("tutorial/tutorial5c"));
                tutorialImages.Add(game.Content.Load<Texture2D>("tutorial/tutorial6a"));
                tutorialImages.Add(game.Content.Load<Texture2D>("tutorial/tutorial6b"));
                tutorialImages.Add(game.Content.Load<Texture2D>("tutorial/tutorial6c"));
                tutorialImages.Add(game.Content.Load<Texture2D>("tutorial/tutorial7"));
                #endregion
            }
            else
            {
                #region Load Tutorial Images 256MB Device
                tutorialImages.Add(game.Content.Load<Texture2D>("tutorial/tutorial0"));
                tutorialImages.Add(game.Content.Load<Texture2D>("tutorial/tutorial1a"));
                tutorialImages.Add(game.Content.Load<Texture2D>("tutorial/tutorial1b"));
                tutorialImages.Add(game.Content.Load<Texture2D>("tutorial/tutorial1c"));
                tutorialImages.Add(game.Content.Load<Texture2D>("tutorial/tutorial1d"));
                tutorialImages.Add(game.Content.Load<Texture2D>("tutorial/tutorial2a"));
                tutorialImages.Add(game.Content.Load<Texture2D>("tutorial/tutorial2b"));
                tutorialImages.Add(game.Content.Load<Texture2D>("tutorial/tutorial3a"));
                tutorialImages.Add(game.Content.Load<Texture2D>("tutorial/tutorial3b"));
                tutorialImages.Add(game.Content.Load<Texture2D>("tutorial/tutorial3c"));
                tutorialImages.Add(game.Content.Load<Texture2D>("tutorial/tutorial4a"));
                tutorialImages.Add(game.Content.Load<Texture2D>("tutorial/tutorial5a"));
                tutorialImages.Add(game.Content.Load<Texture2D>("tutorial/tutorial5b"));
                tutorialImages.Add(game.Content.Load<Texture2D>("tutorial/tutorial5c"));
                tutorialImages.Add(game.Content.Load<Texture2D>("tutorial/tutorial6a"));
                tutorialImages.Add(game.Content.Load<Texture2D>("tutorial/tutorial6b"));
                tutorialImages.Add(game.Content.Load<Texture2D>("tutorial/tutorial6c"));
                tutorialImages.Add(game.Content.Load<Texture2D>("tutorial/tutorial7"));
                #endregion
            }
            screen = new Rectangle(0, 0, Game1.ScreenWidth, Game1.ScreenHeight);
            next = new Button(game, "->", new Vector2(Game1.ScreenWidth - game.OptionsButtonEmpty.Width,
                Game1.ScreenHeight - game.OptionsButtonEmpty.Height), game.OptionsButtonEmpty, game.OptionsButtonFilled, buttonFont);

            prev = new Button(game, "<-", new Vector2(0,
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
                    prev.Held = next.Held = prev.Held = false;
                    Point p = new Point((int)touches[0].Position.X, (int)touches[0].Position.Y);
                    if (next.ButtonFrame.Contains(p))
                        next.Held = true;
                    if (prev.ButtonFrame.Contains(p))
                        prev.Held = true;
                    if (prev.ButtonFrame.Contains(p))
                        prev.Held = true;
                }
                if (t.State == TouchLocationState.Released)
                {
                    initialPress = true;
                    screenHeld = false;
                    if (!screenLock)
                    {
                        Point p = new Point((int)touches[0].Position.X, (int)touches[0].Position.Y);
                        if (next.ButtonFrame.Contains(p) && currentTutorial == tutorialImages.Count - 1)
                            changeScreen = true;
                        if (next.ButtonFrame.Contains(p) && hasNext)
                            ChangeImage(currentTutorial + 1);
                        if (prev.ButtonFrame.Contains(p) && hasPrev)
                            ChangeImage(currentTutorial - 1);
                    }
                    prev.Held = next.Held = prev.Held = false;
                }
            }
            if (currentTutorial < tutorialImages.Count - 1)
            {
                next.Text = "->";
                hasNext = true;
            }
            else if (currentTutorial == tutorialImages.Count - 1)
                next.Text = "go";

            if (currentTutorial > 0)
                hasPrev = true;
            else
                hasPrev = false;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (currentTutorial < tutorialImages.Count)
                spriteBatch.Draw(tutorialImages[currentTutorial], screen, Color.White);
            next.Draw(gameTime, spriteBatch);
            if (hasPrev)
                prev.Draw(gameTime, spriteBatch);
            base.Draw(gameTime, spriteBatch);
        }

        public override void Reset(GameTime gameTime)
        {
            currentTutorial = 0;
            base.Reset(gameTime);
        }

        private void ChangeImage(int image)
        {
            currentTutorial = image;
        }
    }
}
