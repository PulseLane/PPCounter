using System.Collections.Generic;
using System.Linq;
using static PPCounter.Utilities.Structs;

namespace PPCounter.Utilities
{
    static class CurveUtils
    {
        public static float[] GetSlopes(List<Point> curve)
        {
            var slopes = new float[curve.Count - 1];
            for (var i = 0; i < curve.Count - 1; i++)
            {
                float x1 = curve[i].x;
                float y1 = curve[i].y;
                float x2 = curve[i + 1].x;
                float y2 = curve[i + 1].y;

                var m = (y2 - y1) / (x2 - x1);
                slopes[i] = m;
            }

            return slopes;
        }

        public static float GetCurveMultiplier(List<Point> curve, float[] slopes, float accuracy)
        {
            if (accuracy >= curve.Last().x)
                return curve.Last().y;

            if (accuracy <= 0)
                return 0f;

            var i = -1;

            foreach (Point point in curve)
            {
                if (point.x > accuracy)
                    break;
                i++;
            }

            var lowerScore = curve[i].x;
            var lowerGiven = curve[i].y;

            return Lerp(slopes, lowerScore, lowerGiven, accuracy, i);
        }

        public static float Lerp(float[] slopes, float x1, float y1, float x3, int i)
        {
            float m = slopes[i];

            return m * (x3 - x1) + y1;
        }
    }
}
