using System;
using System.IO;

namespace QuickbooksExporter.Models
{
    public class ExportSettings
    {
        public string Dsn { get; set; }
        public string OutputFolder { get; set; }
        public string OutputFileName { get; set; }

        public string OutputFilePath
        {
            get
            {
                var folder = string.IsNullOrWhiteSpace(OutputFolder) ? Environment.GetFolderPath(Environment.SpecialFolder.Desktop) : OutputFolder;
                var fileName = string.IsNullOrWhiteSpace(OutputFileName) ? "item_inventory.csv" : OutputFileName;
                return Path.Combine(folder, fileName);
            }
        }
    }
}