using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace Linergy
{
    class Animation
    {
        string sheetName;              //The location of the spritesheet image
        string playType;               //How the Animations plays out (loop, etc)
        Texture2D spriteSheet;         //The sprites used in the Animation
        int spriteWidth, spriteHeight; //The Width and Height of the sprites in the Animation      
        int currentFrame, frameCount;  //The frame the Animation is currently on, # of frames in the Animation
        float rotation;                //The rotation of the currentFrame
        bool shouldRotate;             //Determines if the Animation rotates
        bool forward;                  //The direction of the Animation

        public Animation(Game1 game, string sheetName, int spriteWidth, int spriteHeight, int frameCount, string playType, bool shouldRotate)
        {
            this.sheetName = sheetName;
            this.playType = playType;
            this.spriteWidth = spriteWidth;
            this.spriteHeight = spriteHeight;
            this.frameCount = frameCount;
            this.shouldRotate = shouldRotate;

            spriteSheet = game.Content.Load<Texture2D>(sheetName);
            currentFrame = 0;
            rotation = 0.0f;
            forward = true;
        }

        public void Update()
        {
            if (forward) //Playing in forward
            {
                currentFrame++;
                if (currentFrame > frameCount && playType == "loop")  
                    currentFrame = 0;
                else if (currentFrame > frameCount && playType == "reverse")
                {
                    currentFrame -= 2;
                    forward = false;
                }
            }
            else         //Playing in reverse
            {
                currentFrame--;
                if (currentFrame < 0)
                {
                    currentFrame = 0;
                    forward = true;
                }
            }

            if (shouldRotate)
                rotation += .02f;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 drawLocation)
        {
            spriteBatch.Draw(spriteSheet, drawLocation, new Rectangle(spriteWidth * currentFrame, 0, spriteWidth, spriteHeight),
                Color.White, rotation, new Vector2(spriteWidth / 2, spriteHeight / 2), 1f, SpriteEffects.None, 0f);
            Update();
        }
    }
}
