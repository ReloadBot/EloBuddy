﻿using System;
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

namespace xRP_Caitlyn
{
    class Program
    {
        public static Spell.Skillshot Q, W, E;
        public static Spell.Targeted R;
        public static AIHeroClient _Player = ObjectManager.Player;
        public static Menu CaitMenu, ComboMenu, DrawMenu, MiscMenu, HarassMenu, FarmMenu, ItemMenu, PotionMenu, PredMenu;
        public static HitChance QHitChance;
        public static HitChance WHitChance;
        public static HitChance EHitChance;



        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoaded;

        }

        private static void OnLoaded(EventArgs args)
        {
            //Check Champ Name
            if (Player.Instance.ChampionName != "Caitlyn")
    return;

                //Spell Instance
                Q = new Spell.Skillshot(SpellSlot.Q, 1200, SkillShotType.Linear);
                W = new Spell.Skillshot(SpellSlot.W, 800, SkillShotType.Circular);
                E = new Spell.Skillshot(SpellSlot.E, 980, SkillShotType.Linear);
                R = new Spell.Targeted(SpellSlot.R, 3000);


            // Menu Settings
            CaitMenu = MainMenu.AddMenu("xRP Caitlyn", "xrpcait");
            CaitMenu.AddGroupLabel("xRP-Caitlyn");
            CaitMenu.AddSeparator();
            CaitMenu.AddGroupLabel("Made by: xRPdev");


            ComboMenu = CaitMenu.AddSubMenu("Combo", "sbtw");
            ComboMenu.AddGroupLabel("Combo Settings");
            ComboMenu.Add("useqcombo", new CheckBox("Use Q"));
            ComboMenu.Add("usewcombo", new CheckBox("Use W"));
            ComboMenu.Add("useecombo", new CheckBox("Use E"));
            ComboMenu.AddSeparator();
            ComboMenu.Add("usercombo", new CheckBox("Use R if Killable"));
            ComboMenu.Add("minR", new Slider("Minimum Range to Cast R [Max for Full Range]", 1500, 0, 3000));


            PredMenu = CaitMenu.AddSubMenu("Prediction", "pred");
            PredMenu.AddGroupLabel("Prediction");
            PredMenu.AddSeparator();
            PredMenu.Add("predq", new CheckBox("Q Hit Chance [CHECK FOR MEDIUM | NO CHECK FOR HIGH]"));
            PredMenu.AddSeparator();
            PredMenu.Add("prede", new CheckBox("E Hit Chance [ CHECK FOR MEDIUM | NO CHECK FOR HIGH]"));
            PredMenu.AddSeparator();
            PredMenu.Add("predw", new CheckBox("W Hit Chance [ CHECK FOR MEDIUM | NO CHECK FOR HIGH]"));

            HarassMenu = CaitMenu.AddSubMenu("Harass", "sbtwharass");
            HarassMenu.AddGroupLabel("Harasss Settings");
            HarassMenu.Add("useQharass", new CheckBox("Use Q Harass"));            
            HarassMenu.Add("useEharasss", new CheckBox("Use E Harass"));
            HarassMenu.Add("useQcc", new CheckBox("Use Q Enemy CC"));

            HarassMenu.Add("waitAA", new CheckBox("wait for AA to finish", false));

            FarmMenu = CaitMenu.AddSubMenu("Farm", "sbtwfarm");
            FarmMenu.AddGroupLabel("Farm Settings");
            FarmMenu.Add("useQfarm", new CheckBox("Use Q to Farm"));

            ItemMenu = CaitMenu.AddSubMenu("Items", "sbtwitem");
            ItemMenu.AddGroupLabel("Itens Settings");
            ItemMenu.Add("useER", new CheckBox("Use Botrk"));
            ItemMenu.Add("ERhealth", new Slider("Min Health % enemy to Botrk", 20));
            ItemMenu.Add("UseYommus", new CheckBox("Use Yommus"));
            ItemMenu.AddSeparator();

            PotionMenu = CaitMenu.AddSubMenu("Potion", "sbtwpotion");
            PotionMenu.AddGroupLabel("Potions Settings");
            PotionMenu.Add("hpPotion", new CheckBox("Hp Potion Use"));
            PotionMenu.Add("hp%", new Slider("Health Percent"));

            MiscMenu = CaitMenu.AddSubMenu("Misc", "sbtwmisc");
            MiscMenu.AddGroupLabel("Misc Settings");
            MiscMenu.Add("ksR", new CheckBox("R Killsteal"));
            MiscMenu.Add("intW", new CheckBox("Interrupt with W"));
            MiscMenu.Add("Egap", new CheckBox("E on Gapcloser"));

            DrawMenu = CaitMenu.AddSubMenu("Draw", "sbtwdraw");
            DrawMenu.AddGroupLabel("Draw Settings");
            DrawMenu.Add("drawAA", new CheckBox("Draw Auto Attack Range"));
            DrawMenu.Add("drawQ", new CheckBox("Draw Q Range"));
            DrawMenu.Add("drawE", new CheckBox("Draw E Range"));
            DrawMenu.Add("drawW", new CheckBox("Draw W Range"));
            DrawMenu.Add("drawR", new CheckBox("Draw R Range"));
            DrawMenu.AddSeparator();
            DrawMenu.Add("disable", new CheckBox("Disable all Drawing"));
            DrawMenu.AddSeparator();
            DrawMenu.Add("drawc", new CheckBox("Draw Combo Damage"));
            


            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
            Game.OnTick += Tick;
            Drawing.OnDraw += OnDraw;
            Gapcloser.OnGapcloser += Gapcloser_OnGapCloser;
            Drawing.OnDraw += OnDamageDraw;


        }

