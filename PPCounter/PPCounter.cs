using CountersPlus.Counters.Custom;
using IPA.Utilities;
using PPCounter.Counters;
using PPCounter.Data;
using PPCounter.Settings;
using PPCounter.Utilities;
using System.Collections.Generic;
using Zenject;
using static PPCounter.Utilities.Structs;

namespace PPCounter
{
    public class PPCounter : BasicCustomCounter
    {
        [Inject] private RelativeScoreAndImmediateRankCounter relativeScoreAndImmediateRank;
        [Inject] private IDifficultyBeatmap difficultyBeatmap;
        [Inject] private GameplayModifiers gameplayModifiers;
        [Inject] private GameEnergyCounter gameEnergyCounter;
        [Inject] private PPData ppData;

        [Inject] private List<IPPCounter> _ppCounters;
        private List<IPPCounter> _activeCounters = new List<IPPCounter>();

        private SongID songID;

        private bool _failed = false;

        internal PPCounter(List<IPPCounter> ppCounters)
        {
            _ppCounters = ppCounters;
        }

        public override void CounterInit()
        {
            gameEnergyCounter.gameEnergyDidReach0Event += OnGameEnergyDidReach0;

            var id = SongDataUtils.GetHash(difficultyBeatmap.level.levelID);
            songID = new SongID(id, difficultyBeatmap.difficulty);

            var gameplayModifiersModelSO = relativeScoreAndImmediateRank.GetField<GameplayModifiersModelSO, RelativeScoreAndImmediateRankCounter>("_gameplayModifiersModel");

            SortPPCounters();

            foreach (var ppCounter in _ppCounters)
            {
                if (ppCounter.IsActive(songID))
                {
                    ppCounter.InitData(songID, gameplayModifiersModelSO, gameplayModifiers, ppData.Curves);
                    ppCounter.InitCounter(CanvasUtility, Settings, _activeCounters.Count);
                    _activeCounters.Add(ppCounter);

                    if (_activeCounters.Count >= PluginSettings.Instance.maxCounters)
                    {
                        break;
                    }
                }
            }

            if (_activeCounters.Count == 0)
            {
                return;
            }

            relativeScoreAndImmediateRank.relativeScoreOrImmediateRankDidChangeEvent += ScoreUpdated;
            ScoreUpdated();
        }

        private void ScoreUpdated()
        {
            var acc = relativeScoreAndImmediateRank.relativeScore;
            foreach (var counter in _activeCounters)
            {
                counter.UpdateCounter(acc, _failed);
            }
        }

        public override void CounterDestroy()
        {
            relativeScoreAndImmediateRank.relativeScoreOrImmediateRankDidChangeEvent -= ScoreUpdated;
        }

        private void SortPPCounters()
        {
            List<PPCounters> preferredOrder = SettingsUtils.GetCounterOrder(PluginSettings.Instance.preferredOrder, PluginSettings.Instance.numCounters);

            _ppCounters.Sort((x, y) =>
            {
                PPCounters xCounter = (PPCounters) System.Enum.Parse(typeof(PPCounters), x.GetType().Name.Replace("Counter", ""), true);
                PPCounters yCounter = (PPCounters) System.Enum.Parse(typeof(PPCounters), y.GetType().Name.Replace("Counter", ""), true);

                int xOrder = preferredOrder.IndexOf(xCounter);
                int yOrder = preferredOrder.IndexOf(yCounter);

                return xOrder.CompareTo(yOrder);
            });
        }

        private void OnGameEnergyDidReach0()
        {
            _failed = true;
        }
    }
}
