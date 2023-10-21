using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
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
    class WorldSelect : Screen
    {
        List<string> worldNames;

        Rectangle zoomBox;
        Rectangle screen;

        Texture2D gold;

        SpriteFont smallTitleFont;
        Button next, prev, back;

        bool hasNext, nextLocked, initialPress = true;
        bool hasPrev, screenHeld, fading = false;

        float fadeOpacity;

        int worldCount;    //The number of world in the game
        int currentWorld;  //each int corresponds to a chapter, 0 is prologue

        public WorldSelect(string name, Game1 game)
        {
            this.name = name;
            this.nextScreen = "level1";
            this.musicName = "worldmusic";

            smallTitleFont = game.Content.Load<SpriteFont>("fonts/SmallTitleFont");
            buttonFont = game.Content.Load<SpriteFont>("fonts/MenuFont");
            gold = game.Content.Load<Texture2D>("sprites/gold");
            bgm = game.Content.Load<Song>("audio/gameplay2");

            worldNames = new List<string>();
            worldNames.Add("Cepheus Nebula");
            worldNames.Add("Tureis");
            worldNames.Add("Amalthea");
            worldNames.Add("Hyperion");
            worldNames.Add("Iapetus");
            worldNames.Add("Aegaeon");

            zoomBox = new Rectangle(Game1.PanX, Game1.PanY, game.WorldBackdrops[0].Width, game.WorldBackdrops[0].Height);
            screen = new Rectangle(0, 0, Game1.ScreenWidth, Game1.ScreenHeight);

            next = new Button(game, "->", new Vector2(Game1.ScreenWidth - game.OptionsButtonEmpty.Width,
                Game1.ScreenHeight - game.OptionsButtonEmpty.Height), game.OptionsButtonEmpty, game.OptionsButtonFilled, buttonFont);

            prev = new Button(game, "<-", new Vector2(0,
                Game1.ScreenHeight - game.OptionsButtonEmpty.Height), game.OptionsButtonEmpty, game.OptionsButtonFilled, buttonFont);

            back = new Button(game, "back", new Vector2(Game1.ScreenWidth / 2 - game.OptionsButtonEmpty.Width / 2,
                                 Game1.ScreenHeight - game.OptionsButtonEmpty.Height), game.OptionsButtonEmpty, game.OptionsButtonFilled, buttonFont);
            currentWorld = game.player.CurrentWorld;
            fadeOpacity = 1;
            worldCount = 5; //0-5 = 6 worlds
            this.game = game;
        }

        public override void Update(GameTime gameTime)
        {
            #region Screen Update
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
                    back.Held = next.Held = prev.Held = false;
                    Point p = new Point((int)touches[0].Position.X, (int)touches[0].Position.Y);
                    if (next.ButtonFrame.Contains(p))
                        next.Held = true;
                    if (prev.ButtonFrame.Contains(p))
                        prev.Held = true;
                    if (back.ButtonFrame.Contains(p))
                        back.Held = true;
                }
                if (t.State == TouchLocationState.Released)
                {
                    initialPress = true;
                    screenHeld = false;
                    if (!screenLock)
                    {
                        Point p = new Point((int)touches[0].Position.X, (int)touches[0].Position.Y);
                        if (back.ButtonFrame.Contains(p))
                        {
                            nextScreen = "mainmenu";
                            changeScreen = true;
                        }
                        else if (next.ButtonFrame.Contains(p) && hasNext)
                        {
                            if(!nextLocked && !Guide.IsTrialMode)
                                ChangeWorld(currentWorld + 1);
                        }
                        else if (prev.ButtonFrame.Contains(p) && hasPrev)
                            ChangeWorld(currentWorld - 1);
                        else
                        {
                            nextScreen = "levelselect";
                            if (Game1.ShouldPlaySound)
                                game.MenuBack.Play();
                            changeScreen = true;
                        }
                    }
                    back.Held = next.Held = prev.Held = false;
                }
            }
            #endregion

            prev.Update(gameTime);
            next.Update(gameTime);
            back.Update(gameTime);

            #region Camera Pan / Zoom
            if (Game1.IsZoomingIn)
            {
                Game1.Zoom -= .001f;
                zoomBox.Width = (int)(game.WorldBackdrops[0].Width * Game1.Zoom);
                zoomBox.Height = (int)(game.WorldBackdrops[0].Height * Game1.Zoom);
                if (Game1.Zoom < .9f && Game1.PanX < Game1.ScreenHeight / 4)
                {
                    Game1.PanX++;
                    if (Game1.PanY < Game1.ScreenHeight / 4)
                        Game1.PanY++;
                    zoomBox.X = Game1.PanX;
                    zoomBox.Y = Game1.PanY;
                }
                else if (Game1.PanX > 0)
                {
                    Game1.PanX--;
                    if (Game1.PanY > 0)
                        Game1.PanY--;
                    zoomBox.X = Game1.PanX;
                    zoomBox.Y = Game1.PanY;
                }
                if (Game1.Zoom < .75f)
                    Game1.IsZoomingIn = false;
            }
            else
            {
                Game1.Zoom += .001f;
                int oldWidth = zoomBox.Width;
                int oldHeight = zoomBox.Height;
                zoomBox.Width = (int)(game.WorldBackdrops[0].Width * Game1.Zoom);
                zoomBox.Height = (int)(game.WorldBackdrops[0].Height * Game1.Zoom);
                if (zoomBox.X > 0)
                    zoomBox.X -= (zoomBox.Width - oldWidth);
                if (zoomBox.Y > 0)
                    zoomBox.Y -= (zoomBox.Height - oldHeight);
                Game1.PanX = zoomBox.X;
                Game1.PanY = zoomBox.Y;

                if (Game1.Zoom >= 1)
                    Game1.IsZoomingIn = true;
            }
            #endregion

            #region Fade Text
            //Fade text in and out
            if (fading)
            {
                fadeOpacity -= .02f;
                if (fadeOpacity < 0)
                    fading = false;
            }
            else
            {
                fadeOpacity += .02f;
                if (fadeOpacity > 1)
                    fading = true;
            }
            #endregion

            #region HasNext/Prev
            if (currentWorld < worldCount)
                hasNext = true;
            else
                hasNext = false;

            if (currentWorld > 0)
                hasPrev = true;
            else
                hasPrev = false;
            #endregion
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Draw(game.WorldBackdrops[currentWorld], screen, zoomBox, Color.White);

            spriteBatch.DrawString(buttonFont, worldNames[currentWorld], Vector2.Zero, Color.White);
            spriteBatch.DrawString(smallTitleFont, "tap to select", new Vector2(0, 50), Color.White * fadeOpacity);

            back.Draw(gameTime, spriteBatch);

            if (hasNext)
                next.Draw(gameTime, spriteBatch);
            if (hasPrev)
                prev.Draw(gameTime, spriteBatch);
            if ((nextLocked && hasNext) || Guide.IsTrialMode)
                spriteBatch.Draw(game.Lock, new Rectangle(next.ButtonFrame.X + next.ButtonFrame.Width / 2 - game.Lock.Width / 2,
                    next.ButtonFrame.Y + next.ButtonFrame.Height / 2 - game.Lock.Height / 2, game.Lock.Width, game.Lock.Height), Color.White);
            if (game.player.IsWorldGold())
                spriteBatch.Draw(gold, new Vector2(20, 100), Color.White);
        }

        public override void Reset(GameTime gameTime)
        {
            //Update the ZoomBox
            zoomBox.X = Game1.PanX;
            zoomBox.Y = Game1.PanY;
            zoomBox.Width = (int)(game.WorldBackdrops[0].Width * Game1.Zoom);
            zoomBox.Height = (int)(game.WorldBackdrops[0].Height * Game1.Zoom);

            back.Held = next.Held = prev.Held = false;
            currentWorld = game.player.CurrentWorld;
            //check to see if the next world is locked
            if (game.player.UnlockedLevels > (currentWorld + 1) * 4)
                nextLocked = false;
            else
                nextLocked = true;

            if (currentWorld < worldCount)
                hasNext = true;
            else
                hasNext = false;

            if (currentWorld > 0)
                hasPrev = true;
            else
                hasPrev = false;
            base.Reset(gameTime);
        }

        private void ChangeWorld(int world)
        {
            currentWorld = world;
            game.CurrentWorld = world;
            game.player.CurrentWorld = currentWorld;

            if (game.player.UnlockedLevels > (currentWorld + 1) * 4)
                nextLocked = false;
            else
                nextLocked = true;
        }

    }
}
