// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace MultipartPOST.Controllers
{
    [Route("api/upload")]
    public class UploadController : ApiController
    {
        public async Task<IHttpActionResult> Post()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                return BadRequest("Expecting a multi-part content type for the Upload POST command");
            }

            Console.WriteLine("Processing a POST request to /api/upload");

            var appDataDirectory = HttpContext.Current.Server.MapPath("~/App_Data");
            if (!Directory.Exists(appDataDirectory))
            {
                try
                {
                    Directory.CreateDirectory(appDataDirectory);
                }
                catch
                {
                    // ignore
                }
            }
            var provider = new MultipartFormDataStreamProvider(appDataDirectory);
            try
            {
                await Request.Content.ReadAsMultipartAsync(provider);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
            
            foreach (var fileData in provider.FileData)
            {
                File.Delete(fileData.LocalFileName);
            }

            Console.WriteLine("Done");

            return Ok();
        }
    }
}
