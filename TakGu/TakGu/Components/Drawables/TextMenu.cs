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
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class TextMenu : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private SpriteFont font;
        private Vector2 position; 

        private int selectedIndex = 0;
        private List<MenuItem> menuItems;

        private KeyboardState oldKState;
        private GamePadState oldGState;

        private SoundEffect sound;

        public TextMenu(Game game, SpriteFont font, SoundEffect sound)
            : base(game)
        {
            this.font = font;
            this.sound = sound;
            this.menuItems = new List<MenuItem>();
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            this.position = new Vector2();

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            #region keybinds
            KeyboardState keyState = Keyboard.GetState();

            InputUtility.ExecuteOnToggle(
                new List<Keys>(){
                    Keys.K
                    , Keys.Up
                }
                , this.oldKState
                , keyState
                , this.GoUp
            );

            InputUtility.ExecuteOnToggle(
                new List<Keys>(){
                    Keys.J
                    , Keys.Down
                }
                , this.oldKState
                , keyState
                , this.GoDown
            );

            InputUtility.ExecuteOnToggle(
                new List<Keys>(){
                    Keys.Space
                    , Keys.Enter
                }
                , this.oldKState
                , keyState
                , this.menuItems[this.selectedIndex].ExecuteSelected
            );

            this.oldKState = keyState;
            #endregion

            #region buttonbinds
            GamePadState padState = GamePad.GetState(PlayerIndex.One);

            InputUtility.ExecuteOnToggle(
                Buttons.DPadUp
                , this.oldGState
                , oldGState
                , this.GoUp
            );

            InputUtility.ExecuteOnToggle(
                Buttons.DPadDown
                , this.oldGState
                , padState
                , this.GoDown
            );

            InputUtility.ExecuteOnToggle(
                new List<Buttons>(){
                    Buttons.A
                    , Buttons.Start
                }
                , this.oldGState
                , padState
                , this.menuItems[this.selectedIndex].ExecuteSelected
            );

            this.oldGState = padState;
            #endregion

            base.Update(gameTime);
        }

        private void GoDown()
        {
            this.sound.Play();
            this.selectedIndex++;
            if (this.selectedIndex == this.menuItems.Count)
                this.selectedIndex = 0;
        }

        private void GoUp()
        {
            this.sound.Play();
            this.selectedIndex--;
            if (this.selectedIndex < 0)
                this.selectedIndex = this.menuItems.Count - 1;
        }

        public override void Draw(GameTime gameTime)
        {
            Color color;
            SpriteBatch sBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));

            for (int i = 0; i < menuItems.Count; i++) 
            {

                if (i == this.selectedIndex) 
                    color = Color.WhiteSmoke;
                else 
                    color = new Color(50, 50, 50, 255);

                Vector2 fontDimensions = this.font.MeasureString(
                    this.menuItems[i].Text
                );

                Vector2 fontPosition = new Vector2(this.position.X - fontDimensions.X / 2
                    , this.position.Y + fontDimensions.Y / 2 +  i * (fontDimensions.Y)
                );

                sBatch.DrawString(font
                    , menuItems[i].Text
                    , fontPosition
                    , color
                );
            }

            base.Draw(gameTime);
        }

        public Vector2 Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        public List<MenuItem> MenuItems
        {
            get { return this.menuItems; }
            set { this.menuItems = value; }
        }

        public void Show()
        {
            this.Visible = true;
            this.Enabled = true;
        }

        public void Hide()
        {
            this.Visible = false;
            this.Enabled = false;
        }

    }
}