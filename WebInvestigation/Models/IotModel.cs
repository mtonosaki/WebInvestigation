using FastEnumUtility;
using Microsoft.Azure.Devices.Client;
using System.Collections.Generic;

namespace WebInvestigation.Models {
    public class IotModel {
        public static readonly IotModel Default = new IotModel {
            HostName = "<your IoT Hub URI>",
            DeviceId = "<your device id>",
            DeviceKey = "<your device shared key>",
        };
        public string HostName { get; set; }
        public string DeviceId { get; set; }
        public string DeviceKey { get; set; }
        public string Message { get; set; }
        public TransportType TransportType { get; set; }
        public string Result { get; set; }
        public string ErrorMessage { get; set; }

        public IEnumerable<string> GetTransportTypeList() {
            return FastEnum.GetNames<TransportType>();
        }
    }
}