using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nini.Config;


namespace TakGu
{
    /// <summary>
    /// This is a game component that implements IDrawable.
    /// </summary>
    public class ActionScene : GameScene
    {
        private Sphere sphere;
        private PaddleManager paddleManager;

        private IniConfigSource configSource;

        private SpriteFont font;

        private Utility.Delegate_Paramless onGameEnd;

        private Texture2D paddleTexture;
        private Texture2D paddleActiveTexture;

        private SoundEffect paddleSound;
        private Model sphereModel;

        private float width;
        private float height;
        private float depth;

        private float paddleWidth;
        private float sphereScale;

        private SubmitPanel submitPanel;
        private Score score;

        private SoundEffect hitSound;
        private SideManager sideManager;

        private bool isTakingInput;

        public ActionScene(Game game
            , IniConfigSource configSource
            , SpriteFont font
            )
            : base(game)
        {
            this.configSource = configSource;
            this.font = font;
        }

        public override void Initialize()
        {
            //settings
            this.width = 10f;
            this.height = width / this.Game.GraphicsDevice.Viewport.AspectRatio;
            this.depth = 15f;
            float sideManagerTextureSize = 1f;

            this.sphereScale = 0.5f;
            this.paddleWidth = 1f;

            float skyBoxWidth = 200f;
            float halfSkyBoxWidth = skyBoxWidth / 2;

            Vector3 skyBoxPosition = new Vector3(
                -halfSkyBoxWidth + this.width / 2
                , -halfSkyBoxWidth + this.height / 2
                , halfSkyBoxWidth - depth / 2
            );

            string textureDetail = this.configSource.Configs["Graphics"].GetString("texture_detail");

            Texture2D wallTexture = this.Game.Content.Load<Texture2D>("texture/wall");
            Texture2D floorTexture = this.Game.Content.Load<Texture2D>("texture/floor");

            this.paddleTexture = this.Game.Content.Load<Texture2D>(
                String.Format("texture/{0}/paddle", textureDetail));
            this.paddleActiveTexture = this.Game.Content.Load<Texture2D>(
                String.Format("texture/{0}/paddleclicked", textureDetail));

            Texture2D spaceBoxLeft = this.Game.Content.Load<Texture2D>(
                String.Format("texture/{0}/interstellar_lf", textureDetail));
            Texture2D spaceBoxFront = this.Game.Content.Load<Texture2D>(
                String.Format("texture/{0}/interstellar_ft", textureDetail));
            Texture2D spaceBoxRight = this.Game.Content.Load<Texture2D>(
                String.Format("texture/{0}/interstellar_rt", textureDetail));
            Texture2D spaceBoxDown = this.Game.Content.Load<Texture2D>(
                String.Format("texture/{0}/interstellar_dn", textureDetail));
            Texture2D spaceBoxUp = this.Game.Content.Load<Texture2D>(
                String.Format("texture/{0}/interstellar_up", textureDetail));

            this.hitSound = this.Game.Content.Load<SoundEffect>("audio/shot");
            this.paddleSound = hitSound;

            this.sphereModel = this.Game.Content.Load<Model>(
                String.Format(
                    "model/{0}/sphere"
                    , this.configSource.Configs["Graphics"].GetString("model_detail")
                )
            );

            this.Add(new SkyBox(this.Game
                ,skyBoxPosition
                ,skyBoxWidth
                ,spaceBoxLeft
                ,spaceBoxFront
                ,spaceBoxRight
                ,spaceBoxDown
                ,spaceBoxUp
                )
            );

            Camera camera = new Camera(
                this.Game
                , width
                , height
                , depth
            );


            this.SetUp();

            this.sideManager = new SideManager(
                this.Game
                , width
                , height
                , depth
                , sideManagerTextureSize
                , wallTexture
                , floorTexture
            );
            this.Add(sideManager);
            this.sphere.CalculateCollisionNormal += this.sideManager.CollisionNormal;

            this.Add(camera);
            this.Game.Services.AddService(typeof(Camera), camera);


            base.Initialize();
        }

