using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using Color = System.Drawing.Color;

namespace xRP_Lux
{
    class Program
    {


        public static AIHeroClient _player { get { return ObjectManager.Player; } }

        static void Main(string[] args)
        {
            if (Player.Instance.ChampionName != "Lux") return;

            Loading.OnLoadingComplete += Game_OnStart;
            Drawing.OnDraw += OnDraw;
            Game.OnTick += Game_OnTick;
            Game.OnUpdate += Game_OnUpdate;
            
        }

        private static void Game_OnStart(EventArgs args)
        {
            Chat.Print("xRP_Lux LOADED \n Have Fun \n Configure Auto Zhonyas");

        }

        private static void Game_OnTick(EventArgs args)
        {
            Item zhonias = new Item((int)ItemId.Zhonyas_Hourglass, 0);
            var usezonias = config.MiscMenu["xz"].Cast<Slider>().CurrentValue;

            if (usezonias <= _player.HealthPercent)
            {
                if (zhonias.IsReady())
                {
                    zhonias.Cast();
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveModesFlags)
            {
                case Orbwalker.ActiveModes.Combo:Combo();
                    break;
                case Orbwalker.ActiveModes.LaneClear:LaneClear();
                    break;
                case Orbwalker.ActiveModes.Harass:Harass();
                    break;
            }



        }

        private static void Combo()
        {
          
            {
                // Cast Q

                var useq = config.ComboMenu["comboq"].Cast<CheckBox>().CurrentValue;
                if (Lux.Q.IsReady() && useq)
                {
                    var target = TargetSelector.GetTarget(Lux.Q.Range, DamageType.Magical);
                    if (target != null && target.IsValid)
                    {
                        Lux.Q.Cast(target);
                    }
                }

                //Cast W
                var usew = config.ComboMenu["combow"].Cast<CheckBox>().CurrentValue;
                if (usew && Lux.W.IsReady())
                {

                    {
                        Lux.W.Cast();
                    }
                }

                //Cast E
                var usee = config.ComboMenu["comboe"].Cast<CheckBox>().CurrentValue;
                if (usee && Lux.E.IsReady())
                {
                    var Target = TargetSelector.GetTarget(Lux.W.Range, DamageType.Magical);
                    if (Target != null && Target.IsValid)
                    {

                        {
                            Lux.E.Cast(Target.Position);
                        }
                    }
                }

                //Cast R
                var user = config.ComboMenu["combor"].Cast<Slider>().CurrentValue;
                if (user <= TargetSelector.GetTarget(Lux.R.Range, DamageType.Magical).HealthPercent && Lux.R.IsReady())
                {
                    var Target = TargetSelector.GetTarget(Lux.R.Range, DamageType.Magical);
                    if (Target != null && Target.IsValid)
                    {

                        {
                            Lux.R.Cast(Target.Position);
                        }
                    }
                }



            }
        }

        private static void LaneClear()
        {
            
                LaneClear();
            {


                var farmq = config.FarMenu["farmq"].Cast<CheckBox>().CurrentValue;
                var farme = config.FarMenu["farme"].Cast<Slider>().CurrentValue;
                var minions = ObjectManager.Get<Obj_AI_Minion>().OrderBy(m => m.Health).Where(m => m.IsEnemy);

                if (minions == null) return;

                foreach (var minion in minions)
                    if (Lux.Q.IsReady() && Lux.Q.IsInRange(minion) && farmq)
                    {
                        Lux.Q.Cast(minion);
                    }

                foreach (var minion in minions)
                    if (Lux.E.IsReady() && Lux.E.IsInRange(minion) && farme >= minion.CastRange)
                    {
                        Lux.E.Cast(minion);
                    }
            }

        }
        

        private static void Harass()
        {
            var harassq = config.HarassMenu["harassq"].Cast<CheckBox>().CurrentValue;
            var harasse = config.HarassMenu["harasse"].Cast<CheckBox>().CurrentValue;

           
            if (harassq && Lux.Q.IsReady())
            {
                var enemy = TargetSelector.GetTarget(Lux.Q.Range, DamageType.Magical);
                if (enemy.IsValid && enemy.IsRanged)
                {
                    Lux.Q.Cast(enemy);
                }
            }

            if (harasse && Lux.E.IsReady())
            {
                var enemy = TargetSelector.GetTarget(Lux.E.Range, DamageType.Magical);
                if (enemy.IsValid && enemy.IsRanged)

                {
                    Lux.E.Cast(enemy.Position);
                }
            }

        }

        public static void OnDraw(EventArgs args)
        {
            var drawq = config.ComboMenu["drawq"].Cast<CheckBox>().CurrentValue;
            var drawR = config.ComboMenu["drawr"].Cast<CheckBox>().CurrentValue;
            var drawe = config.ComboMenu["drawe"].Cast<CheckBox>().CurrentValue;

            if (drawq)
                new Circle() { Color = Color.Red, Radius = Lux.Q.Radius, BorderWidth = 2f };

            if (drawe)
            {
                new Circle() { Color = Color.LimeGreen, Radius = Lux.E.Range, BorderWidth = 2f };
            }

            if (drawR)
            {
                new Circle() { Color = Color.CornflowerBlue, Radius = Lux.R.Range, BorderWidth = 2f };
            }
        }
    }
}
