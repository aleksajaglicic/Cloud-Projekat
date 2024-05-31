using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.Queue;
using PortfolioService.Repository;
using PortfolioService.Model;
using System.Drawing;
using System.Threading;
using Microsoft.WindowsAzure.Storage;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;
using Microsoft.WindowsAzure.ServiceRuntime;
using Common;

namespace ImageConverter_WorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            CloudQueue queue = QueueHelper.GetQueueReference("images");
            Trace.TraceInformation("ImageConverter_WorkerRole entry point called", "Information");

            while (true)
            {
                CloudQueueMessage message = queue.GetMessage();
                if (message == null)
                {
                    Trace.TraceInformation("Currently no messages in the queue.", "Information");
                }
                else
                {
                    Trace.TraceInformation($"Processing message: {message.AsString}", "Information");

                    if (message.DequeueCount > 3)
                    {
                        queue.DeleteMessage(message);
                    }

                    ResizeImage(message.AsString);

                    queue.DeleteMessage(message);
                    Trace.TraceInformation($"Message processed: {message.AsString}", "Information");
                }

                Thread.Sleep(5000);
                Trace.TraceInformation("Working", "Information");
            }
        }

        public override bool OnStart()
        {
            ServicePointManager.DefaultConnectionLimit = 12;
            bool result = base.OnStart();
            Trace.TraceInformation("ImageConverter_WorkerRole has been started");
            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("ImageConverter_WorkerRole is stopping");
            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();
            base.OnStop();
            Trace.TraceInformation("ImageConverter_WorkerRole has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");
                await Task.Delay(1000);
            }
        }

        public void ResizeImage(string email)
        {
            UserDataRepository sdr = new UserDataRepository();
            User user = sdr.RetrieveUser(email);
            if (user == null)
            {
                Trace.TraceInformation($"User with email {email} does not exist!", "Information");
                return;
            }

            BlobHelper blobHelper = new BlobHelper();
            string uniqueBlobName = string.Format("image_{0}", user.RowKey);

            Image image = blobHelper.DownloadImage("userfiles", uniqueBlobName);
            image = Common.ImageConverter.ConvertImage(image);
            string thumbnailUrl = blobHelper.UploadImage(image, "userfiles", uniqueBlobName + "thumb");

            user.ImgUrl = thumbnailUrl;
            sdr.UpdateUser(user);
        }
    }

    public static class QueueHelper
    {
        public static CloudQueue GetQueueReference(string queueName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference(queueName);
            queue.CreateIfNotExists();
            return queue;
        }
    }
}
