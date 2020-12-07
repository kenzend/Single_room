using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Playlist_1.Models
{
    public class Permission
    {
        public bool CanAdd;

        public bool CanRemove;

        public bool CanPlay;

        public bool CanPause;

        public bool CanSeek;

        public Permission()
        {
        }
    }
}
