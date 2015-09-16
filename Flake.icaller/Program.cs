using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Flake.csvReader;
using Flake.icaller.Properties;
using Flake.iCalMaker;

namespace Flake.icaller
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string data = string.Empty;
            string filepath = string.Empty;
            string outPath = string.Empty;

            if (args.Length > 0)
            {
                filepath = args[0];
            }
            else
            {
                string appPath = Path.GetDirectoryName(Application.ExecutablePath).TrimEnd('\\');
                filepath = appPath + @"\sourcedata\Terminplan 12-13RR.csv";
#if DEBUG
                filepath = @"e:\temp\termine.csv";
#endif
            }

            if (args.Length > 1)
            {
                outPath = args[1];
            }
            else
            {
                Console.WriteLine("Use: icaller.exe InputCSVFilePath OutPutICSPath");
#if DEBUG
                outPath = @"e:\temp\ical.ics";
#else
                return;
#endif
            }

            using (var tr = new StreamReader(filepath, Encoding.Default))
            {
                data = tr.ReadToEnd();
            }

            Settings x = Properties.Settings.Default;

            CsvReader reader = new CsvReader(data);
            MakeiCal maker = new MakeiCal(reader.GetTableData())
            {
                CalendarLocation = x.KalenderFreigabeOrt,
                UIDSuffix = x.KalenderUIDSuffix,
                HomeLocation = x.NameHeimatOrt,
                StandardDuration = x.StandardTerminDauerInStunden,
                GetTraining = x.TrainingsExportieren,
                GetTurkoTraining = x.TurkoTrainingsExportieren,
                GetTurniere = x.TurniereExportieren,
                NameFilter = x.FilterName,
                TeamFilter = x.FilterMannschaft,
            };
            string ical = maker.GetiCal();

            using (var tw = new StreamWriter(outPath))
            {
                tw.Write(ical);
            }

            Console.WriteLine("...done.");
        }
    }
}