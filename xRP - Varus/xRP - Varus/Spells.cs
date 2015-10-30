using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace xRP___Varus
{
    public static class Varus
    {
        public static Spell.Chargeable Q;
        public static Spell.Skillshot Q2;
        public static Spell.Active W;
        public static Spell.Skillshot E;
        public static Spell.Skillshot R;

        static Varus()
        {
            Q = new Spell.Chargeable(SpellSlot.Q, 925, 1600, 4);
            Q2 = new Spell.Skillshot(SpellSlot.Q, 1600, SkillShotType.Linear);
            W = new Spell.Active(SpellSlot.W);
            E = new Spell.Skillshot(SpellSlot.E, 925, SkillShotType.Circular);
            R = new Spell.Skillshot(SpellSlot.R, 1100, SkillShotType.Linear);
        }

    }

 
    
}
