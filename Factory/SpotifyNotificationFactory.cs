using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpotifyNotificationParserForExcel.Model;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;

namespace SpotifyNotificationParserForExcel.Factory
{
    public class SpotifyNotificationFactory
    {
        public SpotifyNotification CreateSingleFrom(IRow row)
        {
            return new SpotifyNotification()
            {
                SongTitle = row.Cells[2].StringCellValue,
                Artist = row.Cells[3].StringCellValue,
                Row = row,
                Genres = new List<String>()
            };
        }

        public IEnumerable<SpotifyNotification> CreatemultipleFrom(IEnumerable<IRow> rows)
        {
            foreach (IRow row in rows)
            {
                yield return CreateSingleFrom(row);
            }
        } 
    }
}
