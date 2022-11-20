// (c) 2020 Manabu Tonosaki
// Licensed under the MIT license.

using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;
using System.Text;

namespace WebInvestigation.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class PingController: ControllerBase {
        public string Get(string ip = "127.0.0.1") {
            StringBuilder ret = new StringBuilder();
            _ = ret.AppendLine($"Ping to {ip ?? "(null)"}");

            Ping sender = new Ping();
            PingReply reply = sender.Send(ip);
            _ = reply.Status == IPStatus.Success
                ? ret.AppendLine($"Reply from {reply.Address}: bytes={reply.Buffer.Length} time={reply.RoundtripTime}[ms] TTL={reply.Options?.Ttl}")
                : ret.AppendLine(reply.Status.ToString() ?? "(n/a)");
            return ret.ToString();
        }
    }
}
