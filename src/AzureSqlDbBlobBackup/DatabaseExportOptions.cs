using System;
using System.Reflection;
using CommandLine;
using CommandLine.Text;

namespace Eleks.WindowsAzure.Tools.AzureSqlDbBlobBackup
{
    internal class DatabaseExportOptions : CommandLineOptionsBase
    {
        [Option(null, "dac-service-url", Required = true, HelpText = "DACWebService endpoint URL.")]
        public string DacServiceUrl { get; set; }

        [Option(null, "db-server-name", Required = true, HelpText = "WASQLDB server name.")]
        public string DatabaseServerName { get; set; }

        [Option(null, "db-name", Required = true, HelpText = "WASQLDB name.")]
        public string DatabaseName { get; set; }

        [Option(null, "db-username", Required = true, HelpText = "WASQLDB username.")]
        public string DatabaseUserName { get; set; }

        [Option(null, "db-password", Required = true, HelpText = "WASQLDB password.")]
        public string DatabasePassword { get; set; }

        [Option(null, "storage-account", Required = true, HelpText = "Windows Azure Storage account name.")]
        public string StorageAccountName { get; set; }

        [Option(null, "storage-account-key", Required = true, HelpText = "Windows Azure Storage account key.")]
        public string StorageAccountKey { get; set; }

        [Option(null, "blob-container", Required = true, HelpText = "Destination blob container name.")]
        public string DestinationBlobContainerName { get; set; }

        [Option(null, "blob-name", Required = true, HelpText = "Destination blob name.")]
        public string DestinationBlobName { get; set; }

        [Option(null, "append-timestamp", Required = false, HelpText = "If specified, a timestamp will be appended to the blob name.")]
        public bool UseTimestampInBlobName { get; set; }

        [HelpOption(HelpText = "Display this help screen.")]
        public string GetUsageHelp()
        {
            var help = new HelpText
            {
                Heading = GetProductName(),
                Copyright = GetProductCopyright(),
                AddDashesToOption = true,
                AdditionalNewLineAfterOption = true,
                MaximumDisplayWidth = 75,
            };
            
            help.RenderParsingErrorsText(this, 0);
            help.AddOptions(this);

            return help + Environment.NewLine + Resources.HelpPostOptionsText;
        }

        private string GetProductName()
        {
            string productName = ((AssemblyProductAttribute)GetFirstAssemblyAttribute(typeof(AssemblyProductAttribute))).Product;
            return productName;
        }

        private string GetProductCopyright()
        {
            string copyright = ((AssemblyCopyrightAttribute)GetFirstAssemblyAttribute(typeof(AssemblyCopyrightAttribute))).Copyright;
            return copyright;
        }

        private Attribute GetFirstAssemblyAttribute(Type attributeType)
        {
            return (Attribute)(Assembly.GetEntryAssembly().GetCustomAttributes(attributeType, false)[0]);
        }
    }
}
