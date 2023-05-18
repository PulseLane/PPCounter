using CountersPlus.Custom;
using CountersPlus.Utils;
using UnityEngine;
using static PPCounter.Utilities.Structs;

namespace PPCounter.Counters
{
    internal interface IPPCounter
    {
        public void InitData(SongID songID, GameplayModifiersModelSO gameplayModifiersModelSO, GameplayModifiers gameplayModifiers, Leaderboards leaderboards);

        public void InitCounter(CanvasUtility canvasUtility, CustomConfigModel settings, int counterIndex);

        public void UpdateCounter(float acc, bool failed = false);

        public Sprite GetIcon();

        public bool IsActive(SongID songID);
    }
}
