// (c) 2020 Manabu Tonosaki
// Licensed under the MIT license.

using Microsoft.AspNetCore.Mvc;
using WebInvestigation.Models;

namespace WebInvestigation.Controllers
{
    [RequireHttps]
    public class InfoController : Controller
    {
        public IActionResult Index()
        {
            return View(new InfoModel
            {
                Request = Request,
                Response = Response,
            });
        }
    }
}
