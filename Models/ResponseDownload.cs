using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageUploadApi.Models
{
    public class ResponseDownload
    {
        public bool success { get; set; }
        public string? message { get; set; }
        public IDictionary<string, string> UrlAndNames { get; set; }
    }
}
