using CountersPlus.Custom;
using CountersPlus.Utils;
using HMUI;
using PPCounter.Calculators;
using PPCounter.Settings;
using PPCounter.Utilities;
using System.Globalization;
using TMPro;
using UnityEngine;
using Zenject;
using static PPCounter.Utilities.Structs;

namespace PPCounter.Counters
{
    internal class AccSaberCounter : IPPCounter
    {
        [Inject] private AccSaberCalculator accSaberUtils;

        private SongID _songID;

        private ImageView _image;
        private TMP_Text _text;

        public Sprite GetIcon()
        {
            return BeatSaberMarkupLanguage.Utilities.FindSpriteInAssembly("PPCounter.Images.AccSaber.png");
        }

        public bool IsActive(Structs.SongID songID)
        {
            return accSaberUtils.IsRanked(songID);
        }

        public void InitData(Structs.SongID songID, GameplayModifiersModelSO gameplayModifiersModelSO, GameplayModifiers gameplayModifiers, Structs.Leaderboards leaderboards)
        {
            _songID = songID;
            accSaberUtils.SetCurve(leaderboards.AccSaber);
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
            var ap = accSaberUtils.CalculateAP(_songID, acc * (failed ? 0.5f : 1));

            var apString = ap.ToString($"F{PluginSettings.Instance.decimalPrecision}", CultureInfo.InvariantCulture);
            _text.text = $"{apString}ap";
        }
    }
}
