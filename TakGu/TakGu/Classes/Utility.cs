using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Globalization;
using Microsoft.Xna.Framework.Graphics;

namespace TakGu
{

    public class Utility
    {
        public delegate void Delegate_Paramless();
        public delegate void Delegate_Float(float param);
        public delegate Vector3 Delegate_BoundingSphereReturnVector(BoundingSphere param);

        static public float ElapsedTime(GameTime gameTime)
        {
            return (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        static public float ElapsedTime(float oldtime, float currenttime)
        {
            return currenttime - oldtime;
        }

        static public Texture2D MakeOnePixTexture(GraphicsDevice graphicsDevice)
        {
            Texture2D onePixTexture = new Texture2D(
                graphicsDevice
                , 1
                , 1
                , false
                , SurfaceFormat.Color
            );

            onePixTexture.SetData<Color>(new Color[] { new Color(255, 0, 0, 125) });
            return onePixTexture;
        }

        public static void DrawCenteredText(SpriteBatch sBatch, string text, GraphicsDevice graphicsDevice, SpriteFont font)
        {

            Vector2 fontDimensions = font.MeasureString(
                text
            );

            Vector2 fontPosition = new Vector2(
                graphicsDevice.Viewport.Bounds.Center.X - fontDimensions.X / 2
                , graphicsDevice.Viewport.Bounds.Center.Y - fontDimensions.Y / 2
            );

            fontPosition.X -= 1;
            fontPosition.Y -= 1;
            sBatch.DrawString(
                font
                , text
                , fontPosition
                , new Color(0, 0, 0, 125)
            );

            fontPosition.X += 1;
            fontPosition.Y += 1;
            sBatch.DrawString(
                font
                , text
                , fontPosition
                , Color.White
            );
        }
        static public bool HasUnicode(String text)
        {
            foreach (char character in text)
                if (char.GetUnicodeCategory(character) == UnicodeCategory.OtherLetter)
                    return true;

            return false;
        }
    }
}
