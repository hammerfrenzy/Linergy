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
    class PreludeScreen : Screen
    {
        List<string> chapterText;

        SpriteFont storyFont;
        Texture2D textBox, characterBackdrop;
        Button start;

        bool initialPress = true;
        bool screenHeld = false;

        float lineHeight;
        float fadeOpacity;

        int currentChapter;  //each int corresponds to a chapter, 0 is prologue

        public PreludeScreen(string name, Game1 game)
        {
            this.name = name;
            this.nextScreen = "gamescreen";
            this.musicName = "chaptersmusic";

            storyFont = game.Content.Load<SpriteFont>("fonts/StoryFont");
            buttonFont = game.Content.Load<SpriteFont>("fonts/MenuFont");
            textBox = game.Content.Load<Texture2D>("hud/hudBar");
            characterBackdrop = game.Remma;

            chapterText = new List<string>();
            lineHeight = storyFont.LineSpacing;

            start = new Button(game, "start", new Vector2(Game1.ScreenWidth / 2 - game.OptionsButtonEmpty.Width / 2,
                        Game1.ScreenHeight - game.OptionsButtonEmpty.Height), game.OptionsButtonEmpty, game.OptionsButtonFilled, buttonFont);
            currentChapter = game.player.CurrentChapter;
            fadeOpacity = 0f;
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
                    start.Held = false;
                    Point p = new Point((int)touches[0].Position.X, (int)touches[0].Position.Y);
                    if (start.ButtonFrame.Contains(p))
                        start.Held = true;
                }
                if (t.State == TouchLocationState.Released)
                {
                    initialPress = true;
                    screenHeld = false;
                    if (!screenLock)
                    {
                        Point p = new Point((int)touches[0].Position.X, (int)touches[0].Position.Y);
                        if (start.ButtonFrame.Contains(p))
                            changeScreen = true;
                    }
                    start.Held = false;
                }
            }

            start.Update(gameTime);

            fadeOpacity += .02f;
            if (fadeOpacity > 1)
                fadeOpacity = 1;

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
                spriteBatch.DrawString(storyFont, s, new Vector2(0, lineHeight * counter + .5f * storyFont.LineSpacing), Color.White);
                counter++;
            }

            start.Draw(gameTime, spriteBatch);
        }

        public override void Reset(GameTime gameTime)
        {
            currentChapter = game.player.CurrentChapter;
            fadeOpacity = 0f;
            start.Held = false;
            nextScreen = "gamescreen";
            UpdateText();
            base.Reset(gameTime);
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
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
