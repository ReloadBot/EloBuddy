using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using Color = System.Drawing.Color;

namespace xRP___Varus
{
    class Program
    {
        public static AIHeroClient Player
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

        public static void Main(string[] args)
        {
            if (EloBuddy.Player.Instance.ChampionName != ChampName)
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
            ComboMenu.Add("comboq", new CheckBox("Use (Q) in Combo"));
            ComboMenu.Add("comboe", new CheckBox("Use (E) in Combo"));
            ComboMenu.Add("combor", new Slider  ("Min Enemy to (R)", 3, 0, 5 ));

            ItemMenu = Menu.AddSubMenu("Draw Menu", "xDraw");
            ItemMenu.Add("usemana", new CheckBox("Use ManaMune in Combo"));
            ItemMenu.Add("useer", new CheckBox("Use  BOTRK in Combo"));



            FarmMenu = Menu.AddSubMenu("Lane Menu", "xlane");
            FarmMenu.Add("farme", new Slider("Use (W) Farm Min Minions", 1,0,30));
            FarmMenu.Add("farmq", new CheckBox("Use (Q) to Farm"));

            HarasMenu = Menu.AddSubMenu("Haras Menu", "xharas");
            HarasMenu.Add("hq", new CheckBox("Use (Q) to Harass"));

            DrawMenu = Menu.AddSubMenu("Draw Menu", "xDraw");
            DrawMenu.Add("dq", new CheckBox("Draw (Q)"));
            DrawMenu.Add("de", new CheckBox("Draw (W)"));
            DrawMenu.Add("drawDisable", new CheckBox("Disable all Draws"));

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
                var usee = ComboMenu["comboe"].Cast<CheckBox>().CurrentValue;
                

            if (useq)
            {
                var target = TargetSelector.GetTarget(Varus.Q.MaximumRange - 50, DamageType.Physical);
                var predq = Varus.Q.GetPrediction(target);
                {
                    if (Varus.Q.IsInRange(target) && Varus.Q.IsReady())
                    {
                        if (Varus.Q.IsCharging)
                        {
                            Varus.Q2.Cast(predq.CastPosition);
                        }
                        else
                        {
                            Varus.Q.StartCharging();
                        }
                    }
                }
            }

            if (usee)
            {
                var target = TargetSelector.GetTarget(Varus.E.Radius, DamageType.Physical);
                {
                    if (target.IsValid && Varus.E.IsInRange(target))
                       
                    {
                        Varus.E.Cast(target);
                    }
                }
            }



                // Items Usage
            var menumura = ItemMenu["usemura"].Cast<CheckBox>().CurrentValue;
            var menubotrk = ItemMenu["useer"].Cast<CheckBox>().CurrentValue;


                Item manamune = new Item((int)ItemId.Manamune, 550);
                Item botrk = new Item((int)ItemId.Blade_of_the_Ruined_King, 550);


                if (menumura && manamune.IsReady())
            {
                var enemy = TargetSelector.GetTarget(manamune.Range, DamageType.Physical);

                if (enemy.IsValid && manamune.IsInRange(enemy))
                {
                    manamune.Cast(enemy);
                }
            }


            if (menubotrk && botrk.IsReady())
                
            {
                var enemy = TargetSelector.GetTarget(botrk.Range, DamageType.Physical);
                if (enemy.IsValidTarget(botrk.Range) &&
                Player.Health + Player.GetItemDamage(enemy, (ItemId) botrk.Id) < Player.MaxHealth)
                {
                    botrk.Cast(enemy);
                }
            }

        }
        


        private static void LaneClear()
        {
            var minion = ObjectManager.Get<Obj_AI_Minion>().FirstOrDefault(a => a.IsEnemy && !a.IsDead && a.Distance(Player) < Player.AttackRange);

            if (minion == null)
                return;
            if (Varus.E.IsReady() && Varus.E.IsInRange(minion))
                if (minion.CountEnemiesInRange(Player.GetAutoAttackRange()) >= ComboMenu["farme"].Cast<Slider>().CurrentValue)
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
            var enemy = TargetSelector.GetTarget(Varus.Q.Range, DamageType.Physical);
            var predQ = Varus.Q.GetPrediction(enemy).CastPosition;

            if (enemy.IsValid && Varus.Q.IsReady() && Varus.Q.IsInRange(enemy) && HarasMenu["hq"].Cast<CheckBox>().CurrentValue)
            {

                Varus.Q.Cast(predQ);
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
