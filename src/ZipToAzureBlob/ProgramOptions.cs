using System;
using System.Reflection;
using CommandLine;
using CommandLine.Text;

namespace Eleks.WindowsAzure.Tools.ZipToAzureBlob
{
    internal class ProgramOptions : CommandLineOptionsBase
    {
        public ProgramOptions()
        {
            CompressionLevel = 9;
        }

        [Option(null, "source-path", Required = true, HelpText = "File or folder to be archived to Windows Azure Blob Storage.")]
        public string SourcePath { get; set; }

        [Option(null, "compression-level", Required = false, HelpText = "Optional. Zip compression level [0..9] (0 - none, 9 - best). The default is 9.")]
        public int CompressionLevel { get; set; }

        [Option(null, "archive-password", Required = false, HelpText = "Optional. Password to the Zip archive.")]
        public string ArchivePassword { get; set; }

        [Option(null, "storage-account", Required = true, HelpText = "Windows Azure Storage account name.")]
        public string StorageAccountName { get; set; }

        [Option(null, "storage-account-key", Required = true, HelpText = "Storage account access key.")]
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
