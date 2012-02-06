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
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class DisposableDrawableGameComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private DrawableGameComponent component; 

        private float timeToLive; 
        private float time; 

        public DisposableDrawableGameComponent(Game game, DrawableGameComponent component, float timeToLive)
            : base(game)
        {
            this.component = component;

            this.timeToLive = timeToLive;
            this.time = System.Environment.TickCount;
        }
        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            this.component.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (this.timeToLive <= System.Environment.TickCount - this.time)
            {
                this.component.Dispose();
                this.Dispose();
            }

            this.component.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game component should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            this.component.Draw(gameTime);
        }
    }
}
