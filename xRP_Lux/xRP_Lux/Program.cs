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


        public static float Ignite(Obj_AI_Base enemy)
        {
            return ((10 + (4 * Program._player.Level)) * 5) - ((enemy.HPRegenRate / 2) * 5);
        }
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
            var xz = config.MiscMenu["xz"].Cast<Slider>().CurrentValue;
            Item zhonias = new Item((int)ItemId.Zhonyas_Hourglass, 0);

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) { Combo(); }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear)) { LaneClear(); }
            //if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)) { Harass(); }

            if (zhonias.IsReady() && xz <= _player.HealthPercent)
            {
                zhonias.Cast();

            }
            
        }

        public static void Combo()
        {
            
            var useq = config.ComboMenu["comboq"].Cast<CheckBox>().CurrentValue;
            var usew = config.ComboMenu["combow"].Cast<CheckBox>().CurrentValue;
            var usee = config.ComboMenu["comboe"].Cast<CheckBox>().CurrentValue;
            var user = config.ComboMenu["combor"].Cast<Slider>().CurrentValue;
            var ignite = config.MiscMenu["xs"].Cast<Slider>().CurrentValue;

            var enemy = TargetSelector.GetTarget(3340, DamageType.Magical);
            if (!enemy.IsValid) return;

            if (enemy.IsValid && Lux.Q.IsReady() && enemy.IsValid && Lux.Q.IsInRange(enemy) && useq)
            {
                Lux.Q.Cast(enemy);
            }

            if (enemy.IsValid && Lux.W.IsReady() && usew)
            {
                Lux.W.Cast();
            }

            if (enemy.IsValid && Lux.E.IsReady() && Lux.E.IsInRange(enemy) && enemy.IsValid && usee)
            {
                Lux.E.Cast(enemy.Position);
            }

            if (enemy.IsValid && Lux.R.IsReady() && Lux.R.IsInRange(enemy) && enemy.IsValid && user <= enemy.HealthPercent)
            {
                Lux.R.Cast(enemy);
            }

            if (Lux.Ignite.IsReady() 
                && Lux.Ignite.IsInRange(enemy) 
                && enemy.IsValid)
               
            {
                Lux.Ignite.Cast(enemy);
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
