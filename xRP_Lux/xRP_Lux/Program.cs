
using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace xRP_Lux
{
    internal class Program
    {

        public static Spell.Skillshot Q, E, R;
        public static Spell.Active W;
        public static Spell.Targeted Ignite;
        public static Menu LuxMenu, DrawMenu, ComboMenu, LaneClear, MiscMenu, PredMenu;
        public static AIHeroClient Me = ObjectManager.Player;
        public static HitChance QHitChance;
        public static HitChance EHitChance;


        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoaded;
        }

        public static bool HasSpell(string s)
        {
            return Player.Spells.FirstOrDefault(o => o.SData.Name.Contains(s)) != null;
        }

        private static void OnLoaded(EventArgs args)
        {
            if (Player.Instance.ChampionName != "Lux")
                return;
            Bootstrap.Init(null);

            Q = new Spell.Skillshot(SpellSlot.Q, 1175, SkillShotType.Linear);
            W = new Spell.Active(SpellSlot.W, 1075);
            E = new Spell.Skillshot(SpellSlot.E, 1200, SkillShotType.Linear);
            R = new Spell.Skillshot(SpellSlot.R, 3300, SkillShotType.Linear);

            if (HasSpell("summonerdot"))
                Ignite = new Spell.Targeted(ObjectManager.Player.GetSpellSlotFromName("summonerdot"), 600);

            LuxMenu = MainMenu.AddMenu("xRP Lux", "xrplux");
            LuxMenu.AddGroupLabel("xRP-Lux");
            LuxMenu.AddSeparator();
            LuxMenu.AddLabel("xRP-Lux v1.0.0.0");

            ComboMenu = LuxMenu.AddSubMenu("Combo", "sbtw");
            ComboMenu.AddGroupLabel("Combo Settings");
            ComboMenu.AddSeparator();
            ComboMenu.Add("usecomboq", new CheckBox("Use Q"));
            ComboMenu.Add("usecomboe", new CheckBox("Use E"));
            ComboMenu.Add("usecombor", new CheckBox("Use R"));
            ComboMenu.Add("useignite", new CheckBox("Use Ignite"));
            ComboMenu.AddSeparator();
            ComboMenu.Add("rkill", new CheckBox("R if Killable"));

            MiscMenu = LuxMenu.AddSubMenu("Misc", "misc");
            MiscMenu.AddGroupLabel("Misc");
            MiscMenu.AddSeparator();
            MiscMenu.Add("kse", new CheckBox("KS with E"));
            MiscMenu.Add("kse", new CheckBox("KS with R"));
            MiscMenu.Add("WLowAllies", new CheckBox("Use W on % Hp Allies"));
            MiscMenu.Add("WHPPercent", new Slider("Ally HP %", 45));
            MiscMenu.Add("gapq", new CheckBox("Q on Gapcloser"));

            PredMenu = LuxMenu.AddSubMenu("Prediction", "pred");
            PredMenu.AddGroupLabel("Prediction");
            PredMenu.AddSeparator();
            PredMenu.Add("predq", new CheckBox("Q Hit Chance [CHECK FOR MEDIUM | NO CHECK FOR HIGH]"));
            PredMenu.Add("prede", new CheckBox("E Hit Chance [ CHECK FOR MEDIUM | NO CHECK FOR HIGH]"));


            DrawMenu = LuxMenu.AddSubMenu("Drawings", "drawings");
            DrawMenu.AddGroupLabel("Drawings");
            DrawMenu.AddSeparator();
            DrawMenu.Add("drawq", new CheckBox("Draw Q"));
            DrawMenu.Add("drawe", new CheckBox("Draw E"));

            LaneClear = LuxMenu.AddSubMenu("Lane Clear", "laneclear");
            LaneClear.AddGroupLabel("Lane Clear Settings");
            LaneClear.Add("LCE", new CheckBox("Use E"));

            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
            Game.OnTick += Tick;
            Drawing.OnDraw += OnDraw;
            Gapcloser.OnGapcloser += Gapcloser_OnGapCloser;
        }

        //Interrupt
        private static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender,
            Interrupter.InterruptableSpellEventArgs args)
        {
            var intTarget = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            {
                if (Q.IsReady() && sender.IsValidTarget(Q.Range) && MiscMenu["intq"].Cast<CheckBox>().CurrentValue)
                    Q.Cast(intTarget.ServerPosition);
            }
        }

        //AutoW Life
        private static void AutoW()
        {
            var shieldAllies = MiscMenu["WLowAllies"].Cast<CheckBox>().CurrentValue;
            var shieldHealthPercent = MiscMenu["WHPPercent"].Cast<Slider>().CurrentValue;

            if (shieldAllies)
            {
                var ally =
                    EntityManager.Heroes.Allies
                        .FirstOrDefault(x => x.IsValidTarget(W.Range) && x.HealthPercent < shieldHealthPercent);
                if (ally != null && ally.CountEnemiesInRange(650) >= 1)
                {
                    E.Cast(ally);
                }
            }
        }

        // GapCloser
        private static void Gapcloser_OnGapCloser
            (AIHeroClient sender, Gapcloser.GapcloserEventArgs gapcloser)
        {
            if (!MiscMenu["gapq"].Cast<CheckBox>().CurrentValue) return;
            if (ObjectManager.Player.Distance(gapcloser.Sender, true) <
                Q.Range*Q.Range && sender.IsValidTarget())
            {
                Q.Cast(gapcloser.Sender);
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (!Me.IsDead)
            {
                if (DrawMenu["drawq"].Cast<CheckBox>().CurrentValue && Q.IsLearned)
                {
                    Drawing.DrawCircle(Me.Position, Q.Range, Color.Navy);
                }
                if (DrawMenu["drawe"].Cast<CheckBox>().CurrentValue && Q.IsLearned)
                {
                    Drawing.DrawCircle(Me.Position, W.Range, Color.OrangeRed);
                }
            }

        }

        private static void Tick(EventArgs args)
        {
            QHitChance = PredMenu["predq"].Cast<CheckBox>().CurrentValue ? HitChance.Medium : HitChance.High;
            EHitChance = PredMenu["prede"].Cast<CheckBox>().CurrentValue ? HitChance.Medium : HitChance.High;
            Killsteal();

            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                    Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                //LaneClearA.LaneClear();
            }

            {
                if (!ComboMenu["useignite"].Cast<CheckBox>().CurrentValue ||
                    !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                    return;
                foreach (
                    var source in
                        ObjectManager.Get<AIHeroClient>()
                            .Where(
                                a =>
                                    a.IsEnemy && a.IsValidTarget(Ignite.Range) &&
                                    a.Health < 50 + 20 * Me.Level - (a.HPRegenRate / 5 * 3)))
                {
                    Ignite.Cast(source);
                    return;
                }
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
                }


            }
            if (MiscMenu["ksr"].Cast<CheckBox>().CurrentValue && R.IsReady())
            {
                try
                {
                    foreach (var rtarget in EntityManager.Heroes.Enemies.Where(hero => hero.IsValidTarget(R.Range)))
                    {
                        if (Me.GetSpellDamage(rtarget, SpellSlot.R) >= rtarget.Health)
                        {
                            var poutput = E.GetPrediction(rtarget);
                            if (poutput.HitChance >= HitChance.Medium)
                            {
                                R.Cast(poutput.CastPosition);
                            }
                        }
                    }
                }
                catch
                {

                }
                
            }
        }

        private static void Combo ()
        {
            var useQ = ComboMenu["usecomboq"].Cast<CheckBox>().CurrentValue;
            var useE = ComboMenu["usecomboe"].Cast<CheckBox>().CurrentValue;
            var useR = ComboMenu["rkill"].Cast<CheckBox>().CurrentValue;


            if (useQ && Q.IsReady())
            {
                var Target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
                if (Target.IsValidTarget(Q.Range))
                {
                    if (Q.GetPrediction(Target).HitChance >= QHitChance)
                    {
                       
                            Q.Cast(Target);

                        }
                    }
                }

            if (useE && E.IsReady())
            {
                var Target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
                if (Target.IsValidTarget(E.Range))
                {
                    if (E.GetPrediction(Target).HitChance >= EHitChance)
                    {
                        E.Cast(Target);

                    }
                }
            }

            if (useR && R.IsReady())
            {
                var Target = TargetSelector.GetTarget(R.Range, DamageType.Magical);
                if (Target.IsValidTarget(R.Range))
                {
                    if (Me.GetSpellDamage(Target, SpellSlot.R) >= Target.Health)
                    {
                        R.Cast(Target);

                    }
                }

            }
            }





        }

    }
