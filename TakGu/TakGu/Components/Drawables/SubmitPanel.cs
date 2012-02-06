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
using System.Net;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Nini.Config;


namespace TakGu
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>

    public class SubmitPanel : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private SpriteFont font;
        private float score;

        private Texture2D onePixTexture;

        private KeyboardState oldKState;

        private Utility.Delegate_Paramless onSubmit;

        private MultilineText textComponent;

        private string name;
        private string message;

        private bool isSubmitted;

        private const int MAXCHARS = 13;
        private const string DEFAULTUSER = "USER";

        private IniConfigSource configSource;

        public SubmitPanel(Game game, SpriteFont font, float score, IniConfigSource configSource)
            : base(game)
        {
            this.font = font;
            this.score = score;
            this.configSource = configSource;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            this.isSubmitted = false;
            this.name = DEFAULTUSER;
            this.message = "Press <<Enter>> or Button A to submit your score\nPress <<Escape>> or Big Button/Button Back to cancel.";
            this.onePixTexture = Utility.MakeOnePixTexture(this.GraphicsDevice);

            this.textComponent = new MultilineText(
                this.Game
                , this.font
                , this.CreateText()
                , new Vector2(
                    this.GraphicsDevice.Viewport.Bounds.Center.X
                    , this.GraphicsDevice.Viewport.Bounds.Center.Y
                )
                , true
            );
            this.textComponent.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();

            InputUtility.ExecuteOnToggle(
                Keys.Enter
                , this.oldKState
                , keyState
                , this.Submit
            );

            if(!this.isSubmitted)
                this.name = InputUtility.TypeText(this.oldKState, keyState, this.name, MAXCHARS, DEFAULTUSER);

            this.oldKState = keyState;

            this.textComponent.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game component should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch sBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));

            sBatch.Draw(
                onePixTexture
                , new Rectangle(
                    0
                    , 0
                    , this.GraphicsDevice.Viewport.Bounds.Right
                    , this.GraphicsDevice.Viewport.Bounds.Bottom
                )
                , Color.Black
            );

            this.textComponent.Text = this.CreateText();
            this.textComponent.Draw(gameTime);

            base.Draw(gameTime);
        }

        public void Submit()
        {
            if (!this.isSubmitted)
            {
                new Thread(this.PostScores).Start();
            }
        }

        private List<string> CreateText()
        {
            return new List<string>() {
                    String.Format("Your ending score was {0}.", Math.Round(this.score))
                    , "Type your 12-letter nickname for our highscores!"
                    , this.name
                    , this.message
                };
        }

        private void PostScores()
        {
            this.isSubmitted = true;
            this.message = "Contacting the server, this may take a while...";

            new HighscoreWebHandler().PostScores(
                this.name
                , (int)Math.Round(this.score)
                , this.configSource.Configs["GameType"].Get("type")
            );

            this.message = "Your stats are now on the server! Press the <<Escape>> key to go back to the menu.";

            this.onSubmit();
        }

        public Utility.Delegate_Paramless OnSubmit
        {
            get { return onSubmit; }
            set { onSubmit = value; }
        }
    }
}
