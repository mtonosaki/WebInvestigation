// (c) 2020 Manabu Tonosaki
// Licensed under the MIT license.

using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using Tono;

namespace WebInvestigation.Models {
    public class InfoModel {
        public HttpRequest Request { get; set; }
        public HttpResponse Response { get; set; }
        public string OutboundIP { get; set; }

        public decimal DecimalSample = 100;
        public string TEMP => Environment.GetEnvironmentVariable("TEMP");
        public string HOME => Environment.GetEnvironmentVariable("HOME");

        public string GetIPs() {
            string ret = "";
            foreach (IPAddress adr in Dns.GetHostAddresses(Dns.GetHostName()).Distinct()) {
                if (ret != "") {
                    ret += ", ";
                }
                ret += adr;
            }
            return ret == "" ? null : ret;
        }

        private static readonly HttpClient HTTP = new HttpClient();

        public string GetOutboundIP() {
            System.Threading.Tasks.Task<HttpResponseMessage> restask = HTTP.GetAsync($"http://httpbin.org/ip?dummy={MathUtil.Rand(10000000, 99999999)}");
            HttpResponseMessage res = restask.ConfigureAwait(false).GetAwaiter().GetResult();
            string iptm = res.Content.ReadAsStringAsync().Result;
            OutboundIP = StrUtil.MidOn(iptm, "[0-9]+\\.[0-9]+\\.[0-9]+\\.[0-9]+", iptm);
            return OutboundIP;
        }

        /// <summary>
        /// Get RDAP Name/Country
        /// </summary>
        /// <returns>Not used</returns>
        public string RDap() {
            throw new NotSupportedException();

            //if (string.IsNullOrEmpty(OutboundIP)) {
            //    _ = GetOutboundIP();
            //}
            //System.Threading.Tasks.Task<HttpResponseMessage> restask = HTTP.GetAsync($"https://rdap.apnic.net/ip/{OutboundIP}");
            //HttpResponseMessage res = restask.ConfigureAwait(false).GetAwaiter().GetResult();
            //string rdap = res.Content.ReadAsStringAsync().Result;
            //string name = null;
            //string country = null;
            //if (JsonConvert.DeserializeObject(rdap) is JObject jo) {
            //    System.Collections.Generic.Dictionary<string, JProperty> map = jo.Children().Select(a => a as JProperty).ToDictionary(a => a.Name);
            //    name = map.GetValueOrDefault("name")?.Value?.ToString();
            //    country = map.GetValueOrDefault("country")?.Value?.ToString();
            //}
            //return $"{res.RequestMessage.RequestUri.Host} - <b>{name ?? "(n/a)"}</b> ({country ?? "?"})";
        }


        public string Format(object s, bool isSort = false) {
            if (s == null) {
                return "<i>(null)</i>";
            }
            string ret = s.ToString();
            if (string.IsNullOrEmpty(ret)) {
                return "<i>(empty)</i>";
            }
            ret = ret.Replace("<script>", "＜ｓｃｒｉｐｔ＞");
            string[] cs = ret.Split(';');
            if (isSort) {
                cs = cs.OrderBy(a => a).ToArray();
            }
            if (cs.Length > 1) {
                ret = "";
                for (int i = 0; i < cs.Length; i++) {
                    string line = cs[i];
                    string[] co = line.Split('=');
                    if (co.Length > 1) {
                        ret += $"<strong>{co[0]} = </strong>" + string.Join("=", co.Skip(1));
                    } else {
                        ret += line;
                    }
                    if (i < cs.Length - 1) {
                        ret += "<strong>;</strong>";
                    }
                    ret += "<br>";
                }
            }
            return ret;
        }
    }
}
