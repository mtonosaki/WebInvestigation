// (c) 2020 Manabu Tonosaki
// Licensed under the MIT license.

using System.Linq;
using System.Net.Http;

namespace WebInvestigation.Models {
    public class CallModel: ResponseBase {
        public static readonly CallModel Default = new CallModel {
            Uri = $"https://ntp-a1.nict.go.jp/cgi-bin/json?a=1&b=2",
            Method = "(unknown)",
            Body = "c=3&d=4",
        };
        public string Uri { get; set; }
        public string Method { get; set; }
        public string Body { get; set; }
        public bool SkipCall { get; set; }
        public string SampleUri { get; set; }

        public string[] GetMethodList() {
            System.Type t = typeof(HttpMethod);
            System.Reflection.PropertyInfo[] ms = t.GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            string[] ret = ms.Select(a => a.Name.ToUpper()).OrderBy(a => a).ToArray();
            return ret;
        }
    }
}
