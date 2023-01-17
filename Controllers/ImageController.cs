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
        // GET: ImageController
        [HttpGet("Index")]
        public string Index()
        {   IEnumerable<string> Url= new string[]{ "a","b","c","d","e"};
            string[] source =Url.ToArray();
            int i = 0;
            int chunkSize = 3;
            string[][] result = source.GroupBy(s => i++ / chunkSize).Select(g => g.ToArray()).ToArray();
            Console.WriteLine("kkk");
            return "hello world";

        }

        // GET: ImageController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ImageController/Create

        public ActionResult Create()
        {
            return View();
        }

        // POST: ImageController/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        [HttpPost("UploadFile")]

        public async Task<ResponseDownload> UploadFile([FromBody] RequestDownload req)
        {
            IDictionary<string, string> openWith =new Dictionary<string, string>();
            string[] source = req.ImageUrls.ToArray();
            int i = 0;
            int chunkSize = req.MaxDownloadAtOnce;
            string[][] imageChunks = source.GroupBy(s => i++ / chunkSize).Select(g => g.ToArray()).ToArray();
            int j = 0;
            foreach(string[] imageChunk in imageChunks)
            {
                var urlTasks = imageChunk.Select((url, index) =>
                {
                    WebClient wc = new WebClient();
                    string path = string.Format("{0}image-{1}-{2}.jpg", "wwwroot/Images/", j, index);

                    var downloadTask = wc.DownloadFileTaskAsync(new Uri(url), path);
                    //Output(path);

                    openWith.Add(url+index+j, "image" + index);

                    return downloadTask;
                });

                Console.WriteLine("Starting chunk " + j);
                await Task.WhenAll(urlTasks);
                Console.WriteLine("Completed chunk " + j);
                j++;
            }
      
          
            return new ResponseDownload {
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
            // return File(b, "image/jpeg");
            //byte[] imageArray = System.IO.File.ReadAllBytes(@"image file path");
            string base64ImageRepresentation = Convert.ToBase64String(b);
            return base64ImageRepresentation;
        }
    
    // GET: ImageController/Edit/5
    public ActionResult Edit(int id)
        {
            //[HttpPost("UploadFile")]

            //public async Task<ResponseDownload> UploadFile([FromBody] RequestDownload req)
            //{
            //    IDictionary<string, string> openWith = new Dictionary<string, string>();
            //    string[] source = req.ImageUrls.ToArray();
            //    int i = 0;
            //    int chunkSize = 3;
            //    string[][] Imagechunk = source.GroupBy(s => i++ / chunkSize).Select(g => g.ToArray()).ToArray();

            //    var urlTasks = req.ImageUrls.Select((url, index) =>
            //    {
            //        WebClient wc = new WebClient();
            //        string path = string.Format("{0}image-{1}.jpg", "wwwroot/Images/", index);

            //        var downloadTask = wc.DownloadFileTaskAsync(new Uri(url), path);
            //        //Output(path);

            //        openWith.Add(url, "image" + index);

            //        return downloadTask;
            //    });

            //    Console.WriteLine("Start now");
            //    await Task.WhenAll(urlTasks);
            //    Console.WriteLine("Done");

            //    return new ResponseDownload
            //    {
            //        success = true,
            //        message = "Nice try",
            //        UrlAndNames = openWith


            //    };

            //}
            return View();
        }

        // POST: ImageController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ImageController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ImageController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
