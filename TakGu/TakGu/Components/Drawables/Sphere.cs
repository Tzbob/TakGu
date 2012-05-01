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
    /// This is a game component that implements IDrawable
    /// </summary>
    public class Sphere : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private Model model;

        private Vector3 initialPosition;
        private Vector3 position;
        private Vector3 direction;
        private Vector3 curving;

        private float rotationX;
        private float rotationXdelta;
        private float rotationY;
        private float rotationYdelta;
        private float scale;

        private BoundingSphere bounds;

        private Camera camera;

        private SoundEffect sound;

        private SphereState state;

        private Utility.Delegate_Paramless onBallPassesDepth;
        private Utility.Delegate_Paramless onBallPassesZero;
        private Utility.Delegate_Float onOneNonIdleTick;
        private Utility.Delegate_BoundingSphereReturnVector calculateCollisionNormal;

        private float levelStartTime;

        private float widthRoom;
        private float heightRoom;
        private float depthRoom;


        public enum SphereState
        {
            MOVING
            , COLLIDING
            , IDLE
        }
        
        public Sphere(Game game, Model model, float scale, SoundEffect sound, float widthRoom, float heightRoom, float depthRoom)
            : base(game)
        {
            this.model = model;

            this.scale = scale;

            this.sound = sound;

            this.state = SphereState.IDLE;

            this.widthRoom = widthRoom;
            this.heightRoom = heightRoom;
            this.depthRoom = depthRoom;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            this.camera = (Camera)this.Game.Services.GetService(typeof(Camera));

            this.CreateBounds();

            this.position = new Vector3(
                this.widthRoom / 2
                , this.heightRoom / 2
                , -this.bounds.Radius - this.depthRoom / 2
            );

            this.initialPosition = position;

            this.rotationX = 0;
            this.rotationY = 0;

        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (this.state == SphereState.MOVING)
            {
                this.direction += this.curving * elapsedTime;
                this.position += this.direction * elapsedTime;
            }

            if (this.state == SphereState.COLLIDING)
                this.state = SphereState.MOVING;

            if (this.state != SphereState.IDLE)
            {
                this.onOneNonIdleTick(elapsedTime);

                this.rotationX += this.rotationXdelta;
                this.rotationX %= MathHelper.TwoPi;

                this.rotationY += this.rotationYdelta;
                this.rotationY %= MathHelper.TwoPi;

                this.HandleCollision(elapsedTime);
                this.HandleWins();

            }

            base.Update(gameTime);

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect currentEffect in mesh.Effects)
                {
                    currentEffect.EnableDefaultLighting();

                    currentEffect.World = this.World;

                    currentEffect.View = this.camera.View;
                    currentEffect.Projection = this.camera.Projection;

                    currentEffect.FogEnabled = true;
                    currentEffect.FogColor = new Vector3(0, 0, 0);
                    currentEffect.FogStart = 0f;
                    currentEffect.FogEnd = 70f;
                }

                mesh.Draw();
            }

            base.Draw(gameTime);
        }

        /// <summary>
        /// Handles the collision between the sphere and the room bounds.
        /// </summary>
        private void HandleCollision(float elapsedTime)
        {
            Vector3 normal = this.calculateCollisionNormal(this.bounds);

            if (normal != Vector3.Zero)
                if ((this.direction + normal).Length() <= this.direction.Length())
                {
                    //Rollback position(makes sure the sphere doesn't overhit the bounds
                    this.position.Y -= this.direction.Y * elapsedTime;
                    this.position.X -= this.direction.X * elapsedTime;

                    this.direction = Vector3.Reflect(this.direction, normal);

                    this.sound.Play();

                    //Rollback has been done, need to skip a direction adjustment
                    this.state = SphereState.COLLIDING;
                }
        }

        /// <summary>
        /// Creates the boundingsphere from the model
        /// </summary>
        private void CreateBounds()
        {
            this.bounds = new BoundingSphere();

            foreach (ModelMesh mesh in this.model.Meshes)
            {
                if (this.bounds.Radius == 0)
                    this.bounds = mesh.BoundingSphere;
                else
                    this.bounds = BoundingSphere.CreateMerged(bounds, mesh.BoundingSphere);
            }

            this.bounds.Radius *= this.scale;
        }

        private void HandleWins()
        {
            if (this.position.Z > 0 || this.position.Z < -this.depthRoom)
            {
                if (this.position.Z > 0)
                    this.onBallPassesZero();
                else
                {
                    this.onBallPassesDepth();
                }

                this.ResetValues();
            }

            if(Utility.ElapsedTime(this.levelStartTime, System.Environment.TickCount) > 60000f)
            {
                this.onBallPassesZero();
                this.ResetValues();
            }
        }

        private void ResetValues()
        {
            this.state = SphereState.IDLE;
            this.position = this.initialPosition;
            this.direction = Vector3.Zero;
            this.curving = Vector3.Zero;
            this.rotationX = 0;
            this.rotationXdelta = 0;
            this.rotationY = 0;
            this.rotationYdelta = 0;
        }

        public void AllowMovement()
        {
            this.state = SphereState.MOVING;
            this.levelStartTime = System.Environment.TickCount;
        }

        private Matrix World
        {
            get
            {
                Vector3 rotAxis = Vector3.Right * this.rotationX
                    + Vector3.Up * this.rotationY / 2;

                if(rotAxis != Vector3.Zero)
                    rotAxis.Normalize();

                Matrix matrix = Matrix.CreateScale(this.scale);
                matrix *= Matrix.CreateFromAxisAngle(rotAxis, 
                    (Math.Abs(this.rotationX) + Math.Abs(this.rotationY)) / 2
                );
                matrix *= Matrix.CreateTranslation(position);

                return matrix;
            }
        }

        public BoundingSphere Bounds
        {
            get
            {
                bounds.Center = this.position;
                return bounds;
            }
        }

        public Vector3 Direction
        {
            get { return this.direction; }
            set { this.direction = value; }
        }

        public Vector3 Curving
        {
            get { return this.curving; }
            set { this.curving = value; }
        }

        public Vector3 Position { get { return this.position; } }

        public float RotationXdelta { set { this.rotationXdelta = value; } }
        public float RotationYdelta { set { this.rotationYdelta = value; } }

        public Utility.Delegate_Paramless OnBallPassesDepth
        {
            get { return this.onBallPassesDepth; }
            set { this.onBallPassesDepth = value; }
        }

        public Utility.Delegate_Paramless OnBallPassesZero
        {
            get { return this.onBallPassesZero; }
            set { this.onBallPassesZero = value; }
        }

        public Utility.Delegate_Float OnOneNonIdleTick
        {
            get { return this.onOneNonIdleTick; }
            set { this.onOneNonIdleTick = value; }
        }

        public Utility.Delegate_BoundingSphereReturnVector CalculateCollisionNormal
        {
            get { return this.calculateCollisionNormal; }
            set { this.calculateCollisionNormal = value; }
        }
    }
}
