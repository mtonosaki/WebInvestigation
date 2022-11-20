// (c) 2020 Manabu Tonosaki
// Licensed under the MIT license.

using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;

namespace WebInvestigation.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class DummyController: ControllerBase {
        private string make(string method) {
            StringBuilder ret = new StringBuilder();
            _ = ret.AppendLine($"Method = {method}");
            _ = ret.AppendLine($"Request.Scheme = {Request.Scheme}");
            _ = ret.AppendLine($"Request.Host = {Request.Host}");
            StreamReader sr = new StreamReader(Request.BodyReader.AsStream());
            _ = ret.AppendLine($"Request.Body");
            _ = ret.AppendLine(sr.ReadToEnd());
            return ret.ToString();
        }
        [HttpGet]
        public string Get() {
            return make("Get");
        }

        [HttpPost]
        public string Post() {
            return make("Post");
        }
        [HttpDelete]
        public string Delete() {
            return make("Delete");
        }
        [HttpHead]
        public string Head() {
            return make("Head");
        }
        [HttpOptions]
        public string Options() {
            return make("Options");
        }
        [HttpPatch]
        public string Patch() {
            return make("Patch");
        }
        [HttpPut]
        public string Put() {
            return make("Put");
        }
    }
}
