using CountersPlus.Counters.Custom;
using PPCounter.Settings;
using PPCounter.Utilities;
using System.Globalization;
using TMPro;
using Zenject;
using static PPCounter.Utilities.Structs;

namespace PPCounter
{
    public class PPCounter : BasicCustomCounter
    {
        [Inject] private RelativeScoreAndImmediateRankCounter relativeScoreAndImmediateRank;
        [Inject] private IDifficultyBeatmap difficultyBeatmap;
        [Inject] private GameplayModifiers gameplayModifiers;
        [Inject] private PPUtils ppUtils;

        private SongID songID;
        private TMP_Text counter;

        private float _multiplier;

        public override void CounterInit()
        {
            var id = SongDataUtils.GetHash(difficultyBeatmap.level.levelID);
            songID = new SongID(id, difficultyBeatmap.difficulty);

            // Don't show anything for unranked songs
            if (!ppUtils.IsRanked(songID))
                return;

            var gameplayModifiersModelSO = IPA.Utilities.FieldAccessor<RelativeScoreAndImmediateRankCounter, GameplayModifiersModelSO>.Get(relativeScoreAndImmediateRank, "_gameplayModifiersModel");
            GameplayModifiers updatedModifiers = ppUtils.AllowedPositiveModifiers(songID) ? 
                                                    gameplayModifiers : BeatSaberUtils.RemovePositiveModifiers(gameplayModifiers);
            updatedModifiers = PluginSettings.Instance.ignoreNoFail ?
                                BeatSaberUtils.RemoveNoFail(updatedModifiers) : updatedModifiers;

            _multiplier = gameplayModifiersModelSO.GetTotalMultiplier(updatedModifiers);

            counter = CanvasUtility.CreateTextFromSettings(Settings);
            counter.fontSize = 3;
            UpdateCounterText(ppUtils.CalculatePP(songID, 1f));

            relativeScoreAndImmediateRank.relativeScoreOrImmediateRankDidChangeEvent += ScoreUpdated;
        }

        private void ScoreUpdated()
        {
            var acc = relativeScoreAndImmediateRank.relativeScore * _multiplier;
            UpdateCounterText(ppUtils.CalculatePP(songID, acc));
        }

        private void UpdateCounterText(float pp)
        {
            var ppString = pp.ToString($"F{PluginSettings.Instance.decimalPrecision}", CultureInfo.InvariantCulture);
            counter.text = $"{ppString}pp";
        }
        public override void CounterDestroy()
        {
            relativeScoreAndImmediateRank.relativeScoreOrImmediateRankDidChangeEvent -= ScoreUpdated;
        }
    }
}
