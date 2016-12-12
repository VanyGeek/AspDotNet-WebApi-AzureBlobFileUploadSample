using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using WebApi_AzureBlobFileUploadSample.Models;

namespace WebApi_AzureBlobFileUploadSample.Utils
{
    public class AzureBlobStorageMultipartProvider : MultipartFileStreamProvider
    {
        private CloudBlobContainer _container;
        public AzureBlobStorageMultipartProvider(CloudBlobContainer container)
            : base(Path.GetTempPath())
        {
            _container = container;
            Files = new List<FileItem>();
        }

        public List<FileItem> Files { get; set; }

        public override Task ExecutePostProcessingAsync()
        {
            // Upload the files to azure blob storage and remove them from local disk
            foreach (var fileData in this.FileData)
            {
                var sp = new Stopwatch();
                sp.Start();
                string fileName = Path.GetFileName(fileData.Headers.ContentDisposition.FileName.Trim('"'));
                CloudBlockBlob blob = _container.GetBlockBlobReference(fileName);
                blob.Properties.ContentType = fileData.Headers.ContentType.MediaType;

                //set the number of blocks that may be simultaneously uploaded
                var requestOption = new BlobRequestOptions()
                {
                    ParallelOperationThreadCount = 5,
                    SingleBlobUploadThresholdInBytes = 10 * 1024 * 1024 ////maximum for 64MB,32MB by default
                };

                //upload a file to blob
                blob.UploadFromFile(fileData.LocalFileName, options: requestOption);
                blob.FetchAttributes();

                File.Delete(fileData.LocalFileName);
                sp.Stop();
                Files.Add(new FileItem
                {
                    ContentType = blob.Properties.ContentType,
                    Name = blob.Name,
                    SizeInMB = string.Format("{0:f2}MB", blob.Properties.Length / (1024.0 * 1024.0)),
                    Path = blob.Uri.AbsoluteUri,
                    BlobUploadCostInSeconds = string.Format("{0:f2}s", sp.ElapsedMilliseconds / 1000.0)
                });
            }
            return base.ExecutePostProcessingAsync();
        }
    }
}