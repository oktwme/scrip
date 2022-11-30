﻿using System.Collections.Generic;
using EnsoulSharp;
using EnsoulSharp.SDK;

namespace xSalice_Reworked.Managers
{
    internal static class SpellManager
    {
        //Spells
        public static readonly List<Spell> SpellList = new List<Spell>();

        public static Spell P;
        public static Spell QE;
        public static Spell Q;
        public static Spell Q2;
        public static Spell QExtend;
        public static Spell W;
        public static Spell W2;
        public static Spell E;
        public static Spell E2;
        public static Spell R;
        public static Spell R2;
        public static SpellSlot Flash = SpellSlot.Unknown;
        public static readonly SpellDataInst QSpell = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q);
        public static readonly SpellDataInst ESpell = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E);
        public static readonly SpellDataInst WSpell = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W);
        public static readonly SpellDataInst RSpell = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R);

        //constructor
        static SpellManager() { }
    }
}