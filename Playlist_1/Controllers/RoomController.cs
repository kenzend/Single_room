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
 * Room controller - with UploadFile API endpoint
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Playlist_1.Controllers
{
    public class RoomController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        public RoomController(IWebHostEnvironment hostEnvironment) 
        {
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            //return View(Program.room.Playlist);
            return View(Program.room);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadFile(IList<IFormFile> files)
        {
            List<string> result = new List<string>();
            string wwwRoot = _hostEnvironment.WebRootPath;
            int count = Program.room.Playlist.Count;
            //Directory.CreateDirectory(Path.Combine(wwwRoot, "room"));

            foreach (IFormFile file in files)
            {
                if (!file.ContentType.ToLower().Contains("audio") && !Path.GetExtension(file.FileName).Equals(".mp3") && !Path.GetExtension(file.FileName).Equals(".wav")) // Basic file-integrity checking
                    continue;
                string fileExtension = Path.GetExtension(file.FileName);
                string title = Path.GetFileNameWithoutExtension(file.FileName);
                string fileName = "Playlist_Room_" + count.ToString() + fileExtension; // Before adding to the system, rename the file
                result.Add(fileName);
                result.Add(title);
                string save = Path.Combine(wwwRoot, "room", fileName);
                using (var fileStream = new FileStream(save, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                ++count;
            }

            return Json(result.ToArray());
        }
    }
}
