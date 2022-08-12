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

namespace WebInvestigation.Models
{
    public class InfoModel
    {
        public HttpRequest Request { get; set; }
        public HttpResponse Response { get; set; }
        public string OutboundIP { get; set; }

        public decimal DecimalSample = 100;
        public string TEMP => Environment.GetEnvironmentVariable("TEMP");
        public string HOME => Environment.GetEnvironmentVariable("HOME");

        public string GetIPs()
        {
            string ret = "";
            foreach (var adr in Dns.GetHostAddresses(Dns.GetHostName()).Distinct())
            {
                if (ret != "")
                {
                    ret = ret + ", ";
                }
                ret = ret + adr;
            }
            if (ret == "")
            {
                return null;
            }
            else
            {
                return ret;
            }
        }

        static HttpClient HTTP = new HttpClient();

        public string GetOutboundIP()
        {
            var restask = HTTP.GetAsync($"http://httpbin.org/ip?dummy={MathUtil.Rand(10000000, 99999999)}");
            var res = restask.ConfigureAwait(false).GetAwaiter().GetResult();
            var iptm = res.Content.ReadAsStringAsync().Result;
            OutboundIP = StrUtil.MidOn(iptm, "[0-9]+\\.[0-9]+\\.[0-9]+\\.[0-9]", iptm);
            return OutboundIP;
        }

        /// <summary>
        /// Get RDAP Name/Country
        /// </summary>
        /// <returns>Not used</returns>
        public string RDap()
        {
            throw new NotSupportedException();

            if (string.IsNullOrEmpty(OutboundIP))
            {
                GetOutboundIP();
            }
            var restask = HTTP.GetAsync($"https://rdap.apnic.net/ip/{OutboundIP}");
            var res = restask.ConfigureAwait(false).GetAwaiter().GetResult();
            var rdap = res.Content.ReadAsStringAsync().Result;
            string name = null;
            string country = null;
            if (JsonConvert.DeserializeObject(rdap) is JObject jo)
            {
                var map = jo.Children().Select(a => a as JProperty).ToDictionary(a => a.Name);
                name = map.GetValueOrDefault("name")?.Value?.ToString();
                country = map.GetValueOrDefault("country")?.Value?.ToString();
            }
            return $"{res.RequestMessage.RequestUri.Host} - <b>{(name ?? "(n/a)")}</b> ({(country ?? "?")})";
        }


        public string Format(object s, bool isSort = false)
        {
            if (s == null)
            {
                return "<i>(null)</i>";
            }
            var ret = s.ToString();
            if (string.IsNullOrEmpty(ret))
            {
                return "<i>(empty)</i>";
            }
            ret = ret.Replace("<script>", "＜ｓｃｒｉｐｔ＞");
            var cs = ret.Split(';');
            if (isSort)
            {
                cs = cs.OrderBy(a => a).ToArray();
            }
            if (cs.Length > 1)
            {
                ret = "";
                for (var i = 0; i < cs.Length; i++)
                {
                    var line = cs[i];
                    var co = line.Split('=');
                    if (co.Length > 1)
                    {
                        ret += $"<strong>{co[0]} = </strong>" + string.Join("=", co.Skip(1));
                    }
                    else
                    {
                        ret += line;
                    }
                    if (i < cs.Length - 1)
                    {
                        ret += "<strong>;</strong>";
                    }
                    ret += "<br>";
                }
            }
            return ret;
        }
    }
}
