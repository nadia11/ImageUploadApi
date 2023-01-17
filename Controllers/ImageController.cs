using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ImageUploadApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ImageUploadApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : Controller
    {
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _environment;
        public ImageController(Microsoft.AspNetCore.Hosting.IWebHostEnvironment environment)
        {
            _environment = environment;
        }


        [HttpGet("Index")]
        public string Index()
        {
            return "Author: Nadia Tabassum!!";
        }

        [HttpPost("UploadFile")]
        public async Task<ResponseDownload> UploadFile([FromBody] RequestDownload req)
        {
            IDictionary<string, string> openWith = new Dictionary<string, string>();
            string[] source = req.ImageUrls.ToArray();
            int i = 0, j = 0;
            int chunkSize = req.MaxDownloadAtOnce;
            string[][] imageChunks = source.GroupBy(s => i++ / chunkSize).Select(g => g.ToArray()).ToArray();


            foreach (string[] imageChunk in imageChunks)
            {
                var urlTasks = imageChunk.Select((url, index) =>
                {
                    WebClient wc = new WebClient();
                    string path = string.Format("{0}image-{1}-{2}.jpg", "wwwroot/Images/", j, index);
                    var downloadTask = wc.DownloadFileTaskAsync(new Uri(url), path);

                    openWith.Add(url + index + j, "image" + index);

                    return downloadTask;
                });

                Console.WriteLine("Starting chunk " + j);
                await Task.WhenAll(urlTasks);
                Console.WriteLine("Completed chunk " + j);
                j++;
            }

            return new ResponseDownload
            {
                success = true,
                message = "Nice try",
                UrlAndNames = openWith
            };
        }



        [HttpGet("get-image-by-name/{image_name}")]
        public string GetImage()
        {
            Byte[] b;
            b = System.IO.File.ReadAllBytes(Path.Combine(_environment.ContentRootPath, "Images", $"{Request.RouteValues.Values.ToArray()[2]}"));
            string base64ImageRepresentation = Convert.ToBase64String(b);
            return base64ImageRepresentation;
        }
    }
}