        //Interrupt
        private static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender,
            Interrupter.InterruptableSpellEventArgs args)
        {
            var intTarget = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            {
                if (W.IsReady() && sender.IsValidTarget(W.Range) && MiscMenu["intW"].Cast<CheckBox>().CurrentValue)
                    W.Cast(intTarget.ServerPosition);
                
            }
        }

        // GapCloser
        private static void Gapcloser_OnGapCloser
            (AIHeroClient sender, Gapcloser.GapcloserEventArgs gapcloser)
        {
            if (!MiscMenu["Egap"].Cast<CheckBox>().CurrentValue) return;
            if (ObjectManager.Player.Distance(gapcloser.Sender, true) <
                E.Range * E.Range && sender.IsValidTarget())
            {
                E.Cast(gapcloser.Sender);
            }
        }

        private static void OnDraw(EventArgs args)
        {
            var disable = DrawMenu["disable"].Cast<CheckBox>();

            if (!_Player.IsDead)
            {
                if (disable.CurrentValue)
                    return;

                if (DrawMenu["drawAA"].Cast<CheckBox>().CurrentValue && Q.IsLearned)
                {
                    Drawing.DrawCircle(_Player.Position, _Player.GetAutoAttackRange(), Color.Orange);
                }

                if (DrawMenu["drawQ"].Cast<CheckBox>().CurrentValue && Q.IsLearned)
                {
                    Drawing.DrawCircle(_Player.Position, Q.Range, Color.Navy);
                }

                if (DrawMenu["drawW"].Cast<CheckBox>().CurrentValue && W.IsLearned)
                {
                    Drawing.DrawCircle(_Player.Position, W.Range, Color.Green);
                }
                if (DrawMenu["drawE"].Cast<CheckBox>().CurrentValue && E.IsLearned)
                {
                    Drawing.DrawCircle(_Player.Position, E.Range, Color.Yellow);

                }
                if (DrawMenu["drawR"].Cast<CheckBox>().CurrentValue && R.IsLearned)
                {
                    Drawing.DrawCircle(_Player.Position, R.Range, Color.DeepPink);
                }

            }


        }

        // Draw Combo Kill
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

                    }
                }
            }

        }




        private static void Tick(EventArgs args)
        {
            QHitChance = PredMenu["predq"].Cast<CheckBox>().CurrentValue ? HitChance.Medium : HitChance.High;
            WHitChance = PredMenu["predw"].Cast<CheckBox>().CurrentValue ? HitChance.Medium : HitChance.High;
            EHitChance = PredMenu["prede"].Cast<CheckBox>().CurrentValue ? HitChance.Medium : HitChance.High;

            Killsteal();
            Itens();
            AutoPotion();

            
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                   {
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

          
        }


        private static void Killsteal()
        {
            if (MiscMenu["ksR"].Cast<CheckBox>().CurrentValue && R.IsReady())
            {
                try
                {
                    foreach (var rtarget in EntityManager.Heroes.Enemies.Where(hero => hero.IsValidTarget(R.Range)))
                    {
                        if (_Player.GetSpellDamage(rtarget, SpellSlot.R) >= rtarget.Health)
                        {

                            {
                                R.Cast(rtarget);
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

            var useQ = ComboMenu["useqcombo"].Cast<CheckBox>().CurrentValue;
            var useW = ComboMenu["usewcombo"].Cast<CheckBox>().CurrentValue;
            var useE = ComboMenu["useecombo"].Cast<CheckBox>().CurrentValue;
            var useR = ComboMenu["usercombo"].Cast<CheckBox>().CurrentValue;

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


            if (useW && W.IsReady())
            {
                var target = TargetSelector.GetTarget(W.Range, DamageType.Magical);
                var predw = W.GetPrediction(target).CastPosition;


                if (target.IsValidTarget(W.Range))
                {
                    if (W.GetPrediction(target).HitChance >= WHitChance)
                    {

                        W.Cast(predw);


                    }
                }
            }

            if (useE && E.IsReady())
            {
                var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
                var prede = E.GetPrediction(target).CastPosition;


                if (target.IsValidTarget(E.Range))
                {
                    if (E.GetPrediction(target).HitChance >= EHitChance)
                    {

                        E.Cast(prede);

                    }
                }
            }

            if (useR && R.IsReady())

            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                    return;

                {
                    var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                    var nowH = TargetSelector.GetTarget(R.Range, DamageType.Mixed).Health;

                    // R Damage calculator
                    var rDamage = _Player.CalculateDamageOnUnit(target, DamageType.Physical,
                        new float[] {0, 250, 475, 700}[R.Level] + (2.00f*_Player.FlatPhysicalDamageMod));

                    if (target.IsValidTarget(R.Range) && rDamage > nowH)
                    {
                            R.Cast(target);                      
                    }
                }
            }
        }

        public static void Harass()
        {
            var target = TargetSelector.GetTarget(800, DamageType.Physical);
            Orbwalker.OrbwalkTo(Game.CursorPos);
            if (Orbwalker.IsAutoAttacking && HarassMenu["waitAA"].Cast<CheckBox>().CurrentValue)
                return;
            if (HarassMenu["useQHarass"].Cast<CheckBox>().CurrentValue && Q.IsReady())
            {
                if (target.Distance(_Player) <= Q.Range)
                {
                    var predQ = Q.GetPrediction(target).CastPosition;

                    Q.Cast(predQ);
                    
                }
            }

            if (HarassMenu["useEHarass"].Cast<CheckBox>().CurrentValue && E.IsReady())
            {
                if (target.Distance(_Player) <= E.Range)
                {
                    var predE = E.GetPrediction(target).CastPosition;
                    E.Cast(predE);
                }
            }

            if (target.IsTaunted && target.IsCharmed && target.IsRecalling() && target.IsValidTarget())
            {
                if (HarassMenu["useQcc"].Cast<CheckBox>().CurrentValue)
                {
                    Q.Cast(target);
                }
            }
        }

        private static void Itens()
        {
            var target = TargetSelector.GetTarget(500, DamageType.Physical);

            var botrk = new Item((int)ItemId.Blade_of_the_Ruined_King, 600f);
            var yommus = new Item((int)ItemId.Youmuus_Ghostblade);

            var useEr = ComboMenu["useER"].Cast<CheckBox>().CurrentValue;
            var erHealth = ComboMenu["ERHealth"].Cast<Slider>().CurrentValue;
            var useYommus = ComboMenu["UseYommus"].Cast<CheckBox>().CurrentValue;

            if (botrk.IsReady())
            {
                if (useEr && target.HealthPercent <= erHealth)
                {
                    botrk.Cast(target);
                }
            }

            if (yommus.IsReady() && useYommus)
            {
                yommus.Cast();

            }

        }

        private static void AutoPotion()
        {
            var usePotion = PotionMenu["hpPotion"].Cast<CheckBox>().CurrentValue;
            var hpporcent = PotionMenu["hp%"].Cast<Slider>().CurrentValue;

            var potionhp = new Item((int) ItemId.Health_Potion);

            if (potionhp.IsReady() &&
                usePotion && _Player.Health <= hpporcent)
            {
                potionhp.Cast();
            }

        }

        // Calculate Combo Damage
        public static float ComboDamage(Obj_AI_Base target)
        {
            var damage = 0d;

            if (Q.IsReady(3))
            {
                damage += _Player.GetSpellDamage(target, SpellSlot.Q);
            }

            if (E.IsReady(2))
            {
                damage += _Player.GetSpellDamage(target, SpellSlot.E);
            }

            if (R.IsReady(5))
            {
                damage += _Player.GetSpellDamage(target, SpellSlot.R);
            }

            damage += _Player.GetAutoAttackDamage(target) * 3;
            return (float)damage;
          
        }

       

    }
}
