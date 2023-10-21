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
    class MagneticAnomaly : StaticGameObject
    {
        Vector2 center;
        Animation animation;

        public MagneticAnomaly(Game1 game, Vector2 position)
        {
            this.position = position;
            lifespan = 10000;
            active = true;
            center = new Vector2(position.X + 50, position.Y + 50);
            animation = new Animation(game, "sprites/AnomalySheet", 100, 100, 8, "reverse", true);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            animation.Draw(spriteBatch, position);
        }

        public override void AssertInfluence(Energon e)
        {
            float distance = Vector2.Distance(e.Center(), center);
            float affectiveness = 2 / distance;
            Vector2 newVelocity = e.Velocity;

            if (e.Center().X < center.X)
                newVelocity.X += affectiveness;
            else
                newVelocity.X -= affectiveness;

            if (e.Center().Y < center.Y)
                newVelocity.Y += affectiveness;
            else
                newVelocity.Y -= affectiveness;

            e.Velocity = newVelocity;
        }
    }
}
