using System;
using System.Collections.Generic;
using SimpleJSON;

namespace BeatSaberBeatmapAnalyzer
{
    [Serializable]
    public class BeatMap
    {
        public string version; // format version
        public int beatsPerMinute; // track bpm
        public int beatsPerBar;
        public int noteJumpSpeed;
        public float shuffle;
        public float shufflePeriod;
        public BeatEvent[] events;
        public Note[] notes;
        public Obstacle[] obstacles;

        [Serializable]
        public class BeatEvent
        {
            public float time;
            public int type;
            public int value;
        }

        [Serializable]
        public class Obstacle
        {
            public float time;
            public int type;
            public float duration;
            public int width;
        }

        public BeatMap(string json)
        {
            var data = JSON.Parse(json);

            version = data["_version"];
            beatsPerMinute = data["_beatsPerMinute"];
            beatsPerBar = data["_beatsPerBar"];
            noteJumpSpeed = data["_noteJumpSpeed"];
            shuffle = data["_shuffle"];
            shufflePeriod = data["_shufflePeriod"];

            var obstaclesList = new List<Obstacle>();
            var obs = data["_obstacles"];
            for (int i = 0; i < obs.AsArray.Count; i++)
            {
                var obstacle = obs[i];
                obstaclesList.Add(new Obstacle()
                {
                    time = obstacle["_time"],
                    type = obstacle["_type"],
                    duration = obstacle["_duration"],
                    width = obstacle["_width"]
                });
            }
            obstacles = obstaclesList.ToArray();

            var eventsList = new List<BeatEvent>();
            var evs = data["_events"];
            for (int i = 0; i < evs.AsArray.Count; i++)
            {
                var beatEvent = evs[i];
                eventsList.Add(new BeatEvent()
                {
                    time = beatEvent["_time"],
                    type = beatEvent["_type"],
                    value = beatEvent["_value"]
                });
            }
            events = eventsList.ToArray();

            var notesList = new List<Note>();
            var ns = data["_notes"];
            for (int i = 0; i < ns.AsArray.Count; i++)
            {
                var note = ns[i];
                notesList.Add(new Note()
                {
                    time = note["_time"],
                    lineIndex = note["_lineIndex"],
                    lineLayer = note["_lineLayer"],
                    type = note["_type"],
                    cutDirection = note["_cutDirecetion"]
                });
            }
            notes = notesList.ToArray();
        }
    }
}