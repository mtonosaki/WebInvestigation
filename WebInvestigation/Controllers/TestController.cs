﻿using Microsoft.AspNetCore.Mvc;

namespace WebInvestigation.Controllers {
    public class TestController: Controller {
        public IActionResult HealthGw() {
            return new OkResult();
        }
    }
}
