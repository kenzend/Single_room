using System;

namespace Playlist_1.Models
{
    public class User
    {
        public string Name;

        public string Status;

        public string Role;

        public DateTime Joined;

        public User(string name, string status, string role, DateTime joined)
        {
            this.Name = name;
            this.Status = status;
            this.Role = role;
            this.Joined = joined;
        }

    }
}