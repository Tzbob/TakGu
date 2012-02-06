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


namespace TakGu
{
    /// <summary>
    /// This is a game component that implements IDrawable.
    /// </summary>
    public class HelpScene : TextScene
    {
        private SpriteFont font;

        public HelpScene(Game game, SpriteFont font)
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
            List<string> help = new List<string>();
            help.Add("Tak-Gu - Help");
            help.Add("");
            help.Add("-- Goal");
            help.Add("The objective is getting the ball past the enemy paddle.");
            help.Add("You'll gain points as you advance.");
            help.Add("-- Controls");
            help.Add("Browse songs with Page Up/Down or with the triggers on your pad.");
            help.Add("Control the volume with Ctrl+Page Up/Down.");
            help.Add("Pause the game with Return/P or with the Start on your pad.");
            help.Add("Return to menu with Escape or with the Back/Big button on your pad.");

            this.Add(new MultilineText(
                this.Game
                , font
                , help
                , new Vector2(20f, 100f)
                , false
            ));

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
    }
}