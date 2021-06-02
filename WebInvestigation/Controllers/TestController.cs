using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebInvestigation.Controllers
{
    public class TestController : Controller
    {
        public IActionResult HealthGw()
        {
            return new OkResult();
        }
    }
}
