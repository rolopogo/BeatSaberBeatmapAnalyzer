using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeatSaberBeatmapAnalyzer
{
    class DifficultyNote
    {

        private Note note;
		private double[] strains = { 1, 1 };
        private Vector2 normStart;//, normEnd;

        public static readonly double[] DECAY_BASE = { 0.3, 0.15 };
        public static readonly double[] WEIGHT_SCALING = { 1400, 26.25 };
        public static readonly double STAR_SCALING_FACTOR = 0.0675;
        public const double EXTREME_SCALING_FACTOR = 0.5;
        public const float PLAYFIELD_WIDTH = 512;
        public const double DECAY_WEIGHT = 0.9;

        public const double ALMOST_DIAMETER = 90;
        public const double STREAM_SPACING = 110;
        public const double SINGLE_SPACING = 125;

        public const int STRAIN_STEP = 400;

        public const float CIRCLE_SIZE_BUFF_TRESHOLD = 30;

        public const byte DIFF_SPEED = 0;
        public const byte DIFF_AIM = 1;

        public DifficultyNote(Note note)
        {
            this.note = note;
            
            normStart = note.position;
            //normEnd = normStart;
        }

        private void calculateStrains(DifficultyNote previous, double timeRate)
        {
            CalculateStrain(previous, timeRate, DIFF_SPEED);
            CalculateStrain(previous, timeRate, DIFF_AIM);
        }

        private void CalculateStrain(DifficultyNote previous, double timeRate, byte difficultyType)
        {
            double res = 0;
            double timeElapsed = (note.time - previous.note.time)/ timeRate;
            double decay = Math.Pow(DECAY_BASE[difficultyType], timeElapsed / 1000f);
            double scaling = WEIGHT_SCALING[difficultyType];
            
            double distance = Vector2.Distance(normStart, previous.normStart);
            res = SpacingWeight(distance, difficultyType) * scaling;
            
            res /= Math.Max(timeElapsed, 50);
            strains[difficultyType] = previous.strains[difficultyType] * decay + res;
        }

        private double SpacingWeight(double distance, byte difficultyType)
        {
            if (difficultyType == DIFF_SPEED)
            {
                if (distance > SINGLE_SPACING)
                {
                    return 2.5;
                }
                else if (distance > STREAM_SPACING)
                {
                    return 1.6 + 0.9 * (distance - STREAM_SPACING) / (SINGLE_SPACING - STREAM_SPACING);
                }
                else if (distance > ALMOST_DIAMETER)
                {
                    return 1.2 + 0.4 * (distance - ALMOST_DIAMETER) / (STREAM_SPACING - ALMOST_DIAMETER);
                }
                else if (distance > ALMOST_DIAMETER / 2)
                {
                    return 0.95 + 0.25 * (distance - ALMOST_DIAMETER / 2) / (ALMOST_DIAMETER / 2);
                }
                return 0.95;
            }
            else if (difficultyType == DIFF_AIM)
                return Math.Pow(distance, 0.99);
            else
                return 0;
        }
    }
}
