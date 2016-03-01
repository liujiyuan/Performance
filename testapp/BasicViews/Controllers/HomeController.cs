// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Mvc;

namespace BasicViews.Controllers
{
    public class HomeController : Controller
    {
        [ValidateAntiForgeryToken]
        public IActionResult Index(Person person)
        {
            return View(person);
        }

        [ValidateAntiForgeryToken]
        public IActionResult HtmlHelpers(Person person)
        {
            return View(person);
        }
    }
}
