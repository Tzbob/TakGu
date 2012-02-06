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
using System.Diagnostics;


namespace TakGu
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class AudioVisualizer : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private VisualizationData visualData;

        private Model model;
        private Camera camera;

        private float rotation;
        private float rotationSpeed;

        private State state;

        public enum State
        {
            IDLE
            , SPEEDING
        }

        public AudioVisualizer(Game game)
            : base(game)
        {
            this.rotationSpeed = MathHelper.PiOver4 / 50;
            this.state = State.SPEEDING;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            this.camera = (Camera)this.Game.Services.GetService(typeof(Camera));
            this.visualData = new VisualizationData();

            this.model = this.Game.Content.Load<Model>("model/low/sphere");
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (MediaPlayer.State == MediaState.Playing)
                MediaPlayer.GetVisualizationData(this.visualData);

            this.rotation += this.rotationSpeed;

            if (this.rotation > MathHelper.TwoPi)
                this.state = State.IDLE;

            if(this.state == State.SPEEDING)
                this.rotationSpeed *= 1.005f;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game component should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            foreach (ModelMesh mesh in this.model.Meshes)
            {
                foreach (BasicEffect currentEffect in mesh.Effects)
                {
                    currentEffect.EnableDefaultLighting();

                    currentEffect.World = this.World;
                    currentEffect.View = Matrix.CreateLookAt(Vector3.Zero
                        , Vector3.Forward
                        , Vector3.Up
                    );

                    currentEffect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4
                        , GraphicsDevice.Viewport.AspectRatio
                        , 1f
                        , 100f
                    );
                }

                mesh.Draw();
            }

            base.Draw(gameTime);

        }

        private Matrix World
        {
            get
            {
                Matrix world = Matrix.CreateScale(this.visualData.Frequencies[0] * 1.5f);
                world *= Matrix.CreateRotationY(this.rotation);
                world *= Matrix.CreateTranslation(
                    new Vector3(
                        0f
                        , 1.5f
                        , -10f
                    )
                );

                return world;
            }
        }

    }
}
