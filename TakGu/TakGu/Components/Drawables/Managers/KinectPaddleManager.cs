using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Kinect;

namespace TakGu
{
    public class KinectPaddleManager : PaddleManager
    {
        private SoundEffect sound;

        private float xBound;
        private float yBound;

        private float width;
        private float height;

        private float textureSize;

        private float speed;

        private Texture2D texture;
        private Texture2D textureActive;

        private Utility.Delegate_Paramless onActive;
        private KinectSensor kinectSensor;

        private KeyboardState oldState;
        /// <summary>
        /// Constructs the paddleManager object
        /// </summary>
        /// <param name="game">The base game.</param>
        public KinectPaddleManager(Game game, Texture2D texture, Texture2D textureActive, Vector3 position, Vector3 normal, Vector3 up, float width, float height, float textureSize, SoundEffect sound, float speed, float xBound, float yBound)
            : base(game)
        {
            this.sound = sound;

            this.xBound = xBound;
            this.yBound = yBound;

            this.width = width;
            this.height = height;

            this.textureSize = textureSize;

            this.speed = speed;

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

            //KinectSensor.KinectSensors.StatusChanged += new EventHandler<StatusChangedEventArgs>(KinectSensors_StatusChanged);
            DiscoverKinectSensor();

        }

        private void DiscoverKinectSensor()
        {
            foreach (KinectSensor sensor in KinectSensor.KinectSensors)
            {
                if (sensor.Status == KinectStatus.Connected)
                {
                    kinectSensor = sensor;
                    InitializeKinect();
                    break;
                }
            }
        }

        private void InitializeKinect()
        {
            kinectSensor.SkeletonStream.Enable();
            kinectSensor.SkeletonFrameReady +=new EventHandler<SkeletonFrameReadyEventArgs>(KinectPaddle_SkeletonFrameReady);

            try
            {
                kinectSensor.Start();
                kinectSensor.ElevationAngle = 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void KinectPaddle_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame frame = e.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    int skelAmount = 0;
                    Skeleton[] skelData = new Skeleton[frame.SkeletonArrayLength];
                    frame.CopySkeletonDataTo(skelData); 
                    foreach (Skeleton data in skelData)
                    {
                        if (SkeletonTrackingState.Tracked == data.TrackingState)
                        {
                            skelAmount++;
        
                            if (this.paddles.Count - 1 < skelAmount)
                                this.AddNewPerson();
        
                            SkeletonPoint joint = data.Joints[JointType.Spine].Position;
        
                            float scaleMax = 0.5f;
        
                            Vector3 newPosition = new Vector3(
                                this.Scale(this.xBound, scaleMax, joint.X)
                                , this.Scale(this.yBound, scaleMax / this.GraphicsDevice.Viewport.AspectRatio, joint.Y) - 1f
                                , 0
                            );
        
                            this.paddles[skelAmount].Position = newPosition;
                        }
                    }
        
                    if (this.paddles.Count > skelAmount + 1)
                        this.paddles.RemoveAt(this.paddles.Count - 1);
                }
            }
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            InputUtility.ExecuteOnToggle(
                Keys.Up
                , oldState
                , state
                , tiltKinectUp
            );

            InputUtility.ExecuteOnToggle(
                Keys.Down
                , oldState
                , state
                , tiltKinectDown
            );

            oldState = state;

            foreach (Paddle paddle in this.paddles)
            {
                if (paddle.GetType() == typeof(KinectPaddle))
                {
                   paddle.Activate(Keyboard.GetState().IsKeyDown(Keys.Space)
                       || Mouse.GetState().LeftButton == ButtonState.Pressed
                   );
                }
            }
            base.Update(gameTime);
        }

        private void tiltKinectUp()
        {
            if (kinectSensor != null)
                if (kinectSensor.Status == KinectStatus.Connected)
                    if(kinectSensor.ElevationAngle < kinectSensor.MaxElevationAngle -5)
                        try
                        {
                            kinectSensor.ElevationAngle += 5;
                        }
                        catch (Exception)
                        {
                            //ignore
                        }
        }

        private void tiltKinectDown()
        {
            if (kinectSensor != null)
                if (kinectSensor.Status == KinectStatus.Connected)
                    if(kinectSensor.ElevationAngle > kinectSensor.MinElevationAngle +5)
                        try
                        {
                            kinectSensor.ElevationAngle -= 5;
                        }
                        catch (Exception)
                        {
                            //ignore
                        }
        }

        /// <summary>
        /// This is called when the game component should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        private float Scale(float maxPixel, float maxSkeleton, float position)
        {
            return ((((maxPixel / maxSkeleton) / 2) * position) + (maxPixel / 2));
        }

        private void AddNewPerson()
        {
            Paddle paddle = new KinectPaddle(this.Game
                    , this.texture
                    , this.textureActive
                    , Vector3.Zero
                    , Vector3.Backward
                    , Vector3.Up
                    , this.width
                    , this.height
                    , this.textureSize
                    , this.sound
                    , this.speed
                    , this.xBound
                    , this.yBound
            );

            paddle.OnActive = this.onActive;

            paddle.Initialize();
            this.paddles.Add(paddle);
        }

        public void PutOnStandBy()
        {
            foreach (Paddle paddle in this.paddles)
            {
                if (paddle.GetType() == typeof(KinectPaddle))
                {
                    paddle.PutOnStandBy();
                }
            }
        }

        public Utility.Delegate_Paramless OnActive
        {
            set { this.onActive += value; } 
            get { return this.onActive; }
        }

        public KinectSensor sensor
        {
            set { this.kinectSensor = value; } 
            get { return this.kinectSensor; }
        }
    }
}
