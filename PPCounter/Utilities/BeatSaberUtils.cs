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
            return new GameplayModifiers(
                modifiers.demoNoFail,
                modifiers.demoNoObstacles,
                modifiers.energyType,
                modifiers.noFail,
                modifiers.instaFail,
                modifiers.failOnSaberClash,
                modifiers.enabledObstacleType,
                modifiers.noBombs,
                modifiers.fastNotes,
                modifiers.strictAngles,
                false, // DA
                modifiers.songSpeed.Equals(GameplayModifiers.SongSpeed.Faster) ? GameplayModifiers.SongSpeed.Normal : modifiers.songSpeed,
                modifiers.noArrows,
                false // GN
           );
        }

        public static GameplayModifiers RemoveNoFail(GameplayModifiers modifiers)
        {
           return new GameplayModifiers(
                modifiers.demoNoFail,
                modifiers.demoNoObstacles,
                modifiers.energyType,
                false,
                modifiers.instaFail,
                modifiers.failOnSaberClash,
                modifiers.enabledObstacleType,
                modifiers.noBombs,
                modifiers.fastNotes,
                modifiers.strictAngles,
                modifiers.disappearingArrows,
                modifiers.songSpeed,
                modifiers.noArrows,
                modifiers.ghostNotes
            );
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
