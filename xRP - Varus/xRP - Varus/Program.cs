using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using xRP_Spells;
using Color = System.Drawing.Color;

namespace xRP___Varus
{
    class Program
    {
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }

        }

        public static Menu Menu,
            FarmMenu,
            DrawMenu,
            HarasMenu,
            ItemMenu,
            ComboMenu;

        public const string ChampName = "Varus";

        static void Main(string[] args)
        {
            if (Player.Instance.ChampionName != ChampName)
                return;

            Loading.OnLoadingComplete += Game_OnStart;
            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += OnDraw;
        }

        private static void Game_OnStart(EventArgs args)
        {

            Chat.Print("xRP - Varus LOADED \n 1.0.0v \n HaveFun");

            Menu = MainMenu.AddMenu("xRP Varus", "xvarus");
            Menu.AddSeparator();

            ComboMenu = Menu.AddSubMenu("Combo Menu", "xcombo");
            ComboMenu.Add("comboq", new CheckBox("Use (Q) in Combo", true));
            ComboMenu.Add("comboe", new CheckBox("Use (E) in Combo", true));
            ComboMenu.Add("combor", new Slider  ("Min Enemy to (R)", 3, 0, 5 ));

            ItemMenu = Menu.AddSubMenu("Draw Menu", "xDraw");
            ItemMenu.Add("usemana", new CheckBox("Use ManaMune in Combo", true));
            ItemMenu.Add("useer", new CheckBox("Use  BOTRK in Combo", true));



            FarmMenu = Menu.AddSubMenu("Lane Menu", "xlane");
            FarmMenu.Add("farme", new Slider("Use (W) Farm Min Minions", 1,0,30));
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
            if (Orbwalker.ActiveModesFlags ==(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags == (Orbwalker.ActiveModes.LaneClear))
            {
                LaneClear();
            }

            if (Orbwalker.ActiveModesFlags == (Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
        }

        private static void Combo()
        {

            
                var useq = ComboMenu["comboq"].Cast<CheckBox>().CurrentValue;

                var enemy = TargetSelector.GetTarget(1625, DamageType.Physical);

                if (!enemy.IsValid()) return;


                if (Varus.Q.IsReady() && Varus.Q.IsInRange(enemy) && useq)
                {
                    Varus.Q.Cast(enemy);
                }

                if (enemy.IsValid && Varus.E.IsReady() && Varus.E.IsInRange(enemy) &&
                    ComboMenu["comboe"].Cast<CheckBox>().CurrentValue)
                {
                    Varus.E.Cast(enemy);
                }

                if (Varus.R.IsReady() && Varus.R.IsInRange(enemy) && enemy.IsValid)
                    if (_Player.CountEnemiesInRange(_Player.GetAutoAttackRange()) >=
                        ComboMenu["combor"].Cast<Slider>().CurrentValue)
                    {
                        Varus.R.Cast(enemy);
                    }



                // Items Usage

                Item manamune = new Item((int)ItemId.Manamune, 550);
                Item botrk = new Item((int)ItemId.Blade_of_the_Ruined_King, 550);

                if (ItemMenu["usemura"].Cast<CheckBox>().CurrentValue && manamune.IsReady() &&
                    enemy.IsValidTarget(manamune.Range))
                {
                    manamune.Cast(enemy);
                }

                if (ItemMenu["useer"].Cast<CheckBox>().CurrentValue && botrk.IsReady() &&
                    enemy.IsValidTarget(botrk.Range) &&
                    _Player.Health + _Player.GetItemDamage(enemy, (ItemId)botrk.Id) < _Player.MaxHealth)
                {
                    botrk.Cast(enemy);
                }

            }
        


        private static void LaneClear()
        {
            var minion = ObjectManager.Get<Obj_AI_Minion>().FirstOrDefault(a => a.IsEnemy && !a.IsDead && a.Distance(_Player) < _Player.AttackRange);

            if (minion == null)
                return;
            if (Varus.E.IsReady() && Varus.E.IsInRange(minion))
                if (minion.CountEnemiesInRange(_Player.GetAutoAttackRange()) >= ComboMenu["farme"].Cast<Slider>().CurrentValue)
                {
                    Varus.E.Cast(minion);
                }

            if (Varus.Q.IsReady() && Varus.Q.IsInRange(minion) && FarmMenu["farmq"].Cast<CheckBox>().CurrentValue)
            {
                Varus.Q.Cast(minion);
            }

        }

        private static void Harass()
        {
            var enemy = TargetSelector.GetTarget(1625, DamageType.Physical);

            if (enemy.IsValid && Varus.Q.IsReady() && Varus.Q.IsInRange(enemy) && HarasMenu["hq"].Cast<CheckBox>().CurrentValue)
            {

                Varus.Q.Cast(enemy);
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
                    Radius = Varus.Q.Radius,
                    BorderWidth = 2f

                }.Draw(ObjectManager.Player.Position);
            }

            if (DrawMenu["de"].Cast<CheckBox>().CurrentValue)
            {
                new Circle()
                {
                    Color = Color.MediumPurple,
                    Radius = Varus.E.Radius,
                    BorderWidth = 2f

                }.Draw(ObjectManager.Player.Position);
            }
        }

        



    }
}
