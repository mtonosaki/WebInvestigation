// (c) 2020 Manabu Tonosaki
// Licensed under the MIT license.

using Microsoft.AspNetCore.Mvc;

namespace WebInvestigation.Controllers {
    public class HealthController: Controller {
        public IActionResult Index() {
            return new OkResult();
        }
    }
}
