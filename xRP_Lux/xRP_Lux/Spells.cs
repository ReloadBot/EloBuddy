using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace xRp_Lux
{
    public static class SpellManager
    {

        public static Spell.Skillshot Q { get; private set; }
        public static Spell.Active W { get; private set; }
        public static Spell.Skillshot E { get; private set; }
        public static Spell.Skillshot R { get; private set; }

        static SpellManager()
        {
            

            Q = new Spell.Skillshot(SpellSlot.Q, 1175,  SkillShotType.Linear);
            W = new Spell.Active(SpellSlot.Q, 1075);
            E = new Spell.Skillshot(SpellSlot.Q, 1200, SkillShotType.Linear);
            R = new Spell.Skillshot(SpellSlot.Q, 3200, SkillShotType.Linear);

        }

        public static void Initialize()
        {
        }
    }
}