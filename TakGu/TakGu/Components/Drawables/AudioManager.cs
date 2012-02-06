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
using System.Text;
using Nini.Config;


namespace TakGu
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class AudioManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private List<Song> songs;
        private int songIndex;
        private String filteredSongText;

        private KeyboardState oldKState;
        private GamePadState oldGState;

        private SpriteFont font;

        private IniConfigSource configSource;

        public AudioManager(Game game, SpriteFont font)
            : base(game)
        {
            this.font = font;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            this.configSource = (IniConfigSource)this.Game.Services.GetService(typeof(IniConfigSource));

            MediaPlayer.IsRepeating = false;
            MediaPlayer.IsVisualizationEnabled = true;
            MediaPlayer.Stop();
            MediaPlayer.MediaStateChanged += new EventHandler<EventArgs>(MediaPlayer_MediaStateChanged);
            MediaPlayer.Volume = this.configSource.Configs["Media"].GetFloat("volume") / 100f;

            SoundEffect.MasterVolume = this.configSource.Configs["Media"].GetFloat("effectvolume") / 100f;

            this.songs = new List<Song>();

            using (MediaLibrary library = new MediaLibrary())
                this.songs = library.Songs.ToList<Song>();

            this.PlayNextSong();

            base.Initialize();
        }

        private void MediaPlayer_MediaStateChanged(object sender, EventArgs e)
        {
            if (MediaPlayer.State == MediaState.Stopped)
                this.PlayNextSong();
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
                    Keys.PageDown
                    , Keys.MediaNextTrack
                }
                , this.oldKState
                , keyState
                , this.PlayNextSong
            );

            InputUtility.ExecuteOnToggle(
                new List<Keys>(){
                    Keys.PageUp
                    , Keys.MediaPreviousTrack
                }
                , this.oldKState
                , keyState
                , this.PlayPreviousSong
            );

            InputUtility.ExecuteOnToggle(
                new List<Tuple<Keys, Keys>>(){
                    new Tuple<Keys, Keys>(Keys.LeftControl, Keys.PageUp)
                    ,  new Tuple<Keys, Keys>(Keys.RightControl, Keys.PageUp)
                }
                , this.oldKState
                , keyState
                , this.IncreaseVolume
            );

            InputUtility.ExecuteOnToggle(
                new List<Tuple<Keys, Keys>>(){
                    new Tuple<Keys, Keys>(Keys.LeftControl, Keys.PageDown)
                    ,  new Tuple<Keys, Keys>(Keys.RightControl, Keys.PageDown)
                }
                , this.oldKState
                , keyState
                , this.DecreaseVolume
            );

            this.oldKState = keyState;
            #endregion

            #region buttonbinds
            GamePadState padState = GamePad.GetState(PlayerIndex.One);

            InputUtility.ExecuteOnToggle(
                Buttons.RightTrigger
                , this.oldGState
                , padState
                , this.PlayNextSong
            );

            InputUtility.ExecuteOnToggle(
                Buttons.LeftTrigger
                , this.oldGState
                , padState
                , this.PlayPreviousSong
            );

            InputUtility.ExecuteOnToggle(
                Buttons.RightShoulder
                , this.oldGState
                , padState
                , this.IncreaseVolume
            );

            InputUtility.ExecuteOnToggle(
                Buttons.LeftShoulder
                , this.oldGState
                , padState
                , this.DecreaseVolume
            );

            this.oldGState = padState;
            #endregion

            base.Update(gameTime);
        }

        private void PlayNextSong()
        {
            if (this.songIndex > this.songs.Count - 2)
                this.songIndex = 0;

            MediaPlayer.Play(this.songs[++this.songIndex]);

            this.filteredSongText = this.FilterTextForUnicode();
        }

        private void PlayPreviousSong()
        {
            if (this.songIndex <= 0)
                this.songIndex = this.songs.Count - 1;

            MediaPlayer.Play(this.songs[--this.songIndex]);

            this.filteredSongText = this.FilterTextForUnicode();   
        }

        private void IncreaseVolume()
        {
            MediaPlayer.Volume += 0.1f;
            this.SetCurrentVolumeInSettings();
        }

        private void DecreaseVolume()
        {
            MediaPlayer.Volume -= 0.1f;
            this.SetCurrentVolumeInSettings();
        }

        private void SetCurrentVolumeInSettings()
        {
            this.configSource.Configs["Media"].Set("volume", Math.Round(MediaPlayer.Volume * 100, 0));
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch sBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            this.DrawMediaInfo(sBatch);

            base.Draw(gameTime);
        }

        private void DrawMediaInfo(SpriteBatch sBatch)
        {
            string volumeText = new StringBuilder()
                                    .Append("Volume: ")
                                    .Append(Math.Round(MediaPlayer.Volume * 100, 0).ToString() + "%")
                                    .ToString();

            sBatch.DrawString(font
                    , this.filteredSongText
                    , new Vector2(11, 11)
                    , Color.Black
            );

            sBatch.DrawString(font
                    , this.filteredSongText
                    , new Vector2(10, 10)
                    , Color.WhiteSmoke
            );

            sBatch.DrawString(font
                    , volumeText
                    , new Vector2(
                        this.Game.GraphicsDevice.Viewport.Width - this.font.MeasureString(volumeText).X - 9
                        , 11
                    )
                    , Color.Black
            );
            sBatch.DrawString(font
                    , volumeText
                    , new Vector2(
                        this.Game.GraphicsDevice.Viewport.Width - this.font.MeasureString(volumeText).X - 10
                        , 10
                    )
                    , Color.WhiteSmoke
            );
        }

        public string FilterTextForUnicode()
        {
            StringBuilder builder = new StringBuilder();
            string song = this.CurrentSong.Name;
            string artist = this.CurrentSong.Artist.Name;

            if (!Utility.HasUnicode(song))
                builder.Append(song);
            else
                builder.Append("No Unicode");

            builder.Append(" - ");

            if (!Utility.HasUnicode(artist))
                builder.Append(artist);
            else
                builder.Append("No Unicode");

            return builder.ToString();
        }

        public Song CurrentSong 
        { 
            get 
            { 
                return this.songs[this.songIndex];
            } 
        }

    }
}
