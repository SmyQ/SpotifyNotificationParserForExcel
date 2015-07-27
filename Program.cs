using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SpotifyNotificationParserForExcel.Factory;
using SpotifyNotificationParserForExcel.Model;
using SpotifyNotificationParserForExcel.Services;

namespace SpotifyNotificationParserForExcel
{
    class Program
    {
        static void Main(string[] args)
        {
            string projectPath = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
            string exelFilePath = projectPath +
                @"\res\LaurynasStasysSpotify - sample.xlsx";
            string outputExelFilePath = projectPath +
                @"\res\LaurynasStasysSpotify - final.xlsx";

            SpotifyNotificationFactory spotifyNotificationFactory = new SpotifyNotificationFactory();            
            EchonestWebApi echonestWebApi = new EchonestWebApi();
            EchonestAllGeneres echonestAllGeneres = echonestWebApi.GetAllGeneres();
            List<String> allGenres = echonestAllGeneres.Response.AllGenres.Select(g => g.Name).ToList();
            List<SpotifyNotification> spotifyNotifications = new List<SpotifyNotification>();

            IWorkbook workbook = new XSSFWorkbook(exelFilePath);
            ISheet worksheet = workbook.GetSheetAt(0);
            var enumerator = worksheet.GetRowEnumerator();
            enumerator.MoveNext();
            IRow row = (IRow)enumerator.Current;

            while (enumerator.MoveNext() && enumerator.Current != null)
            {
                row = (IRow) enumerator.Current;
                var spotifyNotification = spotifyNotificationFactory.CreateSingleFrom(row);
                EchonestSongSearch echonestSongSearch = echonestWebApi.FindSong(spotifyNotification.Artist,
                    spotifyNotification.SongTitle);
                if (echonestSongSearch != null && echonestSongSearch.Response.Songs.Any())
                {                  
                    var song = echonestSongSearch.Response.Songs.First();
                    EchonestArtistProfile echonestArtistProfile = echonestWebApi.GetArtist(song.ArtistId);
                    row.Cells[4].SetCellValue(song.AudioSummary.Duration);
                    if (echonestArtistProfile != null)
                    {
                        spotifyNotification.Genres = echonestArtistProfile.Response.Artist.Genres.Select(g => g.Name).ToList();
                    }
                }
                spotifyNotifications.Add(spotifyNotification);
            }

            List<String> usedGenres = new List<string>();
            spotifyNotifications.ForEach(noti => usedGenres.AddRange(noti.Genres));
            usedGenres = usedGenres.Distinct().OrderBy(x => x).ToList();

            row = worksheet.GetRow(0);
            foreach (var usedGenre in usedGenres)
            {
                ICell cell = row.CreateCell(row.LastCellNum);
                cell.SetCellValue("[genre]" + usedGenre);
            }

            foreach (SpotifyNotification spotifyNotification in spotifyNotifications)
            {
                row = spotifyNotification.Row;
                foreach (var usedGenre in usedGenres)
                {
                    bool genereFlag = spotifyNotification.Genres.Contains(usedGenre);
                    ICell cell = row.CreateCell(row.LastCellNum);
                    cell.SetCellValue(genereFlag ? "True" : "False");
                }
            }

            FileStream sw = File.Create(outputExelFilePath); 
            workbook.Write(sw); 
            sw.Close();

            //Console.ReadLine();
        }
    }
}
