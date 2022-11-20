// (c) 2020 Manabu Tonosaki
// Licensed under the MIT license.

using System.Net.Http;
using System.Threading.Tasks;

namespace WebInvestigation.Models {
    public abstract class ResponseBase {
        public string Result { get; set; }
        public string HttpStatusCode { get; set; }

        public virtual void Set(Task<HttpResponseMessage> task) {
            HttpResponseMessage res = task.ConfigureAwait(false).GetAwaiter().GetResult();
            HttpStatusCode = $"{(int)res.StatusCode}({res.StatusCode})";
            Result = res.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
