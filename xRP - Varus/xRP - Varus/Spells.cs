using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace xRP_Spells
{
    public static class Varus
    {
        public static Spell.Skillshot Q;
        public static Spell.Active W;
        public static Spell.Skillshot E;
        public static Spell.Skillshot R;

        static Varus()
        {
            Q = new Spell.Chargeable(SpellSlot.Q, 925, 1625, 4);
            W = new Spell.Active(SpellSlot.W);
            E = new Spell.Skillshot(SpellSlot.E, 925, SkillShotType.Circular);
            R = new Spell.Skillshot(SpellSlot.R, 1075, SkillShotType.Linear);
        }

    }

 
    
}
