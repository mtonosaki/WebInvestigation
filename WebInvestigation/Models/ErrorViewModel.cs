// (c) 2020 Manabu Tonosaki
// Licensed under the MIT license.

namespace WebInvestigation.Models {
    public class ErrorViewModel {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
