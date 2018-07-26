using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeatSaberBeatmapAnalyzer
{
    public class Vector2
    {
        public float x;
        public float y;

        public Vector2(float _x, float _y)
        {
            x = _x;
            y = _y;
        }
        
        public static float Distance(Vector2 a, Vector2 b)
        {
            return (float)Math.Sqrt(Math.Pow((a.x - b.x), 2) + Math.Pow((a.y - b.y), 2));
        }

        public float length
        {
            get
            {
                return (float)Math.Sqrt(x * x + y * y);
            }
        }

        public Vector2 normalized
        {
            get
            {
                return this / length;
            }
        }

        public static Vector2 operator + (Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x + v2.x, v1.y + v2.y);
        }

        public static Vector2 operator -(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x - v2.x, v1.y - v2.y);
        }

        public static Vector2 operator *(Vector2 v, float f)
        {
            return new Vector2(v.x * f, v.y * f);
        }

        public static Vector2 operator /(Vector2 v, float f)
        {
            return new Vector2(v.x / f, v.y / f);
        }
    }
}
