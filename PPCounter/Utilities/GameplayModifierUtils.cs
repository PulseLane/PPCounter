using System;

namespace PPCounter.Utilities
{
    public static class GameplayModifierUtils
    {
        public const float DA_ORIGINAL = 0.07f;
        public const float GN_ORIGINAL = 0.11f;
        public const float FS_ORIGINAL = 0.08f;

        public static GameplayModifiers RemovePositiveModifiers(GameplayModifiers modifiers)
        {
            return new GameplayModifiers(
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

        public static GameplayModifiers RemoveSuperFast(GameplayModifiers modifiers)
        {
            return new GameplayModifiers(
                modifiers.energyType,
                modifiers.noFailOn0Energy,
                modifiers.instaFail,
                modifiers.failOnSaberClash,
                modifiers.enabledObstacleType,
                modifiers.noBombs,
                modifiers.fastNotes,
                modifiers.strictAngles,
                modifiers.disappearingArrows,
                modifiers.songSpeed.Equals(GameplayModifiers.SongSpeed.SuperFast)
                ? GameplayModifiers.SongSpeed.Normal : modifiers.songSpeed,
                modifiers.noArrows,
                modifiers.ghostNotes,
                modifiers.proMode,
                modifiers.zenMode,
                modifiers.smallCubes
           );
        }

        public static GameplayModifierMask ToMask(this GameplayModifiers gameplayModifiers)
        {
            return ((gameplayModifiers.energyType == GameplayModifiers.EnergyType.Battery) ? GameplayModifierMask.BatteryEnergy : GameplayModifierMask.None) | (gameplayModifiers.noFailOn0Energy ? GameplayModifierMask.NoFail : GameplayModifierMask.None) | (gameplayModifiers.instaFail ? GameplayModifierMask.InstaFail : GameplayModifierMask.None) | ((gameplayModifiers.enabledObstacleType == GameplayModifiers.EnabledObstacleType.NoObstacles) ? GameplayModifierMask.NoObstacles : GameplayModifierMask.None) | (gameplayModifiers.noBombs ? GameplayModifierMask.NoBombs : GameplayModifierMask.None) | (gameplayModifiers.fastNotes ? GameplayModifierMask.FastNotes : GameplayModifierMask.None) | (gameplayModifiers.strictAngles ? GameplayModifierMask.StrictAngles : GameplayModifierMask.None) | (gameplayModifiers.disappearingArrows ? GameplayModifierMask.DisappearingArrows : GameplayModifierMask.None) | ((gameplayModifiers.songSpeed == GameplayModifiers.SongSpeed.Faster) ? GameplayModifierMask.FasterSong : GameplayModifierMask.None) | ((gameplayModifiers.songSpeed == GameplayModifiers.SongSpeed.Slower) ? GameplayModifierMask.SlowerSong : GameplayModifierMask.None) | ((gameplayModifiers.songSpeed == GameplayModifiers.SongSpeed.SuperFast) ? GameplayModifierMask.SuperFastSong : GameplayModifierMask.None) | (gameplayModifiers.noArrows ? GameplayModifierMask.NoArrows : GameplayModifierMask.None) | (gameplayModifiers.ghostNotes ? GameplayModifierMask.GhostNotes : GameplayModifierMask.None) | (gameplayModifiers.proMode ? GameplayModifierMask.ProMode : GameplayModifierMask.None) | (gameplayModifiers.zenMode ? GameplayModifierMask.ZenMode : GameplayModifierMask.None) | (gameplayModifiers.smallCubes ? GameplayModifierMask.SmallCubes : GameplayModifierMask.None);
        }
    }
}
