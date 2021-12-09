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

        private static (float, float)[] newPPCurve = new (float, float)[]
        {
            (0f, 0),
            (.45f, .015f),
            (.50f, .03f),
            (.55f, .06f),
            (.60f, .105f),
            (.65f, .15f),
            (.70f, .22f),
            (.75f, .35f),
            (.80f, .5f),
            (.84f, .63f),
            (.89f, .8f),
            (.92f, .91f),
            (.945f, 1.015f),
            (.95f, 1.046f),
            (.96f, 1.115f),
            (.97f, 1.2f),
            (.98f, 1.29f),
            (.99f, 1.39f),
            (1, 1.5f),
        };

        private static HashSet<string> songsAllowingPositiveModifiers = new HashSet<string> {
            "2FDDB136BDA7F9E29B4CB6621D6D8E0F8A43B126", // Overkill Nuketime
            "27FCBAB3FB731B16EABA14A5D039EEFFD7BD44C9" // Overkill Kry
        };

        // Pre-compute to save on division operator
        private float[] slopes;

        private float[] newSlopes;

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

            newSlopes = new float[newPPCurve.Length - 1];
            for (var i = 0; i < newPPCurve.Length - 1; i++)
            {
                var x1 = newPPCurve[i].Item1;
                var y1 = newPPCurve[i].Item2;
                var x2 = newPPCurve[i + 1].Item1;
                var y2 = newPPCurve[i + 1].Item2;

                var m = (y2 - y1) / (x2 - x1);
                newSlopes[i] = m;
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

        public bool DataInitialized()
        {
            return ppData.Init;
        }

        public float CalculatePP(SongID songID, float accuracy, bool newCurve)
        {
            var rawPP = ppData.GetPP(songID);
            return CalculatePP(rawPP, accuracy, newCurve);
        }

        public float CalculatePP(float rawPP, float accuracy, bool newCurve)
        {
            return rawPP * PPPercentage(accuracy, newCurve);
        }

        private float PPPercentage(float accuracy, bool newCurve)
        {
            var max = newCurve ? 1f : 1.14f;
            var maxReward = newCurve ? 1.5f : 1.25f;

            if (accuracy >= max)
                return maxReward;

            if (accuracy <= 0)
                return 0f;

            var i = -1;

            if (newCurve)
            {
                foreach ((float score, float given) in newPPCurve)
                {
                    if (score > accuracy)
                        break;
                    i++;
                }
            }
            else
            {
                foreach ((float score, float given) in ppCurve)
                {
                    if (score > accuracy)
                        break;
                    i++;
                }
            }

            if (newCurve)
            {
                var lowerScore = newPPCurve[i].Item1;
                var higherScore = newPPCurve[i + 1].Item1;
                var lowerGiven = newPPCurve[i].Item2;
                var higherGiven = newPPCurve[i + 1].Item2;
                return Lerp(lowerScore, lowerGiven, higherScore, higherGiven, accuracy, i, newCurve);
            }
            else
            {
                var lowerScore = ppCurve[i].Item1;
                var higherScore = ppCurve[i + 1].Item1;
                var lowerGiven = ppCurve[i].Item2;
                var higherGiven = ppCurve[i + 1].Item2;
                return Lerp(lowerScore, lowerGiven, higherScore, higherGiven, accuracy, i, newCurve);
            }

        }

        private float Lerp(float x1, float y1, float x2, float y2, float x3, int i, bool newCurve)
        {
            float m;
            if (newCurve && newSlopes != null)
            {
                m = newSlopes[i];
            }
            else if (!newCurve && slopes != null)
            {
                m = slopes[i];
            }
            else
            {
                m = (y2 - y1) / (x2 - x1);
            }

            return m * (x3 - x1) + y1;
        }

        public void Dispose() { }
    }
}
