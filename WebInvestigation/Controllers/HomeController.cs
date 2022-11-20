// (c) 2020 Manabu Tonosaki
// Licensed under the MIT license.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net.Http;
using WebInvestigation.Models;

namespace WebInvestigation.Controllers {
    public class HomeController: Controller {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
        }

        public IActionResult Index() {
            return View();
        }

        private static readonly HttpClient HTTP = new HttpClient();

        public IActionResult Ping(PingModel model = null) {
            model ??= new PingModel();
            if (!string.IsNullOrEmpty(model.Ip)) {
                System.Threading.Tasks.Task<HttpResponseMessage> res = HTTP.GetAsync($"{Request.Scheme}://{Request.Host}/api/Ping?ip={model.Ip}");
                model.Set(res);
            }
            return View(model);
        }

        public IActionResult Dns(DnsModel model = null) {
            model ??= new DnsModel();
            if (!string.IsNullOrEmpty(model.Host)) {
                System.Threading.Tasks.Task<HttpResponseMessage> res = HTTP.GetAsync($"{Request.Scheme}://{Request.Host}/api/Dns?host={model.Host}");
                model.Set(res);
            }
            return View(model);
        }

        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
