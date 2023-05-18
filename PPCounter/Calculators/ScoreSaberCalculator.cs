using PPCounter.Data;
using PPCounter.Utilities;
using System.Collections.Generic;
using System.Linq;
using Zenject;
using static PPCounter.Utilities.Structs;

namespace PPCounter.Calculators
{
    internal class ScoreSaberCalculator
    {
        [Inject] private SSData ssData;

        private HashSet<string> songsAllowingPositiveModifiers = new HashSet<string>();

        private List<Point> _curve;
        private float[] _slopes;

        private float _multiplier;

        public void SetCurve(ScoreSaber scoreSaber, SongID songID, GameplayModifiersModelSO gameplayModifiersModelSO, GameplayModifiers gameplayModifiers)
        {
            bool allowedPositiveModifiers = AllowedPositiveModifiers(songID);
            GameplayModifiers updatedModifiers = allowedPositiveModifiers ? gameplayModifiers : GameplayModifierUtils.RemovePositiveModifiers(gameplayModifiers);

            _multiplier = CalculateMultiplier(gameplayModifiersModelSO, updatedModifiers, scoreSaber.modifiers);

            songsAllowingPositiveModifiers = scoreSaber.songsAllowingPositiveModifiers.ToHashSet();

            _curve = allowedPositiveModifiers ? scoreSaber.modifierCurve : scoreSaber.standardCurve;
            _slopes = CurveUtils.GetSlopes(_curve);
        }

        public bool IsRanked(SongID songID)
        {
            return ssData.IsRanked(songID);
        }

        public bool AllowedPositiveModifiers(SongID songID)
        {
            return songsAllowingPositiveModifiers.Contains(songID.id);
        }

        public float CalculatePP(SongID songID, float accuracy, bool failed = false)
        {
            var rawPP = ssData.GetPP(songID);
            return CalculatePP(rawPP, accuracy, failed);
        }

        public float CalculatePP(float rawPP, float accuracy, bool failed = false)
        {
            var multiplier = _multiplier;
            if (failed)
            {
                multiplier -= 0.5f;
            }

            return rawPP * CurveUtils.GetCurveMultiplier(_curve, _slopes, accuracy * multiplier);
        }

        public static float CalculateMultiplier(GameplayModifiersModelSO gameplayModifiersModelSO, GameplayModifiers gameplayModifiers, ScoreSaberModifiers modifierMultipliers)
        {
            List<GameplayModifierParamsSO> modifierParams = gameplayModifiersModelSO.CreateModifierParamsList(gameplayModifiers);
            float multiplier = gameplayModifiersModelSO.GetTotalMultiplier(modifierParams, 1f);

            // ScoreSaber weights these multipliers differently
            if (gameplayModifiers.disappearingArrows)
                multiplier += modifierMultipliers.da - GameplayModifierUtils.DA_ORIGINAL;
            if (gameplayModifiers.ghostNotes)
                multiplier += modifierMultipliers.gn - GameplayModifierUtils.GN_ORIGINAL;
            if (gameplayModifiers.songSpeed.Equals(GameplayModifiers.SongSpeed.Faster))
                multiplier += modifierMultipliers.fs - GameplayModifierUtils.FS_ORIGINAL;
            return multiplier;
        }
    }
}
