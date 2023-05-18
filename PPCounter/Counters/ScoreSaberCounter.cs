using CountersPlus.Custom;
using CountersPlus.Utils;
using HMUI;
using PPCounter.Calculators;
using PPCounter.Data;
using PPCounter.Settings;
using PPCounter.Utilities;
using System.Globalization;
using TMPro;
using UnityEngine;
using Zenject;
using static PPCounter.Utilities.Structs;

namespace PPCounter.Counters
{
    internal class ScoreSaberCounter : IPPCounter
    {
        [Inject] private SSData ssData;
        [Inject] private ScoreSaberCalculator ssUtils;

        private SongID _songID;

        private ImageView _image;
        private TMP_Text _text;

        public void InitData(SongID songID, GameplayModifiersModelSO gameplayModifiersModelSO, GameplayModifiers gameplayModifiers, Leaderboards leaderboards)
        {
            _songID = songID;
            ssUtils.SetCurve(leaderboards.ScoreSaber, songID, gameplayModifiersModelSO, gameplayModifiers);
        }

        public void InitCounter(CanvasUtility canvasUtility, CustomConfigModel settings, int counterIndex)
        {
            float iconOffset = 0;

            if (PluginSettings.Instance.showIcons)
            {
                _image = CounterUtils.CreateIcon(canvasUtility, settings, GetIcon(), counterIndex);
                iconOffset = CounterUtils.ICON_OFFSET;
            }

            _text = canvasUtility.CreateTextFromSettings(settings, CounterUtils.GetTextOffset(counterIndex));
            _text.fontSize = 3;
        }

        public void UpdateCounter(float acc, bool failed = false)
        {
            var pp = ssUtils.CalculatePP(_songID, acc, failed);

            var ppString = pp.ToString($"F{PluginSettings.Instance.decimalPrecision}", CultureInfo.InvariantCulture);
            _text.text = $"{ppString}pp";
        }

        public Sprite GetIcon()
        {
            return BeatSaberMarkupLanguage.Utilities.FindSpriteInAssembly("PPCounter.Images.ScoreSaber.png");
        }

        public bool IsActive(SongID songID)
        {
            return ssData.IsRanked(songID);
        }
    }
}
