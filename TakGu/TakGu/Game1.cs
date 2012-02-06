using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;
using Nini.Config;
using System.IO;

namespace TakGu
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;

        private SpriteBatch spriteBatch;

        private Dictionary<string, GameScene> scenes;
        private GameScene currentscene;

        private KeyboardState oldKState;
        private GamePadState oldGState;

        private SpriteFont font;
        private Texture2D onePixTexture;

        private IniConfigSource configSource;

        public Game1()
        {
            Content.RootDirectory = "Content";

            this.configSource = new IniConfigSource();

            try
                { this.configSource.Load("settings.ini"); }
            catch (FileNotFoundException)
                { this.MakeDefaultConfig(); }

            this.configSource.CaseSensitive = false;

            this.graphics = new GraphicsDeviceManager(this);
            this.Exiting += new EventHandler<EventArgs>(Handle_Exit);
        }
        
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.LoadGraphicsSettings();
            this.scenes = new Dictionary<string, GameScene>();

            base.Initialize();
            this.onePixTexture = Utility.MakeOnePixTexture(this.GraphicsDevice);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            this.spriteBatch = new SpriteBatch(GraphicsDevice);
            Services.AddService(typeof(SpriteBatch), spriteBatch);
            Services.AddService(typeof(IniConfigSource), this.configSource);

            this.font = Content.Load<SpriteFont>("font/verdana");
            this.scenes["action"] = new ActionScene(
                this
                , this.configSource
                , this.font
            );

            ((ActionScene)this.scenes["action"]).OnGameEnd += this.ShowMenuScene;

            this.scenes["credit"] = new CreditScene(
                this
                , this.font
            );

            this.scenes["credit"].Initialize();

            this.scenes["help"] = new HelpScene(
                this
                , this.font
            );

            this.scenes["help"].Initialize();

            this.scenes["highscore"] = new HighscoreScene(
                this
                , this.font
            );

            this.scenes["highscore"].Initialize();

            SoundEffect uiSound = Content.Load<SoundEffect>("audio/uibeep");
            Point center = this.GraphicsDevice.Viewport.Bounds.Center;

            this.scenes["menu"]= new MenuScene(
                this
                , this.font
                , uiSound
            );

            ((MenuScene)this.scenes["menu"]).AddItemsToMainMenu(
                new List<MenuItem>() {
                    new MenuItem("Start", this.Start)
                    ,new MenuItem("Settings", this.Settings)
                    ,new MenuItem("Help", this.Help)
                    ,new MenuItem("Highscores", this.Highscores)
                    ,new MenuItem("Credits", this.Credits)
                    ,new MenuItem("Quit", this.Quit)
                }
            );

            IConfig graphicsConfig = this.configSource.Configs["Graphics"];
            IConfig mediaConfig = this.configSource.Configs["Media"];

            ((MenuScene)this.scenes["menu"]).AddItemsToSettingsMenu(
                 new List<MenuItem>{
                    new MenuItem(
                        String.Format("Resolution: {0}x{1}"
                            , graphicsConfig.Get("width")
                            , graphicsConfig.Get("height")
                        )
                        , this.Resolution
                    )
                    ,new MenuItem(
                        String.Format("Fullscreen: {0}"
                            , this.graphics.IsFullScreen
                        )
                        , this.FullScreen
                    )
                    , new MenuItem(
                        String.Format("Texture detail: {0}"
                            , graphicsConfig.Get("texture_detail")
                        )
                        , this.TextureDetail
                    )
                    , new MenuItem(
                        String.Format("Model detail: {0}"
                            , graphicsConfig.Get("model_detail")
                        )
                        , this.ModelDetail
                    )
                    , new MenuItem(
                        String.Format("Effect volume: {0}%"
                            , mediaConfig.Get("effectvolume")
                        )
                        , this.EffectVolume
                    )
                    , new MenuItem(
                        String.Format("Music volume: {0}%"
                            , mediaConfig.Get("volume")
                        )
                        , this.MusicVolume
                    )
                    ,new MenuItem("Back to main menu"
                        , this.BackToMainMenu
                    )
                }
            );

            ((MenuScene)this.scenes["menu"]).AddItemsToStartMenu(
                new List<MenuItem>{
                    new MenuItem("Mouse", this.MouseType)
                    ,new MenuItem("GamePad", this.GamePadType)
                    ,new MenuItem("Kinect", this.KinectType)
                    ,new MenuItem("Webcam {{BETA}}", this.WebcamType)
                    ,new MenuItem("Back to main menu", this.BackToMainMenu)
                }
            );

            AudioManager audio = new AudioManager(
                this
                , this.font
            );

            this.Components.Add(audio);
            audio.Initialize();
            this.scenes["menu"].Initialize();

            foreach (GameScene scene in this.scenes.Values)
            {
                this.Components.Add(scene);
            }

            this.ShowScene("menu");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            #region keybinds
            KeyboardState keyState = Keyboard.GetState();

            if (this.currentscene == this.scenes["action"])
            {
                if (!((ActionScene)this.currentscene).IsTakingInput)
                {
                    InputUtility.ExecuteOnToggle(
                        Keys.P
                        , this.oldKState
                        , keyState
                        , this.currentscene.TogglePause
                    );
                }

                InputUtility.ExecuteOnToggle(
                    Keys.Escape
                    , this.oldKState
                    , keyState
                    , this.ShowMenuScene
                );
            } 
            else if (this.currentscene == this.scenes["credit"] 
                || this.currentscene == this.scenes["highscore"]
                || this.currentscene == this.scenes["help"] )
            {
                InputUtility.ExecuteOnToggle(
                    Keys.Space
                    , this.oldKState
                    , keyState
                    , this.ShowMenuScene
                );
            }

            this.oldKState = keyState;
            #endregion

            #region buttonbinds
            GamePadState padState = GamePad.GetState(PlayerIndex.One);

            if (this.currentscene == this.scenes["action"])
            {
                if(!((ActionScene)this.currentscene).IsTakingInput)
                {
                    InputUtility.ExecuteOnToggle(
                        Buttons.Start
                        , this.oldGState
                        , padState
                        , this.currentscene.TogglePause
                    );
                }

                InputUtility.ExecuteOnToggle(
                    new List<Buttons>(){
                    Buttons.BigButton
                    , Buttons.Back
                    }
                    , this.oldGState
                    , padState
                    , this.ShowMenuScene
                );
            }
            else if (this.currentscene == this.scenes["credit"] 
                || this.currentscene == this.scenes["highscore"]
                || this.currentscene == this.scenes["help"] )
            {
                InputUtility.ExecuteOnToggle(
                    Buttons.A
                    , this.oldGState
                    , padState
                    , this.ShowMenuScene
                );
            }

            this.oldGState = padState;
            #endregion

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            this.spriteBatch.Begin();

            base.Draw(gameTime);

            if (!this.currentscene.Enabled)
            {
                this.spriteBatch.Draw(
                    onePixTexture
                    , new Rectangle(
                        0
                        , 0
                        , this.GraphicsDevice.Viewport.Bounds.Right
                        , this.GraphicsDevice.Viewport.Bounds.Bottom
                    )
                    , Color.Black
                );

                Utility.DrawCenteredText(this.spriteBatch, "Paused", this.GraphicsDevice, this.font);
            }

            this.spriteBatch.End();
        }

        /// <summary>
        /// Creates a default config in memory(not on disk)
        /// </summary>
        private void MakeDefaultConfig()
        {
            IConfig config = configSource.AddConfig("Graphics");
            config.Set("multisampling", true);
            config.Set("width", 1024);
            config.Set("height", 768);
            config.Set("fullscreen", false);
            config.Set("texture_detail", "high");
            config.Set("model_detail", "low");

            config = configSource.AddConfig("Media");
            config.Set("volume", 100);
            config.Set("effectvolume", 100);

            config = configSource.AddConfig("GameType");
            config.Set("type", Paddle.Type.MOUSE);
        }

        /// <summary>
        /// Loads and applies graphical settings from config file.
        /// </summary>
        private void LoadGraphicsSettings()
        {
            IConfig graphicsConfig = this.configSource.Configs["Graphics"];

            this.graphics.PreferredBackBufferHeight = graphicsConfig.GetInt("height");
            this.graphics.PreferredBackBufferWidth = graphicsConfig.GetInt("width");
            this.graphics.PreferMultiSampling = graphicsConfig.GetBoolean("multisampling");

            if (graphicsConfig.GetBoolean("fullscreen"))
                this.graphics.ToggleFullScreen();
            
        }

        /// <summary>
        /// Exit handler, save configuration changes before closing.
        /// </summary>
        private void Handle_Exit(object sender, EventArgs e)
        {
            this.configSource.Save("settings.ini");
            WebcamPaddle.RequestStop();
        }

        #region mainmenu

        private void Start()
        {
            ((MenuScene)this.scenes["menu"]).ShowStartMenu();
        }

        private void Settings()
        {
            ((MenuScene)this.scenes["menu"]).ShowSettingsMenu();
        }

        private void Help()
        {
            this.ShowScene("help");
        }

        private void Highscores()
        {
            this.ShowScene("highscore");
        }

        private void Credits()
        {
            this.ShowScene("credit");
        }

        private void Quit()
        {
            this.Exit();
        }
        
        #endregion

        #region settingsmenu
        private void BackToMainMenu()
        {
            ((MenuScene)this.scenes["menu"]).ShowMainMenu();
        }

        private void Resolution()
        {
            List<DisplayMode> dpList =
                GraphicsAdapter.DefaultAdapter.SupportedDisplayModes.ToList();

            int width = this.graphics.PreferredBackBufferWidth;
            int height = this.graphics.PreferredBackBufferHeight;

            DisplayMode oldDpMode = null;
            foreach (DisplayMode dpMode in dpList)
                if (dpMode.Height == height && dpMode.Width == width)
                    oldDpMode = dpMode;

            int i = dpList.IndexOf(oldDpMode);

            DisplayMode newDpMode = dpList[(i + 1) % dpList.Count];

            this.graphics.PreferredBackBufferWidth = newDpMode.Width;
            this.graphics.PreferredBackBufferHeight = newDpMode.Height;
            this.graphics.ApplyChanges();

            IConfig graphicsConfig = this.configSource.Configs["Graphics"];
            graphicsConfig.Set("width", newDpMode.Width);
            graphicsConfig.Set("height", newDpMode.Height);

            ((MenuScene)this.scenes["menu"]).UpdateTextOnSettingsMenuItem(
                0
                , String.Format(
                    "Resolution: {0}x{1}"
                    , newDpMode.Width
                    , newDpMode.Height
                )
            );

            ((MenuScene)this.scenes["menu"]).ResetMenuPosition();
        }

        private void FullScreen()
        {
            this.graphics.ToggleFullScreen();

            this.configSource.Configs["Graphics"].Set(
                "fullscreen"
                , this.graphics.IsFullScreen
            );

            ((MenuScene)this.scenes["menu"]).UpdateTextOnSettingsMenuItem(
                1
                , String.Format(
                    "FullScreen: {0}"
                    , this.graphics.IsFullScreen
                )
            );

            ((MenuScene)this.scenes["menu"]).ResetMenuPosition();
        }

        private void TextureDetail()
        {
            IConfig graphicsConfig = this.configSource.Configs["Graphics"];

            string detail = NextDetail(
                graphicsConfig.Get("texture_detail")
            );

            graphicsConfig.Set("texture_detail", detail);

            ((MenuScene)this.scenes["menu"]).UpdateTextOnSettingsMenuItem(
                2
                , String.Format(
                    "Texture detail: {0}"
                    , detail
                )
            );
        }

        private void ModelDetail()
        {
            IConfig graphicsConfig = this.configSource.Configs["Graphics"];

            string detail = NextDetail(
                graphicsConfig.Get("model_detail")
            );

            graphicsConfig.Set("model_detail", detail);

            ((MenuScene)this.scenes["menu"]).UpdateTextOnSettingsMenuItem(
                3
                , String.Format(
                    "Model detail: {0}"
                    , detail
                )
            );
        }

        private void EffectVolume()
        {
            IConfig mediaConfig = this.configSource.Configs["Media"];
            int volume = mediaConfig.GetInt("effectvolume");

            volume += 10;

            if (volume > 100)
                volume = 0;

            mediaConfig.Set("effectvolume", volume);
            SoundEffect.MasterVolume = volume / 100f;

            ((MenuScene)this.scenes["menu"]).UpdateTextOnSettingsMenuItem(
                4
                , String.Format(
                    "Effect volume: {0}%"
                    , volume
                )
            );
        }

        private void MusicVolume()
        {
            IConfig mediaConfig = this.configSource.Configs["Media"];
            int volume = mediaConfig.GetInt("volume");

            volume += 10;

            if (volume > 100)
                volume = 0;

            mediaConfig.Set("volume", volume);
            MediaPlayer.Volume = volume / 100f;

            ((MenuScene)this.scenes["menu"]).UpdateTextOnSettingsMenuItem(
                5
                , String.Format(
                    "Music volume: {0}%"
                    , volume
                )
            );
        }
        #endregion

        #region startmenu
        public void MouseType()
        {
            this.configSource.Configs["GameType"].Set("type", Paddle.Type.MOUSE);
            this.LaunchGame();
        }

        public void GamePadType()
        {
            this.configSource.Configs["GameType"].Set("type", Paddle.Type.GAMEPAD);
            this.LaunchGame();
        }

        public void KinectType()
        {
            this.configSource.Configs["GameType"].Set("type", Paddle.Type.KINECT);
            this.LaunchGame();
        }

        public void WebcamType()
        {
            this.configSource.Configs["GameType"].Set("type", Paddle.Type.WEBCAM);
            this.LaunchGame();
        }

        #endregion

        private void LaunchGame()
        {
            ((MenuScene)this.scenes["menu"]).ShowMainMenu();
            this.ShowScene("action");
        }

        private void BackToMenu()
        {
            this.ShowScene("menu");
        }

        private string NextDetail(string detail)
        {
            if (detail == "low")
                detail = "med";
            else if (detail == "med")
                detail = "high";
            else if (detail == "high")
                detail = "low";
            return detail;
        }

        private void ShowScene(string key)
        {
            foreach (GameScene scene in this.scenes.Values)
                scene.Hide();

            this.scenes[key].Show();
            this.currentscene = this.scenes[key];

            if (!this.currentscene.IsInitialized)
                this.currentscene.Initialize();
            else
                if (this.currentscene is ActionScene)
                    ((ActionScene)this.currentscene).SetUp();
        }

        private void ShowMenuScene()
        {
            this.ShowScene("menu");
        }
    }
}
