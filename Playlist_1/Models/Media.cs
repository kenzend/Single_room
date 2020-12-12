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
 * Media model
 */

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

        public string Owner;

        public string Title;

        public DateTime DatePosted;

        public string Url = "";

        public Media(string owner, string title, string source, DateTime time)
        {
            Link = source;
            Owner = owner;
            Title = title;
            DatePosted = time;
        }

        public string getYoutubeId()
        {
            string url = this.Url;
            Uri uri = new Uri(url);

            var query = HttpUtility.ParseQueryString(uri.Query);
            return query["v"];
        }
    }
}
