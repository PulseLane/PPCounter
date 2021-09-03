using Newtonsoft.Json;
using PPCounter.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Zenject;
using static PPCounter.Utilities.Structs;

namespace PPCounter.Data
{
    class PPData : IInitializable, IDisposable
    {
        public bool Init { get; private set; } = false;
        private GameObject _ppDownloaderObject;
        private Dictionary<string, RawPPData> _songData = new Dictionary<string, RawPPData>();

        private static readonly string FILE_NAME = Path.Combine(Environment.CurrentDirectory, "UserData", "PPCounter", "pp.json");

        public void Initialize()
        {
            Logger.log.Debug("Initializaing PP Data");
            LoadFile();
            _ppDownloaderObject = new GameObject("PPDownloader");
            var ppDownloader = _ppDownloaderObject.AddComponent<PPDownloader>();
            ppDownloader.OnDataDownloaded += OnDataDownloaded;
            ppDownloader.OnError += OnError;
            ppDownloader.StartDownloading();
        }

        public void OnDataDownloaded(Dictionary<string, RawPPData> songData)
        {
            lock (_songData)
            {
                _songData = songData;
                Logger.log.Debug("Downloaded pp data");
                Init = true;
                WriteToFile();
            }

            GameObject.Destroy(_ppDownloaderObject);
        }

        public void OnError()
        {
            GameObject.Destroy(_ppDownloaderObject);
        }

        public float GetPP(Structs.SongID songID)
        {
            if (!Init)
            {
                Logger.log.Error("Tried to use PPData when it wasn't initialized");
                throw new Exception("Tried to use PPData when it wasn't initialized");
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

        public void Dispose()
        {
            Logger.log.Debug("Disposing of PP Data");
        }

        private void LoadFile()
        {
            if (File.Exists(FILE_NAME))
            {
                try
                {
                    Logger.log.Debug("Found pp data file, attempting to load...");
                    lock (_songData)
                    {
                        if (!Init)
                        {
                            var json = JsonConvert.DeserializeObject<Dictionary<string, RawPPData>>(File.ReadAllText(FILE_NAME));
                            _songData = json;
                            Init = true;
                        }
                    }
                }

                catch (Exception e)
                {
                    Logger.log.Error($"Error reading file: {e.Message}");
                }
            }
        }

        private void WriteToFile()
        {
            Logger.log.Debug("Overwriting file...");
            lock (_songData)
            {
                if (!File.Exists(FILE_NAME))
                {
                    (new FileInfo(FILE_NAME)).Directory.Create();
                }
                File.WriteAllText(FILE_NAME, JsonConvert.SerializeObject(_songData));
            }
        }
    }
}
