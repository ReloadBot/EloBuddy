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

#region ignite


     
#endregion

        public static AIHeroClient _player { get { return ObjectManager.Player; } }

        static void Main(string[] args)
        {
            if (Player.Instance.ChampionName != "Lux") return;

            Loading.OnLoadingComplete += Game_OnStart;
            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += OnDraw;
        }

        private static void Game_OnStart(EventArgs args)
        {
            Chat.Print("xRP_Lux LOADED \n Have Fun \n Configure Auto Zhonyas");

        }

        private static void Game_OnTick(EventArgs args)
        {
           
         

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) { Combo(); }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear)) { LaneClear(); }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)) { Harass(); }

            
            
        }

        public static void Combo()
        {
            
            var useq = config.ComboMenu["comboq"].Cast<CheckBox>().CurrentValue;
            var usew = config.ComboMenu["combow"].Cast<CheckBox>().CurrentValue;
            var usee = config.ComboMenu["comboe"].Cast<CheckBox>().CurrentValue;
            var user = config.ComboMenu["combor"].Cast<Slider>().CurrentValue;

            // Cast Q
            if (Lux.Q.IsReady())
            {
                var Target = TargetSelector.GetTarget(Lux.Q.Range, DamageType.Magical);
                

                if (Target != null && Target.IsValid)
                {
                    Lux.Q.Cast(Target.Position);
                }
            }

            //Cast W
            if (usew && Lux.W.IsReady())
            {
                
                {
                    Lux.W.Cast();
                }
            }

            //Cast E
            if (usee && Lux.E.IsReady())
            {
                var Target = TargetSelector.GetTarget(Lux.W.Range, DamageType.Magical);
                var Pred = Lux.E.GetPrediction(Target);

                if (Target != null && Target.IsValid)
                {
                    
                    {
                        Lux.E.Cast(Pred.CastPosition);
                    }
                }
            }

            //Cast R
            if (user <= TargetSelector.GetTarget(Lux.R.Range, DamageType.Magical).HealthPercent && Lux.R.IsReady())
            {
                var Target = TargetSelector.GetTarget(Lux.R.Range, DamageType.Magical);
                var Pred = Lux.R.GetPrediction(Target);

                if (Target != null && Target.IsValid)
                {

                    {
                        Lux.R.Cast(Pred.CastPosition);
                    }
                }
            }



                   
        }

        public static void LaneClear()
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

        public static void Harass()
        {
            var harassq = config.HarassMenu["harassq"].Cast<CheckBox>().CurrentValue;
            var harasse = config.HarassMenu["harasse"].Cast<CheckBox>().CurrentValue;

            var enemy = TargetSelector.GetTarget(3340, DamageType.Magical);

            if (harassq && Lux.Q.IsReady() && Lux.Q.IsInRange(enemy) && enemy.IsValid)
            {
                Lux.Q.Cast(enemy);

            }

            if (harasse && Lux.E.IsReady() && Lux.E.IsInRange(enemy) && enemy.IsValid)
            {
                Lux.E.Cast(enemy.Position);
            }

        }

        private static void OnDraw(EventArgs args)
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
