using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace xRP_Ashe
{
    class Program
    {
        public static AIHeroClient Me = ObjectManager.Player;

        public static Spell.Active Q;
        public static Spell.Skillshot W, E, R;

        public static Menu AsheMenu, ComboMenu, HarassMenu, FarmMenu, MiscMenu, DrawMenu, ItensMenu, PotionMenu;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoad_Complete;
        }

        public static void OnLoad_Complete(EventArgs args)
        {
            if (Player.Instance.ChampionName != "Ashe") return;
            Bootstrap.Init(null);

            Q = new Spell.Active(SpellSlot.Q);
            W = new Spell.Skillshot(SpellSlot.W, 1200, SkillShotType.Cone);
            E = new Spell.Skillshot(SpellSlot.E, 2000000, SkillShotType.Linear);
            R = new Spell.Skillshot(SpellSlot.R, 3000, SkillShotType.Linear, 250, 1600, 130);


            AsheMenu = MainMenu.AddMenu("xRP Ashe", "sbtwashe");
            AsheMenu.AddGroupLabel("xRP-Ashe");
            AsheMenu.AddSeparator();
            AsheMenu.AddLabel("Made by: xRPdev");

            ComboMenu = AsheMenu.AddSubMenu("Combo Mode");
            ComboMenu.AddGroupLabel("Combo Settings");
            ComboMenu.AddSeparator();
            ComboMenu.Add("useQ", new CheckBox("Use Q in Combo"));
            ComboMenu.Add("countP", new CheckBox("Wait 5 Passive Count to Cast Q"));
            ComboMenu.AddSeparator();
            ComboMenu.Add("useW", new CheckBox("Use W in Combo"));
            ComboMenu.AddSeparator();
            ComboMenu.Add("useR", new CheckBox("Use R in combo"));

            HarassMenu = AsheMenu.AddSubMenu("Harass Mode");
            HarassMenu.AddGroupLabel("Harass Settings");
            HarassMenu.AddSeparator();
            HarassMenu.Add("useQ", new CheckBox("Use Q in harass"));
            HarassMenu.Add("countP", new CheckBox("Wait 5 passive count to cast Q in harass"));
            HarassMenu.AddSeparator();
            HarassMenu.Add("useW", new CheckBox("Use W in harass"));

            FarmMenu = AsheMenu.AddSubMenu("LaneClear Mode");
            FarmMenu.AddGroupLabel("Farm Settings");
            FarmMenu.AddSeparator();
            FarmMenu.Add("farmQ", new CheckBox("Use Q to farm"));
            FarmMenu.Add("countP", new CheckBox("Wait 5 Passive Count to Cast Q in laneClear"));
            FarmMenu.AddSeparator();
            FarmMenu.Add("farmW", new CheckBox("Use W to farm"));

            MiscMenu = AsheMenu.AddSubMenu("Misc Settings");
            MiscMenu.AddGroupLabel("Misc Settings");
            MiscMenu.AddSeparator();
            MiscMenu.Add("autoE", new CheckBox("Cast E when lost target"));
            MiscMenu.AddSeparator();
            MiscMenu.Add("useQjungle", new CheckBox("Jungle Steal Q"));

            Game.OnTick += Tick;
            //Gapcloser.OnGapcloser += Gapcloser_OnGapCloser;

        }

        private static void Tick(EventArgs args)
        {
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                    Combo();
            }
        }

        private static void Combo()
        {
            var useQ = ComboMenu["useQ"].Cast<CheckBox>().CurrentValue;

            if ()
            {
                var target = TargetSelector.GetTarget(Me.GetAutoAttackRange(), DamageType.Physical);
               
            }
        }

    }
}
