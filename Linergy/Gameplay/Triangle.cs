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
    class Triangle : Energon
    {
        private float rotation;
        private float rotationSpeed;
        private float minRotationSpeed;
        private float maxRotationSpeed;
        private Vector2 origin;
        private TriCollectedParticleSystem particles;

        public Triangle(Game1 game)
        {
            sprite = game.TriangleSprite;
            this.game = game;
            particles = new TriCollectedParticleSystem(game, 1);
            game.Components.Add(particles);
            rotation = 0f;
            minRotationSpeed = -MathHelper.PiOver4 / 2; //Pi over 8
            maxRotationSpeed = MathHelper.PiOver4 / 2;  //Pi over 8
            rotationSpeed = Game1.RandomBetween(minRotationSpeed, maxRotationSpeed);
            origin.X = sprite.Width / 2;
            origin.Y = sprite.Height / 2;
            Initialize();
            powerLevel = 2;
            energyValue = 7;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            rotation += rotationSpeed;
            base.Update(gameTime);
        }

        public override void Draw()
        {
            if (!Collected)
            {
                if (!reflected)
                    game.SpriteBatch.Draw(sprite, position, null, Color.White, rotation, origin, 1f, SpriteEffects.None, 0);
                else
                    game.SpriteBatch.Draw(sprite, position, null, Color.DarkGray, rotation, origin, 1f, SpriteEffects.None, 0);
            }
        }

        public override void Emit()
        {
            particles.AddParticles(position);
        }

        public override void RemoveParticleSystem()
        {
            game.Components.Remove(particles);
        }

        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
    }
}
