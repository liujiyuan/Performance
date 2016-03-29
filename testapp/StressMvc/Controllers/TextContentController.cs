// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace StarterMvc
{
    public class TextContentController : Controller
    {
        static readonly byte[] StaticContent500B = new byte[500];
        static readonly byte[] StaticContent2KB = new byte[2048];
        static readonly byte[] StaticContent500KB = new byte[500 * 1024];
        static readonly byte[] StaticContent1000KB = new byte[1000 * 1024];
        static readonly byte[] StaticContent5000KB = new byte[5000 * 1024];
        static readonly byte[] StaticContent8000KB = new byte[8000 * 1024];

        static readonly FileContentResult ContentResult500B = new FileContentResult(StaticContent500B, "text/plain");
        static readonly FileContentResult ContentResult2KB = new FileContentResult(StaticContent2KB, "text/plain");
        static readonly FileContentResult ContentResult500KB = new FileContentResult(StaticContent500KB, "text/plain");
        static readonly FileContentResult ContentResult1000KB = new FileContentResult(StaticContent1000KB, "text/plain");
        static readonly FileContentResult ContentResult5000KB = new FileContentResult(StaticContent5000KB, "text/plain");
        static readonly FileContentResult ContentResult8000KB = new FileContentResult(StaticContent8000KB, "text/plain");

        // GET: /<controller>/
        public IActionResult Get500B()
        {
            return ContentResult500B;
        }

        public IActionResult Get2KB()
        {
            return ContentResult2KB;
        }

        public IActionResult Get500KB()
        {
            return ContentResult500KB;
        }

        public IActionResult GetContent(int size)
        {
            //byte[] tmpContent = new byte[size];
            if (size <= 500)
                return ContentResult500B;
            else if (size <= 2000)
                return ContentResult2KB;
            else if (size <= 500 * 1024)
                return ContentResult500KB;
            else if (size <= 1000 * 1024)
                return ContentResult1000KB;
            else if (size <= 5000 * 1024)
                return ContentResult5000KB;
            else
                return ContentResult8000KB;

        }

        public IActionResult AddContent(string content)
        {
            return new StatusCodeResult(201); 
        }
    }
}
