using CountersPlus.Custom;
using CountersPlus.Utils;
using PPCounter.Utilities;
using System;
using UnityEngine;

namespace PPCounter.Counters
{
    internal class HitbloqCounter : IPPCounter
    {
        public Sprite GetIcon()
        {
            return BeatSaberMarkupLanguage.Utilities.FindSpriteInAssembly("PPCounter.Images.Hitbloq.png");
        }

        public bool IsActive(Structs.SongID songID)
        {
            return false;
        }

        public void InitData(Structs.SongID songID, GameplayModifiersModelSO gameplayModifiersModelSO, GameplayModifiers gameplayModifiers, Structs.Leaderboards leaderboards)
        {
            throw new NotImplementedException();
        }

        public void InitCounter(CanvasUtility canvasUtility, CustomConfigModel settings, int counterIndex)
        {
            throw new NotImplementedException();
        }

        public void UpdateCounter(float acc, bool failed = false)
        {
            throw new NotImplementedException();
        }
    }
}
