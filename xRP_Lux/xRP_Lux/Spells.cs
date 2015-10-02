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
            Q = new Spell.Skillshot(SpellSlot.Q, 1175, SkillShotType.Linear);
            W = new Spell.Active(SpellSlot.W, 1075);
            E = new Spell.Skillshot(SpellSlot.E, 1100, SkillShotType.Circular);
            R = new Spell.Skillshot(SpellSlot.R, 3340, SkillShotType.Linear);

            SpellDataInst Sum1 = Program._player.Spellbook.GetSpell(SpellSlot.Summoner1);
            SpellDataInst Sum2 = Program._player.Spellbook.GetSpell(SpellSlot.Summoner2);
            if (Sum1.Name == "summonerdot")
                Ignite = new Spell.Targeted(SpellSlot.Summoner1, 600);
            else if (Sum2.Name == "summonerdot")
                Ignite = new Spell.Targeted(SpellSlot.Summoner2, 600);
        }

    }

}
