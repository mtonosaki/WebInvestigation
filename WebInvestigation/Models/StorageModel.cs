// (c) 2020 Manabu Tonosaki
// Licensed under the MIT license.

using System.Collections.Generic;

namespace WebInvestigation.Models {
    public class StorageModel {
        public static readonly StorageModel Default = new StorageModel {
            Page = "(DUMMY)",
            StrageAccountName = "(Your Storage Account Name)",
            Key = "(Your Storaget Key1 or Key2)",
            BlobContainerName = "(Your Blob Container Name)",
            BlobName = "(Your Blob Name)",
            FileShareName = "(Your File Share Name)",
            FileName = "\\(your dir name)\\(your file name)",
            TableName = "(Your table name)",
            TableKey = "(Your table key)",
            TablePartition = "(Your table partition)",
            QueueName = "(Your queue name)",
        };
        public string Page { get; set; }
        public string StrageAccountName { get; set; }
        public string Key { get; set; }
        public string[] Pages { get; set; } = new string[] { "Blob", "File", "Table", "Queue" };
        public bool Skip { get; set; }
        public string ErrorMessage { get; set; }
        public string Result { get; set; }

        //=== BLOB ===
        public string BlobContainerName { get; set; }
        public string BlobName { get; set; }

        //=== FILE ===
        public string FileShareName { get; set; }
        public string FileName { get; set; }    // \dir1\todo.txt

        //=== TABLE ===
        public string TableName { get; set; }
        public string TablePartition { get; set; }
        public string TableKey { get; set; }
        public List<List<string>> TableResult { get; set; }

        //=== QUEUE ===
        public string QueueName { get; set; }
        public bool QueueSend { get; set; }
        public string QueueSendMessage { get; set; }
    }
}
