using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyNotificationParserForExcel.Services
{
    public class EchonestWebApi
    {
        private readonly String baseUrl = "http://developer.echonest.com/api/v4/";
        private readonly String apiKey = "FHQ6TRBXVJFKSDOUS";
        private readonly int results = 1;
        private readonly String format = "json";
        private readonly String bucket = "audio_summary";

        public void FindSong(String artist, String songName)
        {
            //Get method
            String getSongMethod = "song/search";
            String parameters =
                String.Format("?api_key={0}&format={1}&results={2}&artist={3}&title={4}&bucket={5}", apiKey, format, results, artist, songName, bucket);

            WebRequest req = WebRequest.Create(baseUrl + getSongMethod + parameters);

            req.Method = "GET";

            HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                using (Stream respStream = resp.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(respStream, Encoding.UTF8);
                    Console.WriteLine(reader.ReadToEnd());
                }
            }
            else
            {
                Console.WriteLine(string.Format("Status Code: {0}, Status Description: {1}", resp.StatusCode, resp.StatusDescription));
            }
            Console.Read();
        }
    }
}
