using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpotifyNotificationParserForExcel.Services
{
    public class EchonestRateLimit
    {
        public int Limit { get; set; }
        public int Used { get; set; }
        public int Remaining { get; set; }
    }

    public class EchonestWebApi
    {
        private readonly String baseUrl = "http://developer.echonest.com/api/v4/";
        private readonly String apiKey = "FHQ6TRBXVJFKSDOUS";
        private readonly int results = 1;
        private readonly String format = "json";

        private EchonestRateLimit rateLimit = new EchonestRateLimit()
        {
            Limit = 120,
            Used = 0,
            Remaining = 120
        };

        private T ExecuteRequest<T>(WebRequest req) where T : new()
        {
            Console.WriteLine(String.Format("X-RateLimit-Remaining = {0}", rateLimit.Remaining));
            if (rateLimit.Remaining == 0)
            {
                Console.WriteLine("Echonest remaining rate limit is 0, Thread sleep for minute");
                Thread.Sleep(1000 * 60);
                Console.WriteLine("Woken up");
            }

            try
            {
                HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream respStream = resp.GetResponseStream())
                    {
                        //StreamReader reader = new StreamReader(respStream, Encoding.UTF8);
                        //Console.WriteLine(reader.ReadToEnd());
                        rateLimit.Limit = Int32.Parse(resp.GetResponseHeader("X-RateLimit-Limit"));
                        rateLimit.Used = Int32.Parse(resp.GetResponseHeader("X-RateLimit-Used"));
                        rateLimit.Remaining = Int32.Parse(resp.GetResponseHeader("X-RateLimit-Remaining"));

                        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(T));
                        object objResponse = jsonSerializer.ReadObject(respStream);
                        T obj = (T) objResponse;
                        return obj;
                    }
                }
            }
            catch (Exception)
            {

            }

            return new T();
        }

        // http://developer.echonest.com/api/v4/song/search?api_key=FILDTEOIK2HBORODV&format=json&results=1&artist=radiohead&title=karma%20police&bucket=audio_summary
        public EchonestSongSearch FindSong(String artist, String songName)
        {
            //Get method
            String getSongMethod = "song/search";
            String bucket = "audio_summary";
            String parameters =
                String.Format("?api_key={0}&format={1}&results={2}&artist={3}&title={4}&bucket={5}", apiKey, format, results, Uri.EscapeDataString(artist), Uri.EscapeDataString(songName), bucket);

            String url = baseUrl + getSongMethod + parameters;
            WebRequest req = WebRequest.Create(url);

            req.Method = "GET";

            return ExecuteRequest<EchonestSongSearch>(req);
        }

        // http://developer.echonest.com/api/v4/artist/profile?api_key=FHQ6TRBXVJFKSDOUS&id=ARH6W4X1187B99274F&format=json&bucket=genre
        public EchonestArtistProfile GetArtist(String artistID)
        {
            //Get method
            String getSongMethod = "artist/profile";
            String bucket = "genre";
            String parameters =
                String.Format("?api_key={0}&id={1}&format={2}&bucket={3}", apiKey, artistID, format, bucket);

            WebRequest req = WebRequest.Create(baseUrl + getSongMethod + parameters);

            req.Method = "GET";

            return ExecuteRequest<EchonestArtistProfile>(req);
        }

        // http://developer.echonest.com/api/v4/artist/list_genres?api_key=FHQ6TRBXVJFKSDOUS&format=json
        public EchonestAllGeneres GetAllGeneres()
        {
            //Get method
            String getSongMethod = "artist/list_genres";
            String parameters =
                String.Format("?api_key={0}&format={1}", apiKey, format);

            WebRequest req = WebRequest.Create(baseUrl + getSongMethod + parameters);

            req.Method = "GET";

            return ExecuteRequest<EchonestAllGeneres>(req);
        }
    }

    [DataContract]
    public class EchonestSongSearch
    {
        [DataMember(Name = "response")]
        public EchonestSongSearchResponse Response { get; set; }
    }

    [DataContract]
    public class EchonestSongSearchResponse
    {
        [DataMember(Name = "songs")]
        public EchonestSong[] Songs { get; set; }
    }

    [DataContract]
    public class EchonestSong
    {
        [DataMember(Name = "artist_id")]
        public String ArtistId { get; set; }

        [DataMember(Name = "artist_name")]
        public String ArtistName { get; set; }

        [DataMember(Name = "id")]
        public String Id { get; set; }

        [DataMember(Name = "audio_summary")]
        public EchonesAudioSummary AudioSummary { get; set; }
    }

    [DataContract]
    public class EchonesAudioSummary
    {
        //key": 7,
        //            "analysis_url": "http://echonest-analysis.s3.amazonaws.com/TR/JE7qwwebHMSfiJXdcx8Vh8tqAstDg5Wq-Ow2t-W_4qeDN7bqzViLXcDJKpoZdIZNXHHPncjCmSzjddPI4%3D/3/full.json?AWSAccessKeyId=AKIAJRDFEY23UEVW42BQ&Expires=1437832272&Signature=SZQUpsBF/uN7BXo2pT2jF5HCvA8%3D",
        //            "energy": 0.67329,
        //            "liveness": 0.938715,
        //            "tempo": 75.233,
        //            "speechiness": 0.045268,
        //            "acousticness": 0.005678,
        //            "instrumentalness": 0.004337,
        //            "mode": 1,
        //            "time_signature": 4,
        //            "duration": 248.57288,
        //            "loudness": -7.867,
        //            "audio_md5": "68a69843763d9a96315330d2dcc71d72",
        //            "valence": 0.310738,
        //            "danceability": 0.128355

        [DataMember(Name = "energy")]
        public Double Energy { get; set; }

        [DataMember(Name = "liveness")]
        public Double Liveness { get; set; }

        [DataMember(Name = "tempo")]
        public Double Tempo { get; set; }

        [DataMember(Name = "speechiness")]
        public Double Speechiness { get; set; }

        [DataMember(Name = "acousticness")]
        public Double Acousticness { get; set; }

        [DataMember(Name = "instrumentalness")]
        public Double Instrumentalness { get; set; }

        [DataMember(Name = "duration")]
        public Double Duration { get; set; }

        [DataMember(Name = "loudness")]
        public Double Loudness { get; set; }

        [DataMember(Name = "danceability")]
        public Double Danceability { get; set; }
    }

    [DataContract]
    public class EchonestArtistProfile
    {
        [DataMember(Name = "response")]
        public EchonestArtistProfileResponse Response { get; set; }
    }

    [DataContract]
    public class EchonestArtistProfileResponse
    {
        [DataMember(Name = "artist")]
        public EchonestArtist Artist { get; set; }
    }

    [DataContract]
    public class EchonestArtist
    {
        [DataMember(Name = "name")]
        public String Name { get; set; }

        [DataMember(Name = "genres")]
        public EchonestGenre[] Genres { get; set; }

        [DataMember(Name = "id")]
        public String Id { get; set; }
    }

    [DataContract]
    public class EchonestAllGeneres
    {
        [DataMember(Name = "response")]
        public EchonestAllGeneresResponse Response { get; set; }
    }

    [DataContract]
    public class EchonestAllGeneresResponse
    {
        [DataMember(Name = "genres")]
        public EchonestGenre[] AllGenres { get; set; }
    }

    [DataContract]
    public class EchonestGenre
    {
        [DataMember(Name = "name")]
        public String Name { get; set; }
    }
}
