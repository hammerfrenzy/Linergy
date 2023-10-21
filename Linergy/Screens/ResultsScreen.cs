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
    class ResultsScreen : Screen
    {
        Button cont;
        SpriteFont resultsFont; //font used to draw results info
        Texture2D bronze, silver, gold;          //bronze, silver, gold medal textures
        Texture2D background;  //background texture
        Texture2D victory;
        Rectangle bronzeLoc, silverLoc, goldLoc; //bronze, silver, gold medal draw locations
        int medalCount; //# of medals achieved for this level
        int timer;  //Used in displaying information
        bool initialPress = true;
        bool screenHeld = false;

        public ResultsScreen(string n, Game1 g)
        {
            name = n;
            game = g;
            timer = 0;
            musicName = "postlevel";
            nextScreen = "worldselect";

            resultsFont = game.Content.Load<SpriteFont>("fonts/menuFont");
            bronze = game.Content.Load<Texture2D>("sprites/bronze");
            silver = game.Content.Load<Texture2D>("sprites/silver");
            gold = game.Content.Load<Texture2D>("sprites/gold");
            background = game.Content.Load<Texture2D>("backgrounds/placeholder3");
            victory = game.Content.Load<Texture2D>("backgrounds/victory");

            bronzeLoc = new Rectangle((int)(Game1.ScreenWidth * .25f - bronze.Width / 2),
                (int)(Game1.ScreenHeight * .65f - bronze.Height / 2), bronze.Width, bronze.Height);
            silverLoc = new Rectangle((int)(Game1.ScreenWidth * .5f - silver.Width / 2),
                (int)(Game1.ScreenHeight * .65f - silver.Height / 2), silver.Width, silver.Height);
            goldLoc = new Rectangle((int)(Game1.ScreenWidth * .75f - gold.Width / 2),
                (int)(Game1.ScreenHeight * .65f - gold.Height / 2), gold.Width, gold.Height);

            bgm = game.Content.Load<Song>("audio/postlevel");

            cont = new Button(game, "ok", new Vector2(Game1.ScreenWidth / 2 - g.OptionsButtonEmpty.Width / 2,
                                                Game1.ScreenHeight - g.OptionsButtonEmpty.Height), g.OptionsButtonEmpty, g.OptionsButtonFilled, resultsFont);
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
                    cont.Held = false;
                    Point p = new Point((int)touches[0].Position.X, (int)touches[0].Position.Y);
                    if (cont.ButtonFrame.Contains(p))
                        cont.Held = true;
                }
                if (t.State == TouchLocationState.Released)
                {
                    initialPress = true;
                    screenHeld = false;

                    if (!screenLock)
                    {
                        Point p = new Point((int)touches[0].Position.X, (int)touches[0].Position.Y);
                        if (cont.ButtonFrame.Contains(p))
                            changeScreen = true;
                    }
                    cont.Held = false;
                }
            }
            cont.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //draw background
            spriteBatch.Draw(background, new Rectangle(0, 0, Game1.ScreenWidth, Game1.ScreenHeight), Color.White * .4f);
            if (timer == 0)
                timer = (int)gameTime.TotalGameTime.TotalMilliseconds;

            #region Display Stats
            if (timer <= gameTime.TotalGameTime.TotalMilliseconds)
            {
                string totalEnergy = "Energy Aquired: " + Math.Max(0, (int)game.player.TotalScore) + " units";
                Vector2 origin = new Vector2(resultsFont.MeasureString(totalEnergy).X / 2,
                                                 resultsFont.MeasureString(totalEnergy).Y / 2);
                game.SpriteBatch.DrawString(resultsFont, totalEnergy,
                    new Vector2(Game1.ScreenWidth * .5f, Game1.ScreenHeight * .2f),
                    Color.Gold, 0, origin, 1, SpriteEffects.None, 0);
            }
            if (timer + 2000 <= gameTime.TotalGameTime.TotalMilliseconds)
            {
                string requiredEnergy = "Medals Earned:";
                Vector2 origin = new Vector2(resultsFont.MeasureString(requiredEnergy).X / 2,
                                                 resultsFont.MeasureString(requiredEnergy).Y / 2);
                game.SpriteBatch.DrawString(resultsFont, requiredEnergy,
                    new Vector2(Game1.ScreenWidth * .5f, Game1.ScreenHeight * .4f),
                    Color.Gold, 0, origin, 1, SpriteEffects.None, 0);
                //Draw medals
                if (medalCount > 0)
                    spriteBatch.Draw(bronze, bronzeLoc, Color.White);
                if (medalCount > 1)
                    spriteBatch.Draw(silver, silverLoc, Color.White);
                if (medalCount > 2)
                    spriteBatch.Draw(gold, goldLoc, Color.White);
                cont.Draw(gameTime, spriteBatch);
            }
            if (timer + 3000 <= gameTime.TotalGameTime.TotalMilliseconds && game.player.CurrentChapter == 24)
            {
                game.SpriteBatch.Draw(victory, new Rectangle(0, 0, Game1.ScreenWidth, Game1.ScreenHeight), Color.White);
                cont.Draw(gameTime, spriteBatch);
            }
            #endregion       
        }

        public override void OnScreenActivate()
        {
            
            //Increase Player Progress
            if (game.player.PlayerLevel == Game1.LevelCap)
                if (game.player.UnlockedLevels == game.player.CurrentChapter)
                        game.player.UnlockedLevels++; //unlock the next level.

            //Add Medals as appropriate
            int medalLevel = 0;
            if (game.player.TotalScore > 3500)
                medalLevel = 1;
            if (game.player.TotalScore > 5000)
                medalLevel = 2;
            if (game.player.TotalScore > 6500)
                medalLevel = 3;
            if (medalLevel > game.player.GetMedals())
            {
                game.player.GiveMedals(medalLevel);
                medalCount = medalLevel;
            }
            //Save Game Progress
            game.player.Save();
        }

        public override void Reset(GameTime gameTime)
        {
            timer = 0;
            medalCount = game.player.GetMedals();
            base.Reset(gameTime);
        }
    }
}
