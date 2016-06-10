using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Runtime;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace StarterMvc.Controllers
{
    public class ServerManageController : Controller
    {
        // GET: /<controller>/
        public IActionResult StartGC(bool compactLOH)
        {
            if (compactLOH)
                GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;

            GC.Collect();
            return new StatusCodeResult(200);
        }
    }
}
