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
    class Dot : Energon
    {
        private DotCollectedParticleSystem particles;

        public Dot(Game1 game)
        {
            sprite = game.DotSprite;
            this.game = game;
            particles = new DotCollectedParticleSystem(game, 1);
            game.Components.Add(particles);
            powerLevel = 1;
            energyValue = 5;
            Initialize();       
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Emit()
        {
            particles.AddParticles(position);
        }

        public override void RemoveParticleSystem()
        {
            game.Components.Remove(particles);
        }
    }
}
