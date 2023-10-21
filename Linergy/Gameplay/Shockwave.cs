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
    class Shockwave : StaticGameObject
    {
        Vector2 center;
        Texture2D shockwave;
        float rotation, scale;    //how to draw the Shockwave

        public Shockwave(Game1 game, Vector2 position)
        {
            this.position = position;
            lifespan = 5000;
            active = true;
            shockwave = game.Content.Load<Texture2D>("sprites/shockwave");
            center = new Vector2(position.X + shockwave.Width / 2, position.Y + shockwave.Height / 2);
            rotation = 0;
            scale = 0.01f;
        }

        public override void Update(GameTime gameTime)
        {
            rotation += .05f;
            scale += .1f;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(shockwave, position, null, Color.White, rotation, new Vector2(shockwave.Width / 2,
                shockwave.Height / 2), scale, SpriteEffects.None, 0);           
        }

        public override void AssertInfluence(Energon e)
        {
        }
    }
}
