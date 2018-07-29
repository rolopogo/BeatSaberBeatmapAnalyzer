using System.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;
namespace BeatSaberBeatmapAnalyzer
{
    public static class SongLoader
    {
        public static List<CustomSongInfo> RetrieveAllSongs(string directory)
        {
            var customSongInfos = new List<CustomSongInfo>();
            var path = directory;
            path = path.Replace('\\', '/');

            var currentHashes = new List<string>();

            var songFolders = Directory.GetDirectories(path + "/CustomSongs").ToList();

            foreach (var song in songFolders)
            {
                var results = Directory.GetFiles(song, "info.json", SearchOption.AllDirectories);
                if (results.Length == 0)
                {
                    Console.WriteLine("Custom song folder '" + song + "' is missing info.json!");
                    continue;
                }

                foreach (var result in results)
                {
                    var songPath = Path.GetDirectoryName(result).Replace('\\', '/');
                    var customSongInfo = new CustomSongInfo(songPath);
                    if (customSongInfo == null) continue;
                    customSongInfos.Add(customSongInfo);
                }
            }
            return customSongInfos;
        }
        

        public static BeatMap GetBeatMap(CustomSongInfo song, CustomSongInfo.DifficultyLevel diffLevel)
        {
            if (!File.Exists(song.path + "/" + diffLevel.jsonPath)) return null;
            if (song.songName == "TTFAF")
            {
                Console.WriteLine("heck");
            }
            var json = File.ReadAllText(song.path + "/" + diffLevel.jsonPath);
            return new BeatMap(json);
        }

        public static BeatMap GetBeatMap(string path)
        {
            if (!File.Exists(path)) return null;

            var json = File.ReadAllText(path);
            return new BeatMap(json);
        }
    }
}