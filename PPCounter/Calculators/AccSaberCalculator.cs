using PPCounter.Data;
using PPCounter.Utilities;
using System.Collections.Generic;
using Zenject;
using static PPCounter.Utilities.Structs;

namespace PPCounter.Calculators
{
    internal class AccSaberCalculator
    {
        [Inject] private AccSaberData accSaberData;

        private List<Point> _curve;
        private float[] _slopes;

        private float _scale;
        private float _shift;

        public void SetCurve(AccSaber accSaber)
        {
            _curve = accSaber.curve;
            _scale = accSaber.scale;
            _shift = accSaber.shift;

            _slopes = CurveUtils.GetSlopes(_curve);
        }

        public bool IsRanked(SongID songID)
        {
            return accSaberData.IsRanked(songID);
        }

        public float CalculateAP(SongID songID, float accuracy)
        {
            var complexity = accSaberData.GetComplexity(songID);
            return CalculateAP(complexity, accuracy);
        }

        public float CalculateAP(float complexity, float accuracy)
        {
            return CurveUtils.GetCurveMultiplier(_curve, _slopes, accuracy) * (complexity - _shift) * _scale;
        }
    }
}
