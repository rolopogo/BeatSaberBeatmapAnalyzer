using System;

namespace BeatSaberBeatmapAnalyzer
{
    [Serializable]
    public class Note
    {
        public float time; // Note time position in beats
        public int lineIndex; // (0 to 3, start from left)
        public int lineLayer; // (0 to 2, start from bottom)
        public int type; // 0=red, 1=blue, 3=bomb
        public int cutDirection;

        public Vector2 position
        {
            get
            {
                return new Vector2(lineIndex, lineLayer);
            }
        }

        public Vector2 CutDirectionVector()
        {
            switch (cutDirection)
            {
                case 0:// up
                    return new Vector2(0, 1);
                case 1: // down
                    return new Vector2(0, -1);
                case 2: // left
                    return new Vector2(1, 0);
                case 3: // right
                    return new Vector2(-1, 0);
                case 4: // leftup
                    return new Vector2(1, 1).normalized;
                case 5: // rightup
                    return new Vector2(-1, 1).normalized;
                case 6: // downleft
                    return new Vector2(1, -1).normalized;
                case 7: // downright
                    return new Vector2(-1, -1).normalized;
                default: // no direction ? 
                    return new Vector2(0, 0);
            }
        }
    }
}