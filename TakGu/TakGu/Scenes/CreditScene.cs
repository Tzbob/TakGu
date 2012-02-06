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
    public class CreditScene : TextScene
    {
        private SpriteFont font;

        public CreditScene(Game game, SpriteFont font)
            : base(game, font)
        {
            this.font = font;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            List<string> credits = new List<string>();
            credits.Add("Tak-Gu - Credits");
            credits.Add("");
            credits.Add("Development: Bob Reynders");
            credits.Add("");
            credits.Add("Sphere model:  Alekseev Aleksandr(Turbosquid)");
            credits.Add("Wall textures: illcircuus(deviantArt)");
            credits.Add("Menu beep:     ecrivain(OpenGameArt)");
            credits.Add("Paddle beep:   free-sound-effects-science-fiction(AudioMicro)");

            credits.Add("     You can find the links to these people on");
            credits.Add("     http://sourcetumble.appspot.com/projects/takgu");

            this.Add(new MultilineText(
                this.Game
                , font
                , credits
                , new Vector2(20f, 100f)
                ,false
            ));

            base.Initialize();

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
    }
}