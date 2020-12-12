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
 * User model
 */

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