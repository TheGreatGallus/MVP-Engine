using Microsoft.Xna.Framework.Media;
using MVP_Core.Entities;
using System;
using System.Collections.Generic;

namespace MVP_Core.Managers
{
    public class SongManager : Manager<Song>
    {
        private static readonly Lazy<SongManager> lazy =
            new Lazy<SongManager>(() => new SongManager());

        public static SongManager Instance { get { return lazy.Value; } }

        private SongManager()
        {
            Initialize();
            RoomManager.Instance.RoomChanged += HandleRoomChanged;
        }

        private string songName;
        public bool IsMuted = false;
        public bool IsPaused = false;

        public override void Initialize()
        {
            base.Initialize();
            songName = "";
        }

        public void PlaySong(string songName)
        {
            if (songName != null && this.songName != songName && bank.ContainsKey(songName))
            {
                this.songName = songName;
                MediaPlayer.Stop();
                MediaPlayer.Play(GetItem(songName));
                MediaPlayer.Volume = 0.1f;
            }
        }

        public void MuteAudio()
        {
            IsMuted = true;
            MediaPlayer.IsMuted = true;
        }

        public void UnmuteAudio()
        {
            IsMuted = false;
            MediaPlayer.IsMuted = false;
        }

        public void PauseAudio()
        {
            IsPaused = true;
            MediaPlayer.Pause();
        }

        public void UnpauseAudio()
        {
            IsPaused = false;
            MediaPlayer.Resume();
        }

        void HandleRoomChanged(Room room)
        {
            Instance.PlaySong(room.SongName);
        }

        public List<string> GetSongNames()
        {
            List<string> songNames = new List<string>();
            foreach(KeyValuePair<string, Song> songEntry in bank)
            {
                songNames.Add(songEntry.Key);
            }
            return songNames;
        }

    }
}
