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
    public class SideManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        List<Quadrilateral> sides;

        private float width;
        private float height;
        private float depth;
        private float textureSize;

        private Texture2D wallTexture;
        private Texture2D floorTexture;

        /// <summary>
        /// Constructs the sideManager object
        /// </summary>
        public SideManager(Game game, float width, float height, float depth, float textureSize, Texture2D wallTexture, Texture2D floorTexture )
            : base(game)
        {
            this.width = width;
            this.height = height;
            this.depth = depth;
            this.textureSize = textureSize;

            this.wallTexture = wallTexture;
            this.floorTexture = floorTexture;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            foreach (Quadrilateral side in this.sides)
            {
                side.Initialize();

                side.Effect.EnableDefaultLighting();
                side.Effect.PreferPerPixelLighting = true;

                side.Effect.LightingEnabled = true;

                side.Effect.FogEnabled = true;
                side.Effect.FogColor = new Vector3(0, 0, 0);
                side.Effect.FogStart = 0f;
                side.Effect.FogEnd = 70f;
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            this.MakeSides();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            foreach (Quadrilateral side in this.sides)
            {
                side.Update(gameTime);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game component should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicWrap;
            foreach (Quadrilateral side in this.sides)
            {
                side.Effect.Alpha = 0.3f;
                side.Effect.EnableDefaultLighting();

                side.Draw(gameTime);
            }

            base.Draw(gameTime);
        }

        /// <summary>
        /// Creates the sides of the sideManager.
        /// </summary>
        private void MakeSides()
        {
            this.sides = new List<Quadrilateral>();

            //Left side
            this.sides.Add(
                new Quadrilateral(base.Game
                    , this.wallTexture
                    , Vector3.Zero
                    , Vector3.Right
                    , Vector3.Up
                    , this.depth
                    , this.height
                    , this.textureSize
                )
            );

            //Right side
            this.sides.Add(
                new Quadrilateral(base.Game
                    , this.wallTexture
                    , new Vector3(this.width, 0, -this.depth)
                    , Vector3.Left
                    , Vector3.Up
                    , this.depth
                    , this.height
                    , this.textureSize
                )
            );

            //Up side
            this.sides.Add(
                new Quadrilateral(base.Game
                    , this.wallTexture
                    , new Vector3(0, this.height, -this.depth)
                    , Vector3.Down
                    , Vector3.Backward
                    , this.width
                    , this.depth
                    , this.textureSize
                )
            );

            //down side
            this.sides.Add(
                new Quadrilateral(base.Game
                    , this.floorTexture
                    , new Vector3(0, 0, 0)
                    , Vector3.Up
                    , Vector3.Forward
                    , this.width
                    , this.depth
                    , this.textureSize
                )
            );
        }

        public float Width { get {return this.width;} }
        public float Height { get {return this.height;} }
        public float Depth { get {return this.depth;} }

        /// <summary>
        /// Checks for collision with it's sides
        /// </summary>
        /// <param name="bounds">The bounds that should be checked for collision.</param>
        /// <returns>The normal of the colliding plane.</returns>
        public Vector3 CollisionNormal(BoundingSphere bounds)
        {
            foreach (Quadrilateral quad in this.sides)
                if (quad.Bounds.Intersects(bounds))
                    return quad.Normal;

            return Vector3.Zero;
        }
    }
}
