using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeatSaberBeatmapAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().Start();
            Console.ReadLine();
        }
        
        private Analyzer analyzer;
        private List<SongDifficulty> songDifficulties;
        private string rootPath = @"E:\Games\SteamLibrary\steamapps\common\Beat Saber\";

        private void Start()
        {
            analyzer = new Analyzer();
            songDifficulties = new List<SongDifficulty>();
            AnalyzeAll(rootPath);
            DisplayResult();
        }

        void AnalyzeAll(string rootPath)
        {
            List<CustomSongInfo> songInfos = SongLoader.RetrieveAllSongs(rootPath);
            foreach (CustomSongInfo song in songInfos)
            {
                if (song.songName != "Hit The Blocks") //This map causes an infinite loop, don't know why
                {
                    foreach (CustomSongInfo.DifficultyLevel difficulty in song.difficultyLevels)
                    {
                        BeatMap beatMap = SongLoader.GetBeatMap(song, difficulty);
                        if (beatMap == null) continue;
                        var metrics = analyzer.Analyze(beatMap);
                        SongDifficulty d = new SongDifficulty(song.songName, difficulty.difficulty, metrics);
                        songDifficulties.Add(d);
                    }
                }
            }
        }

        void Analyze(string path)
        {
            BeatMap beatMap = SongLoader.GetBeatMap(path);
            if (beatMap == null) return;
            var metrics = analyzer.Analyze(beatMap);
            SongDifficulty d = new SongDifficulty(path.Split('\\')[path.Split('\\').Length - 1], "-", metrics);
            songDifficulties.Add(d);
        }

        void DisplayResult()
        {
            songDifficulties = songDifficulties.OrderByDescending(o => o.difficulty).ToList();
            string result = String.Format("{0,-30} | {1,-10} | {2,-10} | {3,-10} | {4,-10} | {5,-10} | {6,-10} | {7,-10}\n", "Song Name", "Level", "Avg/Sec", "Max/Sec", "CutDist/Sec", "DirEntropy", "PosEntropy", "Difficulty");
            foreach (SongDifficulty d in songDifficulties)
            {
                result += String.Format("{0,-30} | {1,-10} | {2,-10} | {3,-10} | {4,-10} | {5,-10} | {6,-10} | {7,-10} \n",
                    d.songName,
                    d.difficultyName,
                    d.m.avgNotesPerSec,
                    d.m.maxNotesPerBarPerSec,
                    d.m.cutDistancePerSec,
                    d.m.cutDirectionEntropy,
                    d.m.notePosEntropy,
                    d.difficulty
                );
            }
            System.IO.File.WriteAllText("results.txt", result);
        }

        public class SongDifficulty
        {
            public string songName;
            public string difficultyName;
            public Analyzer.SongMetrics m;
            public float difficulty;

            public SongDifficulty(string songName, string difficultyName, Analyzer.SongMetrics m)
            {
                this.songName = songName;
                this.difficultyName = difficultyName;
                this.m = m;
                this.difficulty = ComputeDifficulty();
            }

            public float ComputeDifficulty()
            {
                return m.avgNotesPerSec * m.cutDistancePerSec;
            }
        }
    }
}
