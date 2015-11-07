using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;
using Color = System.Drawing.Color;

namespace xRP_Lux
{
    internal class Program
    {

        public static Spell.Skillshot Q, W, E, R;
        
        public static Spell.Targeted Ignite;
        public static Menu LuxMenu, DrawMenu, ComboMenu, HarassMenu, LaneClearMenu, MiscMenu, PredMenu;
        public static AIHeroClient Me = ObjectManager.Player;
        public static HitChance QHitChance;
        public static HitChance EHitChance;


        public static void Main(string[] args)
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
            W = new Spell.Skillshot(SpellSlot.W, 1075, SkillShotType.Linear);
            E = new Spell.Skillshot(SpellSlot.E, 1200, SkillShotType.Circular);
            R = new Spell.Skillshot(SpellSlot.R, 3300, SkillShotType.Linear);

            // Ignite Spell
            if (HasSpell("summonerdot"))
                Ignite = new Spell.Targeted(ObjectManager.Player.GetSpellSlotFromName("summonerdot"), 600);

            // Menu Settings
            LuxMenu = MainMenu.AddMenu("xRP Lux", "xrplux");
            LuxMenu.AddGroupLabel("xRP-Lux");
            LuxMenu.AddSeparator();
            LuxMenu.AddLabel("xRP-Lux v1.0.0.0");

            ComboMenu = LuxMenu.AddSubMenu("Combo", "sbtw");
            ComboMenu.AddGroupLabel("Combo Settings");
            ComboMenu.AddSeparator();
            ComboMenu.Add("usecomboq", new CheckBox("Use Q"));
            ComboMenu.Add("usecomboe", new CheckBox("Use E"));
            ComboMenu.AddSeparator();
            ComboMenu.Add("usecombow", new Slider("Min Health % to use W", 15));
            ComboMenu.AddSeparator();
            ComboMenu.Add("useignite", new CheckBox("Use Ignite"));
            ComboMenu.AddSeparator();
            ComboMenu.Add("rkill", new CheckBox("R if Killable"));

            HarassMenu = LuxMenu.AddSubMenu("HarassMenu", "Harass");
            HarassMenu.Add("useQHarass", new CheckBox("Use Q"));
            HarassMenu.Add("useEHarass", new CheckBox("Use E"));
            HarassMenu.Add("waitAA", new CheckBox("wait for AA to finish", false));

            MiscMenu = LuxMenu.AddSubMenu("Misc", "misc");
            MiscMenu.AddGroupLabel("Misc");
            MiscMenu.AddSeparator();
            MiscMenu.Add("kse", new CheckBox("KS with E"));
            MiscMenu.Add("ksr", new CheckBox("KS with R"));
            MiscMenu.AddSeparator();
            MiscMenu.Add("WLowAllies", new CheckBox("Use W on % Hp Allies"));
            MiscMenu.Add("WHPPercent", new Slider("Ally HP %", 45));
            MiscMenu.AddSeparator();
            MiscMenu.Add("gapq", new CheckBox("Q on Gapcloser"));
            MiscMenu.AddSeparator();
            MiscMenu.Add("zhonias", new CheckBox("Auto Zhonias"));
            MiscMenu.Add("ZPercent", new Slider("Zhonias HP %", 20));

            PredMenu = LuxMenu.AddSubMenu("Prediction", "pred");
            PredMenu.AddGroupLabel("Prediction");
            PredMenu.AddSeparator();
            PredMenu.Add("predq", new CheckBox("Q Hit Chance [CHECK FOR MEDIUM | NO CHECK FOR HIGH]"));
            PredMenu.AddSeparator();
            PredMenu.Add("prede", new CheckBox("E Hit Chance [ CHECK FOR MEDIUM | NO CHECK FOR HIGH]"));


            DrawMenu = LuxMenu.AddSubMenu("Drawings", "drawings");
            DrawMenu.AddGroupLabel("Drawings");
            DrawMenu.AddSeparator();
            DrawMenu.Add("drawq", new CheckBox("Draw Q"));
            DrawMenu.Add("drawe", new CheckBox("Draw E"));
            DrawMenu.AddSeparator();

            LaneClearMenu = LuxMenu.AddSubMenu("Lane Clear", "laneclear");
            LaneClearMenu.AddGroupLabel("Lane Clear Settings");
            LaneClearMenu.Add("LCE", new CheckBox("Use E"));


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
        public static void AutoW()
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
                    var predq = Q.GetPrediction(ally).CastPosition;

