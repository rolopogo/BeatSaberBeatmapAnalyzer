using System;
using System.Collections;
using System.Collections.Generic;

namespace BeatSaberBeatmapAnalyzer
{
    public class Analyzer
    {
        private BeatMap beatMap;
        private ArrayList redNotes;
        private ArrayList blueNotes;
        private ArrayList allNotes;
        private const float cutLeadDistance = 2f;
        private const float cutFollowDistance = 1f;

        // Sightreading hardness values
        private const float baseHardness = 1f;
        private const float diagonalHardness = .1f;
        private const float doubleNoteHardness = .1f;
        private const float tripleNoteHardness = .1f;
        private const float multiSaberHardness = .1f;
        private const float oppositeDirectionHardness = .1f;
        private const float mismatchedDirectionHardness = .3f;
        private const float obstacleHardness = .1f;
        // lighting?

        public class SongMetrics
        {
            public float avgNotesPerSec;
            public float maxNotesPerBarPerSec;
            public float cutDistancePerSec;
            public float cutDirectionEntropy;
            public float notePosEntropy;
        }

        private float totalBeats = 0;

        public SongMetrics Analyze(BeatMap _beatMap)
        {
            beatMap = _beatMap;

            redNotes = new ArrayList();
            blueNotes = new ArrayList();
            allNotes = new ArrayList();

            foreach (Note note in beatMap.notes)
            {
                if (note.type == 0)
                {
                    redNotes.Add(note);
                    allNotes.Add(note);
                }
                if (note.type == 1)
                {
                    blueNotes.Add(note);
                    allNotes.Add(note);
                }

                if (note.time > totalBeats) totalBeats = note.time; // Keep track of how long the song is
            }

            byte[] cutDirections = new byte[allNotes.Count];
            byte[] notePos = new byte[allNotes.Count];

            for (int i = 0; i < allNotes.Count; i++)
            {
                Note n = (Note)allNotes[i];
                cutDirections[i] = (byte)((Note)allNotes[i]).cutDirection;
                notePos[i] = (byte)( n.lineIndex * 3 + n.lineLayer);
            }

            SongMetrics sm = new SongMetrics();
            sm.cutDistancePerSec = GetCutDistancePerSecond(redNotes) + GetCutDistancePerSecond(blueNotes);
            return sm;
        }
        
        public float GetAverageNotesPerSecond(ArrayList notes)
        {
            return notes.Count / BeatsToSeconds(totalBeats);
        }
        
        public float GetCutDistance(ArrayList notes)
        {
            float totalDistance = 0f;
            Vector2 bladePos = new Vector2(1.5f, 1.5f); // start at center
            ArrayList notesAtThisTimeStep = new ArrayList();

            for (int i = 0; i < notes.Count; i++)
            {
                notesAtThisTimeStep.Clear();

                // find all notes at this notes time step
                float time = ((Note)notes[i]).time;
                int j = 0;
                while (((Note)notes[i + j]).time == time)
                {
                    notesAtThisTimeStep.Add(((Note)notes[i + j]));
                    j++;
                    if (i + j >= notes.Count) break;
                }

                i += j - 1; // don't count these notes again

                if (notesAtThisTimeStep.Count > 1)
                {
                    // solve group of notes
                    totalDistance += CutNotesWithBlade(notesAtThisTimeStep, ref bladePos);
                }
                else
                {
                    // one note only
                    Note note = (Note)notesAtThisTimeStep[0];
                    totalDistance += CutNoteWithBlade(note, ref bladePos);
                }
            }

            return totalDistance;
        }

        public float GetCutDistancePerSecond(ArrayList notes)
        {
            return GetCutDistance(notes) / BeatsToSeconds(totalBeats);
        }
        
        #region Helper Functions

        private float CutNoteWithBlade(Note note, ref Vector2 bladePos)
        {
            float distance = 0f;
            Vector2 notePos = new Vector2(note.lineIndex, note.lineLayer);

            Vector2 cutDirection;

            if (note.cutDirection == 8)
            {
                // use current direction as cutdirection
                cutDirection = (notePos - bladePos).normalized;
            }
            else
            {
                cutDirection = note.CutDirectionVector();
            }
            // distance from blade pos to start of cut
            distance += Vector2.Distance(bladePos, notePos - cutDirection * cutLeadDistance);
            // length of cut
            distance += cutLeadDistance + cutFollowDistance;
            bladePos = notePos + cutDirection * cutFollowDistance;
            return distance;
        }

        private float CutNotesWithBlade(ArrayList notes, ref Vector2 bladePos)
        {
            if (notes.Count <= 0) return 0f;
            
            // find minimum distance through the set of notes - i.e. the optimum swipe
            float distance = float.PositiveInfinity;
            Vector2 bladeTemp = new Vector2(bladePos.x, bladePos.y);

            foreach (Note note in notes)
            {
                ArrayList notes2 = new ArrayList();
                notes2.AddRange(notes);
                notes2.Remove(note);
                // recursively search the subset of notes, minus this note
                distance = Math.Min(distance,
                    CutNoteWithBlade(note, ref bladeTemp) + CutNotesWithBlade(notes2, ref bladePos));
            }

            return distance;
        }

        

        private float PerBarToPerSec(float perBar)
        {
            return PerBeatToPerSec(perBar / beatMap.beatsPerBar);
        }

        private float PerBeatToPerSec(float perBeat)
        {
            return perBeat / BeatsToSeconds(1);
        }
        
        private float BeatsToSeconds(float beats)
        {
            float minutesPerBeat = 1f / beatMap.beatsPerMinute;
            float secondsPerBeat = minutesPerBeat * 60;
            return beats * secondsPerBeat;
        }

        #endregion
    }
}