        public void SetUp()
        {
            this.isTakingInput = false;

            if (this.paddleManager != null)
            {
                this.Remove(paddleManager);
                this.paddleManager.Dispose();
            }

            if (this.submitPanel != null)
            {
                this.Remove(this.submitPanel);
                this.submitPanel.Dispose();
            }

            if (this.score != null)
            {
                this.Remove(this.score);
                this.score.Dispose();
            }

            if (this.sphere != null)
            {
                this.Remove(this.sphere);
                this.sphere.Dispose();
            }

            this.paddleManager = null;

            this.sphere = new Sphere(this.Game
                , sphereModel
                , sphereScale
                , hitSound
                , width
                , height
                , depth
            );

            this.sphere.Initialize();

            if(this.sideManager != null)
                this.sphere.CalculateCollisionNormal += this.sideManager.CollisionNormal;

            this.Game.Services.RemoveService(typeof(Sphere));
            this.Game.Services.AddService(typeof(Sphere), this.sphere);

            this.Add(sphere);

            if (this.GetGameType() == Paddle.Type.KINECT)
            {
                KinectPaddleManager kinectManager = new KinectPaddleManager(
                    this.Game
                    , paddleTexture
                    , paddleActiveTexture
                    , new Vector3(
                        this.width/2 - this.paddleWidth/2
                        , this.height/2 - this.paddleWidth/2
                        , 0f
                    )
                    , Vector3.Backward
                    , Vector3.Up
                    , paddleWidth
                    , paddleWidth
                    , paddleWidth
                    , paddleSound
                    , 2.5f
                    , width
                    , height
                );

                kinectManager.OnActive += this.PushSphere;
                this.sphere.OnBallPassesDepth += kinectManager.PutOnStandBy;
                this.sphere.OnBallPassesZero += kinectManager.PutOnStandBy;

                this.paddleManager = kinectManager;
            }
            else
            {
                this.paddleManager = new PaddleManager(this.Game);
                Paddle paddle = PaddleManager.CreatePaddle(
                    this.Game
                    , this.GetGameType()
                    , paddleTexture
                    , paddleActiveTexture
                    , paddleSound
                    , width
                    , height
                );

                paddle.OnActive += this.PushSphere;
                this.sphere.OnBallPassesDepth += paddle.PutOnStandBy;
                this.sphere.OnBallPassesZero += paddle.PutOnStandBy;

                this.paddleManager.AddPaddle(paddle);
            }

            ComputerPaddle computerPaddle = new ComputerPaddle(this.Game
                , paddleTexture
                , paddleActiveTexture
                , new Vector3(
                    this.width / 2 - this.paddleWidth / 2
                    , this.height / 2 - this.paddleWidth / 2
                    , -this.depth
                )
                , Vector3.Backward
                , Vector3.Up
                , paddleWidth
                , paddleWidth
                , paddleWidth
                , paddleSound
                , 3
                , width
                , height
            );

            this.sphere.OnBallPassesDepth += computerPaddle.IncreaseSpeed;

            this.paddleManager.AddPaddle(computerPaddle);

            this.Add(paddleManager);
            this.paddleManager.Initialize();

            this.score = new Score(this.Game, this.font);
            this.Add(this.score);
            this.score.Initialize();

            this.sphere.OnOneNonIdleTick += this.score.IncreaseScoreForStayingAlive;
            this.sphere.OnBallPassesZero += this.score.DecreaseLife;
            this.sphere.OnBallPassesDepth += this.score.LevelUp;

            this.score.OnNoMoreLives += this.LockGameAndShowSubmittingPanel;
        }

        private Paddle.Type GetGameType()
        {
            return (Paddle.Type)Enum.Parse(typeof(Paddle.Type)
                , this.configSource.Configs["GameType"].Get("type")
            );
        }

        private void LockGameAndShowSubmittingPanel(float score)
        {
            WebcamPaddle.RequestStop();

            this.submitPanel = new SubmitPanel(
                this.Game
                , this.font
                , score
                , this.configSource
            );

            this.submitPanel.Initialize();

            this.submitPanel.OnSubmit += this.onGameEnd;

            this.Add(this.submitPanel);

            this.sphere.Enabled = false;
            this.isTakingInput = true;
        }
        
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            this.paddleManager.HandleCollisions(Utility.ElapsedTime(gameTime), this.sphere);

            base.Update(gameTime);
        }

        private void PushSphere()
        {
            this.sphere.Direction = 15f * Vector3.Backward;
            this.sphere.AllowMovement();
        }

        public Utility.Delegate_Paramless OnGameEnd
        {
            get { return onGameEnd; }
            set { onGameEnd = value; }
        }

        public bool IsTakingInput
        {
            get { return isTakingInput; }
            set { isTakingInput = value; }
        }

    }
}
