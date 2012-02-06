using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TakGu
{
    class Score : DrawableGameComponent
    {

        private SpriteFont font;
        private float previousScore;
        private float score;
        private float level;
        private float lives;
        private Utility.Delegate_Float onNoMoreLives;

        public Score(Game game, SpriteFont font)
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
            this.lives = 5;
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

            string text = String.Format(
                "Score: {0}    Level: {1}    Lives left: {2}    Time left: {3}"
                , Math.Round(this.score, 0)
                , this.level
                , this.lives
                , Math.Round(60 - this.GetLevelPlayTime(), 0)
            );

            sBatch.DrawString(
                this.font
                , text
                , new Vector2(
                    this.GraphicsDevice.Viewport.Bounds.Center.X + 1 - this.font.MeasureString(text).X / 2
                    , this.GraphicsDevice.Viewport.Bounds.Bottom - 51
                )
                , Color.Black
            );

            sBatch.DrawString(
                this.font
                , text
                , new Vector2(
                    this.GraphicsDevice.Viewport.Bounds.Center.X - this.font.MeasureString(text).X / 2
                    , this.GraphicsDevice.Viewport.Bounds.Bottom - 50
                )
                , Color.WhiteSmoke
            );

            base.Draw(gameTime);
        }

        public void LevelUp()
        {
            this.score += 60 + 60 - this.GetLevelPlayTime();
            this.previousScore = this.score;
            this.level++;
        }

        private float GetLevelPlayTime()
        {
            return this.score - this.previousScore;
        }

        public void DecreaseLife()
        {
            this.previousScore = this.score;
            this.lives--;

            if(this.lives <= 0)
                this.onNoMoreLives(this.score);
        }

        public void IncreaseScoreForStayingAlive(float elapsedTime)
        {
            this.score += elapsedTime;
        }

        public Utility.Delegate_Float OnNoMoreLives
        {
            get { return this.onNoMoreLives; }
            set { this.onNoMoreLives = value; }
        }
    }
}