                    W.Cast(predq);
                }
            }
        }

        // GapCloser
        private static void Gapcloser_OnGapCloser
            (AIHeroClient sender, Gapcloser.GapcloserEventArgs gapcloser)
        {
            if (!MiscMenu["gapq"].Cast<CheckBox>().CurrentValue) return;
            if (ObjectManager.Player.Distance(gapcloser.Sender, true) <
                Q.Range*Q.Range && sender.IsValidTarget() && sender.IsEnemy)
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

        // Combo Draw Damage
        

        private static void Tick(EventArgs args)
        {


            QHitChance = PredMenu["predq"].Cast<CheckBox>().CurrentValue ? HitChance.Medium : HitChance.High;
            EHitChance = PredMenu["prede"].Cast<CheckBox>().CurrentValue ? HitChance.Medium : HitChance.High;
            Killsteal();
            // AutoCast();

            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                    Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                LaneClearA.LaneClear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }

            //Auto Ignite
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
                                    a.Health < 50 + 20*Me.Level - (a.HPRegenRate/5*3)))
                {
                    Ignite.Cast(source);
                    return;
                }
            }

            //Auto Zhonias

            var zhoniascheck = MiscMenu["zhonias"].Cast<CheckBox>().CurrentValue;
            var zpercent = MiscMenu["ZPercent"].Cast<Slider>().CurrentValue;
            var zhonias = new Item((int)ItemId.Zhonyas_Hourglass);

            if (zhoniascheck && zhonias.IsReady())
            {
                if (Me.HealthPercent <= zpercent)
                {
                    zhonias.Cast();
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
                    // ignored
                }
            }
            if (MiscMenu["ksr"].Cast<CheckBox>().CurrentValue && R.IsReady())
            {
                try
                {
                    foreach (var rtarget in EntityManager.Heroes.Enemies.Where(hero => hero.IsValidTarget(R.Range)))
                    {
                        var rDamage = Me.CalculateDamageOnUnit(rtarget, DamageType.Magical,
                            new float[] { 0, 300, 400, 500 }[R.Level] + (75 * Me.FlatMagicDamageMod));

                        if (rDamage > rtarget.Health)
                        {
                            var poutput = R.GetPrediction(rtarget);

                            if (poutput.HitChance >= HitChance.Medium)
                            {
                                R.Cast(poutput.CastPosition);
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


            if (useQ && Q.IsReady())
            {
                var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
                var predq = Q.GetPrediction(target).CastPosition;
                

                if (target.IsValidTarget(Q.Range))
                {
                    if (Q.GetPrediction(target).HitChance >= QHitChance)
                    {
                       
                        Q.Cast(predq);

                    }
                }
            }

            if (W.IsReady() && Me.HealthPercent <= useW)
            {
                W.Cast();
            }

            if (useE && E.IsReady())
            {
                var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
                var prede = E.GetPrediction(target);
                if (target.IsValidTarget(E.Range))
                {
                    if (E.GetPrediction(target).HitChance >= EHitChance)
                    {
                        E.Cast(prede.CastPosition);

                    }
                }
            }

            if (useR && R.IsReady())
            {
                var target = TargetSelector.GetTarget(R.Range, DamageType.Magical);
                var rDamage = Me.CalculateDamageOnUnit(target, DamageType.Magical,
                    new float[] {0, 300, 400, 500}[R.Level] + (75 * Me.FlatMagicDamageMod));
                var predR = R.GetPrediction(target);

                if (target.IsValidTarget(R.Range))
                {
                    if (rDamage >= target.Health)
                    {
                        if (predR.HitChance >= HitChance.Medium)
                        {
                            R.Cast(predR.CastPosition);
                        }
                    }
                }

            }

        }

        public static void Harass()
        {
            var target = TargetSelector.GetTarget(800, DamageType.Magical);
            Orbwalker.OrbwalkTo(Game.CursorPos);
            if (Orbwalker.IsAutoAttacking && HarassMenu["waitAA"].Cast<CheckBox>().CurrentValue)
                return;
            if (HarassMenu["useQHarass"].Cast<CheckBox>().CurrentValue && Q.IsReady())
            {
                if (target.Distance(Me) <= Q.Range )
                {
                    var predQ = Q.GetPrediction(target).CastPosition;
                    Q.Cast(predQ);
                    return;
                }
            }

            if (HarassMenu["useEHarass"].Cast<CheckBox>().CurrentValue && E.IsReady())
            {
                if (target.Distance(Me) <= E.Range)
                {
                    var predE = E.GetPrediction(target).CastPosition;
                    E.Cast(predE);
                }
            }
        }

    }
}
