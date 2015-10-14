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

        public static Spell.Skillshot Q, E, R;
        public static Spell.Active W;
        public static Spell.Targeted Ignite;
        public static Menu LuxMenu, DrawMenu, ComboMenu, LaneClearMenu, MiscMenu, PredMenu;
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
            W = new Spell.Active(SpellSlot.W, 1075);
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
            DrawMenu.Add("drawc", new CheckBox("Draw Combo Damage"));
            DrawMenu.Add("disable", new CheckBox("Disable Draw Combo Damage"));

            LaneClearMenu = LuxMenu.AddSubMenu("Lane Clear", "laneclear");
            LaneClearMenu.AddGroupLabel("Lane Clear Settings");
            LaneClearMenu.Add("LCE", new CheckBox("Use E"));


            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
            Game.OnTick += Tick;
            Drawing.OnDraw += OnDraw; 
            Drawing.OnDraw += OnDamageDraw;
            
            Gapcloser.OnGapcloser += Gapcloser_OnGapCloser;

            Chat.Print( "xRP-Lux full LOADED \n Have Fun");
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
                    W.Cast(ally.Position);
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

        // Combo Draw Damage
        public static void OnDamageDraw(EventArgs args)
        {
            var killableText = new Text("",
            new System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif, 9, System.Drawing.FontStyle.Bold));
            var disable = DrawMenu["disable"].Cast<CheckBox>().CurrentValue;
            var drawDamage = DrawMenu["drawc"].Cast<CheckBox>().CurrentValue;
            if (disable) return;

            if (drawDamage)
            {
                foreach (var ai in EntityManager.Heroes.Enemies)
                {
                    if (ai.IsValidTarget())
                    {
                        var drawn = 0;
                        if (ComboDamage(ai) >= ai.Health && drawn == 0)
                        {
                            killableText.Position = Drawing.WorldToScreen(ai.Position) - new Vector2(40, -40);
                            killableText.Color = Color.Red;
                            killableText.TextValue = "FULL COMBO TO KILL";
                            killableText.Draw();
                            
                        }

                    }}
            }



        }

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
                        if (Me.GetSpellDamage(rtarget, SpellSlot.R) >= rtarget.Health)
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
                if (target.IsValidTarget(Q.Range))
                {
                    if (Q.GetPrediction(target).HitChance >= QHitChance)
                    {

                        Q.Cast(target);

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
                if (target.IsValidTarget(E.Range))
                {
                    if (E.GetPrediction(target).HitChance >= EHitChance)
                    {
                        E.Cast(target);

                    }
                }
            }

            if (useR && R.IsReady())
            {
                var target = TargetSelector.GetTarget(R.Range, DamageType.Magical);
                if (target.IsValidTarget(R.Range))
                {
                    if (Me.GetSpellDamage(target, SpellSlot.R) > target.Health)
                    {
                        R.Cast(target);

                    }
                }

            }

        }


        // Calculate Combo Damage
        public static float ComboDamage(Obj_AI_Base target)
        {
            var damage = 0d;

            if (Q.IsReady(3))
            {
                damage += Me.GetSpellDamage(target, SpellSlot.Q);
            }

            if (E.IsReady(2))
            {
                damage += Me.GetSpellDamage(target, SpellSlot.E);
            }

            if (R.IsReady(5))
            {
                damage += Me.GetSpellDamage(target, SpellSlot.R);
            }

            damage += Me.GetAutoAttackDamage(target) * 3;
            return (float)damage;
        }




    }
}
