using CountersPlus.Counters.Custom;
using CountersPlus.Counters.Interfaces;
using CountersPlus.Counters.NoteCountProcessors;
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
        [Inject] private PlayerDataModel playerDataModel;
        [Inject] private PPUtils ppUtils;

        [Inject] private IReadonlyBeatmapData beatmapData;

        private SongID songID;
        private TMP_Text counter;

        private float _pbPP;
        private float _multiplier = 1f;

        public override void CounterInit()
        {
            var id = SongDataUtils.GetHash(difficultyBeatmap.level.levelID);
            songID = new SongID(id, difficultyBeatmap.difficulty);

            // Don't show anything for unranked songs or if data not initialized
            if (!ppUtils.DataInitialized() || !ppUtils.IsRanked(songID))
            {
                return;
            }

            var gameplayModifiersModelSO = IPA.Utilities.FieldAccessor<RelativeScoreAndImmediateRankCounter, GameplayModifiersModelSO>.Get(relativeScoreAndImmediateRank, "_gameplayModifiersModel");
            GameplayModifiers updatedModifiers = ppUtils.AllowedPositiveModifiers(songID) ?
                                                    gameplayModifiers : GameplayModifierUtils.RemovePositiveModifiers(gameplayModifiers);

            _multiplier = GameplayModifierUtils.CalculateMultiplier(gameplayModifiersModelSO, updatedModifiers);

            counter = CanvasUtility.CreateTextFromSettings(Settings);
            counter.fontSize = 3;

            relativeScoreAndImmediateRank.relativeScoreOrImmediateRankDidChangeEvent += ScoreUpdated;
            UpdateCounterText(ppUtils.CalculatePP(songID, _multiplier, ppUtils.AllowedPositiveModifiers(songID)));

            if (PluginSettings.Instance.relativeGain)
            {
                var highScore = playerDataModel.playerData.GetPlayerLevelStatsData(difficultyBeatmap).highScore;
                if (highScore == 0)
                {
                    _pbPP = 0;
                    return;
                }

                var maxScore = ScoreModel.ComputeMaxMultipliedScoreForBeatmap(beatmapData);
                var acc = (float)highScore / maxScore;
                _pbPP = ppUtils.CalculatePP(songID, acc, ppUtils.AllowedPositiveModifiers(songID));
            }
        }

        private void ScoreUpdated()
        {
            var acc = relativeScoreAndImmediateRank.relativeScore * _multiplier;
            UpdateCounterText(ppUtils.CalculatePP(songID, acc, ppUtils.AllowedPositiveModifiers(songID)));
        }

        private void UpdateCounterText(float pp)
        {
            var ppString = pp.ToString($"F{PluginSettings.Instance.decimalPrecision}", CultureInfo.InvariantCulture);
            counter.text = $"{ppString}pp";
            if (PluginSettings.Instance.relativeGain && _pbPP > 0)
            {
                var ppDiff = pp - _pbPP;
                var colorTextStart = "";
                var colorTextEnd = "";

                if (PluginSettings.Instance.relativeGainColor)
                {
                    var color = ppDiff >= 0 ? "green" : "red";
                    colorTextStart = $"<color=\"{color}\">";
                    colorTextEnd = "</color>";
                }

                var gainText = ppDiff >= 0 ? "+" : "";
                var ppDiffText = ppDiff.ToString($"F{PluginSettings.Instance.decimalPrecision}", CultureInfo.InvariantCulture);
                counter.text += PluginSettings.Instance.relativeGainInline ? $" ({colorTextStart}{gainText}{ppDiffText}{colorTextEnd})"
                                : $"\n{colorTextStart}{gainText}{ppDiffText}{colorTextEnd}";
            }
        }
        public override void CounterDestroy()
        {
            relativeScoreAndImmediateRank.relativeScoreOrImmediateRankDidChangeEvent -= ScoreUpdated;
        }
    }
}
