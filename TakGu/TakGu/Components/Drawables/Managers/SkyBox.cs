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
    /// This is a game component that implements IDrawable.
    /// </summary>
    public class SkyBox : Microsoft.Xna.Framework.DrawableGameComponent{

        private List<Quadrilateral> sides;

        private Vector3 position;
        private float width;

        private Texture2D spaceBoxLeft;
        private Texture2D spaceBoxFront;
        private Texture2D spaceBoxRight;
        private Texture2D spaceBoxDown;
        private Texture2D spaceBoxUp;

        /// <summary>
        /// Constructs the skybox object
        /// </summary>
        /// <param name="game">The base game.</param>
        /// <param name="width">The width of the sideManager.</param>
        /// <param name="height">The height of the sideManager.</param>
        /// <param name="depth">The depth of the sideManager.</param>
        /// <param name="textureSize">The size of the textures on the sideManager.</param>
        /// <param name="wallTexture">The texture that should be used to draw the quad.</param>
        /// <param name="floorTexture">The texture that should be used to draw the quad.</param>
        public SkyBox(Game game
            , Vector3 position
            , float width
            , Texture2D spaceBoxLeft
            , Texture2D spaceBoxFront
            , Texture2D spaceBoxRight
            , Texture2D spaceBoxDown
            , Texture2D spaceBoxUp
        ) : base(game)
        {
            this.position = position;
            this.width = width;

            this.spaceBoxLeft = spaceBoxLeft;
            this.spaceBoxFront = spaceBoxFront;
            this.spaceBoxRight = spaceBoxRight;
            this.spaceBoxDown = spaceBoxDown;
            this.spaceBoxUp = spaceBoxUp;
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
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            MakeSides();
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
            GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicClamp;
            foreach (Quadrilateral side in this.sides)
            {
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
                    , this.spaceBoxLeft
                    , this.position
                    , Vector3.Right
                    , Vector3.Up
                    , this.width
                    , this.width
                    , this.width
                )
            );

            //Right side
            this.sides.Add(
                new Quadrilateral(base.Game
                    , this.spaceBoxRight
                    , this.position 
                        + Vector3.Right * this.width
                        + Vector3.Forward * this.width
                    , Vector3.Left
                    , Vector3.Up
                    , this.width
                    , this.width
                    , this.width
                )
            );

            //Front side
            this.sides.Add(
                new Quadrilateral(base.Game
                    , this.spaceBoxFront
                    , this.position 
                        + Vector3.Forward * this.width
                    , Vector3.Backward
                    , Vector3.Up
                    , this.width
                    , this.width
                    , this.width
                )
            );

            //Up side
            this.sides.Add(
                new Quadrilateral(base.Game
                    , this.spaceBoxUp
                    , this.position 
                        + Vector3.Forward * this.width
                        + Vector3.Up * this.width
                    , Vector3.Down
                    , Vector3.Backward
                    , this.width
                    , this.width
                    , this.width
                )
            );

            //Down side
            this.sides.Add(
                new Quadrilateral(base.Game
                    , this.spaceBoxDown
                    , this.position 
                    , Vector3.Up
                    , Vector3.Forward
                    , this.width
                    , this.width
                    , this.width
                )
            );
        }
    }
}
