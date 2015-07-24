using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyNotificationParserForExcel.Model
{
    public class SpotifyNotification
    {
        public DateTime DateTime { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public String SongTitle { get; set; }
        public String Artist { get; set; }
        public int Duration { get; set; }
        public int AlbumYearRelease { get; set; }
        public List<String> Genres { get; set; }
    }
}
