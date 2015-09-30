using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using Color = System.Drawing.Color;

namespace xRP___Varus
{
    class Program
    {

        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }


        public const string ChampName = "Varus";

        public static Menu Menu,
            FarmMenu,
            DrawMenu,
            HarasMenu,
            ComboMenu;

        public static Spell.Skillshot Q;
        public static Spell.Active W;
        public static Spell.Skillshot E;
        public static Spell.Skillshot R;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Game_OnStart;
            Game.OnUpdate += Game_OnUpdate;
            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += OnDraw;
        }

        private static void Game_OnStart(EventArgs args)
        {
            if (Player.Instance.ChampionName != ChampName)
                return;


            Chat.Print("xRP - Varus LOADED \n 1.0.0v \n HaveFun");

            Q = new Spell.Chargeable(SpellSlot.Q, 850, 1475, 2);
            W = new Spell.Active(SpellSlot.W);
            E = new Spell.Skillshot(SpellSlot.E, 925, SkillShotType.Circular);
            R = new Spell.Skillshot(SpellSlot.R, 1075, SkillShotType.Linear);

            Menu = MainMenu.AddMenu("xRP Varus", "xvarus");
            Menu.AddSeparator();

            ComboMenu = Menu.AddSubMenu("Combo Menu", "xcombo");
            ComboMenu.Add("comboq", new CheckBox("Use (Q) in Combo", true));
            ComboMenu.Add("combow", new CheckBox("Use (W) in Combo", true));
            ComboMenu.Add("comboe", new CheckBox("Use (E) in Combo", true));
            ComboMenu.Add("combor", new Slider  ("Min Enemy to (R)", 3, 0, 5 ));

            FarmMenu = Menu.AddSubMenu("Lane Menu", "xlane");
            FarmMenu.Add("farmw", new Slider("Use (W) Farm Min Minions", 1,0,30));
            FarmMenu.Add("farmq", new CheckBox("Use (Q) to Farm", true));

            HarasMenu = Menu.AddSubMenu("Haras Menu", "xharas");
            HarasMenu.Add("hq", new CheckBox("Use (Q) to Harass", true));

            DrawMenu = Menu.AddSubMenu("Draw Menu", "xDraw");
            DrawMenu.Add("dq", new CheckBox("Draw (Q)", true));
            DrawMenu.Add("de", new CheckBox("Draw (W)", true));
            DrawMenu.Add("drawDisable", new CheckBox("Disable all Draws", true));

        }

        private static void Game_OnTick(EventArgs args)
        {
       
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                LaneClear();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
        }

        public static void Game_OnUpdate(EventArgs args)
        {
           
            var enemy = TargetSelector.GetTarget(1000, DamageType.Physical);
            if (!enemy.IsValid()) return;

            if (Q.IsReady() && Q.IsInRange(enemy) && ComboMenu["comboq"].Cast<CheckBox>().CurrentValue)
            {
                Q.Cast(enemy);
            }

            if (W.IsReady() && enemy.IsInAutoAttackRange(ObjectManager.Player) && ComboMenu["combow"].Cast<CheckBox>().CurrentValue)
            {
                W.Cast();
            }

            if (enemy.IsValid && E.IsReady() && E.IsInRange(enemy) && ComboMenu["comboe"].Cast<CheckBox>().CurrentValue)
            {
                E.Cast(enemy);
            }

            if (R.IsReady() && R.IsInRange(enemy) && enemy.IsValid)
                if (_Player.CountEnemiesInRange(_Player.GetAutoAttackRange()) >= ComboMenu["combor"].Cast<Slider>().CurrentValue)
                {
                    R.Cast(enemy);
                }
           
        }

        private static void LaneClear()
        {
            var minion = ObjectManager.Get<Obj_AI_Minion>().FirstOrDefault(a => a.IsEnemy && !a.IsDead && a.Distance(_Player) < _Player.AttackRange);

            if (minion == null)
                return;
            if (W.IsReady() && W.IsInRange(minion))
                if (minion.CountEnemiesInRange(_Player.GetAutoAttackRange()) >= ComboMenu["farmw"].Cast<Slider>().CurrentValue)
                {
                    W.Cast(minion);
                }

            if (Q.IsReady() && Q.IsInRange(minion) && FarmMenu["farmq"].Cast<CheckBox>().CurrentValue)
            {
                Q.Cast(minion);
            }

        }

        private static void Harass()
        {
            var enemy = TargetSelector.GetTarget(1000, DamageType.Physical);

            if (enemy.IsValid && Q.IsReady() && Q.IsInRange(enemy) && HarasMenu["hq"].Cast<CheckBox>().CurrentValue)
            {
                Q.Cast(enemy);

            }

        }

        private static void OnDraw(EventArgs args)
        {

            if (!DrawMenu["drawDisable"].Cast<CheckBox>().CurrentValue)
            {
                new Circle()
                {
                    Color = Color.Blue,
                    Radius = ObjectManager.Player.GetAutoAttackRange(),
                    BorderWidth = 2f

                }.Draw(ObjectManager.Player.Position);

            }


            if (DrawMenu["dq"].Cast<CheckBox>().CurrentValue)
            {
                new Circle()
                {
                    Color = Color.LawnGreen,
                    Radius = Q.Radius,
                    BorderWidth = 2f

                }.Draw(ObjectManager.Player.Position);
            }

            if (DrawMenu["de"].Cast<CheckBox>().CurrentValue)
            {
                new Circle()
                {
                    Color = Color.MediumPurple,
                    Radius = E.Radius,
                    BorderWidth = 2f

                }.Draw(ObjectManager.Player.Position);
            }
        }


    }
}
