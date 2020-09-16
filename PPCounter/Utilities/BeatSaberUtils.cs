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
    }
}
