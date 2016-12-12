using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi_AzureBlobFileUploadSample.Models
{
    public class FileItem
    {
        /// <summary>
        /// file name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// size in bytes
        /// </summary>
        public string SizeInMB { get; set; }
        public string ContentType { get; set; }
        public string Path { get; set; }
        public string BlobUploadCostInSeconds { get; set; }
    }
}