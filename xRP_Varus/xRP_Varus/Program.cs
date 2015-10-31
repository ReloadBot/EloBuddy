using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using Color = System.Drawing.Color;

namespace xRP_Varus
{
    internal class Program
    {
        public static Spell.Chargeable Q;
        public static Spell.Skillshot E, R;
        public static AIHeroClient Me = ObjectManager.Player;
        public static Menu VarusMenu, ComboMenu, HarassMenu, MiscMenu, LaneClearMenu, DrawMenu;
        public static HitChance HitChance;

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoaded;

        }

        private static void OnLoaded(EventArgs args)
        {
            if (Player.Instance.ChampionName != "Varus")
                return;
            Bootstrap.Init(null);


            // Spell instances
            Q = new Spell.Chargeable(SpellSlot.Q, 925, 1650, 4);
            E = new Spell.Skillshot(SpellSlot.E, 925, SkillShotType.Circular);
            R = new Spell.Skillshot(SpellSlot.R, 1100, SkillShotType.Linear);

            //Menu Instances
            VarusMenu = MainMenu.AddMenu("xRP Varus", "xrpvarus");
            VarusMenu.AddGroupLabel("xRP-Varus");
            VarusMenu.AddSeparator();
            VarusMenu.AddGroupLabel("Made by: xRPdev");


            ComboMenu = VarusMenu.AddSubMenu("Combo", "sbtwcombo");
            ComboMenu.AddGroupLabel("Combo Settings");
            ComboMenu.AddSeparator();
            ComboMenu.Add("usecomboq", new CheckBox("Use Q"));
            ComboMenu.Add("usecomboe", new CheckBox("Use E"));
            ComboMenu.Add("usecombor", new CheckBox("R to Stun"));


            HarassMenu = VarusMenu.AddSubMenu("HarassMenu", "Harass");
            HarassMenu.Add("useQHarass", new CheckBox("Use Q"));
            HarassMenu.Add("useEHarass", new CheckBox("Use E"));
            HarassMenu.Add("waitAA", new CheckBox("wait for AA to finish", false));

            MiscMenu = VarusMenu.AddSubMenu("Misc", "misc");
            MiscMenu.AddGroupLabel("Misc");
            MiscMenu.AddSeparator();
            MiscMenu.Add("kse", new CheckBox("KS with E"));
            MiscMenu.Add("ksq", new CheckBox("KS with Q"));
            MiscMenu.AddSeparator();
            MiscMenu.Add("gapr", new CheckBox("R on Gapcloser"));

            DrawMenu = VarusMenu.AddSubMenu("Drawings", "drawings");
            DrawMenu.AddGroupLabel("Drawings");
            DrawMenu.AddSeparator();
            DrawMenu.Add("drawq", new CheckBox("Draw Q"));
            DrawMenu.Add("drawe", new CheckBox("Draw E"));
            DrawMenu.Add("drawr", new CheckBox("Draw R"));
            DrawMenu.Add("drawAA", new CheckBox("Draw AutoAttack"));

            LaneClearMenu = VarusMenu.AddSubMenu("Lane Clear", "laneclear");
            LaneClearMenu.AddGroupLabel("Lane Clear Settings");
            LaneClearMenu.Add("LCQ", new CheckBox("Use Q"));
            LaneClearMenu.Add("LCE", new CheckBox("Use E"));


            Game.OnTick += Tick;
            Drawing.OnDraw += OnDraw;
            Gapcloser.OnGapcloser += Gapcloser_OnGapCloser;

        }

        // GapCloser
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

        private static void OnDraw(EventArgs args)
        {
            if (!Me.IsDead)
            {
                if (DrawMenu["drawq"].Cast<CheckBox>().CurrentValue && Q.IsLearned)
                {
                    Drawing.DrawCircle(Me.Position, Q.MaximumRange, Color.Navy);
                }
                if (DrawMenu["drawe"].Cast<CheckBox>().CurrentValue && E.IsLearned)
                {
                    Drawing.DrawCircle(Me.Position, E.Range, Color.OrangeRed);
                }

                if (DrawMenu["drawr"].Cast<CheckBox>().CurrentValue && R.IsLearned)
                {
                    Drawing.DrawCircle(Me.Position, R.Range, Color.DarkOrange);
                }

                if (DrawMenu["drawAA"].Cast<CheckBox>().CurrentValue)
                {
                    Drawing.DrawCircle(Me.Position, Me.GetAutoAttackRange(), Color.DeepPink);
                }

            }



        }

        private static void Tick(EventArgs args)
        {
            Killsteal();

            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                    Combo();
            }

            /*
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                LaneClearA.LaneClear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
            */
        }


        private static void Killsteal()
        {
            if (MiscMenu["kse"].Cast<CheckBox>().CurrentValue && E.IsReady())
            {
                try
                {
                    foreach (
                        var etarget in
                            EntityManager.Heroes.Enemies.Where(
                                hero => hero.IsValidTarget(E.Range) && !hero.IsDead && !hero.IsZombie))
                    {
                        if (Me.GetSpellDamage(etarget, SpellSlot.E) >= etarget.Health)
                        {
                            var poutput = E.GetPrediction(etarget);
                            if (poutput.HitChance >= HitChance.Medium)
                            {
                                E.Cast(poutput.CastPosition);
                            }
                        }
                    }
                }
                catch
                {
                    // ignored
                }
            }
            if (MiscMenu["ksq"].Cast<CheckBox>().CurrentValue && R.IsReady())
            {
                try
                {
                    foreach (
                        var qtarget in EntityManager.Heroes.Enemies.Where(hero => hero.IsValidTarget(Q.MaximumRange)))
                    {

                        if (Me.GetSpellDamage(qtarget, SpellSlot.Q) > qtarget.Health)
                        {
                            var poutput = R.GetPrediction(qtarget);

                            if (poutput.HitChance >= HitChance.Medium)
                            {
                                Q.Cast(poutput.CastPosition);
                            }
                        }
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }


        private static void Combo()
        {
            var useQ = ComboMenu["usecomboq"].Cast<CheckBox>().CurrentValue;
            var useW = ComboMenu["usecombow"].Cast<Slider>().CurrentValue;
            var useE = ComboMenu["usecomboe"].Cast<CheckBox>().CurrentValue;
            var useR = ComboMenu["rkill"].Cast<CheckBox>().CurrentValue;


            var target = TargetSelector.GetTarget(Q.MaximumRange, DamageType.Magical);

            if (target != null)

                if (useQ)
            {
                var prediction = Q.GetPrediction(target);
                if (prediction.HitChance >= Q.MinimumHitChance)
                {
                    if (!Q.IsCharging)
                    {
                        Q.StartCharging();
                        return;
                    }
                    if (Q.Range == Q.MaximumRange)
                    {
                        if (Q.Cast(target))
                        {
                            return;
                        }
                    }
                    else
                    {
                        if (Me.IsInRange(prediction.UnitPosition + (prediction.UnitPosition - Me.ServerPosition).Normalized(), Q.Range))
                        {
                            if (Q.Cast(prediction.CastPosition))
                            {
                                return;
                            }
                        }
                    }

                    if (Q.IsCharging)
                    {
                        return;
                    }


                }
            }
        }
    }
}
        
    
