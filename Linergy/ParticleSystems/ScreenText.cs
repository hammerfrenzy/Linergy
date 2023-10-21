using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Linergy
{
    class ScreenText
    {
        public Vector2 Position;

        private Color color;
        public Color TextColor
        {
            get { return color; }
            set { color = value; }
        }

        private string message;
        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        // how long this particle will "live"
        private float lifetime;
        public float Lifetime
        {
            get { return lifetime; }
            set { lifetime = value; }
        }

        // how long it has been since initialize was called
        private float timeSinceStart;
        public float TimeSinceStart
        {
            get { return timeSinceStart; }
            set { timeSinceStart = value; }
        }

        // the scale of this particle
        private float scale;
        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        // is this particle still alive? once TimeSinceStart becomes greater than
        // Lifetime, the particle should no longer be drawn or updated.
        public bool Active
        {
            get { return TimeSinceStart < Lifetime; }
        }

        // initialize is called by ParticleSystem to set up the particle, and prepares
        // the particle for use.
        public void Initialize(string message, Vector2 position, float lifetime, float scale, Color color)
        {
            // set the values to the requested values
            this.Message = message;
            this.Position = position;
            this.Lifetime = lifetime;
            this.Scale = scale;
            this.TextColor = color;

            // reset TimeSinceStart - we have to do this because particles will be
            // reused.
            this.TimeSinceStart = 0.0f;
        }

        // update is called by the ParticleSystem on every frame. This is where the
        // particle's position and that kind of thing get updated.
        public void Update(float dt)
        {
            TimeSinceStart += dt;
        }
    }
}
