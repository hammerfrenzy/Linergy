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
    class GATE : GameObject
    {
        #region Declarations
        Game1 game;
        Texture2D gateLevel1;
        Texture2D gateLevel2;
        Texture2D gateLevel3;
        Texture2D gateTexture;

        Texture2D gateParticle1;
        Texture2D gateParticle2;
        Texture2D gateParticle3;

        Texture2D glow;

        GATEParticleSystem currentParticlesA;
        GATEParticleSystem currentParticlesB;

        Vector2 start = Vector2.Zero;
        Vector2 end = Vector2.Zero;
        Vector2 normal = Vector2.Zero;
        double normalizedLength = 0;

        Rectangle energonArea;
        float glowOpacity;
        bool screenHeld, gateSet, drawGate = false;
        bool initialPress, fading = true;

        int level;
        
        #endregion

        public GATE(Game1 game)
        {
            this.game = game;

            gateLevel1 = game.Content.Load<Texture2D>("player/GATE");
            gateLevel2 = game.Content.Load<Texture2D>("player/GATE2");
            gateLevel3 = game.Content.Load<Texture2D>("player/GATE3");
            gateParticle1 = game.Content.Load<Texture2D>("particles/GATEparticle1");
            gateParticle2 = game.Content.Load<Texture2D>("particles/GATEparticle2");
            gateParticle3 = game.Content.Load<Texture2D>("particles/GATEparticle3");

            gateTexture = gateLevel1;

            glow = game.Content.Load<Texture2D>("player/glow");
            glowOpacity = 1f;

            currentParticlesA = new GATEParticleSystem(game, 1);
            currentParticlesB = new GATEParticleSystem(game, 1);

            game.Components.Add(currentParticlesA);
            game.Components.Add(currentParticlesB);

            energonArea = new Rectangle();
            level = 1;
        }


        /// <summary>
        /// Updates the GATE
        /// </summary>
        /// <param name="touches"></param>
        public void Update(GameTime gameTime, TouchCollection touches)
        {
            foreach (TouchLocation t in touches)
            {
                if (t.State == TouchLocationState.Pressed && initialPress)
                {
                    initialPress = false;
                    screenHeld = true;
                    drawGate = false;
                    gateSet = false;
                    start = t.Position;
                }
                if (t.State == TouchLocationState.Moved && screenHeld)
                {
                    end = t.Position;
                    gateSet = false;
                    drawGate = true;
                }
                if (t.State == TouchLocationState.Released)
                {
                    initialPress = true;
                    screenHeld = false;
                    gateSet = true;
                }
            }
            
            if (start.Y < game.HUDHeight)
                start.Y = game.HUDHeight;
            if (end.Y < game.HUDHeight)
                end.Y = game.HUDHeight;

            if (gateSet) //only do the normal calculation if the gate is able to reflect particles
            {
                normal.X = -(end.Y - start.Y);
                normal.Y = (end.X - start.X);
                normalizedLength = Math.Sqrt(normal.X * normal.X + normal.Y * normal.Y);
                normal.X /= (float)normalizedLength;
                normal.Y /= (float)normalizedLength;
                //Remove energy based on the GATE's length
                game.player.CurrentEnergyAmount -= ((Vector2.DistanceSquared(start, end) + 5) / game.TargetElapsedTime.Ticks);
                //If the length is really small (cheatermode), keep taking energy anyway
                if ((int)Vector2.DistanceSquared(start, end) < 1)
                    game.player.CurrentEnergyAmount -= .02f;
            }
            else //slow decay
                game.player.CurrentEnergyAmount -= .02f;

            if (fading)
            {
                glowOpacity -= .05f;
                if (glowOpacity < 0)
                    fading = false;
            }
            else
            {
                glowOpacity += .05f;
                if (glowOpacity > 1)
                    fading = true;
            }
            Vector2 midPoint = Vector2.Zero;
            midPoint.X = start.X + ((end.X - start.X) / 2);
            midPoint.Y = start.Y + ((end.Y - start.Y) / 2);
        }

        /// <summary>
        /// Draws the GATE
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void drawGATE(SpriteBatch spriteBatch)
        {
            float angle = (float)Math.Atan2(start.Y - end.Y, start.X - end.X);
            float distance = Vector2.Distance(start, end);

            //Draw a glow if we're in OVERCHARGEU
            if (gateSet && game.player.PlayerLevel == Game1.LevelCap)
                spriteBatch.Draw(glow, new Rectangle((int)end.X, (int)end.Y, (int)distance, 6), null, Color.White * glowOpacity, angle, Vector2.Zero, SpriteEffects.None, 0);
            //Draw transparent to show the gate isn't set, draw opaque when the finger is lifted and gate is set.
            if (!gateSet)
                spriteBatch.Draw(gateTexture, new Rectangle((int)end.X, (int)end.Y, (int)distance, 3), null, Color.White * .4f, angle, Vector2.Zero, SpriteEffects.None, 0);
            else
                spriteBatch.Draw(gateTexture, new Rectangle((int)end.X, (int)end.Y, (int)distance, 3), null, Color.White, angle, Vector2.Zero, SpriteEffects.None, 0);
            currentParticlesA.Visible = true;
            currentParticlesB.Visible = true;
            currentParticlesA.AddParticles(start);
            currentParticlesB.AddParticles(end);
            //spriteBatch.Draw(glow, lerpderp, Color.White);
            //spriteBatch.Draw(gateTexture, halfB, Color.White);
        }

        /// <summary>
        /// Cancels the current GATE
        /// </summary>
        public void Cancel()
        {
            initialPress = true;
            screenHeld = false;
            gateSet = false;
            drawGate = false;
            start = Vector2.Zero;
            end = Vector2.Zero;
            currentParticlesA.Visible = false;
            currentParticlesB.Visible = false;
        }

        /// <summary>
        /// Changes the properties of the gate based on the type
        /// </summary>
        /// <param name="type"></param>
        public void SetGateType(int type)
        {
            if (type == 1)
            {
                gateTexture = gateLevel1;
                currentParticlesA.Texture = gateParticle1;
                currentParticlesB.Texture = gateParticle1;
            }
            else if (type == 2)
            {
                gateTexture = gateLevel2;
                currentParticlesA.Texture = gateParticle2;
                currentParticlesB.Texture = gateParticle2;
            }
            else if (type == 3)
            {
                gateTexture = gateLevel3;
                currentParticlesA.Texture = gateParticle3;
                currentParticlesB.Texture = gateParticle3;
            }
        }

        /// <summary>
        /// Returns wether or not the GATE should draw itself.
        /// </summary>
        public bool ShouldDraw { get { return drawGate; } }

        /// <summary>
        /// Checks if the Energon e is inside the rectangle made by extending lines from two points
        /// </summary>
        /// <param name="e">Energon to check</param>
        /// <param name="a">Line point A</param>
        /// <param name="b">Line point B</param>
        /// <returns>true if Trivially Rejected, false otherwise</returns>
        private bool TrivialReject(Energon e, Vector2 a, Vector2 b)
        {
            //Trivial reject if a GATE isn't set.
            if (!gateSet)
                return true;
            //Cohen-Sutherland.  Energon texture is the screen we are 'clipping' to, GATE is the line.
            //Trivial Rejects
            //energon is completely left of the gate
            if (e.Position.X + e.Texture.Width < MathHelper.Min(a.X, b.X))
                return true;
            //energon is completely right of the gate
            else if (e.Position.X > MathHelper.Max(a.X, b.X))
                return true;
            //energon is completely above of the gate
            else if (e.Texture.Height + e.Position.Y < MathHelper.Min(a.Y, b.Y))
                return true;
            //energon is completely below of the gate
            else if (e.Position.Y > MathHelper.Max(a.Y, b.Y))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks a small piece of the entire line (a 2x2 box)
        /// </summary>
        /// <param name="e">Energon being checked</param>
        /// <param name="a">Start of Line</param>
        /// <param name="b">End of Line</param>
        /// <returns>true if collision, false otherwise</returns>
        private bool DiscreteCheck(Energon e, Vector2 a, Vector2 b)
        {
            Rectangle lineBox = new Rectangle((int)MathHelper.Min(a.X, b.X) - 1, (int)MathHelper.Min(a.Y, b.Y) - 1, 2, 2);
            return lineBox.Intersects(e.BoundingBox);
        }

        /// <summary>
        /// Halves the possible line collision area
        /// </summary>
        /// <param name="e">Energon being checked</param>
        /// <param name="a">Start of Line</param>
        /// <param name="b">End of Line</param>
        /// <returns>true if collision, false otherwise</returns>
        private bool SubdivideLineCheck(Energon e, Vector2 a, Vector2 b)
        {
            bool returnValue = false;
            float maxLineLen = 1f;
            Vector2 midPoint = Vector2.Zero;
            midPoint.X = a.X + ((b.X - a.X) / 2);
            midPoint.Y = a.Y + ((b.Y - a.Y) / 2);
            if (Vector2.Distance(a, midPoint) > maxLineLen && !TrivialReject(e, a, midPoint))//Line still too long, but energon in this half
                returnValue = SubdivideLineCheck(e, a, midPoint);
            else if (Vector2.Distance(midPoint, b) > maxLineLen && !TrivialReject(e, midPoint, b))//Line still too long, but energon in this half
                returnValue = SubdivideLineCheck(e, midPoint, b);
            else if (!TrivialReject(e, a, midPoint))
                returnValue = DiscreteCheck(e, a, midPoint);
            else if (!TrivialReject(e, midPoint, b))
                returnValue = DiscreteCheck(e, midPoint, b);

            return returnValue;
        }

        /// <summary>
        /// Check to see if an Energon collides with the gate
        /// </summary>
        public bool Collides(Energon e)
        {
            //Is the gate a straight vertical line?
            if (gateSet && float.IsInfinity((Slope())) && e.Position.Y > MathHelper.Min(start.Y, end.Y) && e.Position.Y < MathHelper.Max(start.Y, end.Y))
                if (e.BoundingBox.Contains(new Point((int)start.X, (int)e.Position.Y)))
                    return true;
            if (gateSet && Slope() == 0 && e.Position.X > MathHelper.Min(start.X, end.X) && e.Position.X < MathHelper.Max(start.X, end.X))
                if (e.BoundingBox.Contains(new Point((int)e.Position.X, (int)start.Y)))
                    return true;

            //If we can trivially reject, no collision
            if (TrivialReject(e, start, end))
                return false;
            
            //If we get to this point then it's possible there is a collision.  Math time!
            //Check to see if the GATE crosses any of the edges
            
            //setup the Energon's area
            energonArea.X = (int)e.Position.X;
            energonArea.Y = (int)e.Position.Y;
            energonArea.Width = e.Texture.Width;
            energonArea.Height = e.Texture.Height;

            //Subdivide the line if it's too long
            return SubdivideLineCheck(e, start, end);
        }

        /// <summary>
        /// The GATE's current power level
        /// </summary>
        public int Level
        {
            get { return level; }
            set { level = value; }
        }

        /// <summary>
        /// The GATE's Normal Vector
        /// </summary>
        public Vector2 Normal
        {
            get { return normal; }
        }

        public float Distance
        {
            get { return Vector2.Distance(start, end); }
        }

        public float Slope()
        {
            return (end.Y - start.Y) / (end.X - start.X);
        }

    }
}
