using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Playlist_1.Models
{
    public class Room
    {

        public List<Media> Playlist;

        public List<User> Users;

        public int AnonIndex;

        public Room()
        {
            Playlist = new List<Media>();
            Users = new List<User>();
            AnonIndex = 0;
        }
    }
}
