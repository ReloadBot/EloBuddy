using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace xRP_Lux
{
    public static class Lux
    {
      
        public static Spell.Skillshot Q;
        public static Spell.Active W;
        public static Spell.Skillshot E;
        public static Spell.Skillshot R;
        public static Spell.Targeted Ignite;
        

        static Lux()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 1300, SkillShotType.Linear);
            W = new Spell.Active(SpellSlot.W, 1150);
            E = new Spell.Skillshot(SpellSlot.E, 1100, SkillShotType.Circular);
            R = new Spell.Skillshot(SpellSlot.R, 3500, SkillShotType.Linear);

            Ignite = new Spell.Targeted(SpellSlot.Summoner1, 500);
            Ignite = new Spell.Targeted(SpellSlot.Summoner2, 500);

        }

    }

}
