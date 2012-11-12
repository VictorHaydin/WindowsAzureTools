using System;
using System.Reflection;
using CommandLine;
using CommandLine.Text;

namespace Eleks.WindowsAzure.Tools.DownloadAzureBlob
{
    internal class ProgramOptions : CommandLineOptionsBase
    {
        [Option(null, "storage-account", Required = true, HelpText = "Windows Azure Storage account name.")]
        public string StorageAccountName { get; set; }

        [Option(null, "storage-account-key", Required = true, HelpText = "Storage account access key.")]
        public string StorageAccountKey { get; set; }

        [Option(null, "blob-container", Required = true, HelpText = "Destination blob container name.")]
        public string BlobContainerName { get; set; }

        [Option(null, "blob-name", Required = true, HelpText = "Destination blob name.")]
        public string BlobName { get; set; }

        [Option(null, "destination-file-name", Required = false, HelpText = "Optional. Destination file name (can include path). If omitted, the name of the blob will be used.")]
        public string DestinationFileName { get; set; }

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

            return help;
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
