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
    class Circle : Energon
    {
        private float rotation;
        private float rotationSpeed;
        private Vector2 origin;

        public Circle(Game1 game)
        {
            this.sprite = game.CircleSprite;
            this.game = game;
            rotation = 0f;
            rotationSpeed = MathHelper.PiOver4 / 2;
            origin.X = sprite.Width / 2;
            origin.Y = sprite.Height / 2;
            Initialize();
            powerLevel = 6;
            energyValue = 40;
            bounceAllowance = 4;
        }

        public Circle(Game1 game, Vector2 pos)
        {
            this.sprite = game.CircleSprite;
            this.game = game;
            rotation = 0f;
            rotationSpeed = MathHelper.PiOver4 / 2;
            origin.X = sprite.Width / 2;
            origin.Y = sprite.Height / 2;
            Initialize();
            powerLevel = 6;
            energyValue = 40;
            bounceAllowance = 4;
            position = pos;
        }

        public override void Initialize()
        {
            base.Initialize();
            bounceAllowance = 4;
            //Make Circles go crazy fast
            if (velocity.X > 0)
                velocity.X += 5f;
            else
                velocity.X -= 5f;

            if (velocity.Y > 0)
                velocity.Y += 5f;
            else
                velocity.Y -= 5f;

        }

        public override void Update(GameTime gameTime)
        {
            if (setActiveTime)
                activatedTime = gameTime.TotalGameTime.TotalMilliseconds;
            rotation += rotationSpeed;
            position += velocity;
            boundingRectangle.X = (int)position.X;
            boundingRectangle.Y = (int)position.Y;

            if (position.X <= 0 && bounceAllowance > 0)
            {
                velocity.X = -velocity.X;
                position.X = 0;
                bounceAllowance--;
            }
            if (position.X >= Game1.ScreenWidth && bounceAllowance > 0)
            {
                velocity.X = -velocity.X;
                position.X = Game1.ScreenWidth - sprite.Width;
                bounceAllowance--;
            }
            if (position.Y <= game.HUDHeight && bounceAllowance > 0)
            {
                velocity.Y = -velocity.Y;
                position.Y = game.HUDHeight;
                bounceAllowance--;
            }
            if (position.Y >= Game1.ScreenHeight && bounceAllowance > 0)
            {
                velocity.Y = -velocity.Y;
                position.Y = Game1.ScreenHeight - sprite.Height;
                bounceAllowance--;
            }
            

            if (bounceAllowance <= 0)
                Deactivate();

            if (activatedTime + 3000 < gameTime.TotalGameTime.TotalMilliseconds)
                Deactivate();
        }

        public override void Draw()
        {
            if (!reflected)
                game.SpriteBatch.Draw(sprite, position, null, Color.White, rotation, origin, 1f, SpriteEffects.None, 0);
            else
                game.SpriteBatch.Draw(sprite, position, null, Color.White, rotation, origin, 1f, SpriteEffects.None, 0);
        }

        public override void Deactivate()
        {
            active = false;
            position = new Vector2(99999, 99999);
            velocity = Vector2.Zero;
        }

        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
    }
}
