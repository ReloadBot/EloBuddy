using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;

namespace xRP_Nasus
{
    class Program
    {
        public static AIHeroClient Me = ObjectManager.Player;
        public static Spell.Active Q, R;
        public static Spell.Targeted W;
        public static Spell.Skillshot E;

        public static Menu NasusMenu, ComboMenu, HarassMenu, FarmMenu, MiscMenu, DrawMenu;



        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoad_Complete;
        }

        public static void OnLoad_Complete(EventArgs args)
        {
            if (Player.Instance.ChampionName != "Nasus") return;
            Bootstrap.Init(null);

            Q = new Spell.Active(SpellSlot.Q, 150);
            W = new Spell.Targeted(SpellSlot.W, 600);
            E = new Spell.Skillshot(SpellSlot.E, 650, SkillShotType.Circular);
            R = new Spell.Active(SpellSlot.R);


            NasusMenu = MainMenu.AddMenu("xRP Nasus","sbtwnasus");
            NasusMenu.AddGroupLabel("xRP_Nasus");
            NasusMenu.AddSeparator();
            NasusMenu.AddGroupLabel("Made by: xRPdev");

            ComboMenu = NasusMenu.AddSubMenu("Combo mode");
            ComboMenu.AddGroupLabel("Combo settings");
            ComboMenu.AddSeparator();
            ComboMenu.Add("useQ", new CheckBox("Use Q in combo"));
            ComboMenu.Add("manaM", new Slider("Q mana manager percent", 10));
            ComboMenu.AddSeparator();
            ComboMenu.Add("useW", new CheckBox("use W when target try run"));
            ComboMenu.AddSeparator();
            ComboMenu.Add("useE", new CheckBox("Use E in combo"));
            ComboMenu.AddSeparator();
            ComboMenu.Add("useR", new CheckBox("use R in combo"));
            ComboMenu.Add("hpPercent", new Slider("Min health percent to cast R", 20));

            HarassMenu = NasusMenu.AddSubMenu("Harass Mode");
            HarassMenu.AddGroupLabel("Harass settings");
            HarassMenu.AddSeparator();
            HarassMenu.Add("useQ", new CheckBox("use Q in harass"));
            HarassMenu.Add("useE", new CheckBox("use E in harass"));

            FarmMenu = NasusMenu.AddSubMenu("Farm mode");
            FarmMenu.AddGroupLabel("Farm Settings");
            FarmMenu.AddSeparator();
            FarmMenu.Add("useQ", new CheckBox("use Q to last hit"));
            FarmMenu.Add("useE", new CheckBox("use E in wave clean"));
            //FarmMenu.Add("minM", new Slider("Min minions to cast E"));

            DrawMenu = NasusMenu.AddSubMenu("Draw settings");
            DrawMenu.AddGroupLabel("Drawings");
            DrawMenu.AddSeparator();
            DrawMenu.Add("drawAA", new CheckBox("Draw auto attack range"));
            DrawMenu.Add("drawW", new CheckBox("Draw W cast range"));
            DrawMenu.Add("drawE", new CheckBox("Draw E cast range"));

            Game.OnTick += Tick;
            Drawing.OnDraw += OnDraw; 

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
                    new Circle() { Color = Color.HotPink, Radius = W.Range, BorderWidth = 2f };
                }
                if (drawE && Q.IsLearned && !Q.IsOnCooldown)
                {
                    new Circle() { Color = Color.Yellow, Radius = E.Range, BorderWidth = 2f };
                }

                if (drawAa)
                {
                    new Circle() { Color = Color.LimeGreen, Radius = Me.GetAutoAttackRange(), BorderWidth = 2f };
                }

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

            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                    Harass();
            }
        }

        private static void Combo()
        {
            var useQ = ComboMenu["useQ"].Cast<CheckBox>().CurrentValue;
            var useW = ComboMenu["useW"].Cast<CheckBox>().CurrentValue;
            var useE = ComboMenu["useE"].Cast<CheckBox>().CurrentValue;
            var useR = ComboMenu["useR"].Cast<CheckBox>().CurrentValue;
            var hpPercent = ComboMenu["hpPercent"].Cast<Slider>().CurrentValue;

            if (Q.IsReady() && useQ)
            {
                if (Me.ManaPercent <= ComboMenu["manaM"].Cast<Slider>().CurrentValue) return;

                var targetQ = TargetSelector.GetTarget(Me.GetAutoAttackRange(), DamageType.Physical);

                if (targetQ.IsValidTarget(Q.Range) && Q.IsInRange(targetQ))
                {
                    Q.Cast();
                }
            }

            if (W.IsReady() && useW)
            {
                var targetW = TargetSelector.GetTarget(W.Range, DamageType.Physical);

                if (targetW.IsValidTarget(W.Range) && !targetW.IsFacing(Me))
                {
                    W.Cast(targetW);

                }

            }

            if (E.IsReady() && useE)
            {
                var targetE = TargetSelector.GetTarget(E.Range, DamageType.Magical);
                var predE = E.GetPrediction(targetE);

                if (targetE.IsValidTarget(E.Range) && predE.HitChance >= HitChance.Medium)
                {
                    E.Cast(predE.CastPosition);
                }
            }

            if (R.IsReady() && useR && Me.HealthPercent <= hpPercent)
            {
                R.Cast();
            }
        }

        private static void LaneClear()
        {
            var useQ = FarmMenu["useQ"].Cast<CheckBox>().CurrentValue;
            var useE = FarmMenu["useE"].Cast<CheckBox>().CurrentValue;

            if (Q.IsReady() && useQ)
            {
                var minion = EntityManager.MinionsAndMonsters.EnemyMinions.OrderBy(a => a.Health).FirstOrDefault(
                        a => a.Distance(Player.Instance) <= Q.Range && !a.IsDead && !a.IsInvulnerable);

                if (minion != null && minion.IsValidTarget(Q.Range) && minion.Health <= Me.GetSpellDamage(minion, SpellSlot.Q))
                {
                    Q.Cast(minion);
                }

            }

            if (E.IsReady() && useE)
            {
                var minionE = EntityManager.MinionsAndMonsters.EnemyMinions.OrderBy(a => a.Health).FirstOrDefault(
                        a => a.Distance(Player.Instance) <= E.Range && !a.IsDead && !a.IsInvulnerable);

                if (minionE.IsValidTarget(E.Range))
                {
                    E.Cast(minionE);
                }
            }
        }

        private static void Harass()
        {
            var useE = HarassMenu["useE"].Cast<CheckBox>().CurrentValue;
            var useQ = HarassMenu["useQ"].Cast<CheckBox>().CurrentValue;

            if (E.IsReady() && useE)
            {
                var targetE = TargetSelector.GetTarget(E.Range, DamageType.Magical);
                var predE = E.GetPrediction(targetE);

                if (targetE.IsValidTarget(E.Range) && predE.HitChance >= HitChance.Medium)
                {
                    E.Cast(predE.CastPosition);
                }


            }

        }

    }
}
