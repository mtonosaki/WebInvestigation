// (c) 2020 Manabu Tonosaki
// Licensed under the MIT license.

using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TonoAspNetCore;
using WebInvestigation.Models;

namespace WebInvestigation.Controllers {
    [RequireHttps]
    public class CallController: Controller {
        [HttpGet]
        public IActionResult Index() {
            return Index(new CallModel {
                Uri = CallModel.Default.Uri,
                Method = CallModel.Default.Method,
                Body = CallModel.Default.Body,
                SkipCall = true,
            });
        }

        [HttpPost]
        public IActionResult Index(CallModel model) {
            // Apply input history from cookie
            ControllerUtils cu = ControllerUtils.From(this);
            cu.PersistInput("Uri", model, CallModel.Default.Uri);
            cu.PersistInput("Method", model, CallModel.Default.Method);
            cu.PersistInput("Body", model, CallModel.Default.Body);

            model.SampleUri = $"{Request.Scheme}://{Request.Host}/api/Dummy?a=11&b=22";

            if (!model.SkipCall) {
                Task<HttpResponseMessage> task;
                switch (model.Method) {
                    case "(unknown)":
                    case "GET":
                        task = HTTP.GetAsync(model.Uri);
                        model.Method = "GET";
                        break;
                    case "POST":
                        task = HTTP.PostAsync(model.Uri, new StringContent(model.Body));
                        break;
                    default:
                        HttpRequestMessage req = new HttpRequestMessage {
                            Method = GetMethod(model.Method),
                            RequestUri = new Uri(model.Uri),
                            Content = new StringContent(model.Body),
                        };
                        task = HTTP.SendAsync(req);
                        break;
                }
                model.Set(task);
            }
            model.SkipCall = false;
            return View(model);
        }

        private static readonly HttpClient HTTP = new HttpClient();
        private HttpMethod GetMethod(string name) {
            System.Reflection.PropertyInfo[] pis = typeof(HttpMethod).GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            System.Reflection.PropertyInfo pi = pis.Where(a => name.Equals(a.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            HttpMethod ret = (HttpMethod)pi.GetValue(null);
            return ret;
        }
    }
}
