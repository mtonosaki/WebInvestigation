// (c) 2020 Manabu Tonosaki
// Licensed under the MIT license.

using Microsoft.Azure.EventHubs;
using System;
using System.Collections.Generic;

namespace WebInvestigation.Models {
    public class EventHubModel {
        public static readonly EventHubModel Default = new EventHubModel {
            ConnectionString = "Endpoint=sb://<your event hub name>.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=12345678901234567890123+123456789=",
            EventHubName = "(Your Event Hub Name)",
            ConsumerGroupName = "(Your ConsumerGroupName)",
            StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=<YOURS>;AccountKey=<YOURS>;EndpointSuffix=core.windows.net",
            StorageContainerName = "(Your Storage Container Name)",
            Message = "(Your message to send)",
        };
        public string ConnectionString { get; set; }
        public string EventHubName { get; set; }
        public string ConsumerGroupName { get; set; }
        public string StorageConnectionString { get; set; }
        public string StorageContainerName { get; set; }
        public string Message { get; set; }
        public string ActionMessage { get; set; }
        public bool SkipSend { get; set; }
        public bool ReceiveRequested { get; set; }
        public TimeSpan ListeningTime { get; set; } = TimeSpan.FromMilliseconds(5000);
        public Dictionary<string, EventHubPartitionRuntimeInformation> PartitionInfo { get; set; }
    }
}
