using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Playlist_1.Models;

namespace Playlist_1.Hubs
{
    public class PlaylistHub : Hub
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        public PlaylistHub(IWebHostEnvironment hostEnvironment) 
        {
            _hostEnvironment = hostEnvironment;
        }

        public async Task ShareMedia(string user, string media)
        {
           var time = DateTime.Now;

           Media new_media = new Media(user, media, time);
           Program.room.Playlist.Add(new_media);

           await Clients.All.SendAsync("ReceiveMedia", time.ToString("MM/dd/yyyy HH:mm"), user, media);
        }

        public async Task RemoveMedia(string user, string index)
        {
            if (!Program.room.Playlist[int.Parse(index)].Link.Contains("http")) // Delete the file in the system associated with this to be removed media
            {
                string wwwRoot = _hostEnvironment.WebRootPath;
                File.Delete(Path.Combine(wwwRoot, "room", Program.room.Playlist[int.Parse(index)].Link));
            }
                
            Program.room.Playlist[int.Parse(index)] = null;

            await Clients.All.SendAsync("RemovedMedia", index);
        }

        public async Task Play(string user, string index)
        {
            await Clients.All.SendAsync("PlayMedia", index);
        }

        public async Task ResetCurrentTime(string index)
        {
            await Clients.All.SendAsync("ResetCurrentTime", index);
        }

        public async Task PlayNext(string index)
        {
            if (Program.room.Playlist.Count != 0)
            {
                bool isNull = true;
                foreach (Media media in Program.room.Playlist)
                {
                    if (media == null)
                        isNull = isNull && true;
                    else
                        isNull = isNull && false;
                }
                if (isNull)
                    return;

                int nextIndex = int.Parse(index) + 1;
                while (Program.room.Playlist[nextIndex] == null)
                {
                    if (nextIndex == (Program.room.Playlist.Count - 1))
                        nextIndex = -1;
                    ++nextIndex;
                }
                await Clients.All.SendAsync("PlayMedia", nextIndex.ToString());
            }
        }

        public async Task Pause(string index)
        {
            await Clients.All.SendAsync("PauseMedia", index);
        }
    }
}
