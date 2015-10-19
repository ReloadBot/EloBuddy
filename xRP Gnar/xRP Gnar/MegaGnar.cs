using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;


namespace xRP_MegaGnar
{

    
    class MegaGnar
    {

        public static Spell.Skillshot Q, W, E;
                


        public static void Initialize()
        {
            //Spells Mega Gnar
            Q = new Spell.Skillshot(SpellSlot.Q, 1100, SkillShotType.Linear);
            W = new Spell.Skillshot(SpellSlot.W, 525, SkillShotType.Circular);
            E = new Spell.Skillshot(SpellSlot.E, 475, SkillShotType.Circular);
        }
        
    }

   
}
