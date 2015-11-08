using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
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
            HarassMenu.AddGroupLabel("Harass Settings");
            HarassMenu.AddSeparator();
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
            LaneClearMenu.AddSeparator();
            LaneClearMenu.Add("LCQ", new CheckBox("Use Q"));
            LaneClearMenu.Add("countM", new Slider("Min minions to Q", 3, 0, 6));
            LaneClearMenu.Add("LCE", new CheckBox("Use E"));
            LaneClearMenu.Add("countME", new Slider("Min minions to E", 3, 0, 6));
            


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

            
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                LaneClear();
            }
            
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
            
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
            
        }


        private static void Combo()
        {
            var useQ = ComboMenu["usecomboq"].Cast<CheckBox>().CurrentValue;         
            var useE = ComboMenu["usecomboe"].Cast<CheckBox>().CurrentValue;
            var useR = ComboMenu["usecombor"].Cast<CheckBox>().CurrentValue;

            if (Q.IsReady() && useQ)
            {
                var target = TargetSelector.GetTarget(Q.MaximumRange, DamageType.Physical);
                var predQ = Q.GetPrediction(target);
                {
                    if (target.IsValidTarget(Q.MaximumRange) && !Q.IsCharging)
                    {
                        Q.StartCharging();
                        return;
                    }

                    if (Q.IsFullyCharged && !target.IsValidTarget(1650))
                    {
                        return;
                    }

                    if (predQ.HitChance >= HitChance.Medium && target.IsInRange(target, 1500))
                    {
                        Q.Cast(predQ.CastPosition);
                    }
                }
            }

            if (E.IsReady() && useE)
            {
                var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                var predE = E.GetPrediction(target);

                if (target.IsValidTarget(E.Range) && predE.HitChance >= HitChance.Medium)
                {
                    E.Cast(predE.CastPosition);
                }
            }

            if (R.IsReady() && useR)
            {
                var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                var predR = R.GetPrediction(target);

                if (target.IsValidTarget(R.Range))
                {
                    R.Cast(predR.CastPosition);
                }
            }

        }

        private static void LaneClear()
        {

            var farmQ = LaneClearMenu["LCQ"].Cast<CheckBox>().CurrentValue;
            var farmE = LaneClearMenu["LCE"].Cast<CheckBox>().CurrentValue;          

            if (Q.IsReady() && farmQ)
            {
                var minions = EntityManager.MinionsAndMonsters.EnemyMinions.OrderBy(a => a.Health).FirstOrDefault(
                        a => a.Distance(Player.Instance) < Q.MaximumRange && !a.IsDead && !a.IsInvulnerable);

                var countMinion = LaneClearMenu["countM"].Cast<Slider>().CurrentValue;

                if (!Q.IsCharging && minions.CountEnemiesInRange(Q.MaximumRange) >= countMinion)
                {
                    Q.StartCharging();
                    return;
                }

                if (Q.IsFullyCharged)
                {
                    Q.Cast(minions);
                }

            }

            if (E.IsReady() && farmE)
            {
                var minions = EntityManager.MinionsAndMonsters.EnemyMinions.OrderBy(a => a.Health).FirstOrDefault(
                        a => a.Distance(Player.Instance) < E.Range && !a.IsDead && !a.IsInvulnerable);

                var countMinion = LaneClearMenu["countME"].Cast<Slider>().CurrentValue;

                if (minions.CountEnemiesInRange(E.Range) >= countMinion)
                {
                    E.Cast(minions);
                }
            }

        }

        private static void Harass()
        {
            var useQ = HarassMenu["useQHarass"].Cast<CheckBox>().CurrentValue;
            var useE = HarassMenu["useEHarass"].Cast<CheckBox>().CurrentValue;
            var waitAA = HarassMenu["waitAA"].Cast<CheckBox>().CurrentValue;
            if (Orbwalker.IsAutoAttacking && waitAA) return;

            if (Q.IsReady() && useQ)
            {
                var target = TargetSelector.GetTarget(Q.MaximumRange, DamageType.Physical);
                var predQ = Q.GetPrediction(target);
                {
                    if (target.IsValidTarget(Q.MaximumRange) && !Q.IsCharging)
                    {
                        Q.StartCharging();
                        return;
                    }

                    if (Q.IsFullyCharged && !target.IsValidTarget(1650))
                    {
                        return;
                    }

                    if (predQ.HitChance >= HitChance.Medium)
                    {
                        Q.Cast(predQ.CastPosition);
                    }
                }
            }

            if (E.IsReady() && useE)
            {
                var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                var predE = E.GetPrediction(target);

                if (target.IsValidTarget(E.Range) && predE.HitChance >= HitChance.Medium)
                {
                    E.Cast(predE.CastPosition);
                }
            }
        }
    }
}
        
    
