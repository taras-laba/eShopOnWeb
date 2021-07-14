using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using OrderItemsReserver.Helpers;

namespace OrderItemsReserver
{
    public static class OrderReserver
    {
        [FunctionName("OrderReserver")]
        public static async Task Run([ServiceBusTrigger("OrderItems", Connection = "AzureWebJobsServiceBus")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");

            Order order = JsonConvert.DeserializeObject<Order>(myQueueItem);

            string cloudStorage = Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process);
            string containerName = Environment.GetEnvironmentVariable("ContainerName", EnvironmentVariableTarget.Process);

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(cloudStorage);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference($"{DateTime.UtcNow.ToUnixTimestamp()}.json");
            await blockBlob.UploadTextAsync(JsonConvert.SerializeObject(order));

         
        }
    }
}
