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
using Emgu.CV;
using System.Diagnostics;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Threading;

namespace TakGu
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Camera : Microsoft.Xna.Framework.GameComponent
    {
        private float width;
        private float height;
        private float depth;

        private Vector3 cameraPosition;
        private Vector3 cameraTarget;

        private Matrix projection;
        private Matrix view;

        /// <summary>
        /// Constructs the quadrilateral
        /// </summary>
        /// <param name="game">The base game.</param>
        /// <param name="width">The width of the view.</param>
        /// <param name="height">The height of the view.</param>
        /// <param name="depth">The depth of the view.</param>
        /// <param name="haarCascade">The location of the haar.</param>
        public Camera(Game game, float width, float height, float depth)
            : base(game)
        {
            this.width = width;
            this.height = height;
            this.depth = depth;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            float nearPlane = 1f;
            float farPlane = 10000f;

            this.cameraPosition = new Vector3(
                this.width/2
                , this.height/2
                , 3.25f
            );

            this.cameraTarget = this.cameraPosition + Vector3.Forward * this.depth;

            float aspectRatio = this.Game.GraphicsDevice.Viewport.AspectRatio;

            this.projection = Matrix.CreatePerspectiveFieldOfView(
                  2 * (float)Math.Atan(this.width / 2 / 3f / aspectRatio)
                , aspectRatio
                , nearPlane
                , farPlane
            );
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            this.view = Matrix.CreateLookAt(this.cameraPosition
                , this.cameraTarget
                , Vector3.Up
            );

            base.Update(gameTime);
        }

        public Matrix Projection
        {
            get { return this.projection; }
        }

        public Matrix View
        {
            get { return this.view; }
        }
    }
}
