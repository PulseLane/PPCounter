using CountersPlus.Custom;
using CountersPlus.Utils;
using HMUI;
using PPCounter.Settings;
using UnityEngine;

namespace PPCounter.Utilities
{
    internal static class CounterUtils
    {
        public static float ICON_OFFSET = 0.15f;
        public static float VERTICAL_OFFSET = -0.3f;

        public static ImageView CreateIcon(CanvasUtility canvasUtility, CustomConfigModel settings, Sprite sprite, int counterIndex)
        {
            Canvas canvas = canvasUtility.GetCanvasFromID(settings.CanvasID);
            if (canvas == null)
            {
                Logger.log.Error("null canvas");
                return null;
            }

            GameObject gameObject = new GameObject("CounterImage");
            gameObject.SetActive(true);

            var image = gameObject.AddComponent<ImageView>();
            image.rectTransform.SetParent(canvas.transform, worldPositionStays: false);
            var positionScale = canvasUtility.GetCanvasSettingsFromCanvas(canvas).PositionScale;
            image.rectTransform.anchoredPosition = positionScale * canvasUtility.GetAnchoredPositionFromConfig(settings) + new Vector3(-0.5f * positionScale, counterIndex * VERTICAL_OFFSET * positionScale, 0);
            image.rectTransform.sizeDelta = new Vector2(2.5f, 2.5f);

            image.material = BeatSaberMarkupLanguage.Utilities.ImageResources.NoGlowMat;
            image.sprite = sprite;
            image.enabled = true;

            return image;
        }

        public static Vector3 GetTextOffset(int counterIndex)
        {
            return new Vector3(PluginSettings.Instance.showIcons ? ICON_OFFSET : 0.0f, counterIndex * CounterUtils.VERTICAL_OFFSET, 0);
        }
    }
}
