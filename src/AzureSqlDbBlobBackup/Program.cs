using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using CommandLine;

namespace Eleks.WindowsAzure.Tools.AzureSqlDbBlobBackup
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            ICommandLineParser parser = new CommandLineParser();
            var options = new DatabaseExportOptions();

            if (!parser.ParseArguments(args, options, Console.Error))
            {
                return 1;
            }

            Guid operationGuid = ExportDatabase(options);
            Console.WriteLine("The database has been successfully exported. Operation GUID: " + operationGuid);
            return 0;
        }

        /// <summary>
        /// Exports a Windows Azure SQL Database to Blob Storage in BACPAC format.
        /// </summary>
        /// <param name="options">Database export options.</param>
        /// <returns>GUID of the operation.</returns>
        private static Guid ExportDatabase(DatabaseExportOptions options)
        {
            // To avoid HTTP 417 error on some DAC servers.
            ServicePointManager.Expect100Continue = false;

            string requestUrl = options.DacServiceUrl + "/Export";
            var request = WebRequest.Create(requestUrl);
            request.Method = "POST";
            request.ContentType = "application/xml";

            // Constructing the destination blob URL.
            string blobName = BuildBlobName(options.DestinationBlobName, options.UseTimestampInBlobName);
            string blobUrl = BuildBlobUrl(options.StorageAccountName, options.DestinationBlobContainerName, blobName);

            // Building the XML body for the backup request.
            using (Stream dataStream = request.GetRequestStream())
            {
                string requestBody = string.Format
                (
                    Resources.BackupRequestTemplate,
                    blobUrl,
                    options.StorageAccountKey,
                    options.DatabaseName,
                    options.DatabasePassword,
                    options.DatabaseServerName,
                    options.DatabaseUserName
                );
                byte[] buffer = Encoding.UTF8.GetBytes(requestBody);
                dataStream.Write(buffer, 0, buffer.Length);
            }

            // The HTTP response contains the job number, a XML-serialized GUID.
            using (WebResponse response = request.GetResponse())
            {
                Encoding encoding = Encoding.GetEncoding(1252);
                using (var responseStream = new StreamReader(response.GetResponseStream(), encoding))
                {
                    using (XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(responseStream.BaseStream, new XmlDictionaryReaderQuotas()))
                    {
                        DataContractSerializer serializer = new DataContractSerializer(typeof(Guid));
                        return (Guid)serializer.ReadObject(reader, true);
                    }
                }
            }
        }

        private static string BuildBlobName(string preferredBlobName, bool appendTimestamp)
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

        private static string BuildBlobUrl(string storageAccountName, string blobContainerName, string blobName)
        {
            string blobUrl = string.Format
            (
                "https://{0}.blob.core.windows.net/{1}/{2}",
                storageAccountName,
                blobContainerName,
                blobName
            );
            return blobUrl;
        }
    }
}