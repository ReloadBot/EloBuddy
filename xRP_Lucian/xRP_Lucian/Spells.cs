using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace xRP_Lucian
{
    public static class Lucian
    {
        public static Spell.Skillshot Q;
        public static Spell.Skillshot W;
        public static Spell.Active E;
        public static Spell.Skillshot R;

        static Lucian()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 1100, SkillShotType.Linear);
            W = new Spell.Skillshot(SpellSlot.W, 1000, SkillShotType.Circular);
            E = new Spell.Active(SpellSlot.E, 425);
            R = new Spell.Skillshot(SpellSlot.R, 1400, SkillShotType.Linear);
        }

    }
}
