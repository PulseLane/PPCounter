using PPCounter.Settings;
using PPCounter.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Profiling.Memory.Experimental;
using Zenject;
using static PPCounter.Utilities.Structs;

namespace PPCounter.Data
{
    internal class AccSaberData : IInitializable, IDisposable
    {
        private bool _dataInitStart = false;
        public bool DataInit { get; private set; } = false;
        [Inject] private PPDownloader _ppDownloader;
        private Dictionary<Structs.SongID, float> _rankedMaps = new Dictionary<Structs.SongID, float>();

        private static readonly string ACCSABER_FILE_NAME = Path.Combine(Environment.CurrentDirectory, "UserData", "PPCounter", "accsaber.json");

        public void Initialize()
        {
            PluginSettings.OnAccSaberEnabled += GetData;
            if (PluginSettings.Instance.accSaberEnabled)
            {
                GetData();   
            }
        }

        private void GetData()
        {
            if (!_dataInitStart)
            {
                PluginSettings.OnAccSaberEnabled -= GetData;
                _dataInitStart = true;

                //LoadAccSaberFile();
                _ppDownloader.OnAccSaberDataDownloaded += OnDataDownloaded;

                _ppDownloader.StartDownloadingAccSaber();
            }
        }

        public void OnDataDownloaded(List<AccSaberRankedMap> rankedMaps)
        {
            lock (_rankedMaps)
            {
                CreateRankedMapsDict(rankedMaps);
                Logger.log.Debug("Downloaded accsaber data");
                DataInit = true;
                //WriteAccSaberFile();
            }
        }

        public float GetComplexity(Structs.SongID songID)
        {
            if (!DataInit)
            {
                Logger.log.Error("Tried to use AccSaberData when it wasn't initialized");
                throw new Exception("Tried to use AccSaberData when it wasn't initialized");
            }

            if (!_rankedMaps.ContainsKey(songID))
            {
                Logger.log.Error($"Tried to get AP for unrecognized map: {songID}");
                throw new Exception("Tried to get AP for unrecognized map");
            }

            return _rankedMaps[songID];
        }

        public bool IsRanked(Structs.SongID songID)
        {
            return _rankedMaps.ContainsKey(songID);
        }

        //private void LoadAccSaberFile()
        //{
        //    if (File.Exists(ACCSABER_FILE_NAME))
        //    {
        //        try
        //        {
        //            Logger.log.Debug("Found accsaber data file, attempting to load...");
        //            lock (_rankedMaps)
        //            {
        //                if (!DataInit)
        //                {
        //                    _rankedMaps = JsonConvert.DeserializeObject<Dictionary<Structs.SongID, float>>(File.ReadAllText(ACCSABER_FILE_NAME));
        //                    DataInit = true;
        //                }
        //            }
        //        }

        //        catch (Exception e)
        //        {
        //            Logger.log.Error($"Error reading file: {e.Message}");
        //        }
        //    }
        //}

        //private void WriteAccSaberFile()
        //{
        //    OSUtils.WriteFile(_rankedMaps, ACCSABER_FILE_NAME);
        //}

        private void CreateRankedMapsDict(List<AccSaberRankedMap> rankedMaps)
        {
            foreach (var rankedMap in rankedMaps)
            {
                var id = rankedMap.songHash.ToUpper();
                BeatmapDifficulty beatmapDifficulty = SongDataUtils.GetDifficulty(rankedMap.difficulty);
                SongID songID = new SongID(id, beatmapDifficulty);
                _rankedMaps[songID] = rankedMap.complexity;
            }
        }

        public void Dispose()
        {
            PluginSettings.OnAccSaberEnabled -= GetData;
        }
    }
}
