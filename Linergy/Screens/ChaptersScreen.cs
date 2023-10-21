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
    class ChaptersScreen : Screen
    {
        List<string> chapterText;

        Texture2D textBox;
        Texture2D characterBackdrop, gold;
        Rectangle goldRect;

        SpriteFont storyFont, consoleFont;
        Button next, prev, back;

        bool hasNext, nextLocked, initialPress = true;
        bool hasPrev, screenHeld = false;

        float lineHeight;

        int currentChapter;  //each int corresponds to a chapter, 0 is prologue

        public ChaptersScreen(string name, Game1 game)
        {
            this.name = name;
            this.nextScreen = "mainmenu";
            this.musicName = "chaptersmusic";

            storyFont = game.Content.Load<SpriteFont>("fonts/StoryFont");
            buttonFont = game.Content.Load<SpriteFont>("fonts/MenuFont");
            consoleFont = game.Content.Load<SpriteFont>("fonts/Console");
            textBox = game.Content.Load<Texture2D>("hud/hudBar");
            gold = game.Content.Load<Texture2D>("sprites/gold");
            characterBackdrop = game.Remma;   

            chapterText = new List<string>();
            lineHeight = storyFont.LineSpacing;

            next = new Button(game, "->", new Vector2(Game1.ScreenWidth - game.OptionsButtonEmpty.Width,
                Game1.ScreenHeight - game.OptionsButtonEmpty.Height), game.OptionsButtonEmpty, game.OptionsButtonFilled, buttonFont);

            prev = new Button(game, "<-", new Vector2(0,
                Game1.ScreenHeight - game.OptionsButtonEmpty.Height), game.OptionsButtonEmpty, game.OptionsButtonFilled, buttonFont);

            back = new Button(game, "back", new Vector2(Game1.ScreenWidth / 2 - game.OptionsButtonEmpty.Width / 2,
                                 Game1.ScreenHeight - game.OptionsButtonEmpty.Height), game.OptionsButtonEmpty, game.OptionsButtonFilled, buttonFont);
            currentChapter = game.player.CurrentChapter;
            this.game = game;
            goldRect = new Rectangle(next.ButtonFrame.X, next.ButtonFrame.Y + (gold.Height / 3), gold.Width, gold.Height);
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
                            changeScreen = true;
                        if (next.ButtonFrame.Contains(p) && hasNext)
                        {
                            if (!nextLocked)
                            {
                                if (currentChapter == 24)
                                {
                                    if (game.player.TotalMedalCount() >= 45)
                                        ChangeText(currentChapter + 1);
                                }
                                else if (currentChapter == 25)
                                {
                                    if (game.player.TotalMedalCount() >= 60)
                                        ChangeText(currentChapter + 1);
                                }
                                else
                                    ChangeText(currentChapter + 1);
                            }
                        }
                        if (prev.ButtonFrame.Contains(p) && hasPrev)
                            ChangeText(currentChapter - 1);
                    }
                    back.Held = next.Held = prev.Held = false;
                }
            }

            //Check if we're trying to access FINAL END
            if (currentChapter == 24 && game.player.TotalMedalCount() < 45)
                next.Text = "x 15";
            else if (currentChapter == 25 && game.player.TotalMedalCount() < 60)
                next.Text = "x 20";
            else
                next.Text = "->";

            prev.Update(gameTime);
            next.Update(gameTime);
            back.Update(gameTime);

            if (currentChapter < 26)
                hasNext = true;
            else
                hasNext = false;

            if (currentChapter > 0)
                hasPrev = true;
            else
                hasPrev = false;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //Draw Background
            spriteBatch.Draw(characterBackdrop, new Rectangle(0, 0, Game1.ScreenWidth, Game1.ScreenHeight), Color.White);

            //Draw TRANSPARTENT BLACK VISUAL NOVEL STYLE BOX
            spriteBatch.Draw(textBox, Vector2.Zero, new Rectangle(0, 0, Game1.ScreenWidth, Game1.ScreenHeight), Color.White * .4f);

            //Draw the Chapter Text to the screen
            int counter = 0;
            foreach (string s in chapterText)
            {
                if (currentChapter < 25)
                    spriteBatch.DrawString(storyFont, s, new Vector2(0, lineHeight * counter + .5f * storyFont.LineSpacing), Color.White);
                else
                    spriteBatch.DrawString(consoleFont, s, new Vector2(0, lineHeight * counter + .5f * storyFont.LineSpacing), Color.White);
                counter++;
            }

            back.Draw(gameTime, spriteBatch);    

            if (hasNext)
                next.Draw(gameTime, spriteBatch);
            if (hasPrev)
                prev.Draw(gameTime, spriteBatch);
            if (nextLocked && hasNext)
                spriteBatch.Draw(game.Lock, new Rectangle(next.ButtonFrame.X + next.ButtonFrame.Width / 2 - game.Lock.Width / 2,
                    next.ButtonFrame.Y + next.ButtonFrame.Height / 2 - game.Lock.Height / 2, game.Lock.Width, game.Lock.Height), Color.White);
            if (currentChapter == 24 && game.player.TotalMedalCount() < 45)
                spriteBatch.Draw(gold, goldRect, Color.White);
            if (currentChapter == 25 && game.player.TotalMedalCount() < 60)
                spriteBatch.Draw(gold, goldRect, Color.White);
        }

        public override void Reset(GameTime gameTime)
        {
            if (game.player.UnlockedLevels == 24)
                game.player.UnlockedLevels = 26;
            back.Held = next.Held = prev.Held = false;
            currentChapter = game.player.CurrentChapter;
            if (game.player.UnlockedLevels < currentChapter + 1)
                nextLocked = true;
            else
                nextLocked = false;

            if (currentChapter < 26)
                hasNext = true;
            else
                hasNext= false;

            if (currentChapter > 0)
                hasPrev = true;
            else
                hasPrev = false;

            UpdateText();
            base.Reset(gameTime);
        }

        private void ChangeText(int chapter)
        {
            currentChapter = chapter;
            game.player.CurrentChapter = currentChapter;
            if (game.player.UnlockedLevels < currentChapter + 1)
                nextLocked = true;
            else
                nextLocked = false;
            UpdateText();
        }

        /// <summary>
        /// Refreshes the Text displayed on the screen by the ChaptersScreen
        /// </summary>
        private void UpdateText()
        {
            chapterText.Clear();
            XmlReader reader = XmlReader.Create("content/chapters/Chapter" + currentChapter.ToString() + ".xml");
            while (reader.Read())
            {
                XmlNodeType nType = reader.NodeType;
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name.ToString() == "Line")
                        {
                            reader.Read();
                            chapterText.Add(reader.Value);
                        }
                        if (reader.Name.ToString() == "Image")
                        {
                            reader.Read();
                            if (reader.Value == "remma")
                                characterBackdrop = game.Remma;
                            if (reader.Value == "isaak")
                                characterBackdrop = game.Isaak;
                            if (reader.Value == "treavor")
                                characterBackdrop = game.Treavor;
                            if (reader.Value == "gunnardr")
                                characterBackdrop = game.GunnardR;
                            if (reader.Value == "gunnardi")
                                characterBackdrop = game.GunnardI;
                            if (reader.Value == "gunnardt")
                                characterBackdrop = game.GunnardT;
                            if (reader.Value == "black")
                                characterBackdrop = game.Black;
                        }
                        if (reader.Name.ToString() == "Music")
                        {
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
