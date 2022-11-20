// (c) 2020 Manabu Tonosaki
// Licensed under the MIT license.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using TonoAspNetCore;
using WebInvestigation.Models;

namespace WebInvestigation.Controllers {
    [RequireHttps]
    public class KeyVaultController: Controller {
        private static readonly HttpClient HTTP = new HttpClient();

        [HttpGet]
        [Obsolete]
        public IActionResult Index() {
            return Index(new KeyVaultModel {
                Url = KeyVaultModel.Default.Url,
                ApplicationID = KeyVaultModel.Default.ApplicationID,
                ClientSecret = KeyVaultModel.Default.ClientSecret,
                Key = KeyVaultModel.Default.Key,
                SkipRequest = true,
            });
        }

        [HttpPost]
        [Obsolete]
        public IActionResult Index(KeyVaultModel model) {
            Request.Headers.Add("X-WI-ApplicationID", model.ApplicationID);
            Request.Headers.Add("X-WI-ClientSecret", model.ClientSecret);

            // Apply input history from cookie
            ControllerUtils cu = ControllerUtils.From(this);
            cu.PersistInput("Url", model, KeyVaultModel.Default.Url);
            cu.PersistInput("ApplicationID", model, KeyVaultModel.Default.ApplicationID);
            cu.PersistInput("ClientSecret", model, KeyVaultModel.Default.ClientSecret);
            cu.PersistInput("Key", model, KeyVaultModel.Default.Key);

            if (!model.SkipRequest) {
                try {
                    model.Value = GetSecretAsync(model).ConfigureAwait(false).GetAwaiter().GetResult();
                } catch (Exception ex) {
                    model.ErrorMessage = ex.Message;
                }
            }
            model.SkipRequest = false;

            return View(model);
        }

        [Obsolete]
        private async Task<string> GetSecretAsync(KeyVaultModel model) {
            KeyVaultClient client = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetAccessTokenAsync), HTTP);
            Microsoft.Azure.KeyVault.Models.SecretBundle secret = await client.GetSecretAsync(model.Url, model.Key);
            return secret.Value;
        }

        [Obsolete]
        private async Task<string> GetAccessTokenAsync(string authority, string resource, string scope) {
            string clientid = Request.Headers["X-WI-ApplicationID"].ToString();
            string clientsecret = Request.Headers["X-WI-ClientSecret"].ToString();

            ClientCredential appCredentials = new ClientCredential(clientid, clientsecret);
            AuthenticationContext context = new AuthenticationContext(authority, TokenCache.DefaultShared);
            AuthenticationResult result = await context.AcquireTokenAsync(resource, appCredentials);
            return result.AccessToken;
        }
    }
}
