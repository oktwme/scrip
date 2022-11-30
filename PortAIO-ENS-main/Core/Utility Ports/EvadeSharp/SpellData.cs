﻿#region

using System;
using EnsoulSharp;

#endregion

namespace Evade
{
    public class SpellData
    {
        public bool AddHitbox;
        public bool CanBeRemoved = false;
        public bool Centered;
        public string ChampionName;
        public CollisionObjectTypes[] CollisionObjects = { };
        public int DangerValue;
        public int Delay;
        public bool DisabledByDefault = false;
        public bool DisableFowDetection = false;
        public bool DontAddExtraDuration;
        public bool DontCheckForDuplicates = false;
        public bool DontCross = false;
        public bool DontRemove = false;
        public int ExtraDuration;
        public string[] ExtraMissileNames = { };
        public int ExtraRange = -1;
        public int MinimalRange = -1;
        public int BehindStart = -1;
        public int DashDelayedAction = -1;
        public int ParticleDetectDelay = 0;
        public string[] ExtraSpellNames = { };
        public bool FixedRange;
        public bool ForceRemove = false;
        public bool FollowCaster = false;
        public bool IsDash = false;
        public Func<AIBaseClient, AIBaseClientNewPathEventArgs, bool> CanDetectDash = null;
        public string FromObject = "";
        public string EndAtParticle = "";
        public string[] FromObjects = { };
        public int Id = -1;
        public bool Invert;
        public bool IsDangerous = false;
        public int MissileAccel = 0;
        public bool MissileDelayed;
        public bool MissileFollowsUnit;
        public int MissileMaxSpeed;
        public int MissileMinSpeed;
        public int MissileSpeed;
        public string MissileSpellName = "";
        public float MultipleAngle;
        public int MultipleNumber = -1;
        public int RingRadius;
        public string SourceObjectName = "";
        public float ParticleRotation = 0f;
        public SpellSlot Slot;
        public string SpellName;
        public bool TakeClosestPath = false;
        public string ToggleParticleName = "";
        public SkillShotType Type;
        private float _radius;
        private float _range;
        
        public SpellData() { }

        public SpellData(string championName,
            string spellName,
            SpellSlot slot,
            SkillShotType type,
            int delay,
            int range,
            int radius,
            int missileSpeed,
            bool addHitbox,
            bool fixedRange,
            int defaultDangerValue)
        {
            ChampionName = championName;
            SpellName = spellName;
            Slot = slot;
            Type = type;
            Delay = delay;
            Range = range;
            _radius = radius;
            MissileSpeed = missileSpeed;
            AddHitbox = addHitbox;
            FixedRange = fixedRange;
            DangerValue = defaultDangerValue;
        }

        public string MenuItemName
        {
            get { return ChampionName + " - " + SpellName; }
        }

        public float Radius
        {
            get
            {
                if (Type == SkillShotType.SkillshotCone)
                    return _radius;

                return (!AddHitbox)
                    ? _radius + Config.SkillShotsExtraRadius
                    : Config.SkillShotsExtraRadius + _radius + (int) ObjectManager.Player.BoundingRadius;
            }
            set { _radius = value; }
        }

        public float RawRadius
        {
            get { return _radius; }
        }

        public float RawRange
        {
            get { return _range; }
        }

        public float Range
        {
            get
            {
                return _range +
                       ((Type == SkillShotType.SkillshotLine || Type == SkillShotType.SkillshotMissileLine)
                           ? Config.SkillShotsExtraRange
                           : 0);
            }
            set { _range = value; }
        }
    }
}