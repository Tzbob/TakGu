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
    public abstract class Paddle : Quadrilateral
    {
        private SoundEffect sound;

        private float xBound;
        private float yBound;

        private float xCap;
        private float yCap;

        private float width;
        private float height;

        private Texture2D texture;
        private Texture2D textureActive;

        private Vector3 oldPosition;

        private PaddleState paddlestate;

        private Utility.Delegate_Paramless onActive;

        public enum Type
        {
            MOUSE
            , GAMEPAD
            , KINECT
            , WEBCAM
        }

        public enum PaddleState
        {
            STANDBY
            , PLAYING
        }

        public Paddle(Game game, Texture2D texture, Texture2D textureActive, Vector3 position, Vector3 normal, Vector3 up, float width, float height, float textureSize, SoundEffect sound, float xBound, float yBound)
            : base(game, texture, position, normal, up, width, height, textureSize)
        {
            this.sound = sound;

            this.xBound = xBound;
            this.yBound = yBound;

            this.width = width;
            this.height = height;

            this.texture = texture;
            this.textureActive = textureActive;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            this.Effect.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            this.Effect.Alpha = 0.5f;

            this.xCap = this.xBound - this.width;
            this.yCap = this.yBound - this.height;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.KeepInBounds();

            this.Activate(
                Mouse.GetState().LeftButton == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Space)
                || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.A)
            );

            if (this.Position != this.oldPosition)
            {
                base.FillVertices();
                base.CopyToBuffer();
            }

            if (this.paddlestate != PaddleState.STANDBY)
                this.Effect.Texture = this.textureActive;
            else
                this.Effect.Texture = this.texture;
        }

        /// <summary>
        /// Handles the collision between the paddle and the sphere.
        /// </summary>
        public void HandleCollision(float elapsedTime, Sphere sphere)
        {
            if (this.paddlestate == PaddleState.PLAYING)
            {
                if (this.Bounds.Intersects(sphere.Bounds))
                {
                    // Removes sticky ball phenomenon
                    // by only allowing to bounce in the opposite direction
                    if (this.IsMovingAway(sphere))
                    {
                        sound.Play();

                        sphere.Direction = Vector3.Reflect(sphere.Direction, this.Normal);

                        this.SetSphereImpactEffects(
                            elapsedTime
                            , sphere
                        );
                    }
                }
            }

            this.oldPosition = this.Position;
        }

        private bool IsMovingAway(Sphere sphere)
        {
            return sphere.Direction.Z < 0 && this.GetType() == typeof(ComputerPaddle)
                || sphere.Direction.Z > 0 && this.GetType() != typeof(ComputerPaddle);
        }

        /// <summary>
        /// Handles the effect on the sphere
        /// </summary>
        /// <param name="elapsedTime">The elapsedTime that has to be used to calculate the effect.</param>
        /// <param name="sphere">The sphere on which collisions should be detected.</param>
        private void SetSphereImpactEffects(float elapsedTime, Sphere sphere)
        {
            float deltaX = this.oldPosition.X - this.Position.X;
            float deltaY = this.oldPosition.Y - this.Position.Y;
            deltaX = -deltaX;

            sphere.Curving = Vector3.Left * deltaX + Vector3.Up * deltaY;
            sphere.Curving *= 750 * elapsedTime;
            sphere.RotationYdelta = MathHelper.Pi / 20 * Math.Sign(deltaX);
            sphere.RotationXdelta = MathHelper.Pi / 20 * Math.Sign(deltaY);
        }

        /// <summary>
        /// Makes sure that the paddle is within bounds.
        /// </summary>
        private void KeepInBounds()
        {
            Vector3 position = this.Position;

            if (position.X < 0)
                position.X = 0;

            if (position.X > this.xCap)
                position.X = this.xCap;

            if (position.Y < 0)
                position.Y = 0;

            if (position.Y > this.yCap)
                position.Y = this.yCap;

            this.Position = position;
        }

        public void Activate(bool start)
        {
            if (start && this.paddlestate == PaddleState.STANDBY)
            {
                this.paddlestate = PaddleState.PLAYING;
                this.onActive();
            }
        }

        public void PutOnStandBy()
        {
            this.paddlestate = PaddleState.STANDBY;
        }

        protected PaddleState State
        {
            get { return this.paddlestate; }
            set { this.paddlestate = value; }
        }

        public Utility.Delegate_Paramless OnActive
        {
            get { return this.onActive; }
            set { this.onActive = value; }
        }
    }
}
