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
    class MousePaddle : Paddle
    {
        private float sensitivity;
        private Point center;

        public MousePaddle(Game game, Texture2D texture, Texture2D textureActive, Vector3 position, Vector3 normal, Vector3 up, float width, float height, float textureSize, float sensitivity, SoundEffect sound, float xBound, float yBound)
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
            MouseState mstate = Mouse.GetState();

            float elapsedTime = Utility.ElapsedTime(gameTime);

            float deltaX = mstate.X - this.center.X;
            float deltaY = mstate.Y - this.center.Y;

            if (this.HasChange(deltaX, deltaY))
            {
                Vector3 position = new Vector3(
                    this.CalculateXposition(elapsedTime, deltaX)
                    , this.CalculateYposition(elapsedTime, deltaY)
                    , this.Position.Z
                );

                this.Position = position;

                Mouse.SetPosition(center.X, center.Y);
            }

            base.Update(gameTime);
        }

        private bool HasChange(float deltaX, float deltaY)
        {
            return deltaX != 0 || deltaY != 0;
        }

        private float CalculateYposition(float elapsedTime, float deltaY)
        {
            return -deltaY * elapsedTime * this.sensitivity + this.Position.Y;
        }

        private float CalculateXposition(float elapsedTime, float deltaX)
        {
            return deltaX * elapsedTime * this.sensitivity + this.Position.X;
        }
    }
}
