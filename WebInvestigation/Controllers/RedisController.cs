// (c) 2020 Manabu Tonosaki
// Licensed under the MIT license.

using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System;
using Tono;
using TonoAspNetCore;
using WebInvestigation.Models;

namespace WebInvestigation.Controllers {
    [RequireHttps]
    public class RedisController: Controller {
        [HttpGet]
        public IActionResult Index() {
            return Index(new RedisModel {
                HostName = RedisModel.Default.HostName,
                AccessKey = RedisModel.Default.AccessKey,
                Mode = RedisModel.Modes.Nothing,
            });
        }

        [HttpPost]
        public IActionResult Index(RedisModel model) {
            if (model.HostName?.EndsWith(".redis.cache.windows.net") ?? false) {
                model.HostName = StrUtil.LeftBefore(model.HostName, "\\.");
            }
            ControllerUtils cu = ControllerUtils.From(this);
            cu.PersistInput("HostName", model, RedisModel.Default.HostName);
            cu.PersistInput("AccessKey", model, RedisModel.Default.AccessKey);
            cu.PersistInput("Key", model, RedisModel.Default.Key);

            try {
                switch (model.Mode) {
                    case RedisModel.Modes.Ping:
                        TimeSpan ret = GetRedis(model).Ping();
                        model.Result = $"PONG {ret.TotalMilliseconds}[ms]";
                        break;
                    case RedisModel.Modes.Get:
                        RedisValue rv = GetRedis(model).StringGet(new RedisKey(model.Key));
                        if (rv.HasValue) {
                            model.Result = rv.ToString();
                        } else {
                            model.ErrorMessage = $"Key '{model.Key}' has not a value";
                        }
                        break;
                    case RedisModel.Modes.Set:
                        bool sr = GetRedis(model).StringSet(new RedisKey(model.Key), new RedisValue(model.Value));
                        if (sr) {
                            model.Result = $"OK : Set '{model.Key}' = '{model.Value}'";
                        } else {
                            model.ErrorMessage = $"Error : Set '{model.Key}' = '{model.Value}'";
                        }
                        break;
                    default:
                        break;
                }
            } catch (Exception ex) {
                model.ErrorMessage = ex.Message;
            }
            return View(model);
        }

        private IDatabase GetRedis(RedisModel model) {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(model.ConnectionString());
            return redis.GetDatabase();
        }
    }
}
