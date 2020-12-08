using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Playlist_1.Models
{
    public class Room
    {

        public List<Media> Playlist;

        public Dictionary<string, User> Users;

        public int AnonIndex;

        public Room()
        {
            Playlist = new List<Media>();
            Users = new Dictionary<string, User>();
            AnonIndex = 0;
        }
    }
}
