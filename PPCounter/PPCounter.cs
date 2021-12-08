using CountersPlus.Counters.Custom;
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

        [Inject] private NoteCountProcessor noteCountProcessor; // yoink from C+

        private SongID songID;
        private TMP_Text counter;

        private float _pbPP;

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
                                                    gameplayModifiers : BeatSaberUtils.RemovePositiveModifiers(gameplayModifiers);

            counter = CanvasUtility.CreateTextFromSettings(Settings);
            counter.fontSize = 3;
            UpdateCounterText(ppUtils.CalculatePP(songID, 1f, PluginSettings.Instance.newCurve));

            relativeScoreAndImmediateRank.relativeScoreOrImmediateRankDidChangeEvent += ScoreUpdated;

            if (PluginSettings.Instance.relativeGain)
            {
                var highScore = playerDataModel.playerData.GetPlayerLevelStatsData(difficultyBeatmap).highScore;
                if (highScore == 0)
                {
                    _pbPP = 0;
                    return;
                }

                var maxScore = ScoreModel.MaxRawScoreForNumberOfNotes(noteCountProcessor.NoteCount);
                var acc = (float)highScore / maxScore;
                _pbPP = ppUtils.CalculatePP(songID, acc, PluginSettings.Instance.newCurve);
            }
        }

        private void ScoreUpdated()
        {
            var acc = relativeScoreAndImmediateRank.relativeScore;
            UpdateCounterText(ppUtils.CalculatePP(songID, acc, PluginSettings.Instance.newCurve));
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
