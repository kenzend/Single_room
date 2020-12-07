using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Playlist_1.Areas.Identity.Data;
using Playlist_1.Data;

[assembly: HostingStartup(typeof(Playlist_1.Areas.Identity.IdentityHostingStartup))]
namespace Playlist_1.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<UserRoleDB>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("UserRoleDBConnection")));

                services.AddDefaultIdentity<Playlist_1User>(options => options.SignIn.RequireConfirmedAccount = false)
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<UserRoleDB>();
            });
        }
    }
}