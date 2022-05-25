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

        private static (float, float)[] oldPPCurve = new (float, float)[]
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
            (.6f, .25f),
            (.65f, .29f),
            (.7f, .34f),
            (.75f, .40f),
            (.8f, .47f),
            (.825f, .51f),
            (.85f, .57f),
            (.875f, .655f),
            (.9f, .75f),
            (.91f, .79f),
            (.92f, .835f),
            (.93f, 0.885f),
            (.94f, 0.94f),
            (.95f, 1f),
            (.955f, 1.05f),
            (.96f, 1.115f),
            (.965f, 1.195f),
            (.97f, 1.3f),
            (.9725f, 1.36f),
            (.975f, 1.43f),
            (.9775f, 1.515f),
            (.98f, 1.625f),
            (.9825f, 1.775f),
            (.985f, 2.0f),
            (.9875f, 2.31f),
            (.99f, 2.73f),
            (.9925f, 3.31f),
            (.995f, 4.14f),
            (.9975f, 5.31f),
            (.999f, 6.24f),
            (1, 7f),
        };

        private static HashSet<string> songsAllowingPositiveModifiers = new HashSet<string> {
            "2FDDB136BDA7F9E29B4CB6621D6D8E0F8A43B126", // Overkill Nuketime
            "27FCBAB3FB731B16EABA14A5D039EEFFD7BD44C9" // Overkill Kry
        };

        // Pre-compute to save on division operator
        private float[] oldSlopes;

        private float[] newSlopes;

        public void Initialize()
        {
            Logger.log.Debug("Initializing PPUtils");
            oldSlopes = new float[oldPPCurve.Length - 1];
            for (var i = 0; i < oldPPCurve.Length - 1; i++)
            {
                var x1 = oldPPCurve[i].Item1;
                var y1 = oldPPCurve[i].Item2;
                var x2 = oldPPCurve[i + 1].Item1;
                var y2 = oldPPCurve[i + 1].Item2;

                var m = (y2 - y1) / (x2 - x1);
                oldSlopes[i] = m;
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

        public float CalculatePP(SongID songID, float accuracy, bool oldCurve)
        {
            var rawPP = ppData.GetPP(songID);
            return CalculatePP(rawPP, accuracy, oldCurve);
        }

        public float CalculatePP(float rawPP, float accuracy, bool oldCurve)
        {
            return rawPP * PPPercentage(accuracy, oldCurve);
        }

        private float PPPercentage(float accuracy, bool oldCurve)
        {
            var max = oldCurve ? 1.14f : 1f;
            var maxReward = oldCurve ? 1.25f : 7f;

            if (accuracy >= max)
                return maxReward;

            if (accuracy <= 0)
                return 0f;

            var i = -1;

            if (!oldCurve)
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
                foreach ((float score, float given) in oldPPCurve)
                {
                    if (score > accuracy)
                        break;
                    i++;
                }
            }

            if (!oldCurve)
            {
                var lowerScore = newPPCurve[i].Item1;
                var higherScore = newPPCurve[i + 1].Item1;
                var lowerGiven = newPPCurve[i].Item2;
                var higherGiven = newPPCurve[i + 1].Item2;
                return Lerp(lowerScore, lowerGiven, higherScore, higherGiven, accuracy, i, oldCurve);
            }
            else
            {
                var lowerScore = oldPPCurve[i].Item1;
                var higherScore = oldPPCurve[i + 1].Item1;
                var lowerGiven = oldPPCurve[i].Item2;
                var higherGiven = oldPPCurve[i + 1].Item2;
                return Lerp(lowerScore, lowerGiven, higherScore, higherGiven, accuracy, i, oldCurve);
            }

        }

        private float Lerp(float x1, float y1, float x2, float y2, float x3, int i, bool oldCurve)
        {
            float m;
            if (!oldCurve && newSlopes != null)
            {
                m = newSlopes[i];
            }
            else if (!oldCurve && oldSlopes != null)
            {
                m = oldSlopes[i];
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
