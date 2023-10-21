using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
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
    class Level : Screen
    {
        #region Declarations     
        GATE gate;               //The GATE to be used in this level
         
        List<Energon> energons;  //A list of energons that will be active in this level
        List<StaticGameObject> gameObjects; //A list of GameObjects that will be active in the level.

        TouchCollection touches; //Collection of Touches on the screen
        HeadsUpDisplay hud;      //The Level's Heads Up Display

        ScreenTextSystem floatingScoreText;       //"Particle" system used to draw floating text on the screen

        Texture2D glowLeft, glowRight;

        int comboTimer;          //countdown timer for reseting combos
        int screenChangeTimer;   //used to delay screen change
        int energonCap;          //max number of energons allowed in the level at once
        int cleanupTimer;        //cleanup lists every X updates
        bool changeTimerStarted; //used to only start the screen change timer once
        float requiredEnergy;    //requirement to pass level
        float glowOpacity;       //changes visiblity of the OVERCHARGE glow
        bool increaseEnergonCap; //lets the game know if energon cap should be increased for Overcharge
        bool magnetFieldActive;  //lets the game know if magnetic anomalies can spawn
        bool rogueParticleActive;//lets the game know if rogue particles can spawn
        bool supernovaActive;    //lets the game know if supernovas can spawn
        bool flash;              //tells the screen to flash when you try to collect an energon you are not allowed to.
        bool causeShockwave;     //tells the game to create a shockwave effect
        bool fading;             //tells if the glow is fading
        Vector2 shockwavePos;    //the location of the shockwave
        #endregion

        public Level(string name, Game1 game)
        {
            nextScreen = "results";      
            this.name = name;
            this.game = game;
            this.musicName = "gameplay";
         
            bgm = game.Content.Load<Song>("audio/gameplay1");
            glowLeft = game.Content.Load<Texture2D>("backgrounds/glowL");
            glowRight = game.Content.Load<Texture2D>("backgrounds/glowR");

            screenChangeTimer = 0;
            changeTimerStarted = false;
            flash = false;
            increaseEnergonCap = true;
            causeShockwave = false;
            shockwavePos = Vector2.Zero;
            comboTimer = 0;
            cleanupTimer = 0;
            game.player.Combo = 1;
            energonCap = 35; //start of with a max of 30 energons on screen
            //Level-Specific Info:: Should probably move this out to another class later
            
            gate = new GATE(game);
            hud = new HeadsUpDisplay(game, this);
            energons = new List<Energon>();
            gameObjects = new List<StaticGameObject>();
            //gameObjects.Add(new Supernova(game, new Vector2(400, 150)));

            floatingScoreText = new ScreenTextSystem(game, "fonts/ComboFont", 10);
            game.Components.Add(floatingScoreText);
            requiredEnergy = 100;
            game.player.CurrentEnergyAmount = 25;
            glowOpacity = .9f;
        }
        
        /// <summary>
        /// Update updates all of Level components.
        /// Also as an intermediary between the GATE and other things in the level. 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            //Increase cap during Overcharge
            if (game.player.PlayerLevel == Game1.LevelCap && increaseEnergonCap)
            {
                energonCap += 40;
                increaseEnergonCap = false;
            }
            #region Cleanup/Simulation
            //Perform cleanup on the Energon and GameObject lists (every X updates), spawn approiate Energons/Objects
            cleanupTimer++;
            if (cleanupTimer > 4)
            {
                CleanupEnergons(energons);
                CleanupGameObjects(gameObjects);
                cleanupTimer = 0;
            }
            SimulateLevel(energons.Count);
            EmployFieldEffects();
            #endregion

            #region GameOverLogic
            if (game.player.CurrentEnergyAmount <= 0)
            {
                if (!changeTimerStarted)
                    screenChangeTimer = (int)gameTime.TotalGameTime.TotalMilliseconds;
                if (game.player.PlayerLevel < Game1.LevelCap)
                    hud.LevelFailed = true;
                else
                    hud.LevelPassed = true;
                changeTimerStarted = true;
                gate.Cancel();
            }
            #endregion

            touches = TouchPanel.GetState();
            if (!hud.LevelFailed && !hud.LevelPassed) 
                gate.Update(gameTime, touches);

            #region Energon Update Loop
            foreach (Energon e in energons)
            {
                if (e.Active)
                {
                    //see if the energon goes across the GATE
                    e.Update(gameTime);

                    //Give GameObjects a chance to act on the energons
                    foreach (StaticGameObject obj in gameObjects)
                        if (obj.Active)
                            obj.AssertInfluence(e);

                    //check if collision
                    if (gate.Collides(e) && !e.Reflected && !hud.LevelFailed)
                    {
                        if (e.Power > game.player.PlayerLevel) //Reflect the Energon
                        {
                            flash = true;
                            if (Game1.ShouldPlaySound)
                                game.Reflected.Play();
                            e.ChangeDirection(gate.Normal);
                            e.Reflected = true;
                            //Remove energy for trying to get a too powerful energon
                            if (!hud.LevelFailed && !hud.LevelPassed) //Only remove value if level still active
                            {
                                game.player.TakeDamage(e.EnergyValue);
                                floatingScoreText.AddParticles("-" + e.EnergyValue, e.Position, Color.PaleVioletRed);
                            }
                            ResetCombo(gameTime);
                        }
                        else if (!e.Collected) //Collect the Energon, start or continue timer
                        {                                                    
                            e.Emit();
                            if(Game1.ShouldPlaySound)
                                game.Collected.Play();
                            if (!hud.LevelFailed && !hud.LevelPassed) //Only add value if level still active, and not in Overcharge
                            {
                                float comboValue = game.player.Combo * (gate.Distance * .007f);
                                game.player.AddEnergy(e.EnergyValue + comboValue);
                                floatingScoreText.AddParticles("+" + (int)(e.EnergyValue + comboValue), e.Position, Color.Gold);                               
                            }
                            e.Collected = true;
                            game.player.Combo++;
                            hud.ChangeMultiplier(game.player.Combo, gameTime);
                            comboTimer = (int)gameTime.TotalGameTime.TotalMilliseconds;
                        }
                    }
                    if (e.Reflected || (e.Power == 6 && e.Active)) //power of 6 is a circle
                    {
                        //This needs to be faster
                        //see if this hits anything lower value
                        //e.Emit();
                        foreach (Energon e2 in energons)
                        {
                            if (e.CollidesWith(e2) && e2.Active && e2.Power <= e.Power)
                            {
                                e2.Emit();
                                if (Game1.ShouldPlaySound && !e2.Collected)
                                    game.Shatter.Play();
                                e2.Collected = true;
                            }
                        }
                    }
                }
            }

            #endregion

            #region GameObject Update Loop
            foreach (StaticGameObject obj in gameObjects)
                if (obj.Active)
                {
                    obj.Update(gameTime);
                    if (obj.ActivateEffect)
                    {
                        shockwavePos = obj.Position;
                        if (Game1.ShouldPlaySound)
                            game.Explosion.Play(.7f, 0, 0);
                        causeShockwave = true;
                        obj.ActivateEffect = false;
                    }
                }

            if (causeShockwave)
            {
                gameObjects.Add(new Shockwave(game, shockwavePos));
                //Give the Shockwave a payload
                for (int x = 0; x < 3; x++)
                    energons.Add(new Circle(game, shockwavePos));
                causeShockwave = false;
                energonCap += 3;
            }
            #endregion

            //end combo if it has been too long since a collection.  Set at 2 seconds right now
            if (comboTimer < gameTime.TotalGameTime.TotalMilliseconds - 2000 && game.player.Combo > 1)
                ResetCombo(gameTime);

            if (fading)
            {
                glowOpacity -= .05f;
                if (glowOpacity < 0)
                    fading = false;
            }
            else
            {
                glowOpacity += .05f;
                if (glowOpacity > .9f)
                    fading = true;
            }

            hud.Update();
            game.player.Update(gameTime);

            //End the Level where appropriate
            if (screenChangeTimer <= gameTime.TotalGameTime.TotalMilliseconds - 2000 && changeTimerStarted)
                this.ChangeScreen = true;      
        }

        /// <summary>
        /// Draw the Level
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //Draw overcharge glow
            if (game.player.PlayerLevel == Game1.LevelCap)
            {
                spriteBatch.Draw(glowLeft, new Vector2(0, game.HUDHeight), Color.White * glowOpacity);
                spriteBatch.Draw(glowRight, new Vector2(Game1.ScreenWidth - glowRight.Width, game.HUDHeight), Color.White * glowOpacity);
            }

            if (flash)
            {
                spriteBatch.GraphicsDevice.Clear(Color.White);
                flash = false;
            }
            else
            {
                //Draw a backdrop
                spriteBatch.Draw(game.WorldBackdrops[game.player.CurrentWorld], Vector2.Zero, Color.White * .2f);

                foreach (Energon e in energons)
                    if (e.Active)
                        e.Draw();

                if (gate.ShouldDraw)
                    gate.drawGATE(spriteBatch);

                foreach (StaticGameObject obj in gameObjects)
                    if (obj.Active)
                        obj.Draw(gameTime, spriteBatch);

                hud.Draw(gameTime);
            }
            game.player.Draw(spriteBatch, hud.MessageFont);            
       }

        /// <summary>
        /// Resets gameplay-related values and HuD values in addition to Screen values
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Reset(GameTime gameTime)
        {
            //Reset gameplay related values
            screenChangeTimer = 0;
            comboTimer = 0;
            cleanupTimer = 0;
            changeTimerStarted = false;
            flash = false;
            causeShockwave = false;
            magnetFieldActive = false;
            rogueParticleActive = false;
            supernovaActive = false;
            increaseEnergonCap = true;
            shockwavePos = Vector2.Zero;      
            game.player.Combo = 1;
            energonCap = 35;
            requiredEnergy = 100;
            glowOpacity = .9f;
            hud.Reset();
            game.player.Reset();
            gate.Cancel();
            CleanupEnergons(energons);
            gameObjects.Clear();
            energons.Clear();
            base.Reset(gameTime);
        }

        public override void TellWorld(int world)
        {
            //Change what Gameplay elements are available based on which World
            if (world >= 1) //World 2
                magnetFieldActive = true;
            if (world >= 2) //World 3
                gate.SetGateType(2);
            if (world >= 3) //World 4
                rogueParticleActive = true;
            if (world >= 4) //World 5
                gate.SetGateType(3);
            if (world >= 5) //World 6
                supernovaActive = true;
            base.TellWorld(world);
        }

        /// <summary>
        /// Makes decisions regarding which Energons to spawn, how many to spawn, when to spawn, etc.
        /// </summary>
        private void SimulateLevel(int energonCount)
        {
            for (int x = 0; x < 10; x++)
            {
                if (energonCount < energonCap)
                {
                    double type = Game1.Random.NextDouble();
                    #region RandomizeEnergons
                    switch (game.player.PlayerLevel)
                    {
                        case 1:
                            if (type < .66)
                                energons.Add(new Dot(game));
                            else
                                energons.Add(new Triangle(game));
                            break;

                        case 2:
                            if (type < .33)
                                energons.Add(new Dot(game));
                            else if (type >= .33 && type < .66)
                                energons.Add(new Triangle(game));
                            else
                                energons.Add(new Quad(game));
                            break;

                        case 3:
                            if (type < .25)
                                energons.Add(new Dot(game));
                            else if (type >= .25 && type < .5)
                                energons.Add(new Triangle(game));
                            else if (type >= .5 && type < .75)
                                energons.Add(new Quad(game));
                            else
                                energons.Add(new Penta(game));
                            break;

                        case 4:
                            if (type < .2)
                                energons.Add(new Dot(game));
                            else if (type >= .2 && type < .4)
                                energons.Add(new Triangle(game));
                            else if (type >= .4 && type < .6)
                                energons.Add(new Quad(game));
                            else if (type >= .6 && type < .8)
                                energons.Add(new Penta(game));
                            else
                                energons.Add(new Hex(game));
                            break;

                        case 5:
                            if (type < .1)
                                energons.Add(new Dot(game));
                            else if (type >= .1 && type < .3)
                                energons.Add(new Triangle(game));
                            else if (type >= .3 && type < .5)
                                energons.Add(new Quad(game));
                            else if (type >= .5 && type < .7)
                                energons.Add(new Penta(game));
                            else
                                energons.Add(new Hex(game));
                            break;

                        default:
                            break;
                    }
                    #endregion
                }
            }
        }

        /// <summary>
        /// Chooses when to spawn the appropriate gameplay elements
        /// </summary>
        private void EmployFieldEffects()
        {
            double doSomething = Game1.Random.NextDouble();
            if (doSomething > .994) //.006% spawn chance per update???
            {
                double chooseEvent = Game1.Random.NextDouble();
                if (chooseEvent < .33 && magnetFieldActive)
                    gameObjects.Add(new MagneticAnomaly(game, new Vector2(Game1.RandomBetween(0, Game1.ScreenWidth),
                        Game1.RandomBetween(game.HUDHeight, Game1.ScreenHeight))));
                else if (chooseEvent >= .33 && chooseEvent < .66 && supernovaActive)
                    gameObjects.Add(new Supernova(game, new Vector2(Game1.RandomBetween(0, Game1.ScreenWidth),
                        Game1.RandomBetween(game.HUDHeight, Game1.ScreenHeight))));
                else if (rogueParticleActive)
                {
                    energons.Add(new Circle(game));
                    energonCap++;
                }
            }
        }

        #region HelperMethods/Accessors

        private void CleanupEnergons(List<Energon> e)
        {
            for (int x = 0; x < e.Count; x++)
            {
                if (!e[x].Active)
                {
                    e[x].RemoveParticleSystem();
                    e.RemoveAt(x);
                    x--;
                }
            }
        }

        private void CleanupGameObjects(List<StaticGameObject> objs)
        {
            for (int x = 0; x < objs.Count; x++)
            {
                if (!objs[x].Active)
                {
                    objs.RemoveAt(x);
                    x--;
                }
            }
        }

        public void CancelGate()
        {
            gate.Cancel();
        }

        private void ResetCombo(GameTime gameTime)
        {
            //if (game.player.Combo > 1)
            //{
                game.player.Combo = 1;
                hud.ChangeMultiplier(game.player.Combo, gameTime);
            //}
        }

        public float EnergyRequirement
        {
            get { return requiredEnergy; }
        }
        #endregion
    }
}
