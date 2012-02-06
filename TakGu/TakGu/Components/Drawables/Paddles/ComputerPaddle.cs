using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using Microsoft.Xna.Framework.Audio;

namespace TakGu
{
    /// <summary>
    /// This is a game component that inherits Quadrilateral.
    /// </summary>
    public class ComputerPaddle : Paddle
    {
        private Sphere sphere;
        private float width;
        private float height;
        private float speed;

        /// <summary>
        /// Constructs the slide object
        /// </summary>
        /// <param name="game">The base game.</param>
        /// <param name="texture">The texture that should be used to draw the quad.</param>
        /// <param name="position">The position of the leftbottom corner.</param>
        /// <param name="normal">The vector that defines the normal.</param>
        /// <param name="up">The vector that defines which way is up.</param>
        /// <param name="width">The width of the quad.</param>
        /// <param name="height">The height of the quad.</param>
        /// <param name="textureSize">
        ///     The scale at which the texture should be drawn
        ///     higher means less repetitions
        /// </param>
        /// <param name="speed">The speed of the computer.</param>
        public ComputerPaddle(Game game, Texture2D texture, Texture2D textureActive, Vector3 position, Vector3 normal, Vector3 up, float width, float height, float textureSize, SoundEffect sound, float speed, float xBound, float yBound)
            : base(game, texture, textureActive, position, normal, up, width, height, textureSize, sound, xBound, yBound)
        {
            this.width = width;
            this.height = height;
            this.speed = speed;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            this.sphere = (Sphere)this.Game.Services.GetService(typeof(Sphere));
            this.State = PaddleState.PLAYING;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            Vector3 newPosition = this.Position;

            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (this.Position.X + this.width / 2 < this.sphere.Position.X)
                newPosition.X += speed * elapsedTime;

            if (this.Position.X + this.width / 2 > this.sphere.Position.X)
                newPosition.X -= speed * elapsedTime;

            if (this.Position.Y + this.width / 2 < this.sphere.Position.Y)
                newPosition.Y += speed * elapsedTime;

            if (this.Position.Y + this.width / 2 > this.sphere.Position.Y)
                newPosition.Y -= speed * elapsedTime;

            this.Position = newPosition;

            base.Update(gameTime);
        }

        public void IncreaseSpeed()
        {
            this.speed *= 1.05f;
        }
    }
}
