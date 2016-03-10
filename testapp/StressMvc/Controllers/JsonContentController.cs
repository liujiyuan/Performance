using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace StarterMvc.Controllers
{
    public class JsonContentController : Controller
    {
        const string testContent = "1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890";

        // GET: /<controller>/
        public IActionResult Index()
        {
            return RedirectToAction(nameof(GetObjects), new { numberOfObjs = 1});
        }

        public IActionResult GetObjects(int count)
        {
            var content = new List<KeyValuePair<int, string>>();
            for (int i = 0; i < count; i++)
            {
                content.Add(new KeyValuePair<int, string>(i, testContent));
            }

            return new JsonResult(content);
        }

        [HttpPost]
        public IActionResult AddObjects(string objects)
        {
            return new StatusCodeResult(201);
        }
    }
}
