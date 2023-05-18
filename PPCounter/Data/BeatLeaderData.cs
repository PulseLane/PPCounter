using BeatLeader.Models;
using Newtonsoft.Json;
using PPCounter.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using Zenject;
using static PPCounter.Utilities.Structs;

namespace PPCounter.Data
{
    internal class BeatLeaderData : IInitializable
    {
        private static readonly string BL_CACHE_FILE = Path.Combine(Environment.CurrentDirectory, "UserData", "BeatLeader", "LeaderboardsCache");
        public bool DataInit { get; private set; } = false;

        private Dictionary<SongID, BeatLeaderLeaderboardCacheEntry> _cache = new Dictionary<SongID, BeatLeaderLeaderboardCacheEntry>();

        public void Initialize()
        {
            // TODO: support this better - won't work on first cache creation, or respect in-game cache updates.
            // Could use reflection to access BL cache, but may also want to load data myself so it doesn't rely on bl mod
            TryLoadCache();
        }

        public bool IsRanked(Structs.SongID songID)
        {
            return _cache.ContainsKey(songID) && _cache[songID].DifficultyInfo.stars > 0;
        }

        public BeatLeaderRating GetStars(Structs.SongID songID)
        {
            if (!DataInit)
            {
                Logger.log.Error("Tried to use BeatLeaderData when it wasn't initialized");
                throw new Exception("Tried to use BeatLeaderData when it wasn't initialized");
            }

            if (!_cache.ContainsKey(songID))
            {
                Logger.log.Error($"Tried to get stars for unrecognized map: {songID}");
                throw new Exception("Tried to get stars for unrecognized map");
            }

            var diffInfo = _cache[songID].DifficultyInfo;
            return new BeatLeaderRating(diffInfo.accRating, diffInfo.passRating, diffInfo.techRating);
        }

        public ModifiersMap GetModifiersMap(Structs.SongID songID)
        {
            if (!DataInit)
            {
                Logger.log.Error("Tried to use BeatLeaderData when it wasn't initialized");
                throw new Exception("Tried to use BeatLeaderData when it wasn't initialized");
            }

            if (!_cache.ContainsKey(songID))
            {
                Logger.log.Error($"Tried to get modifiers map for unrecognized map: {songID}");
                throw new Exception("Tried to get modifiers map for unrecognized map");
            }

            var diffInfo = _cache[songID].DifficultyInfo;
            return _cache[songID].DifficultyInfo.modifierValues;
        }

        private void TryLoadCache()
        {
            if (File.Exists(BL_CACHE_FILE))
            {
                try
                {
                    var data = File.ReadAllText(BL_CACHE_FILE);
                    BeatLeaderCacheFileData cacheFileData = JsonConvert.DeserializeObject<BeatLeaderCacheFileData>(data);
                    CreateCache(cacheFileData);
                    DataInit = true;
                }
                catch (Exception e)
                {
                    Logger.log.Error($"Error trying to load bl cache: {e}");
                }
            }
        }

        private void CreateCache(BeatLeaderCacheFileData cacheFileData)
        {
            foreach (var entry in cacheFileData.Entries)
            {
                SongID songID = new SongID(entry.SongInfo.hash.ToUpper(), SongDataUtils.GetDifficulty(entry.DifficultyInfo.difficultyName));
                _cache[songID] = entry;
            }
        }
    }
}
