using PPCounter.Data;
using System;
using System.Collections.Generic;
using Zenject;
using static PPCounter.Utilities.Structs;

namespace PPCounter.Utilities
{
    class PPUtils : IInitializable, IDisposable
    {
        [Inject] private PPData ppData;

        private static (float, float)[] ppCurve = new (float, float)[]
        {
            (0f, 0),
            (.45f, .015f),
            (.50f, .03f),
            (.55f, .06f),
            (.60f, .105f),
            (.65f, .16f),
            (.68f, .24f),
            (.70f, .285f),
            (.80f, .563f),
            (.84f, .695f),
            (.88f, .826f),
            (.945f, 1.015f),
            (.95f, 1.046f),
            (1.00f, 1.12f),
            (1.10f, 1.18f),
            (1.14f, 1.25f)
        };

        private static HashSet<string> songsAllowingPositiveModifiers = new HashSet<string> {
            "2FDDB136BDA7F9E29B4CB6621D6D8E0F8A43B126", // Overkill Nuketime
            "27FCBAB3FB731B16EABA14A5D039EEFFD7BD44C9" // Overkill Kry
        };

        // Pre-compute to save on division operator
        private float[] slopes;

        public void Initialize()
        {
            Logger.log.Debug("Initializing PPUtils");
            slopes = new float[ppCurve.Length - 1];
            for (var i = 0; i < ppCurve.Length - 1; i++)
            {
                var x1 = ppCurve[i].Item1;
                var y1 = ppCurve[i].Item2;
                var x2 = ppCurve[i + 1].Item1;
                var y2 = ppCurve[i + 1].Item2;

                var m = (y2 - y1) / (x2 - x1);
                slopes[i] = m;
            }
        }

        public bool IsRanked(SongID songID)
        {
            return ppData.IsRanked(songID);
        }

        public bool AllowedPositiveModifiers(SongID songID)
        {
            return songsAllowingPositiveModifiers.Contains(songID.id);
        }

        public float CalculatePP(SongID songID, float accuracy)
        {
            var rawPP = ppData.GetPP(songID);
            return CalculatePP(rawPP, accuracy);
        }

        public float CalculatePP(float rawPP, float accuracy)
        {
            return rawPP * PPPercentage(accuracy);
        }

        private float PPPercentage(float accuracy)
        {
            if (accuracy >= 1.14)
                return 1.25f;
            if (accuracy <= 0)
                return 0f;

            var i = -1;
            foreach ((float score, float given) in ppCurve)
            {
                if (score > accuracy)
                    break;
                i++;
            }

            var lowerScore = ppCurve[i].Item1;
            var higherScore = ppCurve[i + 1].Item1;
            var lowerGiven = ppCurve[i].Item2;
            var higherGiven = ppCurve[i + 1].Item2;
            return Lerp(lowerScore, lowerGiven, higherScore, higherGiven, accuracy, i);
        }

        private float Lerp(float x1, float y1, float x2, float y2, float x3, int i)
        {
            float m;
            if (slopes != null)
                m = slopes[i];
            else
                m = (y2 - y1) / (x2 - x1);
            return m * (x3 - x1) + y1;
        }

        public void Dispose() { Logger.log.Debug("Disposing pp utils"); }
    }
}
