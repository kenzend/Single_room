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
 * Seeding User database
 */

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Playlist_1.Areas.Identity.Data;
using Playlist_1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace URC.Areas.Identity.Data
{
    public static class Seeding
    {
        public static void Initialize(UserRoleDB context)
        {
            context.Database.EnsureCreated();

            if (context.Roles.Any() && context.Users.Any() && context.UserRoles.Any())
                return;

            string[] validRoles = new string[] { "Administrator", "RoomAdmin", "Speaker", "Remote" };

            var roleStore = new RoleStore<IdentityRole>(context);
            foreach (string role in validRoles)
            {
                IdentityRole newRole = new IdentityRole();
                newRole.Name = role;
                newRole.NormalizedName = role.ToUpper();
                if (!context.Roles.Any(e => e.Name == role))
                    roleStore.CreateAsync(newRole).Wait();
            }

            var admin = new Playlist_1User
            {
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                //Name = "Admin",
                Email = "admin@utah.edu",
                NormalizedEmail = "ADMIN@UTAH.EDU",
                EmailConfirmed = true,
            };

            var professor = new Playlist_1User
            {
                UserName = "Jim",
                NormalizedUserName = "JIM",
                //Name = "John",
                Email = "professor@utah.edu",
                NormalizedEmail = "PROFESSOR@UTAH.EDU",
                EmailConfirmed = true,
            };

            var professor_mary = new Playlist_1User
            {
                UserName = "Mary",
                NormalizedUserName = "MARY",
                //Name = "Mary",
                Email = "professor_mary@utah.edu",
                NormalizedEmail = "PROFESSOR_MARY@UTAH.EDU",
                EmailConfirmed = true,
            };

            var u0000000 = new Playlist_1User
            {
                UserName = "u0000000",
                NormalizedUserName = "U0000000",
                //Name = "Mike",
                Email = "u0000000@utah.edu",
                NormalizedEmail = "U0000000@UTAH.EDU",
                EmailConfirmed = true,
            };

            var u1234567 = new Playlist_1User
            {
                UserName = "u1234567",
                NormalizedUserName = "U1234567",
                //Name = "Bob",
                Email = "u1234567@utah.edu",
                NormalizedEmail = "U1234567@UTAH.EDU",
                EmailConfirmed = true,
            };

            var u0000001 = new Playlist_1User
            {
                UserName = "u0000001",
                NormalizedUserName = "U0000001",
                //Name = "Bob",
                Email = "u0000001@utah.edu",
                NormalizedEmail = "U0000001@UTAH.EDU",
                EmailConfirmed = true,
            };

            var u0000002 = new Playlist_1User
            {
                UserName = "u0000002",
                NormalizedUserName = "U0000002",
                //Name = "Bob",
                Email = "u0000002@utah.edu",
                NormalizedEmail = "U0000002@UTAH.EDU",
                EmailConfirmed = true,
            };

            var u0000003 = new Playlist_1User
            {
                UserName = "u0000003",
                NormalizedUserName = "U0000003",
                //Name = "Bob",
                Email = "u0000003@utah.edu",
                NormalizedEmail = "U0000003@UTAH.EDU",
                EmailConfirmed = true,
            };

            Playlist_1User[] users = new Playlist_1User[] { admin, professor, professor_mary, u0000000, u1234567, u0000001, u0000002, u0000003 };
            var userStore = new UserStore<Playlist_1User>(context);
            foreach (var user in users)
            {
                if (!context.Users.Any(u => u.UserName == user.UserName))
                {
                    var password = new PasswordHasher<Playlist_1User>();
                    var hashed = password.HashPassword(user, "qwerty");
                    user.PasswordHash = hashed;

                    userStore.CreateAsync(user).Wait();
                }
            }


            userStore.AddToRoleAsync(context.Users.AsNoTracking().FirstOrDefault(e => e.UserName == admin.UserName), "ADMINISTRATOR").Wait();
            userStore.AddToRoleAsync(context.Users.AsNoTracking().FirstOrDefault(e => e.UserName == professor.UserName), "ROOMADMIN").Wait();
            userStore.AddToRoleAsync(context.Users.AsNoTracking().FirstOrDefault(e => e.UserName == professor_mary.UserName), "ROOMADMIN").Wait();
            userStore.AddToRoleAsync(context.Users.AsNoTracking().FirstOrDefault(e => e.UserName == u0000000.UserName), "SPEAKER").Wait();
            userStore.AddToRoleAsync(context.Users.AsNoTracking().FirstOrDefault(e => e.UserName == u1234567.UserName), "SPEAKER").Wait();
            userStore.AddToRoleAsync(context.Users.AsNoTracking().FirstOrDefault(e => e.UserName == u0000001.UserName), "REMOTE").Wait();
            userStore.AddToRoleAsync(context.Users.AsNoTracking().FirstOrDefault(e => e.UserName == u0000002.UserName), "REMOTE").Wait();
            userStore.AddToRoleAsync(context.Users.AsNoTracking().FirstOrDefault(e => e.UserName == u0000003.UserName), "REMOTE").Wait();

            context.SaveChanges();


        }
    }
}
