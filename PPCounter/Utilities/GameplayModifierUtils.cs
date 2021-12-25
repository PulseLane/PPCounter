using System;
using System.Collections.Generic;

namespace PPCounter.Utilities
{
    public static class GameplayModifierUtils
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
                modifiers.noFailOn0Energy,
                modifiers.instaFail,
                modifiers.failOnSaberClash,
                modifiers.enabledObstacleType,
                modifiers.noBombs,
                modifiers.fastNotes,
                modifiers.strictAngles,
                false, // DA
                (modifiers.songSpeed.Equals(GameplayModifiers.SongSpeed.Faster) || modifiers.songSpeed.Equals(GameplayModifiers.SongSpeed.SuperFast))
                ? GameplayModifiers.SongSpeed.Normal : modifiers.songSpeed,
                modifiers.noArrows,
                false, // GN
                modifiers.proMode,
                modifiers.zenMode,
                modifiers.smallCubes
           );
        }

        public static float CalculateMultiplier(GameplayModifiersModelSO gameplayModifiersModelSO, GameplayModifiers gameplayModifiers)
        {
            List<GameplayModifierParamsSO> modifierParams = gameplayModifiersModelSO.CreateModifierParamsList(gameplayModifiers);
            float multiplier = gameplayModifiersModelSO.GetTotalMultiplier(modifierParams, 1f) ;

            // ScoreSaber weights these multipliers differently
            if (gameplayModifiers.disappearingArrows)
                multiplier += (DA_SS - DA_ORIGINAL);
            if (gameplayModifiers.ghostNotes)
                multiplier += (GN_SS - GN_ORIGINAL);
            if (gameplayModifiers.songSpeed.Equals(GameplayModifiers.SongSpeed.Faster))
                multiplier += (FS_SS - FS_ORIGINAL);
            return multiplier;
        }

        public static GameplayModifierMask ToMask(this GameplayModifiers gameplayModifiers)
        {
            return ((gameplayModifiers.energyType == GameplayModifiers.EnergyType.Battery) ? GameplayModifierMask.BatteryEnergy : GameplayModifierMask.None) | (gameplayModifiers.noFailOn0Energy ? GameplayModifierMask.NoFail : GameplayModifierMask.None) | (gameplayModifiers.instaFail ? GameplayModifierMask.InstaFail : GameplayModifierMask.None) | ((gameplayModifiers.enabledObstacleType == GameplayModifiers.EnabledObstacleType.NoObstacles) ? GameplayModifierMask.NoObstacles : GameplayModifierMask.None) | (gameplayModifiers.noBombs ? GameplayModifierMask.NoBombs : GameplayModifierMask.None) | (gameplayModifiers.fastNotes ? GameplayModifierMask.FastNotes : GameplayModifierMask.None) | (gameplayModifiers.strictAngles ? GameplayModifierMask.StrictAngles : GameplayModifierMask.None) | (gameplayModifiers.disappearingArrows ? GameplayModifierMask.DisappearingArrows : GameplayModifierMask.None) | ((gameplayModifiers.songSpeed == GameplayModifiers.SongSpeed.Faster) ? GameplayModifierMask.FasterSong : GameplayModifierMask.None) | ((gameplayModifiers.songSpeed == GameplayModifiers.SongSpeed.Slower) ? GameplayModifierMask.SlowerSong : GameplayModifierMask.None) | ((gameplayModifiers.songSpeed == GameplayModifiers.SongSpeed.SuperFast) ? GameplayModifierMask.SuperFastSong : GameplayModifierMask.None) | (gameplayModifiers.noArrows ? GameplayModifierMask.NoArrows : GameplayModifierMask.None) | (gameplayModifiers.ghostNotes ? GameplayModifierMask.GhostNotes : GameplayModifierMask.None) | (gameplayModifiers.proMode ? GameplayModifierMask.ProMode : GameplayModifierMask.None) | (gameplayModifiers.zenMode ? GameplayModifierMask.ZenMode : GameplayModifierMask.None) | (gameplayModifiers.smallCubes ? GameplayModifierMask.SmallCubes : GameplayModifierMask.None);
        }
    }
}
