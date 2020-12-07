using Microsoft.AspNetCore.Http;
using Playlist_1.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Playlist_1.Models
{
    public class Media
    {
        public string Link;

        public double Duration = 0; // Could be useful

        public string Owner;

        public DateTime DatePosted;

        public Media(string owner, string source, DateTime time)
        {
            Link = source;
            Owner = owner;
            DatePosted = time;
        }

        //public Media(User owner, IFormFile file)
        //{
        //    MP3File = file;
        //    Owner = owner;
        //}

        public void setDuration(double duration)
        {
            Duration = duration;
        }

        public string getYoutubeId()
        {
            string url = Link;
            Uri uri = new Uri(url);

            var query = HttpUtility.ParseQueryString(uri.Query);
            return query["v"];
        }
    }
}
