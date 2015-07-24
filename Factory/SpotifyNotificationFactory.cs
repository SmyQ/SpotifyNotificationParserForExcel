using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpotifyNotificationParserForExcel.Model;
using Excel;

namespace SpotifyNotificationParserForExcel.Factory
{
    public class SpotifyNotificationFactory
    {
        public SpotifyNotification CreateSingleFrom(Row row)
        {
            return new SpotifyNotification()
            {
                SongTitle = row.Cells[2].Text,
                Artist = row.Cells[3].Text
            };
        }

        public IEnumerable<SpotifyNotification> CreatemultipleFrom(IEnumerable<Row> rows)
        {
            foreach (Row row in rows)
            {
                yield return CreateSingleFrom(row);
            }
        } 
    }
}
