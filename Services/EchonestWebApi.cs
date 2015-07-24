using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.AccessControl;
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
        
        // http://developer.echonest.com/api/v4/song/search?api_key=FILDTEOIK2HBORODV&format=json&results=1&artist=radiohead&title=karma%20police&bucket=audio_summary
        public void FindSong(String artist, String songName)
        {
            //Get method
            String getSongMethod = "song/search";
            String bucket = "audio_summary";
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

        // http://developer.echonest.com/api/v4/artist/profile?api_key=FHQ6TRBXVJFKSDOUS&id=ARH6W4X1187B99274F&format=json&bucket=genre
        public void GetArtist(String artistID)
        {
            
        }

        // http://developer.echonest.com/api/v4/artist/list_genres?api_key=FHQ6TRBXVJFKSDOUS&format=json
        public EchonestAllGeneresResponse GetAllGeneres()
        {
            //Get method
            String getSongMethod = "artist/list_genres";
            String bucket = "audio_summary";
            String parameters =
                String.Format("?api_key={0}&format={1}", apiKey, format);

            WebRequest req = WebRequest.Create(baseUrl + getSongMethod + parameters);

            req.Method = "GET";

            HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                using (Stream respStream = resp.GetResponseStream())
                {
                    DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(EchonestAllGeneresResponse));
                    object objResponse = jsonSerializer.ReadObject(resp.GetResponseStream());
                    EchonestAllGeneresResponse echonestAllGeneresResponse
                    = objResponse as EchonestAllGeneresResponse;
                    return echonestAllGeneresResponse;
                }
            }
            return null;
        }
    }

    [DataContract]
    public class EchonestSong
    {
        [DataMember(Name = "copyright")]
        public String Title { get; set; }

        [DataMember(Name = "copyright")]
        public double Energy { get; set; }

        [DataMember(Name = "copyright")]
        public double Liveness { get; set; }

        [DataMember(Name = "copyright")]
        public double Tempo { get; set; }

        [DataMember(Name = "copyright")]
        public double Speechiness { get; set; }

        [DataMember(Name = "copyright")]
        public double Duration { get; set; }

        [DataMember(Name = "copyright")]
        public double Loudness { get; set; }

        [DataMember(Name = "copyright")]
        public double Danceability { get; set; }
    }

    public class EchonestArtist
    {
        public String Title { get; set; }
        public List<String> Genres { get; set; }
    }

    [DataContract]
    public class EchonestAllGeneresResponse
    {
        [DataMember(Name = "generes")]
        public EchonestGenre[] AllGenres { get; set; }
    }

    [DataContract]
    public class EchonestGenre
    {
        [DataMember(Name = "name")]
        public String Name { get; set; }
    }
}
