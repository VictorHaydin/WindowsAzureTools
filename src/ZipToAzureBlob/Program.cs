using System;
using System.Diagnostics;
using System.IO;
using CommandLine;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace Eleks.WindowsAzure.Tools.ZipToAzureBlob
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            ICommandLineParser parser = new CommandLineParser();
            var options = new ProgramOptions();

            if (!parser.ParseArguments(args, options, Console.Error))
            {
                return 1;
            }

            // Archiving the source to a temporary file.
            string zipFileName = Path.GetTempFileName();
            ZipCompress(options.SourcePath, zipFileName, options.ArchivePassword, options.CompressionLevel);

            // Uploading the archive to Azure Storage.
            UploadFileToBlobStorage(zipFileName, options);

            // Removing the temporary file.
            try
            {
                File.Delete(zipFileName);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }

            return 0;
        }

        private static void ZipCompress(string inputFileOrFolder, string outputFileName, string password, int compressionLevel)
        {
            if (compressionLevel < 0 || compressionLevel > 9)
            {
                throw new ArgumentException("Compression level should be in the [0-9] range", "compressionLevel");
            }

            FileStream outFileStream = File.Create(outputFileName);
            ZipOutputStream zipStream = new ZipOutputStream(outFileStream);

            zipStream.SetLevel(compressionLevel);
            zipStream.Password = password;

            if (Directory.Exists(inputFileOrFolder))
            {
                DirectoryInfo inputDirInfo = new DirectoryInfo(inputFileOrFolder);
                InternalZipCompressFolder(inputDirInfo.FullName, zipStream, inputDirInfo.FullName);
            }
            else if (File.Exists(inputFileOrFolder))
            {
                FileInfo inputFileInfo = new FileInfo(inputFileOrFolder);
                InternalZipCompressFile(inputFileInfo.FullName, zipStream, inputFileInfo.DirectoryName);
            }

            // Makes the Close() also close the underlying stream.
            zipStream.IsStreamOwner = true;
            zipStream.Close();
        }

        private static void InternalZipCompressFile(string inputFileName, ZipOutputStream zipStream, string ownerFolderPath)
        {
            FileInfo fileInfo = new FileInfo(inputFileName);

            string entryName = inputFileName.Substring(ownerFolderPath.Length + 1);
            entryName = ZipEntry.CleanName(entryName);

            ZipEntry newEntry = new ZipEntry(entryName);
            newEntry.DateTime = fileInfo.LastWriteTime;
            newEntry.Size = fileInfo.Length;

            zipStream.PutNextEntry(newEntry);

            // Zip the file in buffered chunks.
            // The the stream will be closed even if an exception occurs.
            byte[] buffer = new byte[4096];
            using (FileStream streamReader = File.OpenRead(inputFileName))
            {
                StreamUtils.Copy(streamReader, zipStream, buffer);
            }

            zipStream.CloseEntry();
        }

        private static void InternalZipCompressFolder(string path, ZipOutputStream zipStream, string ownerFolderPath)
        {
            string[] files = Directory.GetFiles(path);
            foreach (string filename in files)
            {
                InternalZipCompressFile(filename, zipStream, ownerFolderPath);
            }

            string[] folders = Directory.GetDirectories(path);
            foreach (string folder in folders)
            {
                InternalZipCompressFolder(folder, zipStream, ownerFolderPath);
            }
        }

        private static void UploadFileToBlobStorage(string fileName, ProgramOptions options)
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
            CloudBlobContainer container = blobClient.GetContainerReference(options.DestinationBlobContainerName);

            // Retrieve reference to a blob.
            string blobName = GetBlobNameForUpload(options.DestinationBlobName, options.UseTimestampInBlobName);
            CloudBlob blob = container.GetBlobReference(blobName);

            // Create or overwrite the blob with contents from a local file.
            using (var fileStream = File.OpenRead(fileName))
            {
                blob.UploadFromStream(fileStream);
            }
        }

        private static string GetBlobNameForUpload(string preferredBlobName, bool appendTimestamp)
        {
            if (!appendTimestamp)
            {
                return preferredBlobName;
            }

            string blobNameWithoutExtension = preferredBlobName;
            string blobExtension = string.Empty;

            int dotIndex = preferredBlobName.IndexOf(".", StringComparison.Ordinal);
            if (dotIndex >= 0)
            {
                blobNameWithoutExtension = preferredBlobName.Substring(0, dotIndex);
                blobExtension = preferredBlobName.Substring(dotIndex);
            }

            return string.Format
            (
                "{0}-{1:yyyyMMdd}-T{1:HHmmss}-UTC{2}",
                blobNameWithoutExtension,
                DateTime.UtcNow,
                blobExtension
            );
        }
    }
}
