namespace PPCounter.Utilities
{
    static class Structs
    {
        public class SongID
        {
            public string id;
            public BeatmapDifficulty difficulty;

            public SongID(string id, BeatmapDifficulty difficulty)
            {
                this.id = id;
                this.difficulty = difficulty;
            }

            public override bool Equals(System.Object obj)
            {
                return this.id == ((SongID)obj).id && this.difficulty == ((SongID)obj).difficulty;
            }

            public override int GetHashCode()
            {
                return id.GetHashCode() + difficulty.GetHashCode();
            }
        }

        public class RawPPData
        {
            public float _Easy_SoloStandard { get; set; }
            public float _Normal_SoloStandard { get; set; }
            public float _Hard_SoloStandard { get; set; }
            public float _Expert_SoloStandard { get; set; }
            public float _ExpertPlus_SoloStandard { get; set; }
        }
    }
}
