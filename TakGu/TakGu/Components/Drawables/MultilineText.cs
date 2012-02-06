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
    public class MultilineText : DrawableGameComponent
    {
        private SpriteFont font;
        private List<string> text;

        private Vector2 position;
        private bool center;

        public MultilineText(Game game, SpriteFont font, List<string> text, Vector2 position, bool center)
            : base(game)
        {
            this.text = text;
            this.font = font;
            this.position = position;
            this.center = center;
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

            for (int i = 0; i < this.text.Count; i++)
            {
                Vector2 fontDimensions = this.font.MeasureString(
                    this.text[i]
                );

                Vector2 fontPosition;
                if (!this.center)
                {
                    fontPosition = new Vector2(
                        this.position.X
                        , this.position.Y - fontDimensions.Y / 2 + i * (fontDimensions.Y)
                    );
                }
                else
                {
                    fontPosition = new Vector2(
                        this.position.X - fontDimensions.X / 2
                        , this.position.Y - fontDimensions.Y / 2 + i * (fontDimensions.Y)
                    );
                }

                fontPosition.X -= 1;
                fontPosition.Y -= 1;
                sBatch.DrawString(
                    font
                    , this.text[i]
                    , fontPosition
                    , Color.Black
                );

                fontPosition.X += 1;
                fontPosition.Y += 1;
                sBatch.DrawString(
                    font
                    , this.text[i]
                    , fontPosition
                    , Color.White
                );
            }

            base.Draw(gameTime);
        }

        public List<string> Text
        {
            get { return text; }
            set { text = value; }
        }
    }
}