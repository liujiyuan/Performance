// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace MultipartPost.Controllers
{
    [Route("api/upload")]
    public class UploadController : Controller
    {
        private const int DefaultBufferSize = 4096;

        public async Task<IActionResult> Post()
        {
            if (!HasMultipartFormContentType(Request.ContentType))
            {
                return BadRequest("Expecting a multipart content type for the Upload POST command");
            }

            PrintLine("Processing a POST request to /api/upload");

            var buffer = new byte[DefaultBufferSize];
            var boundary = GetBoundary(Request.ContentType);
            var reader = new MultipartReader(boundary, Request.Body);
            while (true)
            {
                var section = await reader.ReadNextSectionAsync();
                if (section == null)
                {
                    break;
                }
                PrintLine("Reading section");
                PrintLine($"Content-Disposition:{ section.ContentDisposition }");
                long bytesRead = 0;
                //try reading the entire message
                while (true)
                {
                    var length = section.Body.Read(buffer, 0, buffer.Length);
                    if (length <= 0) break;
                    bytesRead += length;
                }
                PrintLine($"Read { bytesRead } bytes");
            }

            PrintLine("Done");

            return Ok();
        }

        private static bool HasMultipartFormContentType(string contentType)
        {
            return contentType != null && contentType.StartsWith("multipart/", StringComparison.OrdinalIgnoreCase);
        }

        private static string GetBoundary(string contentType)
        {
            var elements = contentType.Split(' ');
            var element = elements.First(entry => entry.StartsWith("boundary="));
            var boundary = element.Substring("boundary=".Length);
            // Remove quotes if present
            if (boundary.Length >= 2 && boundary[0] == '"' && boundary[boundary.Length - 1] == '"')
            {
                boundary = boundary.Substring(1, boundary.Length - 2);
            }
            return boundary;
        }

        private void PrintLine(string input, params object[] paramStrings)
        {
            Console.Write($"[{ DateTime.UtcNow.ToString(CultureInfo.InvariantCulture) }] ");
            Console.WriteLine(input, paramStrings);
        }
    }
}
