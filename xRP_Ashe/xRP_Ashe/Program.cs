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
    internal class Program
    {
        public static AIHeroClient Me = ObjectManager.Player;

        public static Spell.Active Q;
        public static Spell.Skillshot W, E, R;

        public static Menu AsheMenu, ComboMenu, HarassMenu, FarmMenu, MiscMenu, DrawMenu, ItensMenu, PotionMenu;

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoad_Complete;
        }

        public static void OnLoad_Complete(EventArgs args)
        {
            if (Player.Instance.ChampionName != "Ashe") return;
            Bootstrap.Init(null);

            Q = new Spell.Active(SpellSlot.Q);
            W = new Spell.Skillshot(SpellSlot.W, 1200, SkillShotType.Cone);
            E = new Spell.Skillshot(SpellSlot.E, 2500, SkillShotType.Linear);
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
            ComboMenu.Add("hpPercent", new CheckBox("Minimum Hp % to stun"));

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
            FarmMenu.Add("countM", new Slider("Min Minions to cast W"));

            MiscMenu = AsheMenu.AddSubMenu("Misc Settings");
            MiscMenu.AddGroupLabel("Misc Settings");
            MiscMenu.AddSeparator();
            MiscMenu.Add("autoE", new CheckBox("Cast E when lost target"));
            MiscMenu.AddSeparator();
            MiscMenu.Add("useQjungle", new CheckBox("Jungle Steal Q"));
            MiscMenu.AddSeparator();
            MiscMenu.Add("gapr", new CheckBox("R in gapcloser"));
            MiscMenu.Add("intr", new CheckBox("Interrupter with R"));

            Game.OnTick += Tick;
            Gapcloser.OnGapcloser += Gapcloser_OnGapCloser;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;

        }

        //Interrupt
        private static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender,
            Interrupter.InterruptableSpellEventArgs args)
        {
            var intTarget = TargetSelector.GetTarget(R.Range, DamageType.Magical);
            {
                if (R.IsReady() && sender.IsValidTarget(R.Range) && MiscMenu["intr"].Cast<CheckBox>().CurrentValue)
                    R.Cast(intTarget.ServerPosition);
            }
        }

        //gapcloser
        private static void Gapcloser_OnGapCloser
            (AIHeroClient sender, Gapcloser.GapcloserEventArgs gapcloser)
        {
            if (!MiscMenu["gapr"].Cast<CheckBox>().CurrentValue) return;
            if (ObjectManager.Player.Distance(gapcloser.Sender, true) <
                R.Range*R.Range && sender.IsValidTarget())
            {
                R.Cast(gapcloser.Sender);
            }
        }

        private static void Tick(EventArgs args)
        {
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                    Combo();
            }

            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                    LaneClear();
            }
        }


        private static void Combo()
        {
            var useQ = ComboMenu["useQ"].Cast<CheckBox>().CurrentValue;
            var useW = ComboMenu["useW"].Cast<CheckBox>().CurrentValue;
            var useR = ComboMenu["useR"].Cast<CheckBox>().CurrentValue;
            var waitP = ComboMenu["countP"].Cast<CheckBox>().CurrentValue;
            var minHP = ComboMenu["hpPercent"].Cast<Slider>().CurrentValue;

            if (Q.IsReady() && useQ)
            {
                var targetq = TargetSelector.GetTarget(Me.GetAutoAttackRange() - 50, DamageType.Physical);

                if (targetq != null)
                {
                    foreach (var buff in Player.Instance.Buffs)
                    {
                        if (waitP && buff.Name == "asheqcastready" && buff.Count == 5)
                        {
                            Q.Cast();
                        }
                    }
                }
            }

            if (W.IsReady() && useW)
            {
                var targetw = TargetSelector.GetTarget(W.Range, DamageType.Physical);
                var predW = W.GetPrediction(targetw);

                if (targetw != null)
                {
                    if (predW.HitChance >= HitChance.Medium)
                    {
                        W.Cast(predW.CastPosition);
                    }
                }
            }

            if (R.IsReady() && useR)
            {
                var targetr = TargetSelector.GetTarget(R.Range, DamageType.Magical);
                var predR = R.GetPrediction(targetr);
                {
                    if (targetr != null)
                    {
                        if (targetr.HealthPercent <= minHP)
                        {
                            if (predR.HitChance >= HitChance.Medium)
                            {
                                R.Cast(predR.CastPosition);
                            }
                        }
                    }
                }
            }

        }

        private static void LaneClear()
        {
            var farmQ = FarmMenu["farmQ"].Cast<CheckBox>().CurrentValue;
            var waitP = FarmMenu["countP"].Cast<CheckBox>().CurrentValue;
            var farmW = FarmMenu["farmW"].Cast<CheckBox>().CurrentValue;
            var countM = FarmMenu["countM"].Cast<Slider>().CurrentValue;

            if (Q.IsReady() && farmQ)
            {
                foreach (var buff in Player.Instance.Buffs)
                {
                    if (waitP && buff.Name == "asheqcastready" && buff.Count == 5)
                    {
                        Q.Cast();
                    }
                }
            }

            if (W.IsReady() && farmW)
            {
                var minionW = EntityManager.MinionsAndMonsters.EnemyMinions.OrderBy(a => a.Health).FirstOrDefault(
                        a => a.Distance(Player.Instance) <= W.Range && !a.IsDead && !a.IsInvulnerable);

                if (minionW.CountEnemiesInRange(W.Width) >= countM)
                {
                    W.Cast(minionW);
                }
            }
        }
    }
}
