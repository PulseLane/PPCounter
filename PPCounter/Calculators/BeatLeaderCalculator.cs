using BeatLeader.Models;
using PPCounter.Data;
using PPCounter.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using static PPCounter.Utilities.Structs;

namespace PPCounter.Calculators
{
    internal class BeatLeaderCalculator
    {
        [Inject] private BeatLeaderData beatLeaderData;

        private List<Point> _accCurve;
        private float[] _accSlopes;
        private float _accMultiplier;

        private float _passExponential;
        private float _passMultiplier;
        private float _passShift;

        private float _techExponentialMultiplier;
        private float _techMultiplier;

        private float _inflateExponential;
        private float _inflateMultiplier;

        private float _modifierMultiplier;
        private ModifiersMap _modifiersMap;
        private float _powerBottom;

        BeatLeaderRating _rating;
        private float _passPP;

        public void SetCurve(Structs.BeatLeader beatLeader, SongID songID, GameplayModifiers modifiers)
        {
            _accCurve = beatLeader.accCurve;
            _accMultiplier = beatLeader.accMultiplier;

            _passExponential = beatLeader.passExponential;
            _passMultiplier = beatLeader.passMultiplier;
            _passShift = beatLeader.passShift;

            _techExponentialMultiplier = beatLeader.techExponentialMultiplier;
            _techMultiplier = beatLeader.techMultiplier;

            _inflateExponential = beatLeader.inflateExponential;
            _inflateMultiplier = beatLeader.inflateMultiplier;

            _modifiersMap = beatLeaderData.GetModifiersMap(songID);

            CalculateModifiersMultiplier(songID, modifiers);

            _powerBottom = 0;

            _rating = beatLeaderData.GetStars(songID);
            _passPP = GetPassPP(_rating.passRating * _modifierMultiplier);

            _accSlopes = CurveUtils.GetSlopes(_accCurve);
        }

        public bool IsRanked(SongID songID)
        {
            return beatLeaderData.IsRanked(songID);
        }

        // hopefully this doesn't take too long to run...
        public float CalculatePP(SongID songID, float accuracy, bool failed = false)
        {
            var multiplier = _modifierMultiplier + (failed ? _modifiersMap.nf : 0);

            float passPP = _passPP;

            // TODO: don't calculate this every time
            if (failed)
            {
                passPP = GetPassPP(_rating.passRating * multiplier);
            }

            float accPP = GetAccPP(_rating.accRating * multiplier, accuracy);
            float techPP = GetTechPP(_rating.techRating * multiplier, accuracy);

            float rawPP = Inflate(passPP + accPP + techPP);

            if (float.IsInfinity(rawPP) || float.IsNaN(rawPP) || float.IsNegativeInfinity(rawPP))
            {
                rawPP = 0;
            }

            return rawPP;
        }

        private float Inflate(float pp)
        {
            if (Mathf.Approximately(_powerBottom, 0))
            {
                _powerBottom = Mathf.Pow(_inflateMultiplier, _inflateExponential);
            }

            return _inflateMultiplier * Mathf.Pow(pp, _inflateExponential) / _powerBottom;
        }

        private float GetPassPP(float passRating)
        {
            float passPP = _passMultiplier * Mathf.Exp(Mathf.Pow(passRating, _passExponential)) + _passShift;
            if (float.IsInfinity(passPP) || float.IsNaN(passPP) || float.IsNegativeInfinity(passPP) || passPP < 0)
            {
                passPP = 0;
            }

            return passPP;
        }

        private float GetAccPP(float accRating, float accuracy)
        {
            return CurveUtils.GetCurveMultiplier(_accCurve, _accSlopes, accuracy) * accRating * _accMultiplier;
        }

        private float GetTechPP(float techRating, float accuracy)
        {
            return (float) Math.Exp(_techExponentialMultiplier * accuracy) * _techMultiplier * techRating;
        }

        private void CalculateModifiersMultiplier(SongID songID, GameplayModifiers modifiers)
        {
            _modifierMultiplier = 1;

            if (modifiers.disappearingArrows)
            {
                _modifierMultiplier += _modifiersMap.da;
            }
            if (modifiers.songSpeed.Equals(GameplayModifiers.SongSpeed.Faster))
            {
                _modifierMultiplier += _modifiersMap.fs;
            }
            else if (modifiers.songSpeed.Equals(GameplayModifiers.SongSpeed.Slower))
            {
                _modifierMultiplier += _modifiersMap.ss;
            }
            else if (modifiers.songSpeed.Equals(GameplayModifiers.SongSpeed.SuperFast))
            {
                _modifierMultiplier += _modifiersMap.sf;
            }
            if (modifiers.ghostNotes)
            {
                _modifierMultiplier += _modifiersMap.gn;
            }
            if (modifiers.noArrows)
            {
                _modifierMultiplier += _modifiersMap.na;
            }
            if (modifiers.noBombs)
            {
                _modifierMultiplier += _modifiersMap.nb;
            }
            if (modifiers.enabledObstacleType.Equals(GameplayModifiers.EnabledObstacleType.NoObstacles))
            {
                _modifierMultiplier += _modifiersMap.no;
            }
            if (modifiers.proMode)
            {
                _modifierMultiplier += _modifiersMap.pm;
            }
            if (modifiers.smallCubes)
            {
                _modifierMultiplier += _modifiersMap.sc;
            }
        }
    }
}
