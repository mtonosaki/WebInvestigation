// (c) 2020 Manabu Tonosaki
// Licensed under the MIT license.

using FastEnumUtility;
using System.Collections.Generic;
using System.Linq;

namespace WebInvestigation.Models {
    public class RedisModel {
        public static readonly RedisModel Default = new RedisModel {
            HostName = "<your host name>",
            AccessKey = "<your access key>",
            Key = "<Key name>",
        };
        public enum Modes {
            Nothing, Get, Set, Ping,
        };
        public Modes Mode { get; set; } = Modes.Nothing;
        public string AccessKey { get; set; }
        public string HostName { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string Result { get; set; }
        public string ErrorMessage { get; set; }

        public string ConnectionString() {
            return $"{HostName}.redis.cache.windows.net,abortConnect=false,ssl=true,allowAdmin=true,password={AccessKey}";
        }

        public IEnumerable<string> GetModeList() {
            return FastEnum.GetNames<Modes>().Skip(1);
        }
    }
}
