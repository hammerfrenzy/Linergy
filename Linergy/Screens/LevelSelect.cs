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
    class LevelSelect : Screen
    {
        Texture2D frameEmpty, frameFilled, gold, silver, bronze, empty;
        Texture2D level1, level2, level3, level4;
        Button back, one, two, three, four;

        Rectangle zoomBox;
        Rectangle screen;

        bool initialPress = true;
        bool screenHeld = false;
        bool twoLocked, threeLocked, fourLocked = true;

        int currentWorld;  //each int corresponds to a chapter, 0 is prologue

        public LevelSelect(string name, Game1 game)
        {
            this.name = name;
            this.nextScreen = "level1";
            this.musicName = "worldmusic";
            buttonFont = game.Content.Load<SpriteFont>("fonts/MenuFont");
            bgm = game.Content.Load<Song>("audio/gameplay2");

            //load medal textures
            bronze = game.Content.Load<Texture2D>("sprites/bronze");
            silver = game.Content.Load<Texture2D>("sprites/silver");
            gold = game.Content.Load<Texture2D>("sprites/gold");
            empty = game.Content.Load<Texture2D>("sprites/invisible");
            level1 = level2 = level3 = level4 = empty;

            frameEmpty = game.OptionsButtonEmpty;
            frameFilled = game.OptionsButtonFilled;

            zoomBox = new Rectangle(Game1.PanX, Game1.PanY, game.WorldBackdrops[0].Width, game.WorldBackdrops[0].Height);
            screen = new Rectangle(0, 0, Game1.ScreenWidth, Game1.ScreenHeight);

            int spacer = Game1.ScreenWidth / 16;

            one = new Button(game, "1 - 1", new Vector2(Game1.ScreenWidth * .45f - frameEmpty.Width - spacer,
                         Game1.ScreenHeight *.4f - frameEmpty.Height - spacer), frameEmpty, frameFilled, buttonFont);

            two = new Button(game, "1 - 2", new Vector2(Game1.ScreenWidth * .55f + spacer,
                                     Game1.ScreenHeight * .4f - frameEmpty.Height - spacer), frameEmpty, frameFilled, buttonFont);

            three = new Button(game, "1 - 3", new Vector2(Game1.ScreenWidth * .45f - frameEmpty.Width - spacer,
                                     Game1.ScreenHeight * .4f + spacer), frameEmpty, frameFilled, buttonFont);

            four = new Button(game, "1 - 4", new Vector2(Game1.ScreenWidth * .55f + spacer,
                                     Game1.ScreenHeight * .4f + spacer), frameEmpty, frameFilled, buttonFont);

            back = new Button(game, "back", new Vector2(Game1.ScreenWidth / 2 - game.OptionsButtonEmpty.Width / 2,
                                 Game1.ScreenHeight - game.OptionsButtonEmpty.Height), game.OptionsButtonEmpty, game.OptionsButtonFilled, buttonFont);
            currentWorld = game.player.CurrentWorld;
            this.game = game;
        }

        public override void Update(GameTime gameTime)
        {
            #region Update Screen
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
                    one.Held = two.Held = three.Held = four.Held = back.Held = false;
                    Point p = new Point((int)touches[0].Position.X, (int)touches[0].Position.Y);
                    if (one.ButtonFrame.Contains(p))
                        one.Held = true;
                    if (two.ButtonFrame.Contains(p))
                        two.Held = true;
                    if (three.ButtonFrame.Contains(p))
                        three.Held = true;
                    if (four.ButtonFrame.Contains(p))
                        four.Held = true;
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
                        if (one.ButtonFrame.Contains(p))
                        {
                            game.player.CurrentChapter = ((game.player.CurrentWorld + 1) * 4) - 3;
                            if (game.player.CurrentChapter == 1)
                                nextScreen = "tutorial";
                            else
                                nextScreen = "prelude";                           
                            changeScreen = true;
                        }
                        if (two.ButtonFrame.Contains(p) && !twoLocked)
                        {
                            nextScreen = "prelude";
                            game.player.CurrentChapter = ((game.player.CurrentWorld + 1) * 4) - 2;
                            changeScreen = true;
                        }
                        if (three.ButtonFrame.Contains(p) && !threeLocked)
                        {
                            nextScreen = "prelude";
                            game.player.CurrentChapter = ((game.player.CurrentWorld + 1) * 4) - 1;
                            changeScreen = true;
                        }
                        if (four.ButtonFrame.Contains(p) && !fourLocked)
                        {
                            nextScreen = "prelude";
                            game.player.CurrentChapter = (game.player.CurrentWorld + 1) * 4;
                            changeScreen = true;
                        }
                        if (back.ButtonFrame.Contains(p))
                        {
                            nextScreen = "worldselect";
                            changeScreen = true;
                        }
                        one.Held = two.Held = three.Held = four.Held = back.Held = false;
                    }
                }
            }
            #endregion

            one.Update(gameTime);
            two.Update(gameTime);
            three.Update(gameTime);
            four.Update(gameTime);
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

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            TellWorld(game.player.CurrentWorld);
            spriteBatch.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Draw(game.WorldBackdrops[currentWorld], screen, zoomBox, Color.White);

            one.Draw(gameTime, spriteBatch);
            two.Draw(gameTime, spriteBatch);
            three.Draw(gameTime, spriteBatch);
            four.Draw(gameTime, spriteBatch);
            back.Draw(gameTime, spriteBatch);

            //Draw locks
            if (twoLocked)
                spriteBatch.Draw(game.Lock, new Rectangle(two.ButtonFrame.X + two.ButtonFrame.Width / 2 - game.Lock.Width / 2,
                    two.ButtonFrame.Y + two.ButtonFrame.Height / 2 - game.Lock.Height / 2, game.Lock.Width, game.Lock.Height), Color.White); 
            if(threeLocked)
                spriteBatch.Draw(game.Lock, new Rectangle(three.ButtonFrame.X + three.ButtonFrame.Width / 2 - game.Lock.Width / 2,
                    three.ButtonFrame.Y + three.ButtonFrame.Height / 2 - game.Lock.Height / 2, game.Lock.Width, game.Lock.Height), Color.White);
            if (fourLocked)
                spriteBatch.Draw(game.Lock, new Rectangle(four.ButtonFrame.X + four.ButtonFrame.Width / 2 - game.Lock.Width / 2,
                    four.ButtonFrame.Y + four.ButtonFrame.Height / 2 - game.Lock.Height / 2, game.Lock.Width, game.Lock.Height), Color.White);
            //Draw Medals
            spriteBatch.Draw(level1, new Rectangle(one.ButtonFrame.X + one.ButtonFrame.Width / 2 - level1.Width / 2,
                    one.ButtonFrame.Y + one.ButtonFrame.Height / 2 - level1.Height / 2, level1.Width, level1.Height), Color.White);
            spriteBatch.Draw(level2, new Rectangle(two.ButtonFrame.X + two.ButtonFrame.Width / 2 - level2.Width / 2,
                   two.ButtonFrame.Y + two.ButtonFrame.Height / 2 - level2.Height / 2, level2.Width, level2.Height), Color.White);
            spriteBatch.Draw(level3, new Rectangle(three.ButtonFrame.X + three.ButtonFrame.Width / 2 - level3.Width / 2,
                   three.ButtonFrame.Y + three.ButtonFrame.Height / 2 - level3.Height / 2, level3.Width, level3.Height), Color.White);
            spriteBatch.Draw(level4, new Rectangle(four.ButtonFrame.X + four.ButtonFrame.Width / 2 - level4.Width / 2,
                   four.ButtonFrame.Y + four.ButtonFrame.Height / 2 - level4.Height / 2, level4.Width, level4.Height), Color.White);
        }

        public override void Reset(GameTime gameTime)
        {
            //Update the ZoomBox
            zoomBox.X = Game1.PanX;
            zoomBox.Y = Game1.PanY;
            zoomBox.Width = (int)(game.WorldBackdrops[0].Width * Game1.Zoom);
            zoomBox.Height = (int)(game.WorldBackdrops[0].Height * Game1.Zoom);

            #region Set Medals

            List<int> medalList = game.player.WorldMedals();
            if (medalList[0] == 0)
                level1 = empty;
            else if (medalList[0] == 1)
                level1 = bronze;
            else if (medalList[0] == 2)
                level1 = silver;
            else
                level1 = gold;

            if (medalList[1] == 0)
                level2 = empty;
            else if (medalList[1] == 1)
                level2 = bronze;
            else if (medalList[1] == 2)
                level2 = silver;
            else
                level2 = gold;

            if (medalList[2] == 0)
                level3 = empty;
            else if (medalList[2] == 1)
                level3 = bronze;
            else if (medalList[2] == 2)
                level3 = silver;
            else
                level3 = gold;

            if (medalList[3] == 0)
                level4 = empty;
            else if (medalList[3] == 1)
                level4 = bronze;
            else if (medalList[3] == 2)
                level4 = silver;
            else
                level4 = gold;
            #endregion

            one.Held = two.Held = three.Held = four.Held = back.Held = false;

            if (game.player.UnlockedLevels < ((game.player.CurrentWorld+1) * 4) - 2)
                twoLocked = true;
            else
                twoLocked = false;

            if (game.player.UnlockedLevels < ((game.player.CurrentWorld+1) * 4) -1)
                threeLocked = true;
            else
                threeLocked = false;

            if (game.player.UnlockedLevels < ((game.player.CurrentWorld+1) * 4))
                fourLocked = true;
            else
                fourLocked = false;
            base.Reset(gameTime);
        }

        /// <summary>
        /// Makes the button values consistent for the current world
        /// </summary>
        /// <param name="world"></param>
        public override void TellWorld(int world)
        {
            currentWorld = world;
            one.Text = (world + 1).ToString() + " - 1";
            two.Text = (world + 1).ToString() + " - 2";
            three.Text = (world + 1).ToString() + " - 3";
            four.Text = (world + 1).ToString() + " - 4";
            base.TellWorld(world);
        }
    }
}
