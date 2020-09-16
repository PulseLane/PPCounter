namespace PPCounter.Utilities
{
    public static class SongDataUtils
    {
        public static string GetHash(string levelId)
        {
            if (levelId.Contains("custom_level_"))
            {
                var splits = levelId.Split('_');
                return splits[2];
            }
            return levelId;
        }
    }
}
