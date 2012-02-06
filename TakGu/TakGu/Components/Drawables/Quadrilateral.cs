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
    public class Quadrilateral : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private Texture2D texture;

        private VertexPositionNormalTexture[] vertices;
        private short[] indices;

        private Vector3 position;
        private Vector3 normal;
        private Vector3 up;

        float height;
        float width;

        float textureSize;

        private VertexBuffer vBuffer;
        private IndexBuffer iBuffer;

        private BasicEffect effect;

        private BoundingBox bounds;
        private Camera camera;

        /// <summary>
        /// Constructs the quadrilateral
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
        public Quadrilateral(Game game,Texture2D texture, Vector3 position, Vector3 normal, Vector3 up, float width, float height, float textureSize)
            : base(game)
        {
            if (normal == Vector3.Zero || up == Vector3.Zero)
                throw new System.ArgumentException("Up or normal can't be a zero vector!");

            this.texture = texture;
            
            if (normal.Length() != 1)
                normal.Normalize();

            this.position = position;
            this.normal = normal;
            this.up = up;

            this.height = height;
            this.width = width;

            this.textureSize = textureSize;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            this.camera = (Camera)this.Game.Services.GetService(typeof(Camera));

            base.Initialize();

            this.effect = new BasicEffect(GraphicsDevice);

            this.effect.TextureEnabled = true;
            this.effect.Texture = this.texture;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            this.FillVertices();

            this.vBuffer = new VertexBuffer(GraphicsDevice
                , VertexPositionNormalTexture.VertexDeclaration
                , this.vertices.Length
                , BufferUsage.WriteOnly
            );

            this.iBuffer = new IndexBuffer(GraphicsDevice
                , typeof(short)
                , this.indices.Length
                , BufferUsage.WriteOnly
            );

            this.CopyToBuffer();
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
            this.effect.Projection = this.camera.Projection;
            this.effect.View = this.camera.View;

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            foreach (EffectPass pass in this.effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                GraphicsDevice.Indices = this.iBuffer;

                GraphicsDevice.SetVertexBuffer(this.vBuffer);
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList
                    , 0
                    , 0
                    , this.vertices.Length
                    , 0
                    , this.indices.Length
                );
            }

            GraphicsDevice.SetVertexBuffer(null);
            GraphicsDevice.Indices = null;

            base.Draw(gameTime);
        }
        /// <summary>
        /// Fill vertices and indices.
        /// </summary>
        protected void FillVertices()
        {
            Vector3 right = Vector3.Cross(this.up, this.normal);

            Vector3 bottomLeft = this.position;
            Vector3 topLeft = this.position + (this.up * this.height);
            Vector3 bottomRight = this.position + (right * this.width);
            Vector3 topRight = topLeft + (right * this.width);

            this.bounds = BoundingBox.CreateFromPoints(new List<Vector3>{bottomLeft
                , topLeft
                , bottomRight
                , topRight
            });

            //4 unique vertices for 2 triangles
            this.vertices = new VertexPositionNormalTexture[4];

            for (int i = 0; i < this.vertices.Length; i++)
            {
                this.vertices[i].Normal = this.normal;
            }

            this.vertices[0].Position = bottomLeft;
            this.vertices[0].TextureCoordinate.X = 0;
            this.vertices[0].TextureCoordinate.Y = this.height / this.textureSize;

            this.vertices[1].Position = topLeft;
            this.vertices[1].TextureCoordinate.X = 0;
            this.vertices[1].TextureCoordinate.Y = 0;

            this.vertices[2].Position = bottomRight;
            this.vertices[2].TextureCoordinate.X = this.width / this.textureSize;
            this.vertices[2].TextureCoordinate.Y = this.vertices[0].TextureCoordinate.Y;

            this.vertices[3].Position = topRight;
            this.vertices[3].TextureCoordinate.X = this.vertices[2].TextureCoordinate.X;
            this.vertices[3].TextureCoordinate.Y = 0;

            //6 points for 2 triangles
            this.indices = new short[6];

            this.indices[0] = 0;
            this.indices[1] = 1;
            this.indices[2] = 3;

            this.indices[3] = 0;
            this.indices[4] = 3;
            this.indices[5] = 2;
        }

        /// <summary>
        /// Copy vertices and indices to buffer.
        /// </summary>
        protected void CopyToBuffer()
        {
            this.vBuffer.SetData<VertexPositionNormalTexture>(this.vertices);
            this.iBuffer.SetData(this.indices);
        }

        public BoundingBox Bounds 
        {
            get { return this.bounds; }
        }

        public Vector3 Position
        {
            get { return this.position; }
            set { position = value; }
        }

        public BasicEffect Effect {
            get { return this.effect; }
            set { this.effect = value; }
        }

        public Vector3 Normal
        {
            get { return this.normal; }
        }
    }
}