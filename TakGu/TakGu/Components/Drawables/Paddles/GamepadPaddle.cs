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
    /// This is a game component that inherits Paddle
    /// </summary>
    class GamepadPaddle : Paddle
    {
        private float sensitivity;
        private Point center;

        /// <summary>
        /// Constructs the quadrilateral
        /// </summary>
        /// <param name="game">The base game.</param>
        /// <param name="texture">The texture that should be used to draw the quad.</param>
        /// <param name="texture">The texture that should be used to draw the quad when the mouse button is clicked.</param>
        /// <param name="position">The position of the leftbottom corner.</param>
        /// <param name="normal">The vector that defines the normal.</param>
        /// <param name="up">The vector that defines which way is up.</param>
        /// <param name="width">The width of the quad.</param>
        /// <param name="height">The height of the quad.</param>
        /// <param name="textureSize">
        ///     The scale at which the texture should be drawn
        ///     higher means less repetitions
        /// </param>
        public GamepadPaddle(Game game, Texture2D texture, Texture2D textureActive, Vector3 position, Vector3 normal, Vector3 up, float width, float height, float textureSize, float sensitivity, SoundEffect sound, float xBound, float yBound)
            : base(game, texture, textureActive, position, normal, up, width, height, textureSize, sound, xBound, yBound)
        {
            this.sensitivity = sensitivity;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            this.center = GraphicsDevice.Viewport.Bounds.Center;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            GamePadState padState = GamePad.GetState(PlayerIndex.One);

            float elapsedTime = Utility.ElapsedTime(gameTime);

            float deltaX = padState.ThumbSticks.Right.X;
            float deltaY = padState.ThumbSticks.Right.Y;

            if (deltaX != 0 || deltaY != 0)
            {
                Vector3 position = new Vector3(
                    deltaX * elapsedTime * this.sensitivity + this.Position.X
                    , deltaY * elapsedTime * this.sensitivity + this.Position.Y
                    , this.Position.Z
                );

                this.Position = position;

                Mouse.SetPosition(this.center.X, this.center.Y);
            }

            base.Update(gameTime);

        }
    }
}
