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
 * Program
 */

using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Playlist_1.Data;
using Playlist_1.Models;
using URC.Areas.Identity.Data;

namespace Playlist_1
{
    public class Program
    {
        public static Room room;
        public static void Main(string[] args)
        {
            room = new Room();
            var host = CreateHostBuilder(args).Build();

            CreateDbIfNotExists(host);

            host.Run();
        }

        private static void CreateDbIfNotExists(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    // Seed the Identity-related database if have not done so
                    var UR_context = services.GetRequiredService<UserRoleDB>();
                    Seeding.Initialize(UR_context);

                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred creating the DB.");
                }
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
