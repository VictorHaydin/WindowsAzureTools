using System;
using CommandLine;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace Eleks.WindowsAzure.Tools.DownloadAzureBlob
{
    public class Program
    {
        public static int Main(string[] args)
        {
            ICommandLineParser parser = new CommandLineParser();
            var options = new ProgramOptions();

            if (!parser.ParseArguments(args, options, Console.Error))
            {
                return 1;
            }

            DownloadAzureBlob(options);

            return 0;
        }

        private static void DownloadAzureBlob(ProgramOptions options)
        {
            string connectionString = string.Format
            (
                "DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}",
                options.StorageAccountName,
                options.StorageAccountKey
            );

            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference(options.BlobContainerName);

            // Retrieve reference to a blob.
            CloudBlob blob = container.GetBlobReference(options.BlobName);

            // Download blob as a file.
            string destFileName = string.IsNullOrEmpty(options.DestinationFileName) ? options.BlobName : options.DestinationFileName;
            blob.DownloadToFile(destFileName);
        }
    }
}
