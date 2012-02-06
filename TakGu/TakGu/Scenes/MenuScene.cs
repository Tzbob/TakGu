using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using Nini.Config;
using Microsoft.Xna.Framework.Media;

namespace TakGu
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class MenuScene : GameScene
    {
        private Dictionary<string, TextMenu> menus;

        public MenuScene(Game1 game
            , SpriteFont font
            , SoundEffect uiSound
            )
            : base(game)
        {
            this.menus = new Dictionary<string, TextMenu>();
            this.menus["main"] = this.MakeStandardMenu(font, uiSound);
            this.menus["settings"] = this.MakeStandardMenu(font, uiSound);
            this.menus["settings"].Hide();
            this.menus["start"] = this.MakeStandardMenu(font, uiSound);
            this.menus["start"].Hide();
        }
         
        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            this.Add(new AudioVisualizer(this.Game));

            base.Initialize();

            this.AddMenus();
            this.ResetMenuPosition();
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
            base.Draw(gameTime);
        }

        public void ResetMenuPosition()
        {
            Point center = this.Game.GraphicsDevice.Viewport.Bounds.Center;

            foreach (TextMenu menu in this.menus.Values)
            {
                menu.Position = new Vector2(
                    center.X
                    , center.Y
                );
            }
        }

        private void AddMenus()
        {
            foreach (TextMenu menu in this.menus.Values)
            {
                this.Add(menu);
            }
        }

        public TextMenu MakeStandardMenu(SpriteFont font, SoundEffect sound)
        {
            return new TextMenu(
                this.Game
                , font
                , sound
            );
        }

        private void ShowMenu(string key)
        {
            foreach (TextMenu menu in this.menus.Values)
            {
                menu.Hide();
            }

            this.menus[key].Show();
        }

        public void ShowMainMenu()
        {
            this.ShowMenu("main");
        }

        public void ShowSettingsMenu()
        {
            this.ShowMenu("settings");
        }

        public void ShowStartMenu()
        {
            this.ShowMenu("start");
        }

        private void AddItemsToMenu(string key, IEnumerable<MenuItem> items)
        {
            this.menus[key].MenuItems.AddRange(items);
        }
        
        public void AddItemsToMainMenu(IEnumerable<MenuItem> items)
        {
            this.AddItemsToMenu("main", items);
        }

        public void AddItemsToSettingsMenu(IEnumerable<MenuItem> items)
        {
            this.AddItemsToMenu("settings", items);
        }

        public void AddItemsToStartMenu(IEnumerable<MenuItem> items)
        {
            this.AddItemsToMenu("start", items);
        }

        private void UpdateTextOnMenuItem(string key, int index, string newText)
        {
            this.menus["settings"].MenuItems[index].Text = newText;
        }

        public void UpdateTextOnSettingsMenuItem(int itemIndex, string newText)
        {
            this.UpdateTextOnMenuItem("settings", itemIndex, newText);
        }
    }
}
