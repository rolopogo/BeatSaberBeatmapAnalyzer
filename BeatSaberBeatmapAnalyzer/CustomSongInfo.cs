using System;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;

namespace BeatSaberBeatmapAnalyzer
{
    [Serializable]
    public class CustomSongInfo
    {
        public string songName;
        public string songSubName;
        public string authorName;
        public float beatsPerMinute;
        public float previewStartTime;
        public float previewDuration;
        public string environmentName;
        public string coverImagePath;
        public string videoPath;
        public DifficultyLevel[] difficultyLevels;
        public string path;
        public string levelId;

        [Serializable]
        public class DifficultyLevel
        {
            public string difficulty;
            public int difficultyRank;
            public string audioPath;
            public string jsonPath;
            public string json;
        }

        public CustomSongInfo (string songPath)
        {
            var infoText = File.ReadAllText(songPath + "/info.json");
            path = songPath;
            
            var diffLevels = new List<DifficultyLevel>();
            var n = JSON.Parse(infoText);

            songName = n["songName"];
            songSubName = n["songSubName"];
            authorName = n["authorName"];
            beatsPerMinute = n["beatsPerMinute"];
            previewStartTime = n["previewStartTime"];
            previewDuration = n["previewDuration"];
            environmentName = n["environmentName"];
            coverImagePath = n["coverImagePath"];
            videoPath = n["videoPath"];
            levelId = n["levelId"];

            var diffs = n["difficultyLevels"];
            for (int i = 0; i < diffs.AsArray.Count; i++)
            {
                n = diffs[i];
                diffLevels.Add(new CustomSongInfo.DifficultyLevel()
                {
                    difficulty = n["difficulty"],
                    difficultyRank = n["difficultyRank"].AsInt,
                    audioPath = n["audioPath"],
                    jsonPath = n["jsonPath"]
                });
            }

            difficultyLevels = diffLevels.ToArray();
        }
    }
}