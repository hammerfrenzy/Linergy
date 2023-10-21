using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Linergy
{
    class GATEParticleSystemLv1 : GATEParticleSystem
    {
        public GATEParticleSystemLv1(Game1 game, int howManyEffects)
            : base(game, howManyEffects)
        {
        }

        protected override void InitializeConstants()
        {
            textureFilename = "particles/GATEparticle1";
            base.InitializeConstants();
        }
    }
}
