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

namespace Linergy
{
    class Supernova : StaticGameObject
    {
        Vector2 center;
        Texture2D novaTexture;
        Rectangle boundingBox;
        float mass, criticalMass; //how much energy the Supernova has absorbed, and amount to go critical
        float rotation, scale;    //how to draw the Supernova
        bool imploding;           //changes how the scale updates
        bool hasFired;            //changed to true after the signal for the shockwave has been sent

        public Supernova(Game1 game, Vector2 position)
        {
            this.position = position;
            lifespan = 7000;
            active = true;
            novaTexture = game.Content.Load<Texture2D>("sprites/nova");
            center = new Vector2(position.X + novaTexture.Width / 2, position.Y + novaTexture.Height / 2);
            boundingBox = new Rectangle((int)position.X, (int)position.Y, (int)novaTexture.Width, (int)novaTexture.Height);
            mass = 0f;
            criticalMass = 100f;
            rotation = 0;
            scale = 1f;
            imploding = false;
            hasFired = false;
        }

        public override void Update(GameTime gameTime)
        {
            rotation -= .01f;

            if (imploding)
            {
                scale -= .2f;
                if (scale < 0)
                {
                    scale = 0;
                    if (!hasFired)
                        activateEffect = true;
                    hasFired = true;
                }
            }
            else
                scale = .5f + (mass / criticalMass) / 2;

            boundingBox.X = (int)position.X - (int)(novaTexture.Width / 2 * scale);
            boundingBox.Y = (int)position.Y - (int)(novaTexture.Height / 2 * scale);
            boundingBox.Width = (int)((float)novaTexture.Width * scale);
            boundingBox.Height = (int)((float)novaTexture.Height * scale);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(novaTexture, boundingBox, Color.White);
            spriteBatch.Draw(novaTexture, position, null, Color.White, rotation, new Vector2(novaTexture.Width / 2,
                novaTexture.Height / 2), scale, SpriteEffects.None, 0);
            spriteBatch.Draw(novaTexture, position, null, Color.White, -rotation, new Vector2(novaTexture.Width / 2,
                novaTexture.Height / 2), scale, SpriteEffects.None, 0);
        }

        public override void AssertInfluence(Energon e)
        {
            //Absorb the Energon if they collide
            if (e.BoundingBox.Intersects(boundingBox) && !imploding)
            {
                mass += e.EnergyValue; //Grow
                lifespan += 3000;      //Live longer
                e.Deactivate();        //Remove Energon

                if (mass >= criticalMass)
                    ASPLODE();
            }
        }

        private void ASPLODE()
        {
            lifespan = 1000;
            imploding = true;
        }
    }
}
