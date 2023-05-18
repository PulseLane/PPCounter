namespace PPCounter.Utilities
{
    public static class SongDataUtils
    {
        public static string GetHash(string levelId)
        {
            if (levelId.Contains("custom_level_"))
            {
                var splits = levelId.Split('_');
                return splits[2].ToUpper();
            }
            return levelId;
        }

        public static BeatmapDifficulty GetDifficulty(string difficulty)
        {
            switch (difficulty.ToLower())
            {
                case "easy":
                    return BeatmapDifficulty.Easy;
                case "normal":
                    return BeatmapDifficulty.Normal;
                case "hard":
                    return BeatmapDifficulty.Hard;
                case "expert":
                    return BeatmapDifficulty.Expert;
                case "expertplus":
                    return BeatmapDifficulty.ExpertPlus;
                default:
                    throw new System.Exception("Unrecognized difficulty");
            }
        }
    }
}
