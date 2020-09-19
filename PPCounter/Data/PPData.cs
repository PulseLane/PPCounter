using PPCounter.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using static PPCounter.Utilities.Structs;

namespace PPCounter.Data
{
    class PPData : IInitializable, IDisposable
    {
        public bool Init { get; private set; } = false;
        private GameObject _ppDownloaderObject;
        private Dictionary<string, RawPPData> _songData;

        public void Initialize()
        {
            Logger.log.Debug("Initializaing PP Data");
            _ppDownloaderObject = new GameObject("PPDownloader");
            var ppDownloader = _ppDownloaderObject.AddComponent<PPDownloader>();
            ppDownloader.OnDataDownloaded += OnDataDownloaded;
            ppDownloader.OnError += OnError;
            ppDownloader.StartDownloading();
        }

        public void OnDataDownloaded(Dictionary<string, RawPPData> songData)
        {
            _songData = songData;
            Logger.log.Debug("Loaded pp data");
            Init = true;
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
    }
}
