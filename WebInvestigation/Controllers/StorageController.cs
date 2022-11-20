// (c) 2020 Manabu Tonosaki
// Licensed under the MIT license.

using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TonoAspNetCore;
using WebInvestigation.Models;

namespace WebInvestigation.Controllers {
    [RequireHttps]
    public class StorageController: Controller {
        [HttpGet]
        public IActionResult Index() {
            return Index(new StorageModel {
                Skip = true,
                Page = StorageModel.Default.Page,
                StrageAccountName = StorageModel.Default.StrageAccountName,
                Key = StorageModel.Default.Key,
                BlobContainerName = StorageModel.Default.BlobContainerName,
                BlobName = StorageModel.Default.BlobName,
                FileShareName = StorageModel.Default.FileShareName,
                FileName = StorageModel.Default.FileName,
                TableName = StorageModel.Default.TableName,
                TablePartition = StorageModel.Default.TablePartition,
                TableKey = StorageModel.Default.TableKey,
                QueueName = StorageModel.Default.QueueName,
            });
        }

        [HttpPost]
        public IActionResult Index(StorageModel model) {
            // Apply input history from cookie
            ControllerUtils cu = ControllerUtils.From(this);
            cu.PersistInput("StrageAccountName", model, StorageModel.Default.StrageAccountName);
            cu.PersistInput("Key", model, StorageModel.Default.Key);
            cu.PersistInput("Page", model, StorageModel.Default.Page);
            cu.PersistInput("BlobContainerName", model, StorageModel.Default.BlobContainerName);
            cu.PersistInput("BlobName", model, StorageModel.Default.BlobName);
            cu.PersistInput("FileShareName", model, StorageModel.Default.FileShareName);
            cu.PersistInput("FileName", model, StorageModel.Default.FileName);
            cu.PersistInput("TableName", model, StorageModel.Default.TableName);
            cu.PersistInput("TablePartition", model, StorageModel.Default.TablePartition);
            cu.PersistInput("TableKey", model, StorageModel.Default.TableKey);
            cu.PersistInput("QueueName", model, StorageModel.Default.QueueName);

            if (!model.Pages.Contains(model.Page)) {
                model.Page = "Blob";
            }
            return model.Page switch {
                "Blob" => Blob(model),
                "File" => File(model),
                "Table" => Table(model),
                "Queue" => Queue(model),
                _ => NotFound($"Page {model.Page} not found."),
            };
        }

        private IActionResult Blob(StorageModel model) {
            try {
                if (!model.Skip) {
                    StorageSharedKeyCredential credential = new StorageSharedKeyCredential(model.StrageAccountName, model.Key);
                    Uri serviceUri = new Uri($"https://{model.StrageAccountName}.{model.Page.ToLower()}.core.windows.net/");
                    BlobServiceClient service = new BlobServiceClient(serviceUri, credential);
                    BlobContainerClient container = service.GetBlobContainerClient(model.BlobContainerName);
                    BlobClient blob = container.GetBlobClient(model.BlobName);
                    Azure.Response<Azure.Storage.Blobs.Models.BlobDownloadInfo> data = blob.Download();
                    StreamReader sr = new StreamReader(data.Value.Content);
                    model.Result = sr.ReadToEnd();
                }
            } catch (Exception ex) {
                model.ErrorMessage = $"Blob Error : {ex.Message}";
            }

            model.Skip = false;
            return View(model);
        }

        public IActionResult File(StorageModel model) {
            try {
                if (!model.Skip) {
                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse($"DefaultEndpointsProtocol=https;AccountName={model.StrageAccountName};AccountKey={model.Key}");
                    Microsoft.WindowsAzure.Storage.File.CloudFileClient fc = storageAccount.CreateCloudFileClient();
                    Microsoft.WindowsAzure.Storage.File.CloudFileShare fs = fc.GetShareReference(model.FileShareName);
                    Microsoft.WindowsAzure.Storage.File.CloudFileDirectory clouddir = fs.GetRootDirectoryReference();
                    string[] dirs = model.FileName.Split('\\', StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < dirs.Length - 1; i++) {
                        string dir = dirs[i];
                        clouddir = clouddir.GetDirectoryReference(dir);
                    }
                    Microsoft.WindowsAzure.Storage.File.CloudFile fr = clouddir.GetFileReference(dirs[^1]);
                    model.Result = fr.DownloadTextAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                }
            } catch (Exception ex) {
                model.ErrorMessage = $"File Share Error : {ex.Message}";
            }

            model.Skip = false;
            return View(model);
        }
        public IActionResult Table(StorageModel model) {
            try {
                if (!model.Skip) {
                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse($"DefaultEndpointsProtocol=https;AccountName={model.StrageAccountName};AccountKey={model.Key}");
                    CloudTableClient fc = storageAccount.CreateCloudTableClient();
                    CloudTable tr = fc.GetTableReference(model.TableName);
                    TableOperation to = TableOperation.Retrieve(model.TablePartition, model.TableKey);
                    TableResult res = tr.ExecuteAsync(to).ConfigureAwait(false).GetAwaiter().GetResult();
                    if (res.Result == null) {
                        model.Result = "(no record found)";
                    } else {
                        model.TableResult = new List<List<string>>();
                        List<string> H, R;
                        model.TableResult.Add(H = new List<string>());
                        model.TableResult.Add(R = new List<string>());
                        H.Add("PartitionKey");
                        R.Add(model.TablePartition);
                        H.Add("Key");
                        R.Add(model.TableKey);
                        if (res.Result is DynamicTableEntity dt) {
                            foreach (KeyValuePair<string, EntityProperty> pp in dt.Properties) {
                                H.Add(pp.Key);
                                R.Add(pp.Value.StringValue);
                            }
                            model.Result = $"{dt.Properties.Count} properties are found.";
                        } else {
                            string json = JsonConvert.SerializeObject(res.Result);
                            model.Result = json ?? "(null)";
                        }
                    }
                }
            } catch (Exception ex) {
                model.ErrorMessage = $"Table Handling Error : {ex.Message}";
            }

            model.Skip = false;
            return View(model);
        }
        public IActionResult Queue(StorageModel model) {
            try {
                if (!model.Skip) {
                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse($"DefaultEndpointsProtocol=https;AccountName={model.StrageAccountName};AccountKey={model.Key}");
                    CloudQueueClient qc = storageAccount.CreateCloudQueueClient();
                    CloudQueue qr = qc.GetQueueReference(model.QueueName);
                    if (model.QueueSend) {
                        qr.AddMessageAsync(new CloudQueueMessage(model.QueueSendMessage)).ConfigureAwait(false).GetAwaiter().GetResult();
                        model.Result = $"Enqueue '{model.QueueSendMessage}' to {model.QueueName}";
                    } else {
                        CloudQueueMessage mes = qr.GetMessagesAsync(1).ConfigureAwait(false).GetAwaiter().GetResult()?.FirstOrDefault();
                        if (mes != null) {
                            qr.DeleteMessageAsync(mes).ConfigureAwait(false).GetAwaiter().GetResult();
                            model.Result = mes.AsString == null ? "(null)" : mes.AsString == "" ? "(empty)" : mes.AsString;
                        } else {
                            model.Result = "(no message in the queue)";
                        }
                    }
                }
            } catch (Exception ex) {
                model.ErrorMessage = $"Queue Handling Error : {ex.Message}";
            }

            model.QueueSend = false;
            model.Skip = false;
            return View(model);
        }
    }
}
