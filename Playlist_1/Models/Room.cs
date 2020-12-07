using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Playlist_1.Models
{
    public class Room
    {

        public List<Media> Playlist;

        public List<string> Users;

        public List<string> ActiveUsers;

        public List<string> Blocked;

        public int AnonIndex;

        public Room()
        {
            Playlist = new List<Media>();
            Users = new List<string>();
            ActiveUsers = new List<string>();
            Blocked = new List<string>();
            AnonIndex = 0;
        }
    }
}
