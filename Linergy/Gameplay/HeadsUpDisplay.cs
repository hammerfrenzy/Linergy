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
    class HeadsUpDisplay
    {
        #region Declarations
        Game1 game;                    //The Game using this HUD
        Level level;                   //The level using this HUD

        Texture2D energyBar;           //Energy bar texture
        Texture2D shieldBar;           //Shield bar texture
        Texture2D energySymbol;        //Lightning bolt energy symbol
        Texture2D background;          //Hud background texture
        Texture2D separator;           //Hud separator texture

        SpriteFont hudFont;            //Font used to display HuD text
        SpriteFont overchargeFont;     //Font used to display Overcharge notification

        Rectangle energyBarArea;       //Area to draw the energy bar in
        Rectangle shieldBarArea;       //Area to draw the shield bar in
        Rectangle backgroundArea;      //Area to draw the hud background in

        string multiplierText;         //The multiplier text in the hud bar
        string onScreenMultiplierText; //The text to display in the middle of the screen
        string overchargeText;         //The text that displays to notify overcharge
        bool explodingText;            //should exploding text be drawn
        bool failed;                   //is level failed
        bool passed;                   //is the level completed successfully?
        bool overcharge;               //when to display overcharge status
        double timer;                  //timer used for various text display times
        float eRotation;               //Rotation for the energon display
        int overchargeTimer;           //timer used for overcharge display

#endregion
        /// <summary>
        /// Construct a new heads up display for use in a level
        /// </summary>
        /// <param name="game">Game object/param>
        /// <param name="level">Level the HUD appears in</param>
        public HeadsUpDisplay(Game1 game, Level level)
        {
            this.game = game;
            this.level = level;

            energySymbol = game.Content.Load<Texture2D>("hud/energySymbol");
            energyBar = game.Content.Load<Texture2D>("hud/energyBar");
            energyBarArea = new Rectangle(0, 0, energyBar.Width, energyBar.Height);

            shieldBar = game.Content.Load<Texture2D>("hud/shieldBar");
            shieldBarArea = new Rectangle(0, 0, shieldBar.Width, shieldBar.Height);

            background = game.Content.Load<Texture2D>("hud/hudBar");
            backgroundArea = new Rectangle(0, 0, Game1.ScreenWidth, game.HUDHeight);

            separator = game.Content.Load<Texture2D>("hud/hudLine");

            hudFont = game.Content.Load<SpriteFont>("fonts/StoryFont");
            overchargeFont = game.Content.Load<SpriteFont>("fonts/OverchargeFont");

            multiplierText = "1x Combo!";
            overchargeText = ">>OVERCHARGE<<";
            explodingText = false;
            failed = false;
            passed = false;
            overcharge = false;
            timer = 0;
        }

        /// <summary>
        /// Update the HUD elements
        /// </summary>
        /// <param name="touches"></param>
        public void Update()
        {
            eRotation += .02f;
            if (eRotation > MathHelper.TwoPi)
                eRotation = 0;
            //Update the energy bar
            energyBarArea.Width = (int)((game.player.CurrentEnergyAmount / level.EnergyRequirement) * energyBar.Width);
            if (energyBarArea.Width > energyBar.Width)
                energyBarArea.Width = energyBar.Width;

            //Update the shield bar
            shieldBarArea.Width = (int)((game.player.CurrentShieldAmount / shieldBar.Width) * shieldBar.Width);
            if (shieldBarArea.Width > shieldBar.Width)
                shieldBarArea.Width = shieldBar.Width;
        }

        /// <summary>
        /// Draw the HUD elements
        /// </summary>
        /// <param name="gameTime"></param>
        public void Draw(GameTime gameTime)
        {
            if (failed)
            {
                //Draw Level Failure String
                string failedText = "Level Failed!";
                Vector2 origin = new Vector2(overchargeFont.MeasureString(failedText).X / 2,
                                                 overchargeFont.MeasureString(failedText).Y / 2);
                game.SpriteBatch.DrawString(overchargeFont, failedText, 
                        new Vector2(game.GraphicsDevice.Viewport.Width / 2, game.GraphicsDevice.Viewport.Height / 2),
                        Color.IndianRed, 0, origin, 1, SpriteEffects.None, 0);
            }
            if (passed)
            {
                //Draw Level Success String
                string failedText = "Level Complete!";
                Vector2 origin = new Vector2(overchargeFont.MeasureString(failedText).X / 2,
                                                 overchargeFont.MeasureString(failedText).Y / 2);
                game.SpriteBatch.DrawString(overchargeFont, failedText,
                        new Vector2(game.GraphicsDevice.Viewport.Width / 2, game.GraphicsDevice.Viewport.Height / 2),
                        Color.DeepSkyBlue, 0, origin, 1, SpriteEffects.None, 0);
            }
            //Draw the HudBar           
            game.SpriteBatch.Draw(background, backgroundArea, Color.White);
            game.SpriteBatch.Draw(energySymbol, new Rectangle(0, 0, energySymbol.Width, energySymbol.Height), Color.White);
            game.SpriteBatch.Draw(separator, new Rectangle(0, game.HUDHeight - 1, Game1.ScreenWidth, 1), Color.White);
            game.SpriteBatch.Draw(energyBar, new Rectangle(energySymbol.Width, 0, energyBarArea.Width, energyBarArea.Height), energyBarArea, Color.White);
            game.SpriteBatch.Draw(shieldBar, new Rectangle(energyBarArea.Width + energySymbol.Width, 0,
                shieldBarArea.Width, shieldBarArea.Height), shieldBarArea, Color.White);
            
            if (explodingText && !failed)
            {
                if (gameTime.TotalGameTime.TotalMilliseconds - 800 > timer) //end of text effect
                    explodingText = false;
                else
                {
                    Vector2 origin = new Vector2(hudFont.MeasureString(onScreenMultiplierText).X / 2, 
                                                 hudFont.MeasureString(onScreenMultiplierText).Y / 2);
                    float scale = 1 + (float)((gameTime.TotalGameTime.TotalMilliseconds - 800) / timer);
                    game.SpriteBatch.DrawString(hudFont, onScreenMultiplierText, 
                        new Vector2(game.GraphicsDevice.Viewport.Width / 2, game.GraphicsDevice.Viewport.Height / 2), 
                        Color.DeepSkyBlue, 0, origin, scale, SpriteEffects.None, 0);
                }
            }

            if (game.player.PlayerLevel == Game1.LevelCap && !overcharge)
            {
                game.SpriteBatch.DrawString(overchargeFont, overchargeText, new Vector2(Game1.ScreenWidth / 2 - overchargeFont.MeasureString(overchargeText).X / 2,
                    Game1.ScreenHeight / 2 - overchargeFont.MeasureString(overchargeText).Y / 2), Color.White);
                overchargeTimer += gameTime.ElapsedGameTime.Milliseconds;
                if (overchargeTimer > 1500)
                    overcharge = true;
            }

            //static combo notifier
            game.SpriteBatch.DrawString(hudFont, multiplierText, new Vector2(Game1.ScreenWidth * .53f, 20 - hudFont.LineSpacing / 2), Color.DeepSkyBlue);
            //Player Level Notifier
            if (game.player.PlayerLevel < 5)
                game.SpriteBatch.DrawString(hudFont, "Line Level:", new Vector2(Game1.ScreenWidth * .70f, 20 - hudFont.LineSpacing / 2), Color.DeepSkyBlue);
            if (game.player.PlayerLevel == 1)
                game.SpriteBatch.Draw(game.DotSprite, new Vector2(Game1.ScreenWidth * .93f, game.HUDHeight / 2 - game.DotSprite.Height / 2), Color.White);
            else if (game.player.PlayerLevel == 2)
                game.SpriteBatch.Draw(game.TriangleSprite, new Vector2(Game1.ScreenWidth * .93f,
                game.HUDHeight / 2 + game.TriangleSprite.Height / 4), null, Color.White, eRotation,
                new Vector2(game.TriangleSprite.Width / 2, game.TriangleSprite.Height / 2), 1f, SpriteEffects.None, 0);
            else if (game.player.PlayerLevel == 3)
                game.SpriteBatch.Draw(game.QuadSprite, new Vector2(Game1.ScreenWidth * .93f,
                game.HUDHeight / 2 + game.QuadSprite.Height / 4), null, Color.White, eRotation,
                new Vector2(game.QuadSprite.Width / 2, game.QuadSprite.Height / 2), 1f, SpriteEffects.None, 0);
            else if (game.player.PlayerLevel == 4)
                game.SpriteBatch.Draw(game.PentagonSprite, new Vector2(Game1.ScreenWidth * .93f,
                game.HUDHeight / 2 + game.PentagonSprite.Height / 4), null, Color.White, eRotation,
                new Vector2(game.PentagonSprite.Width / 2, game.PentagonSprite.Height / 2), 1f, SpriteEffects.None, 0);
            else
                game.SpriteBatch.DrawString(hudFont, overchargeText, new Vector2(Game1.ScreenWidth * .75f, 20 - hudFont.LineSpacing / 2), Color.DeepSkyBlue);
        }

        /// <summary>
        /// Resets gameplay-related values
        /// </summary>
        public void Reset()
        {
            multiplierText = "1x Combo!";
            explodingText = false;
            failed = false;
            passed = false;
            overcharge = false;
            timer = 0;
            overchargeTimer = 0;
        }

        /// <summary>
        /// Sets the value of the multipier string to whatever multiplier is
        /// </summary>
        /// <param name="multiplier"></param>
        public void ChangeMultiplier(int multiplier, GameTime gameTime)
        {
            multiplierText = multiplier + "x Combo!";
            onScreenMultiplierText = multiplier + "x";
            explodingText = true;
            timer = gameTime.TotalGameTime.TotalMilliseconds;
        }

        public void DrawString(SpriteBatch spriteBatch, string message, Vector2 position, Color color, float rotation, Vector2 origin, float scale)
        {
            spriteBatch.DrawString(hudFont, message, position, color, rotation, origin, scale, SpriteEffects.None, 0);
        }

        public SpriteFont ComboFont
        {
            get { return hudFont; }
        }

        public SpriteFont MessageFont
        {
            get { return overchargeFont; }
        }

        public bool LevelFailed
        {
            get { return failed; }
            set { failed = value; }
        }

        public bool LevelPassed
        {
            get { return passed; }
            set { passed = value; }
        }
    }
}
