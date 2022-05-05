using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml;


namespace TMERD
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
        static string mapPath = "";
        static string? mapFolderPath;
        static string folderPrefix = "TMERMD";
        static string etags = "";
        static List<int> ChoosenTags = new List<int>();
        static List<int> eTags = new List<int>();
        static List<Tag> tags = new List<Tag>();
        static List<string> TMERMD_Folders = new List<string>();
        static int mapCount;
        static HttpClient httpClient = new HttpClient();
        static Random rand;
        static string path = Environment.CurrentDirectory;

        static Config config;
        static XmlTextReader reader;

        static void Main(string[] args)
        {
            rand = new Random(Guid.NewGuid().GetHashCode());
            RunAsync().GetAwaiter().GetResult();
        }

        static void DecodeTagString(string tagString)
        {
            var stringArray = tagString.Split("%2C");
            foreach (var str in stringArray)
            {
                eTags.Add(Convert.ToInt32(str));
            }
        }
        static void drawTags()
        {
            int row = 2;
            foreach (var tag in tags)
            {
                if (!eTags.Contains(tag.ID))
                {

                    if (tag.ID > 23)
                    {
                        Console.SetCursorPosition(23, row);
                        Console.WriteLine($"{tag.ID} = {tag.Name}");
                        row++;
                    }
                    else
                    {
                        Console.WriteLine($"{tag.ID} = {tag.Name}");
                    }
                }
            }

            Console.SetCursorPosition(0, 25);
        }

        static async void drawMenu()
        {
            bool menu = true;
            int input;

            if (!File.Exists(@$"{path}\config.json"))
            {

                bool createFolderActive;
                do
                {
                    createFolderActive = false;

                    Console.Clear();
                    Console.WriteLine("Couldnt find the config.json on your system. Lets create it:");
                    Console.WriteLine("1. Copy the Path of the map Folder of your Trackmania installation.");
                    Console.WriteLine("2. Paste it here: ");
                    Console.Write("Path: ");
                    mapFolderPath = Console.ReadLine();

                    if (!Directory.Exists(mapFolderPath))
                    {
                        Console.WriteLine("Path could not be found! \n");
                        Console.WriteLine(mapFolderPath);
                        Console.WriteLine("\n Please try again!");
                        Console.ReadKey();
                        createFolderActive = true;
                    }

                } while (createFolderActive);

                do
                {
                    Console.Clear();
                    Console.WriteLine("Now you can set Tags that should be excluded.");

                    if (eTags.Count != 0)
                    {
                        var eTagstring = "";
                        eTags.ForEach(t =>
                            {
                                eTagstring += t + " ,";
                            }
                        );

                        eTagstring = RemoveCharsEnd(eTagstring, 2);

                        Console.Write($"Excluded Tags: {eTagstring}");
                        Console.Write("\n");
                    }

                    drawTags();

                    Console.Write("Input: ");
                    input = Convert.ToInt32(Console.ReadLine());
                    if (eTags.Contains(input))
                    {
                        Console.WriteLine("");
                        Console.WriteLine("Tag bereits benutzt!");
                    }
                    else
                    {
                        eTags.Add(input);
                    }

                } while (input != 0);

                var etags = BuildTagString(eTags);

                config = new Config()
                {
                    mapFolder = @$"{mapFolderPath}\",
                    excludedTags = etags
                };

                string json = JsonConvert.SerializeObject(config);
                File.WriteAllText(@$"{path}\config.json", json);


                mapPath = @$"{mapFolderPath}\";

            }
            else
            {
                var json = File.ReadAllText(@$"{path}\config.json");
                var config = JsonConvert.DeserializeObject<Config>(json);

                if (config != null)
                {
                    mapPath = config.mapFolder;
                    etags = config.excludedTags;
                }
                else
                {
                    Console.WriteLine("Error Reading config! Delete config.json from Application folder and try again!");
                    Console.ReadKey();
                    Environment.Exit(0);
                }

                DecodeTagString(etags);
            }

            Console.Clear();

            
            foreach (var folder in Directory.GetDirectories(mapPath))
            {
                string folderName = getFolderNames(folder);
                if (folderName.StartsWith(folderPrefix))
                {
                    TMERMD_Folders.Add(folder);
                }
            }

            if (TMERMD_Folders.Count > 0)
            {
                Console.WriteLine($"{TMERMD_Folders.Count} Folders of Random Maps already exisit");

                DrawTMERMDfolders();
                
                Console.WriteLine("\n");
                Console.WriteLine("Do you want to delete one or more?");
                Console.WriteLine("0 = All");
                Console.WriteLine("1 = Specific");
                Console.WriteLine("3 = None");
                Console.WriteLine("Input: ");
                input = Convert.ToInt32(Console.ReadLine());

                switch (input)
                {
                    case 0:
                        {
                            foreach (var folderPath in TMERMD_Folders)
                            {
                                Directory.Delete(folderPath, true);
                            }
                            break;
                        }
                    case 1:
                        {
                            bool delFolders = true;
                            do
                            {
                                if (TMERMD_Folders.Count != 0)
                                {
                                    Console.Clear();
                                    Console.WriteLine("Folders: ");
                                    DrawTMERMDfolders();

                                    Console.Write("Number of Folder (99 to end): ");
                                    input = Convert.ToInt32(Console.ReadLine());
                                    if (input != 99)
                                    {
                                        Directory.Delete(TMERMD_Folders[input], true);
                                        TMERMD_Folders.Remove(TMERMD_Folders[input]);
                                    }
                                    else
                                    {
                                        delFolders = false;
                                    }
                                }
                                else
                                {
                                    delFolders = false;
                                }
                            } while (delFolders);
                            break;
                        }
                    case 3:
                        {
                            break;
                        }
                }
            }

            Console.WriteLine(spacer);
            Console.WriteLine("Creating new folder");

            var time = DateTime.Now;
            var name = $"{folderPrefix} - {time.DayOfWeek}-{time.Year}_{time.Hour}.{time.Minute}.{time.Second}";
            Directory.CreateDirectory(@$"{mapPath}{name}");

            mapPath += name;

            Console.WriteLine($"Folder with name {name} created");
            Console.WriteLine(spacer);
            Console.Write("<Press Any Key to continue>");
            Console.ReadKey();
            do
            {
                Console.Clear();

                if (ChoosenTags.Count != 0)
                {
                    var choosenTagsString = "";
                    ChoosenTags.ForEach(t =>
                        {
                            choosenTagsString += t + " ,";
                        }
                    );

                    choosenTagsString = RemoveCharsEnd(choosenTagsString, 2);

                    Console.Write($"Tags: {choosenTagsString}");
                    Console.Write("\n");
                }
                else
                {
                    Console.WriteLine("Tags: (Default -> all)");
                }

                drawTags();
                Console.Write("Input: ");
                input = Convert.ToInt32(Console.ReadLine());
                if (ChoosenTags.Contains(input))
                {
                    Console.WriteLine("");
                    Console.WriteLine("Tag bereits benutzt!");
                }
                else
                {
                    ChoosenTags.Add(input);
                }

            } while (input != 0);

            Console.WriteLine("How many maps? ");
            mapCount = Convert.ToInt16(Console.ReadLine());
        }

        private static void DrawTMERMDfolders()
        {
            int i = 0;
            foreach (var folder in TMERMD_Folders)
            {

                Console.WriteLine($"{i} {getFolderNames(folder)}");
            }
        }

        private static string getFolderNames(string folder)
        {
            var StrArr = folder.Split("\\");
            var folderName = StrArr[StrArr.Length - 1];
            return folderName;
        }

        static async Task RunAsync()
        {
            httpClient.BaseAddress = new Uri(uri);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.132 Safari/537.36");

            tags = GetTags().Result;
            drawMenu();


            Console.Clear();
            Console.WriteLine(spacer);
            Console.WriteLine("Building Tagstring");

            string tagsString = "";
            tagsString = BuildTagString(ChoosenTags);


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

        private static string BuildTagString(List<int> tagList)
        {
            string tagsString = "";
            tagList.ForEach(t =>
            {
                if (t != 0)
                    tagsString += $"{t}%2C";
            });

            if (tagsString != "")
                tagsString = tagsString.Remove(tagsString.Length - 3, 3);

            return tagsString;
        }

        static async Task<List<Tag>> GetTags()
        {
            var uri = "/api/tags/gettags";
            var response = await httpClient.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var tagString = await response.Content.ReadAsStringAsync();
                var tags = JsonConvert.DeserializeObject<List<Tag?>>(tagString);
                return tags;
            }
            else
                return null;
        }

        static async Task DownloadMap(string? id, string? mapName)
        {
            var response = await httpClient.GetAsync($"/maps/download/{id}");
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

        private static string RemoveCharsEnd(string str, int noOfChars)
        {
            return str.Remove(str.Length - noOfChars, noOfChars);
        }
    }
}