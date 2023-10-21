using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Navigation;
using Microsoft.Phone.Info;
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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Declarations
        GraphicsDeviceManager graphics;

        SpriteBatch spriteBatch;
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        //Random for the whole project to use
        private static Random random = new Random();
        public static Random Random
        {
            get { return random; }
        }

        int currentWorld = 0;
        public int CurrentWorld
        {
            get { return currentWorld; }
            set { currentWorld = value; }
        }

        public const int levelCap = 5;
        public static int LevelCap
        {
            get { return levelCap; }
        }

        List<Texture2D> worldBackdrops;
        public List<Texture2D> WorldBackdrops
        {
            get { return worldBackdrops; }
        }

        #region World Camera Pan Variables
        static int panX = 0;
        public static int PanX
        {
            get { return panX; }
            set { panX = value; }
        }
        static int panY = 0;
        public static int PanY
        {
            get { return panY; }
            set { panY = value; }
        }
        static float zoom = 1f;
        public static float Zoom
        {
            get { return zoom; }
            set { zoom = value; }
        }

        static bool isZoomingIn = true;
        public static bool IsZoomingIn
        {
            get { return isZoomingIn; }
            set { isZoomingIn = value; }
        }
        #endregion

        #region Options

        static bool shouldPlayMusic = true;
        public static bool ShouldPlayMusic
        {
            get { return shouldPlayMusic; }
            set { shouldPlayMusic = value; }
        }

        static bool musicOff = false;
        public static bool MusicOff
        {
            get { return musicOff; }
            set { musicOff = value; }
        }

        static bool shouldPlaySound = true;
        public static bool ShouldPlaySound
        {
            get { return shouldPlaySound; }
            set { shouldPlaySound = value; }
        }

        #endregion

        //Reusable textures for the Energons, Buttons
        BackgroundMusicManager musicManager;
        Texture2D dot, tri, quad, penta, hex, circle, locked;
        Texture2D menuButtonEmpty, menuButtonFilled, optionsButtonEmpty, optionsButtonFilled;
        Texture2D remma, isaak, treavor, gunnardr, gunnardi, gunnardt, black;
        Texture2D loadSplash;
        SpriteFont font;
        Screen currentScreen;
        Stack<Screen> previousScreens;
        Dictionary<string, Screen> gameScreens;
        public PlayerData player;
        static int screenHeight, screenWidth;
        int hudHeight;
        bool loadContent = true;
        bool loadingScreenShowing;
        //NavigationService navService;
        #endregion
        

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            gameScreens = new Dictionary<string, Screen>();
            worldBackdrops = new List<Texture2D>();
            Content.RootDirectory = "Content";
            loadingScreenShowing = false;
            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);
            // Extend battery life under lock.
            InactiveSleepTime = TimeSpan.FromSeconds(1);
            //navService = new NavigationService();
            // Create our music manager component and add it to the game.
            musicManager = new BackgroundMusicManager(this);
            musicManager.PromptGameHasControl += MusicManagerPromptGameHasControl;
            musicManager.PlaybackFailed += MusicManagerPlaybackFailed;
            Components.Add(musicManager);            
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //GAME STATE IS LOADED IN PLAYERDATA CONSTRUCTOR
            player = new PlayerData();
            //CHEATER MODE ACTIVATE, ALL LEVELS UNLOCKED
            //Guide.SimulateTrialMode = true;
            //player.UnlockedLevels = 24;
            //END CHEATER MODE
            hudHeight = graphics.GraphicsDevice.Viewport.Height / 12 + 1;
            screenHeight = graphics.GraphicsDevice.Viewport.Height;
            screenWidth = graphics.GraphicsDevice.Viewport.Width;

            loadSplash = Content.Load<Texture2D>("backgrounds/loading");
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here
           
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        //Called after the Loading screen is drawn, to stop the OS from shutting down on 256MB devices.
        private void LoadBulkContent()
        {
            #region LoadATonOfImages
            //Load commonly used images
            dot = Content.Load<Texture2D>("sprites/point");
            tri = Content.Load<Texture2D>("sprites/triangle");
            quad = Content.Load<Texture2D>("sprites/quad");
            penta = Content.Load<Texture2D>("sprites/penta");
            hex = Content.Load<Texture2D>("sprites/hex");
            circle = Content.Load<Texture2D>("sprites/circle");

            menuButtonEmpty = Content.Load<Texture2D>("backgrounds/menuFrameEmpty");
            menuButtonFilled = Content.Load<Texture2D>("backgrounds/menuFrameFilled");
            optionsButtonEmpty = Content.Load<Texture2D>("backgrounds/optionsFrameEmpty");
            optionsButtonFilled = Content.Load<Texture2D>("backgrounds/optionsFrameFilled");
            locked = Content.Load<Texture2D>("sprites/lock");

            //Create a list of World Backdrops for use on world select and level select screens
            worldBackdrops.Add(Content.Load<Texture2D>("backgrounds/nebula"));
            worldBackdrops.Add(Content.Load<Texture2D>("backgrounds/callirrhoe"));
            worldBackdrops.Add(Content.Load<Texture2D>("backgrounds/amalthea"));
            worldBackdrops.Add(Content.Load<Texture2D>("backgrounds/hyperion"));
            worldBackdrops.Add(Content.Load<Texture2D>("backgrounds/iapetus"));
            worldBackdrops.Add(Content.Load<Texture2D>("backgrounds/aegaeon"));

            remma = Content.Load<Texture2D>("characters/remma");
            isaak = Content.Load<Texture2D>("characters/isaak");
            treavor = Content.Load<Texture2D>("characters/treavor");
            gunnardr = Content.Load<Texture2D>("characters/bro_r");
            gunnardi = Content.Load<Texture2D>("characters/bro_i");
            gunnardt = Content.Load<Texture2D>("characters/bro_t");
            black = Content.Load<Texture2D>("characters/black");
            #endregion

            #region LoadATonOfSFX
            menuSelect = Content.Load<SoundEffect>("audio/menuSelect");
            menuBack = Content.Load<SoundEffect>("audio/menuBack");
            explosion = Content.Load<SoundEffect>("audio/explosion");
            collected = Content.Load<SoundEffect>("audio/collected");
            reflected = Content.Load<SoundEffect>("audio/reflected");
            shatter = Content.Load<SoundEffect>("audio/shatter");
            #endregion

            #region Create Screens
            //List of Screens
            Screen titleScreen = new TitleScreen("title", this);
            Screen mainMenu = new MainMenu("mainmenu", this);
            Screen worldSelect = new WorldSelect("worldselect", this);
            Screen levelSelect = new LevelSelect("levelselect", this);
            Screen tutorial = new TutorialScreen("tutorial", this);
            Screen preludeScreen = new PreludeScreen("prelude", this);
            Screen level = new Level("gamescreen", this);
            Screen options = new OptionsScreen("options", this);
            Screen chapters = new ChaptersScreen("chapters", this);
            Screen exit = new ExitScreen("exit", this);
            Screen results = new ResultsScreen("results", this);
            Screen credits = new CreditScreen("credits", this);

            //Add the screens to a Dictionary of screens
            gameScreens.Add(titleScreen.Name, titleScreen);
            gameScreens.Add(mainMenu.Name, mainMenu);
            gameScreens.Add(worldSelect.Name, worldSelect);
            gameScreens.Add(levelSelect.Name, levelSelect);
            gameScreens.Add(tutorial.Name, tutorial);
            gameScreens.Add(preludeScreen.Name, preludeScreen);
            gameScreens.Add(level.Name, level);
            gameScreens.Add(options.Name, options);
            gameScreens.Add(chapters.Name, chapters);
            gameScreens.Add(exit.Name, exit);
            gameScreens.Add(results.Name, results);
            gameScreens.Add(credits.Name, credits);
            #endregion
            font = Content.Load<SpriteFont>("fonts/MenuFont");
            currentScreen = titleScreen;
            previousScreens = new Stack<Screen>();
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = .1f;
            if (Game1.ShouldPlayMusic)
                musicManager.Play(currentScreen.Music);
            loadContent = false;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                NavigateBack(gameTime);
                   
            }

            if (loadContent)
            {
                if (loadingScreenShowing)
                    LoadBulkContent();
            }
            else
            {
                if (currentScreen.ChangeScreen)
                    ChangeScreens(gameTime);

                currentScreen.Update(gameTime);
            }
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            if (loadContent)
            {
                loadingScreenShowing = true;
                spriteBatch.Draw(loadSplash, new Rectangle(0, 0, Game1.screenWidth, Game1.screenHeight), Color.White);           
            }
            else
            {
                spriteBatch.GraphicsDevice.Clear(Color.Black);
                //Draw the current screen
                currentScreen.Draw(gameTime, spriteBatch);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void ChangeScreens(GameTime gameTime)
        {
            //close old screen
            string oldMusic = currentScreen.MusicName;
            previousScreens.Push(currentScreen);
            currentScreen = gameScreens[currentScreen.NextScreen];
            //setup new screen
            currentScreen.Reset(gameTime);
            currentScreen.OnScreenActivate();
            currentScreen.TellWorld(player.CurrentWorld);
            //Manage Music
            if (oldMusic == currentScreen.MusicName && ShouldPlayMusic) //Keep playing same if new screen uses same music
            {
                if (MusicOff && ShouldPlayMusic)         //Turn the music back on after it's been changed in the options
                    musicManager.Play(currentScreen.Music);
            }
            else if (currentScreen.Music != null && ShouldPlayMusic)
                musicManager.Play(currentScreen.Music);  //change the music for the new screen
            else
                musicManager.Stop();                     //this screen has no associated music
        }

        //Move back a screen on Back Button press
        private void NavigateBack(GameTime gameTime)
        {
            //navService.GoBack();
            
            string oldMusic = currentScreen.MusicName;
            if (previousScreens.Count > 0)
            {
                //get previous screen
                currentScreen = previousScreens.Pop();
                //reset screen
                currentScreen.Reset(gameTime);
                currentScreen.OnScreenActivate();
                currentScreen.TellWorld(player.CurrentWorld);
                //Manage Music
                if (oldMusic == currentScreen.MusicName && ShouldPlayMusic) //Keep playing same if new screen uses same music
                {
                    if (MusicOff && ShouldPlayMusic)         //Turn the music back on after it's been changed in the options
                        musicManager.Play(currentScreen.Music);
                }
                else if (currentScreen.Music != null && ShouldPlayMusic)
                    musicManager.Play(currentScreen.Music);  //change the music for the new screen
                else
                    musicManager.Stop();                     //this screen has no associated music
            }
            else
            {
                player.Save();
                this.Exit();
            }
        }

        #region Music Manager Event Handlers

        /// <summary>
        /// Invoked if the user is listening to music when we tell the music manager to play our song.
        /// We can respond by prompting the user to turn off their music, which will cause our game music
        /// to start playing.
        /// </summary>
        private void MusicManagerPromptGameHasControl(object sender, EventArgs e)
        {
            // Show a message box to see if the user wants to turn off their music for the game's music.
            Guide.BeginShowMessageBox(
                "Use game music?",
                "Would you like to turn off your music to listen to the game's music?",
                new[] { "Yes", "No" },
                0,
                MessageBoxIcon.None,
                result =>
                {
                    // Get the choice from the result
                    int? choice = Guide.EndShowMessageBox(result);

                    // If the user hit the yes button, stop the media player. Our music manager will
                    // see that we have a song the game wants to play and that the game will now have control
                    // and will automatically start playing our game song.
                    if (choice.HasValue && choice.Value == 0)
                        MediaPlayer.Stop();
                },
                null);
        }

        /// <summary>
        /// Invoked if music playback fails. The most likely case for this is that the Phone is connected to a PC
        /// that has Zune open, such as while debugging. Most games can probably just ignore this event, but we 
        /// can prompt the user so that they know why we're not playing any music.
        /// </summary>
        private void MusicManagerPlaybackFailed(object sender, EventArgs e)
        {
            // We're going to show a message box so the user knows why music didn't start playing.
            Guide.BeginShowMessageBox(
                "Music playback failed",
                "Music playback cannot begin if the phone is connected to a PC running Zune.",
                new[] { "Ok" },
                0,
                MessageBoxIcon.None,
                null,
                null);
        }

        #endregion

        #region Helper Functions

        //  a handy little function that gives a random float between two
        // values. This will be used in several places in the sample, in particilar in
        // ParticleSystem.InitializeParticle.
        public static float RandomBetween(float min, float max)
        {
            return min + (float)random.NextDouble() * (max - min);
        }

        //Accessors for the Energon Sprites
        #region Texture Accessors
        public Texture2D DotSprite
        {
            get { return dot; }
        }
        public Texture2D TriangleSprite
        {
            get { return tri; }
        }
        public Texture2D QuadSprite
        {
            get { return quad; }
        }
        public Texture2D PentagonSprite
        {
            get { return penta; }
        }
        public Texture2D HexagonSprite
        {
            get { return hex; }
        }
        public Texture2D CircleSprite
        {
            get { return circle; }
        }
        public Texture2D MenuButtonEmpty
        {
            get { return menuButtonEmpty; }
        }
        public Texture2D MenuButtonFilled
        {
            get { return menuButtonFilled; }
        }
        public Texture2D OptionsButtonEmpty
        {
            get { return optionsButtonEmpty; }
        }
        public Texture2D OptionsButtonFilled
        {
            get { return optionsButtonFilled; }
        }
        public Texture2D Lock
        {
            get { return locked; }
        }
        public Texture2D Remma
        {
            get { return remma; }
        }
        public Texture2D Isaak
        {
            get { return isaak; }
        }
        public Texture2D Treavor
        {
            get { return treavor; }
        }
        public Texture2D GunnardR
        {
            get { return gunnardr; }
        }
        public Texture2D GunnardI
        {
            get { return gunnardi; }
        }
        public Texture2D GunnardT
        {
            get { return gunnardt; }
        }
        public Texture2D Black
        {
            get { return black; }
        }
        #endregion

        //Accessors for Sound Effects
        #region SFX Accessors
        SoundEffect menuSelect;
        public SoundEffect MenuSelect
        {
            get { return menuSelect; }
        }

        SoundEffect menuBack;
        public SoundEffect MenuBack
        {
            get { return menuBack; }
        }

        SoundEffect explosion;
        public SoundEffect Explosion
        {
            get { return explosion; }
        }

        SoundEffect collected;
        public SoundEffect Collected
        {
            get { return collected; }
        }

        SoundEffect reflected;
        public SoundEffect Reflected
        {
            get { return reflected; }
        }

        SoundEffect shatter;
        public SoundEffect Shatter
        {
            get { return shatter; }
        }
        #endregion

        static int id = 0;
        public static int GetID()
        {
            return id++;
        }

        public int HUDHeight
        {
            get { return hudHeight; }
        }

        public static int ScreenHeight
        {
            get { return screenHeight; }
        }

        public static  int ScreenWidth
        {
            get { return screenWidth; }
        }

        #endregion
    }
}
