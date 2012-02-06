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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;


namespace TakGu
{
    /// <summary>
    /// This is a game component that implements IDrawable.
    /// </summary>
    public class GameScene : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private List<GameComponent> components;
        private bool isInitialized;

        public GameScene(Game game)
            : base(game)
        {
            this.Visible = false;
            this.Enabled = false;
            this.components = new List<GameComponent>();
            this.isInitialized = false;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            this.isInitialized = true;
            base.Initialize();

            for (int i = 0; i < this.components.Count; i++)
            {
                components[i].Initialize();
            }
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < this.components.Count; i++)
            {
               if (components[i].Enabled)
                   components[i].Update(gameTime);
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game component should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {   
            for (int i = 0; i < this.components.Count; i++)
            {
                if (components[i] is DrawableGameComponent)
                {
                    DrawableGameComponent drawComponent = (DrawableGameComponent)components[i];

                    if (drawComponent.Visible)
                        drawComponent.Draw(gameTime);
                }
            }

            base.Draw(gameTime);
        }

        public virtual void Show()
        {
            this.Visible = true;
            this.Play();
        }

        public virtual void Hide()
        {
            this.Visible = false;
            this.Pause();
        }

        public virtual void Pause()
        {
            this.Enabled = false;
        }

        public virtual void Play()
        {
            this.Enabled = true;
        }

        public void TogglePause()
        {
            if (this.Enabled)
                this.Pause();
            else
                this.Play();
        }

        virtual public void Add(GameComponent component)
        {
            this.components.Add(component);
        }

        virtual public void Remove(GameComponent component)
        {
            if(this.components.IndexOf(component) > -1)
                this.components.Remove(component);
        }

        public bool IsInitialized { get { return this.isInitialized; } }
    }
}