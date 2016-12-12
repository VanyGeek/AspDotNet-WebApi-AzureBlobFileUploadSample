using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebApi_AzureBlobFileUploadSample.Models;
using WebApi_AzureBlobFileUploadSample.Utils;

namespace WebApi_AzureBlobFileUploadSample.Controllers
{
    public class FilesController : ApiController
    {   
        [RouteAttribute("api/uploadFile")]
        public Task<List<FileItem>> uploadFile()
        {
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var multipartStreamProvider = new AzureBlobStorageMultipartProvider(AzureBlobHelper.GetWebApiContainer());
            return Request.Content.ReadAsMultipartAsync<AzureBlobStorageMultipartProvider>(multipartStreamProvider).ContinueWith<List<FileItem>>(t =>
            {
                if (t.IsFaulted)
                {
                    throw t.Exception;
                }

                AzureBlobStorageMultipartProvider provider = t.Result;
                return provider.Files;
            });
        }

        [RouteAttribute("api/getFiles")]
        public IEnumerable<FileItem> getFiles()
        {
            CloudBlobContainer container = AzureBlobHelper.GetWebApiContainer();
            foreach (CloudBlockBlob blob in container.ListBlobs())
            {
                yield return new FileItem
                {
                    Name = blob.Name,
                    SizeInMB = string.Format("{0:f2}MB", blob.Properties.Length / (1024.0 * 1024.0)),
                    ContentType = blob.Properties.ContentType,
                    Path = blob.Uri.AbsoluteUri
                };
            }
        }
    }
}