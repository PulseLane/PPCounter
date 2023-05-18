using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using HMUI;
using PPCounter.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
using static BeatSaberMarkupLanguage.Components.CustomListTableData;

namespace PPCounter.Settings
{
    class SettingsHandler
    {
        [UIParams]
        readonly BSMLParserParams parserParams = null;

        public event PropertyChangedEventHandler PropertyChanged;

        [UIValue("showIcons")]
        public bool showIcons
        {
            get => PluginSettings.Instance.showIcons;
            set
            {
                PluginSettings.Instance.showIcons = value;
            }
        }

        [UIValue("scoreSaberEnabled")]
        public bool scoreSaberEnabled
        {
            get => PluginSettings.Instance.scoreSaberEnabled;
            set
            {
                PluginSettings.Instance.scoreSaberEnabled = value;
            }
        }

        [UIValue("beatLeaderEnabled")]
        public bool beatLeaderEnabled
        {
            get => PluginSettings.Instance.beatLeaderEnabled;
            set
            {
                PluginSettings.Instance.beatLeaderEnabled = value;
            }
        }

        [UIValue("accSaberEnabled")]
        public bool accSaberEnabled
        {
            get => PluginSettings.Instance.accSaberEnabled;
            set
            {
                PluginSettings.Instance.accSaberEnabled = value;
            }
        }

        [UIValue("maxCounters")]
        public int maxCounters
        {
            get => PluginSettings.Instance.maxCounters;
            set
            {
                PluginSettings.Instance.maxCounters = value;
            }
        }

        [UIValue("decimalPrecision")]
        public int decimalPrecision
        {
            get => PluginSettings.Instance.decimalPrecision;
            set
            {
                PluginSettings.Instance.decimalPrecision = value;
            }
        }

        [UIValue("relativeGain")]
        public bool relativeGain
        {
            get => PluginSettings.Instance.relativeGain;
            set
            {
                PluginSettings.Instance.relativeGain = value;
            }
        }

        [UIValue("relativeGainInline")]
        public bool relativeGainInline
        {
            get => PluginSettings.Instance.relativeGainInline;
            set
            {
                PluginSettings.Instance.relativeGainInline = value;
            }
        }

        [UIValue("relativeGainColor")]
        public bool relativeGainColor
        {
            get => PluginSettings.Instance.relativeGainColor;
            set
            {
                PluginSettings.Instance.relativeGainColor = value;
            }
        }

        #region Preferred order
        public class PPCounterCell : CustomCellInfo
        {
            public PPCounterCell(string text, string subtext = null, Sprite icon = null) : base(text, subtext, icon)
            {
            }
        }

        private List<PPCounters> _preferredOrder;

        [UIComponent("counterList")]
        internal CustomListTableData _customListTableData;

        private PPCounterCell _selectedCounter;

        [UIAction("SelectCounter")]
        void SelectCounter(TableView tableView, int row)
        {
            _selectedCounter = _customListTableData.data[row] as PPCounterCell;

            UpButton.interactable = row > 0;
            DownButton.interactable = row < _customListTableData.data.Count - 1;
        }

        [UIAction("EditPreferredOrder")]
        public void EditPreferredOrder()
        {
            ReloadTable();

            parserParams.EmitEvent("open-preferred-order");
        }
        [UIComponent("up-button")]
        private Button UpButton;

        [UIComponent("down-button")]
        private Button DownButton;

        [UIAction("IncreaseOrder")]
        public void IncreaseOrder()
        {
            AdjustOrder(IncreaseCounterPreferred);
        }

        [UIAction("DecreaseOrder")]
        public void DecreaseOrder()
        {
            AdjustOrder(DecreaseCounterPreferred);
        }

        private delegate bool AdjustCounterPreferred(PPCounters counter, out int newIndex);

        private void AdjustOrder(AdjustCounterPreferred Func)
        {
            if (_selectedCounter == null)
            {
                return;
            }


            if (Enum.TryParse(_selectedCounter.text, out PPCounters selectedCounter))
            {
                if (Func(selectedCounter, out int newIndex))
                {
                    PluginSettings.Instance.preferredOrder = SettingsUtils.GetPreferredOrderNumber(_preferredOrder);

                    ReloadTable();
                    _customListTableData.tableView.SelectCellWithIdx(newIndex, true);
                }
            }
            else
            {
                Logger.log.Error($"Unrecognized PPCounter: {_selectedCounter.text}");
            }
        }

        private bool IncreaseCounterPreferred(PPCounters counter, out int newIndex)
        {
            var currentOrder = _preferredOrder.IndexOf(counter);
            if (currentOrder <= 0)
            {
                newIndex = currentOrder;
                return false;
            }

            newIndex = currentOrder - 1;
            SwapPreferredIndices(currentOrder, currentOrder - 1);

            return true;
        }

        private bool DecreaseCounterPreferred(PPCounters counter, out int newIndex)
        {
            var currentOrder = _preferredOrder.IndexOf(counter);
            if (currentOrder >= _preferredOrder.Count - 1)
            {
                newIndex = currentOrder;
                return false;
            }

            newIndex = currentOrder + 1;
            SwapPreferredIndices(currentOrder, currentOrder + 1);

            return true;
        }

        private void SwapPreferredIndices(int x, int y)
        {
            PPCounters tmp = _preferredOrder[x];
            _preferredOrder[x] = _preferredOrder[y];
            _preferredOrder[y] = tmp;
        }

        private void ReloadTable()
        {
            UpButton.interactable = false;
            DownButton.interactable = false;
            _customListTableData.data.Clear();

            _preferredOrder = SettingsUtils.GetCounterOrder(PluginSettings.Instance.preferredOrder, PluginSettings.Instance.numCounters);
            foreach (var counter in _preferredOrder)
            {
                _customListTableData.data.Add(new PPCounterCell(counter.ToString()));
            }

            _customListTableData.tableView.ReloadData();
            _customListTableData.tableView.ClearSelection();
        }

        #endregion
    }
}
