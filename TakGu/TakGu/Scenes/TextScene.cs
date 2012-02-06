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
    public class TextScene : GameScene
    {
        private SpriteFont font;

        public TextScene(Game game, SpriteFont font)
            : base(game)
        {
            this.font = font;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
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
            SpriteBatch sBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));

            string returnText = "Press <<Space>> or Button A to return";
            Vector2 dimensions = this.font.MeasureString(returnText);
            sBatch.DrawString(
                font
                , returnText
                , new Vector2(
                    this.GraphicsDevice.Viewport.Bounds.Center.X - dimensions.X/2
                    , this.GraphicsDevice.Viewport.Bounds.Bottom - 50
               )
               , Color.White
           );

            base.Draw(gameTime);
        }
    }
}