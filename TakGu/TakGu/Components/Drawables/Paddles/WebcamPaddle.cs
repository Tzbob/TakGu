using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Research.Kinect.Nui;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Threading;

namespace TakGu
{
    /// <summary>
    /// This is a game component that inherits Quadrilateral.
    /// </summary>
    public class WebcamPaddle : Paddle
    {
        private float width;
        private float height;
        private float speed;

        private float xBound;
        private float yBound;

        private static volatile bool shouldStop = false;

        /// <summary>
        /// Constructs the slide object
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
        /// <param name="speed">The speed of the computer.</param>
        public WebcamPaddle(Game game, Texture2D texture, Texture2D textureActive, Vector3 position, Vector3 normal, Vector3 up, float width, float height, float textureSize, SoundEffect sound, float speed, float xBound, float yBound)
            : base(game, texture, textureActive, position, normal, up, width, height, textureSize, sound, xBound, yBound)
        {
            this.width = width;
            this.height = height;
            this.speed = speed;

            this.xBound = xBound;
            this.yBound = yBound;
        }
        
        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            this.StartTracking();
            shouldStop = false;
        }

        private void StartTracking()
        {
            Thread t = new Thread(GetFacePosition);
            t.Start();
        }

        /// <summary>
        /// Tracks the face position and maps it to the camera position to create
        /// a fake 3d effect.
        /// </summary>
        private void GetFacePosition()
        {

            const string HAARCASCADE = "frontface.xml";
            Capture capture = new Capture(0);
            HaarCascade haar = new HaarCascade(HAARCASCADE);

            while (!shouldStop)
            {
                using (Image<Gray, byte> nextFrame = capture.QueryGrayFrame())
                {
                    if (nextFrame != null)
                    {
                        MCvAvgComp[] faces = haar.Detect(
                            nextFrame
                             , 1.1
                             , 3
                             , HAAR_DETECTION_TYPE.FIND_BIGGEST_OBJECT
                             , new System.Drawing.Size(nextFrame.Width / 8, nextFrame.Height / 8)
                         );

                        if (faces.Length > 0)
                        {
                            MCvAvgComp face = faces[0];

                            float relativeXPosition = ((float)face.rect.X + (float)face.rect.Width / 2) / (capture.Width);
                            float relativeYPosition = ((float)face.rect.Y + (float)face.rect.Height / 2) / (capture.Height);

                            //webcam = spiegelbeeld
                            Vector3 newPosition = new Vector3(
                                (1 - relativeXPosition) * this.xBound 
                                , (1 - relativeYPosition) * this.yBound 
                                , 0
                            );

                            this.Position = newPosition;
                        }
                    }
                }
            }
        }
 
        private float Scale(float maxPixel, float maxSkeleton, float position)
        {
            return ((((maxPixel / maxSkeleton) / 2) * position) + (maxPixel / 2));
        }
       
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            base.FillVertices();
            base.CopyToBuffer();
        }

        public static void RequestStop()
        {
            shouldStop = true;
        }
    }
}
