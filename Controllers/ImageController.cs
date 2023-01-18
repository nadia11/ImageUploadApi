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

        //This endpoint redirects to Postman collection page for showing list of web APIs
        [HttpGet("Index")]
        public void Index()
        {
            Response.Redirect("https://documenter.getpostman.com/view/18525620/2s8ZDVZip9");
        }


        [HttpPost("UploadFile")]
        public async Task<ResponseDownload> UploadFile([FromBody] RequestDownload req)
        {
          
            string[] ImageUrlList = req.ImageUrls.ToArray();
            int i = 0, chunkIndex = 0;
            int chunkSize = req.MaxDownloadAtOnce;
            string[][] imageChunks = ImageUrlList.GroupBy(s => i++ / chunkSize).Select(g => g.ToArray()).ToArray();
            IDictionary<string, string> urlAndNames = new Dictionary<string, string>();


            foreach (string[] imageChunk in imageChunks)
            {
                var urlTasks = imageChunk.Select((url, index) =>
                {
                    Guid uniqueName = Guid.NewGuid();
                    WebClient wc = new WebClient();

                    //adding chunkIndex and urlIndex inside the chunk to check the process
                    string path = string.Format("{0}{1}-{2}-{3}.jpg", "wwwroot/Images/", uniqueName , chunkIndex, index);
                    var downloadTask = wc.DownloadFileTaskAsync(new Uri(url), path);

                    urlAndNames.Add(string.Format("{0}-{1}-{2}.jpg", uniqueName, chunkIndex, index), url);

                    return downloadTask;
                });

                Console.WriteLine("Starting chunk " + chunkIndex);
                await Task.WhenAll(urlTasks);
                Console.WriteLine("Completed chunk " + chunkIndex);
                chunkIndex++;
            }

            return new ResponseDownload
            {
                success = true,
                message = "Succesfully Downloaded and uploaded",
                UrlAndNames = urlAndNames
            };
        }



        [HttpGet("get-image-by-name/{image_name}")]
        public object GetImage()
        {
            Byte[] byt;

            //reading files from wwwroot/Images path
            byt = System.IO.File.ReadAllBytes(Path.Combine(_environment.WebRootPath, "Images", $"{Request.RouteValues.Values.ToArray()[2]}"));
            
            //creating anonymus object for a better representation
            var base64ImageRepresentation = new { base64String= Convert.ToBase64String(byt) };
            return base64ImageRepresentation;
        }
    }
}
