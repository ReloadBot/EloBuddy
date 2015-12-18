using System;
using System.Drawing;
using System.Linq;
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

        private static int GetQPassiveBuff
        {
            get
            {
                var data = Player.Instance.Buffs
                    .FirstOrDefault(b => b.DisplayName == "asheqcastready");

                return data != null ? data.Count : 0;
            }
        }


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
            ComboMenu.Add("countP", new CheckBox("Wait Passive Count to Cast Q"));
            ComboMenu.AddSeparator();
            ComboMenu.Add("useW", new CheckBox("Use W in Combo"));
            ComboMenu.AddSeparator();
            ComboMenu.Add("useR", new CheckBox("Use R in combo"));
            ComboMenu.Add("hpPercent", new Slider("Minimum Hp % to stun", 15));

            PotionMenu = AsheMenu.AddSubMenu("Potions Manager");
            PotionMenu.AddGroupLabel("Potions Settings");
            PotionMenu.AddSeparator();
            PotionMenu.Add("useHP", new CheckBox("Use Health Potion"));
            PotionMenu.Add("hpbar", new Slider("Minimum Health Percent", 75));
            PotionMenu.AddSeparator();

            ItensMenu = AsheMenu.AddSubMenu("Itens Settings");
            ItensMenu.AddGroupLabel("itens settings");
            ItensMenu.AddSeparator();
            ItensMenu.Add("useER", new CheckBox("use Botrk in Combo"));
            ItensMenu.Add("hpPorcent", new Slider("Enemy Health Porcent to use Botrk", 30));
            ItensMenu.Add("mehpPorcent", new Slider("My Health Porcent to use Botrk", 50));
            ItensMenu.AddSeparator();
            ItensMenu.Add("useYommus", new CheckBox("Use yommus in Combo"));


            HarassMenu = AsheMenu.AddSubMenu("Harass Mode");
            HarassMenu.AddGroupLabel("Harass Settings");
            HarassMenu.AddSeparator();
            HarassMenu.Add("useQ", new CheckBox("Use Q in harass"));
            HarassMenu.Add("countP", new CheckBox("Wait passive count to cast Q in harass"));
            HarassMenu.AddSeparator();
            HarassMenu.Add("useW", new CheckBox("Use W in harass"));

            FarmMenu = AsheMenu.AddSubMenu("LaneClear Mode");
            FarmMenu.AddGroupLabel("Farm Settings");
            FarmMenu.AddSeparator();
            FarmMenu.Add("farmQ", new CheckBox("Use Q to farm"));
            FarmMenu.Add("countP", new CheckBox("Wait Passive Count to Cast Q in laneClear"));
            FarmMenu.AddSeparator();
            FarmMenu.Add("farmW", new CheckBox("Use W to farm"));
            FarmMenu.Add("countM", new Slider("Min Minions to cast W", 5));

            MiscMenu = AsheMenu.AddSubMenu("Misc Settings");
            MiscMenu.AddGroupLabel("Misc Settings");
            MiscMenu.AddSeparator();
            MiscMenu.Add("autoE", new CheckBox("Cast E when lost target"));
            MiscMenu.AddSeparator();
            MiscMenu.Add("useQjungle", new CheckBox("Jungle Steal Q"));
            MiscMenu.AddSeparator();
            MiscMenu.Add("gapr", new CheckBox("W in gapcloser"));
            MiscMenu.Add("intr", new CheckBox("Interrupter with R"));

            DrawMenu = AsheMenu.AddSubMenu("Drawings");
            DrawMenu.AddGroupLabel("Drawing Settings");
            DrawMenu.AddSeparator();
            DrawMenu.Add("drawW", new CheckBox("Draw Q range"));
            DrawMenu.Add("drawE", new CheckBox("Draw E range"));
            DrawMenu.Add("drawAA", new CheckBox("Draw Auto Attack range"));

            Game.OnTick += Tick;
            Drawing.OnDraw += OnDraw; 
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
                W.Range*W.Range && sender.IsValidTarget() && sender.IsEnemy)
            {
                W.Cast(gapcloser.Sender);
            }
        }

        private static void Tick(EventArgs args)
        {
           
            Potions();
            Itens();
            

            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                    Combo();
                    
            }

            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                    LaneClear();
            }

            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                    Harass();
            }
        }

        private static void Potions()
        {
            var usehp = PotionMenu["useHP"].Cast<CheckBox>().CurrentValue;
            var hpbar = PotionMenu["hpbar"].Cast<Slider>().CurrentValue;

            //potions instance
            var hppot = new Item(ItemId.Health_Potion);

            if (usehp && hppot.IsReady() && Me.HealthPercent <= hpbar)
            {
                hppot.Cast();

            }          


        }

        private static void Itens()
        {
            var useEr = ItensMenu["useER"].Cast<CheckBox>().CurrentValue;
            var hpPorcent = ItensMenu["hpPorcent"].Cast<Slider>().CurrentValue;
            var mehpPorcent = ItensMenu["mehpPorcent"].Cast<Slider>().CurrentValue;
            var useYommus = ItensMenu["useYommus"].Cast<CheckBox>().CurrentValue;

            //itens 
            var botrk = new Item(ItemId.Blade_of_the_Ruined_King, 500);
            var yommus = new Item(ItemId.Youmuus_Ghostblade);

            if (botrk.IsReady() && useEr)
            {
                var targetEr = TargetSelector.GetTarget(botrk.Range, DamageType.Mixed);

                if (targetEr.IsValidTarget(500) && hpPorcent >= targetEr.HealthPercent)
                {
                    botrk.Cast();
                }
                if (Me.HealthPercent <= mehpPorcent)
                {
                    botrk.Cast();
                }

            }

            if (yommus.IsReady() && useYommus)
            {
                var targetY = TargetSelector.GetTarget(600, DamageType.Physical);

                if (targetY.IsValidTarget(600))
                {
                    yommus.Cast();
                }

            }

        }

        
        private static void Combo()
        {


            var useQ = ComboMenu["useQ"].Cast<CheckBox>().CurrentValue;
            var useW = ComboMenu["useW"].Cast<CheckBox>().CurrentValue;
            var useR = ComboMenu["useR"].Cast<CheckBox>().CurrentValue;
            var waitP = ComboMenu["countP"].Cast<CheckBox>().CurrentValue;
            var minHp = ComboMenu["hpPercent"].Cast<Slider>().CurrentValue;

            if (Q.IsReady() && useQ)
            {
                var targetq = TargetSelector.GetTarget(600, DamageType.Physical);

                if (targetq.IsValidTarget(600))
                {
                    if (Me.GetBuffCount("asheqcastready") >= 4)
                        {
                            Q.Cast();
                        }
                    
                }
            }

            if (W.IsReady() && useW)
            {
                var targetw = TargetSelector.GetTarget(W.Range, DamageType.Physical);
                var predW = W.GetPrediction(targetw);

                if (targetw.IsValidTarget(W.Range))
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
                    if (targetr.IsValidTarget(R.Range))
                    {
                        if (targetr.HealthPercent <= minHp)
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

                if (Me.GetBuffCount("asheqcastready") >= 4)
                {
                        Q.Cast();
                    
                }
            }

            if (W.IsReady() && farmW)
            {
                var minionW = EntityManager.MinionsAndMonsters.EnemyMinions.OrderBy(a => a.Health).FirstOrDefault(
                        a => a.Distance(Player.Instance) <= W.Range && !a.IsDead && !a.IsInvulnerable);

                if (minionW.CountEnemiesInRange(W.Range) >= countM)
                {
                    W.Cast(minionW);
                }
            }
        }

        private static void Harass()
        {
            var useQ = HarassMenu["useQ"].Cast<CheckBox>().CurrentValue;
            var countP = HarassMenu["countP"].Cast<CheckBox>().CurrentValue;
            var useW = HarassMenu["useW"].Cast<CheckBox>().CurrentValue;

            if (Q.IsReady() && useQ)
            {
                var targetq = TargetSelector.GetTarget(Me.GetAutoAttackRange(), DamageType.Physical);

                if (countP && Me.GetBuffCount("asheqcastready") >= 4 && targetq.IsValidTarget(Me.GetAutoAttackRange() - 50))
                {
                    Q.Cast();
                }
            }

            if (W.IsReady() && useW)
            {
                var targetw = TargetSelector.GetTarget(W.Range, DamageType.Physical);
             

                if (targetw.IsValidTarget(W.Range))
                {
                    W.Cast(targetw.Position);
                }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            var drawW = DrawMenu["drawW"].Cast<CheckBox>().CurrentValue;
            var drawE = DrawMenu["drawE"].Cast<CheckBox>().CurrentValue;
            var drawAa = DrawMenu["drawAA"].Cast<CheckBox>().CurrentValue;

            if (!Me.IsDead)
            {
                if (drawW && Q.IsLearned && !W.IsOnCooldown)
                {
                    Drawing.DrawCircle(Me.Position, Q.Range, Color.Navy);
                }
                if (drawE && Q.IsLearned && !Q.IsOnCooldown)
                {
                    Drawing.DrawCircle(Me.Position, W.Range, Color.Fuchsia);
                }

                if (drawAa)
                {
                    Drawing.DrawCircle(Me.Position, 600, Color.Red);
                }

            }

        }

    }
}
