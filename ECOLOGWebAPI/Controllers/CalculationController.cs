using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ECOLOGWebAPI.APIControllers
{
    [Route("api/[controller]")]
    public class CalculationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // POST api/calculation
        [HttpPost]
        public IActionResult Post([FromBody]string value)
        {
            System.Console.WriteLine("entered calculator post method.");
            // System.Console.WriteLine(value.ToString());
            return Ok("{\"status\": \"ok\"}");
        }

        [HttpGet]
        public string Get()
        {
            return "get method succeeded.";
        }
    }
}
