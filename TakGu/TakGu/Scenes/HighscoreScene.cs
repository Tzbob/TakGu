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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.Threading;


namespace TakGu
{
    /// <summary>
    /// This is a game component that implements IDrawable.
    /// </summary>
    public class HighscoreScene : TextScene
    {
        private SpriteFont font;

        private MultilineText textComponent;

        public HighscoreScene(Game game, SpriteFont font)
            : base(game, font)
        {
            this.font = font;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            List<string> topten = this.SetUpText();

            this.textComponent = new MultilineText(
                this.Game
                , font
                , topten
                , new Vector2(
                    20f
                    , 100f
                )
                , false
            );

            this.Add(this.textComponent);

            base.Initialize();

        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game component should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        private List<string> SetUpText()
        {
            return new List<string>(){
                "Tak-Gu - Top 5"
                ,""
                ,"Contacting server, this might take a while..."
            };
        }

        private void SetTopTen()
        {
            List<string> highscores =  new HighscoreWebHandler().GetTop();

            this.textComponent.Text.RemoveAt(
                this.textComponent.Text.Count - 1
            );

            this.textComponent.Text.AddRange(highscores);
        }

        public override void Show()
        {
            this.textComponent.Text = this.SetUpText();
            new Thread(this.SetTopTen).Start();
            base.Show();
        }
    }
}