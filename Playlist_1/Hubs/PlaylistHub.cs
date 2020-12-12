/**
 * Author:    Thang Nguyen
 * Date:      12/11/2020
 * Course:    CS 4540, University of Utah, School of Computing
 * Copyright: CS 4540 and Thang Nguyen - This work may not be copied for use in Academic Coursework.
 *
 * I, Thang Nguyen, certify that I wrote this code from scratch and did 
 * not copy it in part or whole from another source.  Any references used 
 * in the completion of the assignment are cited in my README file and in
 * the appropriate method header.
 *
 * File Contents
 * 
 * Playlisthub's back end
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Playlist_1.Models;
using VideoLibrary;

namespace Playlist_1.Hubs
{
    public class PlaylistHub : Hub
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private List<string> role_hierarchy;

        public PlaylistHub(IWebHostEnvironment hostEnvironment) 
        {
            _hostEnvironment = hostEnvironment;
            role_hierarchy = new List<string>(3);
            role_hierarchy.Add("Anon");
            role_hierarchy.Add("User");
            role_hierarchy.Add("RoomAdmin");
        }

        public async Task ShareMedia(string user, string media, string title)
        {
           var time = DateTime.Now;

           Media new_media = new Media(user, title, media, time);
           Program.room.Playlist.Add(new_media);

           await Clients.All.SendAsync("ReceiveMedia", time.ToString("MM/dd/yyyy HH:mm"), user, media, title, "");
        }

        public async Task ShareLink(string user, string media)
        {
            var time = DateTime.Now;
            int index = Program.room.Playlist.Count;
            Tuple<string, string> result = await GetTitle(media, index);

            Media new_media = new Media(user, result.Item1, result.Item2, time);
            new_media.Url = media;
            Program.room.Playlist.Add(new_media);

            await Clients.All.SendAsync("ReceiveMedia", time.ToString("MM/dd/yyyy HH:mm"), user, new_media.Link, new_media.Title, new_media.getYoutubeId());
        }

        public async Task RemoveMedia(string user, string index)
        {
            var caller = Program.room.Users[Context.ConnectionId];
            var name = Program.room.Playlist[int.Parse(index)].Owner;
            User callee= null;
            try
            {
                callee = Program.room.Users.Values.Where(x => x.Name == name).First();
            }
            catch (InvalidOperationException)
            {
                callee = new User("", "", "Anon", DateTime.Now); // mock up variable
            }

            if (role_hierarchy.IndexOf(caller.Role) < role_hierarchy.IndexOf(callee.Role))
                return;

            if (role_hierarchy.IndexOf(caller.Role) == role_hierarchy.IndexOf(callee.Role) && role_hierarchy.IndexOf(caller.Role) != 2)
                return;

            // Release media tags file access first then delete the file later
            string temp_link = Program.room.Playlist[int.Parse(index)].Link;
            string temp_url = Program.room.Playlist[int.Parse(index)].Url;
            Program.room.Playlist[int.Parse(index)] = null;
            await Clients.All.SendAsync("RemovedMedia", index);

            string wwwRoot = _hostEnvironment.WebRootPath;
            try
            {
                if(temp_url != "")
                {
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();
                }
                File.Delete(Path.Combine(wwwRoot, "room", temp_link));
            }
            catch(Exception)
            {
                return;
            }              
        }

        public async Task Play(string user, string index)
        {
            if (Program.room.Users[Context.ConnectionId].Role == "Anon")
                return;

            await Clients.All.SendAsync("PlayMedia", index);
        }

        public async Task ResetCurrentTime(string index)
        {
            if (Program.room.Users[Context.ConnectionId].Role == "Anon")
                return;

            await Clients.All.SendAsync("ResetCurrentTime", index);
        }

        public async Task PlayNext(string index)
        {
            if (Program.room.Users[Context.ConnectionId].Role == "Anon")
                return;

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

        public async Task Pause(string user, string index)
        {
            if (Program.room.Users.Values.Where(x => x.Name == user).First().Role == "Anon")
                return;

            await Clients.All.SendAsync("PauseMedia", index);
        }

        public async Task JoinRoom(string user)
        {
            var now = DateTime.Now;

            // First logged in user is room admin
            if (!user.Contains("anon") && !Program.room.Users.Values.Where(x=>x.Role == "User").Any() && !Program.room.Users.Values.Where(x => x.Role == "RoomAdmin").Any())
            {
                Program.room.Users.Add(Context.ConnectionId, new User(user, "online", "RoomAdmin", now));
                await Clients.All.SendAsync("UserJoined", user, "online", "RoomAdmin", now.ToString("MM/dd/yyyy HH:mm"));
                return;
            }

            if (user.Contains("anon"))
            {
                Program.room.Users.Add(Context.ConnectionId, new User(user, "online", "Anon", now));
                Program.room.AnonIndex++;
                await Clients.All.SendAsync("UserJoined", user, "online", "Anon", now.ToString("MM/dd/yyyy HH:mm"));
                return;
            }

            var usr = Program.room.Users.Values.Where(x => x.Name == user);
            if (!usr.Any())
            {
                Program.room.Users.Add(Context.ConnectionId, new User(user, "online", "User", now));
                await Clients.All.SendAsync("UserJoined", user, "online", "User", now.ToString("MM/dd/yyyy HH:mm"));
                return;
            }
            else
            {
                if (usr.First().Status == "blocked")
                {
                    await Clients.Caller.SendAsync("Leave", user);
                    return;
                }
                var result = new User(usr.First().Name, "online", usr.First().Role, usr.First().Joined);
                var item = Program.room.Users.First(kvp => kvp.Value.Name == user);
                Program.room.Users.Remove(item.Key);
                Program.room.Users.Add(Context.ConnectionId, result);
                await Clients.All.SendAsync("UserJoined", result.Name, result.Status, result.Role, result.Joined.ToString("MM/dd/yyyy HH:mm"));
            }
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var user = Program.room.Users[Context.ConnectionId];
            if (user.Name.Contains("anon"))
            {
                Program.room.Users.Remove(Context.ConnectionId);
                Clients.All.SendAsync("RemoveUser", user.Name).Wait();
                return base.OnDisconnectedAsync(exception);
            }
            if (user.Status == "blocked")
                return base.OnDisconnectedAsync(exception);
            user.Status = "offline";
            Clients.All.SendAsync("UserOff", user.Name, user.Status, user.Role, user.Joined.ToString("MM/dd/yyyy HH:mm")).Wait();
            return base.OnDisconnectedAsync(exception);
        }

        public async Task Promote(string name)
        {
            var caller = Program.room.Users[Context.ConnectionId];
            var to_be_promoted = Program.room.Users.Values.Where(x=>x.Name == name).First();

            if (role_hierarchy.IndexOf(to_be_promoted.Role) == 2)
                return;

            if (role_hierarchy.IndexOf(caller.Role) <= role_hierarchy.IndexOf(to_be_promoted.Role))
                return;

            // An anonymous user can never be promoted to RoomAdmin
            if (name.Contains("anon") && role_hierarchy.IndexOf(to_be_promoted.Role) == 1)
                return;
            string new_role = role_hierarchy.ElementAt(role_hierarchy.IndexOf(to_be_promoted.Role) + 1);
            to_be_promoted.Role = new_role;
            await Clients.All.SendAsync("Promoted", name, new_role);
        }

        public async Task Demote(string name)
        {
            var caller = Program.room.Users[Context.ConnectionId];
            var to_be_demoted = Program.room.Users.Values.Where(x => x.Name == name).First();

            if (role_hierarchy.IndexOf(to_be_demoted.Role) == 0)
                return;

            if (role_hierarchy.IndexOf(caller.Role) <= role_hierarchy.IndexOf(to_be_demoted.Role))
                return;

            string new_role = role_hierarchy.ElementAt(role_hierarchy.IndexOf(to_be_demoted.Role) - 1);
            to_be_demoted.Role = new_role;
            await Clients.All.SendAsync("Demoted", name, new_role);
        }

        public async Task Block(string name)
        {
            var caller = Program.room.Users[Context.ConnectionId];
            var to_be_blocked = Program.room.Users.Values.Where(x => x.Name == name).First();

            if (to_be_blocked.Status == "blocked")
                return;

            if (role_hierarchy.IndexOf(caller.Role) <= role_hierarchy.IndexOf(to_be_blocked.Role))
                return;

            to_be_blocked.Status = "blocked";
            var blocked_connectionId = Program.room.Users.First(kvp => kvp.Value.Name == name).Key;
            await Clients.All.SendAsync("Blocked", to_be_blocked.Name, to_be_blocked.Status, to_be_blocked.Role, to_be_blocked.Joined.ToString("MM/dd/yyyy HH:mm"));
            await Clients.Client(blocked_connectionId).SendAsync("Leave", name);
        }

        public async Task Unblock(string name)
        {
            var caller = Program.room.Users[Context.ConnectionId];
            var to_be_unblocked = Program.room.Users.Values.Where(x => x.Name == name).First();

            if (to_be_unblocked.Status != "blocked")
                return;

            // Only RoomAdmin can do unblock
            if (role_hierarchy.IndexOf(caller.Role) != 2)
                return;

            to_be_unblocked.Status = "offline";
            await Clients.All.SendAsync("UserOff", to_be_unblocked.Name, to_be_unblocked.Status, to_be_unblocked.Role, to_be_unblocked.Joined.ToString("MM/dd/yyyy HH:mm"));
        }

        public async Task RemoveMediaOf(string name)
        {
            List<int> indices = new List<int>(); 
            var caller = Program.room.Users[Context.ConnectionId];
            var callee = Program.room.Users.Values.Where(x => x.Name == name).First();

            if (role_hierarchy.IndexOf(caller.Role) < role_hierarchy.IndexOf(callee.Role))
                return;

            if (role_hierarchy.IndexOf(caller.Role) == role_hierarchy.IndexOf(callee.Role) && role_hierarchy.IndexOf(caller.Role) != 2)
                return;

            List<Media> to_be_removed = new List<Media>();
            foreach(Media media in Program.room.Playlist)
            {
                if (media == null)
                    continue;
                if (media.Owner == name)
                    to_be_removed.Add(media);
            }
            foreach(var item in to_be_removed)
            {
                indices.Add(Program.room.Playlist.IndexOf(item));
            }
            foreach(var index in indices)
            {
                await RemoveMedia(caller.Name, index.ToString());
            }

        }

        //********
        //* Helper classes
        //*************

        private async Task<Tuple<string, string>> GetTitle(string url, int index)
        {
            Tuple<string, string> result;
            YouTube ytb = YouTube.Default;
            var video = await ytb.GetVideoAsync(url);
            string wwwRoot = _hostEnvironment.WebRootPath;
            //Directory.CreateDirectory(Path.Combine(wwwRoot, "room"));
            await File.WriteAllBytesAsync(Path.Combine(wwwRoot, "room", "Playlist_Room_" + index.ToString() + video.FileExtension), await video.GetBytesAsync());

            result = new Tuple<string, string>(video.Title, "Playlist_Room_" + index.ToString() + video.FileExtension);

            return result;
        }



    }
}
