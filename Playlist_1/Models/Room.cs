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
 * Room model
 */

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
