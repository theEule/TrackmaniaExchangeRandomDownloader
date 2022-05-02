using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace HelloWorld
{
    class Track
    {
        public string? TrackID { get; set; }
        public string? TrackName { get; set; }
        public bool Downloadable { get; set; }

    }


    class Program
    {
        static string spacer = "-------------------------------------------------------------";
        static string spacerEnd = "-------------------------------------------------------------\n\n";
        static string uri = "https://trackmania.exchange/";
        static string mapPath = @"C:\Users\loris\Documents\Trackmania\Maps\My Maps\";
        static string etags = "34%2C23%2C5%2C8%2C28%2C31%2C6%2C37%2C4%2C12%2C10";
        static List<int> tags = new List<int>();
        static int mapCount;
        static HttpClient httpClient = new HttpClient();
        static Random rand;
        static string path = Environment.CurrentDirectory;
        static void Main(string[] args)
        {

            bool menu = true;

            int input;


            Console.Clear();

            var time = DateTime.Now;
            var name = $"{time.DayOfWeek}-{time.Year}_{time.Hour}.{time.Minute}.{time.Second}";
            Directory.CreateDirectory(@$"{mapPath}{name}");

            mapPath += name;

            Console.WriteLine($"Folder with name {name} created");

            Console.ReadKey();

            do
            {
                Console.Clear();

                if (tags.Count != 0)
                {
                    Console.Write("Tags: ");
                    tags.ForEach(t =>
                        {
                            Console.Write(t + " ,");
                        }

                    );

                    Console.Write("\n");
                }
                else
                {
                    Console.WriteLine("Tags: (Default -> all)");
                }
                Console.WriteLine("Race = 1");
                Console.WriteLine("Tech = 3");
                Console.WriteLine("SpeedTech = 7");
                Console.WriteLine("Dirt = 15");
                Console.WriteLine("Grass = 33");
                Console.Write("Input (0 = done): ");
                input = Convert.ToInt32(Console.ReadLine());
                if (tags.Contains(input))
                {
                    Console.WriteLine("");
                    Console.WriteLine("Tag bereits benutzt!");
                }
                else
                {
                    tags.Add(input);
                }

            } while (input != 0);

            Console.WriteLine("How many maps? ");
            mapCount = Convert.ToInt16(Console.ReadLine());

            rand = new Random(Guid.NewGuid().GetHashCode());
            RunAsync().GetAwaiter().GetResult();
        }

        static async Task RunAsync()
        {

            httpClient.BaseAddress = new Uri(uri);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.132 Safari/537.36");


            Console.Clear();
            Console.WriteLine(spacer);
            Console.WriteLine("Building Tagstring");

            string tagsString = "";
            tags.ForEach(t =>
            {
                if (t != 0)
                    tagsString += $"{t}%2C";
            });

            if (tagsString != "")
                tagsString = tagsString.Remove(tagsString.Length - 3, 3);

            Console.WriteLine($"Tagstring build: {tagsString}");
            Console.WriteLine(spacerEnd);

            Console.WriteLine(spacer);
            Console.WriteLine("Getting Map count");
            var totalItemCount = GetTotalItemCount(tagsString, 100).Result;
            Console.WriteLine($"{totalItemCount} Maps found..");
            Console.WriteLine(spacerEnd);

            Console.WriteLine(spacer);
            Console.WriteLine("Getting Maps from random page with set tags");
            var tracks = GetMaps(totalItemCount, 100, tagsString).Result;

            Console.WriteLine("Got Maps!");
            Console.WriteLine(spacerEnd);

            if (tracks != null)
            {
                List<Track> trackList = new List<Track>();
                List<Track> RandomMapPack = new List<Track>();


                foreach (var result in tracks.results)
                {
                    var newTrack = new Track()
                    {
                        TrackID = result.TrackID.ToString(),
                        TrackName = result.GbxMapName,
                        Downloadable = result.Downloadable
                    };

                    trackList.Add(newTrack);
                }


                Console.WriteLine(spacer);
                Console.WriteLine($"Getting {mapCount} random maps");




                for (int i = 0; i < mapCount; i++)
                {
                    var RandomIndex = rand.Next(trackList.Count);
                    try
                    {
                        await DownloadMap(trackList[RandomIndex].TrackID, trackList[RandomIndex].TrackName);
                    }
                    catch (Exception)
                    {
                        string name = "DefaultName-" + Convert.ToString(rand.Next(1, 3000));
                        await DownloadMap(trackList[RandomIndex].TrackID, name);
                    }

                    Console.WriteLine(trackList[RandomIndex].TrackName + ".Map.Gbx Downloaded!");
                }
            }
            else
            {
                Console.WriteLine("No Maps found!");
            }

            Console.WriteLine("Process finished!");
            Console.WriteLine(spacerEnd);

            Console.ReadKey();
        }


        static async Task DownloadMap(string? id, string? mapName)
        {
            var response = await httpClient.GetAsync($"/tracks/download/{id}");
            using (var fs = new FileStream(@$"{mapPath}\{mapName}.Map.Gbx", FileMode.CreateNew))
            {
                await response.Content.CopyToAsync(fs);
            }
        }


        static async Task<Root?> GetMaps(int itemCount, int limit, string tags)
        {



            var pages = itemCount / 100.0;
            var pagesRound = Math.Round(pages, 0);

            if ((pages % 1) != 0)
            {
                pagesRound += 1;
            }
            Console.WriteLine($"Getting Random Page from {pagesRound} pages");

            var randomPage = rand.Next(1, Convert.ToInt32(pagesRound) + 1);



            var uri = CheckForTags(tags, limit);
            uri += $"&page={randomPage}";

            Console.WriteLine($"Getting maps from Page: {randomPage}");

            HttpResponseMessage response = await httpClient.GetAsync(
                uri);
            if (response.IsSuccessStatusCode)
            {
                var maps = await response.Content.ReadAsStringAsync();
                var tracks = JsonConvert.DeserializeObject<Root?>(maps);
                return tracks;
            }
            else
                return null;
        }

        static async Task<int> GetTotalItemCount(string tag, int limit)
        {
            string uri = CheckForTags(tag, limit);

            HttpResponseMessage response = await httpClient.GetAsync(
                uri);
            if (response.IsSuccessStatusCode)
            {
                var maps = await response.Content.ReadAsStringAsync();
                var tracks = JsonConvert.DeserializeObject<Root>(maps);
                var count = tracks.totalItemCount;
                return count;
            }
            else
                return 0;

        }

        private static string CheckForTags(string tag, int limit)
        {
            var uri = $"/mapsearch2/search?api=on&format=json&limit={limit}&etags={etags}";

            if (tag != "")
            {
                uri += $"&tags={tag}";
            }

            return uri;
        }

    }
}