using System;
using System.Configuration;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Text;
using QuickbooksExporter.Models;

namespace QuickbooksExporter
{
    class Program
    {
        static void Main()
        {
            try
            {
                var settings = GetExportSettings();

                Console.WriteLine("Retrieving Item Inventories from Quickbooks...");

                var itemInventories = GetItemInventories(settings);

                Console.WriteLine($"Found {itemInventories.Rows.Count} Item Inventories.");

                Console.WriteLine("Dumping to CSV...");

                DumpToCsv(settings, itemInventories);
            }
            catch (Exception ex)
            {
                LogError(ex);
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Press any key to exit...");

            Console.ReadKey();
        }

        static void DumpToCsv(ExportSettings settings, DataTable dt)
        {
            var sb = new StringBuilder();

            sb.AppendLine(string.Join("\t", dt.Columns.Cast<DataColumn>().Select(i => i.ColumnName).ToArray()));

            foreach (var row in dt.Rows.Cast<DataRow>())
            {
                sb.AppendLine(string.Join("\t", Enumerable.Range(0, dt.Columns.Count).Select(i => row[i].ToString())));
            }

            File.WriteAllText(settings.OutputFilePath, sb.ToString());
        }

        static DataTable GetItemInventories(ExportSettings settings)
        {
            var results = new DataTable();

            var cb = new OdbcConnectionStringBuilder { Dsn = settings.Dsn };
            using (var connection = new OdbcConnection(cb.ConnectionString))
            {
                connection.Open();

                using (var adapter = new OdbcDataAdapter("SELECT * FROM ItemInventory", connection))
                {
                    adapter.Fill(results);
                }

                connection.Close();
            }

            return results;
        }

        static ExportSettings GetExportSettings()
        {
            return new ExportSettings
            {
                Dsn = ConfigurationManager.AppSettings["OdbcDsn"],
                OutputFolder = ConfigurationManager.AppSettings["OutputFolder"],
                OutputFileName = ConfigurationManager.AppSettings["OutputFileName"]
            };
        }

        static void LogError(Exception ex)
        {
            File.AppendAllText("error.log", $"[{DateTime.Now}]: {ex}" + Environment.NewLine);
        }
    }
}
