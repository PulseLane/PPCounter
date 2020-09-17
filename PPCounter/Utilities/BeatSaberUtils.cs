using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPCounter.Utilities
{
    public static class BeatSaberUtils
    {
        private const float DA_ORIGINAL = 0.07f;
        private const float DA_SS = 0.02f;
        private const float GN_ORIGINAL = 0.11f;
        private const float GN_SS = 0.04f;
        private const float FS_ORIGINAL = 0.08f;
        private const float FS_SS = 0.08f;

        public static GameplayModifiers RemovePositiveModifiers(GameplayModifiers modifiers)
        {
            GameplayModifiers newModifiers = new GameplayModifiers(modifiers);
            newModifiers.disappearingArrows = false;
            newModifiers.ghostNotes = false;
            newModifiers.songSpeed = newModifiers.songSpeed.Equals(GameplayModifiers.SongSpeed.Faster) ? GameplayModifiers.SongSpeed.Normal : newModifiers.songSpeed;
            return newModifiers;
        }

        public static GameplayModifiers RemoveNoFail(GameplayModifiers modifiers)
        {
            GameplayModifiers newModifiers = new GameplayModifiers(modifiers);
            newModifiers.noFail = false;
            return newModifiers;
        }

        public static float CalculateMultiplier(GameplayModifiersModelSO gameplayModifiersModelSO, GameplayModifiers modifiers)
        {
            float multiplier = gameplayModifiersModelSO.GetTotalMultiplier(modifiers);

            // ScoreSaber weights these multipliers differently
            if (modifiers.disappearingArrows)
                multiplier += (DA_SS - DA_ORIGINAL);
            if (modifiers.ghostNotes)
                multiplier += (GN_SS - GN_ORIGINAL);
            if (modifiers.songSpeed.Equals(GameplayModifiers.SongSpeed.Faster))
                multiplier += (FS_SS - FS_ORIGINAL);

            return multiplier;
        }
    }
}
