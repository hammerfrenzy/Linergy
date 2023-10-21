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
    class OptionsScreen : Screen
    {
        SpriteFont optionsFont;
        OptionsButton musicToggle, soundToggle;
        Button back;
        Texture2D background;

        bool initialPress = true;
        bool screenHeld = false;

        public OptionsScreen(string name, Game1 game)
        {
            this.name = name;
            this.nextScreen = "mainmenu";
            this.musicName = "menumusic";

            optionsFont = game.Content.Load<SpriteFont>("fonts/SmallTitleFont");
            background = game.Content.Load<Texture2D>("backgrounds/placeholder3");
            bgm = game.Content.Load<Song>("audio/title");

            musicToggle = new OptionsButton(game, "on", "off", new Vector2(Game1.ScreenWidth / 2 - game.OptionsButtonEmpty.Width / 2,
                                            Game1.ScreenHeight / 5), game.OptionsButtonEmpty, game.OptionsButtonFilled, optionsFont);

            soundToggle = new OptionsButton(game, "on", "off", new Vector2(Game1.ScreenWidth / 2 - game.OptionsButtonEmpty.Width / 2,
                                            Game1.ScreenHeight * .5f), game.OptionsButtonEmpty, game.OptionsButtonFilled, optionsFont);

            back = new Button(game, "back", new Vector2(Game1.ScreenWidth / 2 - game.OptionsButtonEmpty.Width / 2,
                                Game1.ScreenHeight - game.OptionsButtonEmpty.Height), game.OptionsButtonEmpty, game.OptionsButtonFilled, optionsFont);
        
            //Change the button text if setting has been changed in a previous game session
            if (!Game1.ShouldPlayMusic)
                musicToggle.Toggle();
            if (!Game1.ShouldPlaySound)
                soundToggle.Toggle();
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
                    back.Held = musicToggle.Held = soundToggle.Held = false;
                    Point p = new Point((int)touches[0].Position.X, (int)touches[0].Position.Y);
                    if (musicToggle.ButtonFrame.Contains(p))
                        musicToggle.Held = true;
                    if (soundToggle.ButtonFrame.Contains(p))
                        soundToggle.Held = true;
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
                        if (musicToggle.ButtonFrame.Contains(p))
                        {
                            musicToggle.Toggle();
                            Game1.ShouldPlayMusic = !Game1.ShouldPlayMusic;
                            if (Game1.ShouldPlayMusic)
                                MediaPlayer.Play(this.Music);
                            else
                                MediaPlayer.Stop();
                        }
                        if (soundToggle.ButtonFrame.Contains(p))
                        {
                            soundToggle.Toggle();
                            Game1.ShouldPlaySound = !Game1.ShouldPlaySound;
                        }
                        if (back.ButtonFrame.Contains(p))
                            changeScreen = true;
                    }
                    back.Held = musicToggle.Held = soundToggle.Held = false;
                }
            }
            musicToggle.Update(gameTime);
            soundToggle.Update(gameTime);
            back.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //draw background
            spriteBatch.Draw(background, new Rectangle(0, 0, Game1.ScreenWidth, Game1.ScreenHeight), Color.White * .4f);
            spriteBatch.DrawString(optionsFont, "music:", new Vector2(Game1.ScreenWidth / 6, Game1.ScreenWidth / 6), Color.White);
            spriteBatch.DrawString(optionsFont, "sound:", new Vector2(Game1.ScreenWidth / 6, Game1.ScreenWidth / 3), Color.White);
            musicToggle.Draw(gameTime, spriteBatch);
            soundToggle.Draw(gameTime, spriteBatch);
            back.Draw(gameTime, spriteBatch);
        }

        public override void Reset(GameTime gameTime)
        {
            back.Held = false;
            base.Reset(gameTime);
        }
    }
}
