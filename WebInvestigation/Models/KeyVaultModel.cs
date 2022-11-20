// (c) 2020 Manabu Tonosaki
// Licensed under the MIT license.

namespace WebInvestigation.Models {
    public class KeyVaultModel {
        public static readonly KeyVaultModel Default = new KeyVaultModel {
            Url = "https://<your keyvault name>.azure.net/",
            Key = "secret key name here",
            ApplicationID = "ZZZZZZZZ-ZZZZ-ZZZZ-ZZZZ-ZZZZZZZZZZZZ",
            ClientSecret = "1234567890123456789012345678901234",
        };
        public string ApplicationID { get; set; }
        public string ClientSecret { get; set; }
        public string Url { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string ErrorMessage { get; set; }
        public bool SkipRequest { get; set; }
    }
}
