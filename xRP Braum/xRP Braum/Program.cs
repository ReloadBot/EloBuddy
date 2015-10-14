
using System;
using System.Dynamic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace xRP_Braum
{
    internal class Program
    {
        public static Spell.Skillshot Q, R;
        public static Spell.Active W, E;
        public static Menu BrauMenu, HarassMenu, FarMenu, PredMenu, MiscMenu, PermaMenu;
        public static HitChance QHitChance;
        public static AIHeroClient Me = ObjectManager.Player;



        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoaded;
        }

        private static void OnLoaded(EventArgs args)
        {
            if (Player.Instance.ChampionName != "Braum")
                return;
            Bootstrap.Init(null);

            Q = new Spell.Skillshot(SpellSlot.Q, 1000, SkillShotType.Linear);
            W = new Spell.Active(SpellSlot.W, 650);
            E = new Spell.Active(SpellSlot.E, 0);
            R = new Spell.Skillshot(SpellSlot.R, 1200, SkillShotType.Linear);


            BrauMenu = MainMenu.AddMenu("xRP Braum", "xrpbraum");
            BrauMenu.AddGroupLabel("xRP-Braum");
            BrauMenu.AddSeparator();
            BrauMenu.AddLabel("xRP-Braum v1.0.0.0");

            HarassMenu = BrauMenu.AddSubMenu("HarassMenu", "Harass");
            HarassMenu.Add("useQHarass", new CheckBox("Use Q"));

            FarMenu = BrauMenu.AddSubMenu("Farm Menu", "farmenu");
            FarMenu.AddGroupLabel("Farm");
            FarMenu.Add("relicariofarm", new CheckBox("Farm with Relic Shield"));

            PermaMenu = BrauMenu.AddSubMenu("Auto Cast Spells", "permacast");
            PermaMenu.AddGroupLabel("Auto Cast");
            PermaMenu.Add("autow", new CheckBox("Auto Protect with W"));
            PermaMenu.Add("autoe", new CheckBox("Use Shield on Skillshots"));



            MiscMenu = BrauMenu.AddSubMenu("Misc", "misc");
            MiscMenu.AddGroupLabel("Misc");
            MiscMenu.AddSeparator();
            FarMenu.Add("intq", new CheckBox("Interrupt Recall with Q"));

            PredMenu = BrauMenu.AddSubMenu("Prediction", "pred");
            PredMenu.AddGroupLabel("Prediction");
            PredMenu.AddSeparator();
            PredMenu.Add("predq", new CheckBox("Q Hit Chance [CHECK FOR MEDIUM | NO CHECK FOR HIGH]"));


            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
        }

        //Interrupt Spells
        private static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender,
            Interrupter.InterruptableSpellEventArgs args)
        {
            var intTarget = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            {
                if (Q.IsReady() && intTarget.IsRecalling && MiscMenu["intq"].Cast<CheckBox>().CurrentValue)
                    Q.Cast(intTarget.ServerPosition);
            }
        }

        public static void AutoE()
        {
            var shieldAllies = MiscMenu["autoe"].Cast<CheckBox>().CurrentValue;
            var target = TargetSelector.GetTarget(650f, DamageType.Mixed);

            if (shieldAllies)
            {
                var ally = EntityManager.Heroes.Allies.FirstOrDefault(x => x.IsValidTarget(650f));

                if (ally != null)
                {
                    if(target)
                    E.Cast(target.Direction);
                }
            }
        }


    }
}
