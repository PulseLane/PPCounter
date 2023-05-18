using Newtonsoft.Json;
using PPCounter.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using Zenject;
using static PPCounter.Utilities.Structs;

namespace PPCounter.Data
{
    internal class SSData : IInitializable
    {
        public bool DataInit { get; private set; } = false;
        [Inject] private PPDownloader _ppDownloader;
        private Dictionary<string, RawPPData> _songData = new Dictionary<string, RawPPData>();

        private static readonly string SS_PP_FILE_NAME = Path.Combine(Environment.CurrentDirectory, "UserData", "PPCounter", "pp.json");

        public void Initialize()
        {
            LoadPPFile();
            _ppDownloader.OnSSDataDownloaded += OnDataDownloaded;

            _ppDownloader.StartDownloadingSS();
        }

        public void OnDataDownloaded(Dictionary<string, RawPPData> songData)
        {
            lock (_songData)
            {
                _songData = songData;
                Logger.log.Debug("Downloaded ss pp data");
                DataInit = true;
                WritePPFile();
            }
        }

        public float GetPP(Structs.SongID songID)
        {
            if (!DataInit)
            {
                Logger.log.Error("Tried to use SSData when it wasn't initialized");
                throw new Exception("Tried to use SSData when it wasn't initialized");
            }

            switch (songID.difficulty)
            {
                case BeatmapDifficulty.Easy:
                    return _songData[songID.id]._Easy_SoloStandard;
                case BeatmapDifficulty.Normal:
                    return _songData[songID.id]._Normal_SoloStandard;
                case BeatmapDifficulty.Hard:
                    return _songData[songID.id]._Hard_SoloStandard;
                case BeatmapDifficulty.Expert:
                    return _songData[songID.id]._Expert_SoloStandard;
                case BeatmapDifficulty.ExpertPlus:
                    return _songData[songID.id]._ExpertPlus_SoloStandard;
                default:
                    Logger.log.Error("Unknown beatmap difficulty: " + songID.difficulty.ToString());
                    throw new Exception("Unknown difficultry");
            }
        }

        public bool IsRanked(Structs.SongID songID)
        {
            return _songData.ContainsKey(songID.id) && GetPP(songID) > 0;
        }

        private void LoadPPFile()
        {
            if (File.Exists(SS_PP_FILE_NAME))
            {
                try
                {
                    Logger.log.Debug("Found SS pp data file, attempting to load...");
                    lock (_songData)
                    {
                        if (!DataInit)
                        {
                            var json = JsonConvert.DeserializeObject<Dictionary<string, RawPPData>>(File.ReadAllText(SS_PP_FILE_NAME));
                            _songData = json;
                            DataInit = true;
                        }
                    }
                }

                catch (Exception e)
                {
                    Logger.log.Error($"Error reading file: {e.Message}");
                }
            }
        }

        private void WritePPFile()
        {
            OSUtils.WriteFile(_songData, SS_PP_FILE_NAME);
        }
    }
}
