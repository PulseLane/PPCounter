using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPCounter.Utilities
{
    public static class BeatSaberUtils
    {
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
    }
}
