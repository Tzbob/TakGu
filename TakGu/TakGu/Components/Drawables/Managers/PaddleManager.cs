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


namespace TakGu
{
    /// <summary>
    /// This is a game component that implements IDrawable and SideManager.
    /// </summary>
    public class PaddleManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        protected List<Paddle> paddles;

        /// <summary>
        /// Constructs the paddleManager object
        /// </summary>
        /// <param name="game">The base game.</param>
        /// <param name="width">The width of the paddleManager.</param>
        /// <param name="height">The height of the paddleManager.</param>
        /// <param name="depth">The depth of the paddleManager.</param>
        /// <param name="textureSize">The size of the textures on the paddleManager.</param>
        /// <param name="wallTexture">The texture that should be used to draw the quad.</param>
        /// <param name="floorTexture">The texture that should be used to draw the quad.</param>
        public PaddleManager(Game game)
            : base(game)
        {
            this.paddles = new List<Paddle>();
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            foreach (Paddle paddle in this.paddles)
                paddle.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            foreach (Paddle paddle in this.paddles)
                paddle.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game component should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicWrap;

            foreach (Paddle paddle in this.paddles)
                paddle.Draw(gameTime);

            base.Draw(gameTime);
        }

        /// <summary>
        /// Checks for collision with it's paddles
        /// </summary>
        /// <param name="bounds">The bounds that should be checked for collision.</param>
        /// <returns>The normal of the colliding plane.</returns>
        public void HandleCollisions(float elapsedTime, Sphere sphere)
        {
            foreach (Paddle paddle in this.paddles)
                paddle.HandleCollision(elapsedTime, sphere);
        }

        public void AddPaddle(Paddle paddle)
        {
            this.paddles.Add(paddle);
        }

        public static Paddle CreatePaddle(Game game
            , Paddle.Type type
            , Texture2D paddleTexture
            , Texture2D paddleActiveTexture
            , SoundEffect paddleSound
            , float width
            , float height
            )
        {
            float sensitivity = 2.5f;
            float halfWidth = width /2;
            float halfHeight = height /2;
            float paddleWidth = width / 10;
            float halfPaddleWidth = paddleWidth / 2;

            switch (type)
            {
                case Paddle.Type.MOUSE:
                    return new MousePaddle(game
                        , paddleTexture
                        , paddleActiveTexture
                        , new Vector3(halfWidth - halfPaddleWidth
                            , halfHeight - halfPaddleWidth
                            , 0f
                        )
                        , Vector3.Backward
                        , Vector3.Up
                        , paddleWidth
                        , paddleWidth
                        , paddleWidth
                        , sensitivity
                        , paddleSound
                        , width
                        , height
                    );

                case Paddle.Type.GAMEPAD:
                    return new GamepadPaddle(game
                        , paddleTexture
                        , paddleActiveTexture
                        , new Vector3(halfWidth - halfPaddleWidth
                            , halfHeight - halfPaddleWidth
                            , 0f
                        )
                        , Vector3.Backward
                        , Vector3.Up
                        , paddleWidth
                        , paddleWidth
                        , paddleWidth
                        , sensitivity * 5
                        , paddleSound
                        , width
                        , height
                    );

                case Paddle.Type.WEBCAM:
                    return new WebcamPaddle(game
                        , paddleTexture
                        , paddleActiveTexture
                        , new Vector3(halfWidth - halfPaddleWidth
                            , halfHeight - halfPaddleWidth
                            , 0f
                        )
                        , Vector3.Backward
                        , Vector3.Up
                        , paddleWidth
                        , paddleWidth
                        , paddleWidth
                        , paddleSound
                        , sensitivity
                        , width
                        , height
                    );

                default:
                    throw new ArgumentException();
            }
        }
    }
}