using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel;
using SpotifyNotificationParserForExcel.Factory;

namespace SpotifyNotificationParserForExcel
{
    class Program
    {
        static void Main(string[] args)
        {
            string projectPath = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
            string exelFilePath = projectPath +
                @"\res\LaurynasStasysSpotify - removedDuplicatedRows.xlsx";

            SpotifyNotificationFactory spotifyNotificationFactory = new SpotifyNotificationFactory();
            var worksheet = Workbook.Worksheets(exelFilePath).First();
            var rows = worksheet.Rows.Skip(1);
            var spotifyNotifications = spotifyNotificationFactory.CreatemultipleFrom(rows);

            Console.ReadLine();
        }
    }
}
