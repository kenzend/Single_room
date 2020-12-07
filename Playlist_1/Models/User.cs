using System;

namespace Playlist_1.Models
{
    public class User
    {
        public string Name;

        public string Status;

        public DateTime Joined;

        public User(string name, string status, DateTime joined)
        {
            this.Name = name;
            this.Status = status;
            this.Joined = joined;
        }

    }
}