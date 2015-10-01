using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using Color = System.Drawing.Color;

namespace xRP_Lucian
{
    class Program
    {
        public static AIHeroClient _player {get { return ObjectManager.Player; }}
        
        static void Main(string[] args)        
       {
            if (Player.Instance.ChampionName != "Lucian") return;
            Loading.OnLoadingComplete += Game_OnStart;
            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += OnDraw;
        }

        private static void Game_OnStart(EventArgs args)
        {
            Chat.Print("xRP_Lucian LOADED \n 1.0.0v \n HaveFun");          
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) {Combo();}
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear)) { LaneClear(); }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)) { Harass(); }
        }

        public static void Combo()
        {
            var useq = config.ComboMenu["comboq"].Cast<CheckBox>().CurrentValue;
            var usew = config.ComboMenu["combow"].Cast<CheckBox>().CurrentValue;
            var usee = config.ComboMenu["comboe"].Cast<CheckBox>().CurrentValue;
          

            var enemy = TargetSelector.GetTarget(1400, DamageType.Physical);
            if (!enemy.IsValid) return;

            if (_player.GetBuff("Lightslinger").IsActive) return;
            if (enemy.IsValid && Lucian.Q.IsReady() && enemy.IsValid && Lucian.Q.IsInRange(enemy) && useq)
            {
                Lucian.Q.Cast(enemy);
            }

            if (enemy.IsValid && Lucian.W.IsReady() && enemy.IsValid && Lucian.W.IsInRange(enemy) && usew)
            {
                Lucian.W.Cast(enemy.Position);
            }

            if (enemy.IsValid && Lucian.E.IsReady() && enemy.IsValid && usee)
            {
                Lucian.E.Cast(Game.CursorPos);
            }
    }

        private static void LaneClear()
        {
            var farmq = config.ComboMenu["farmq"].Cast<CheckBox>().CurrentValue;
            var farmw = config.ComboMenu["farmw"].Cast<CheckBox>().CurrentValue;
            var farme = config.ComboMenu["farme"].Cast<CheckBox>().CurrentValue;

           var minions = ObjectManager.Get<Obj_AI_Minion>().OrderBy(m => m.Health).Where(m => m.IsEnemy);
           
            if (_player.GetBuff("Lightslinger").IsActive) return;
            if (minions == null)return;


            foreach (var minion in minions)
            if (Lucian.Q.IsReady() && Lucian.Q.IsInRange(minion) && farmq )
            {
                Lucian.Q.Cast(minion);
            }

            foreach (var minion in minions)
            if (Lucian.W.IsReady() && Lucian.W.IsInRange(minion) && farmw)
            {
                Lucian.W.Cast(minion);
            }

            foreach (var minion in minions)
                if (Lucian.E.IsReady() && farme && Lucian.E.IsInRange(minion))
                {
                    Lucian.E.Cast(Game.CursorPos);

                }
}

        private static void Harass()
        {
            
        }

        private static void OnDraw(EventArgs args)
        {
            var drawq = config.ComboMenu["drawq"].Cast<CheckBox>().CurrentValue;
            var draww = config.ComboMenu["draww"].Cast<CheckBox>().CurrentValue;
            var drawe = config.ComboMenu["drawe"].Cast<CheckBox>().CurrentValue;

            if (drawq)
                new Circle() {Color = Color.Red, Radius = Lucian.Q.Radius, BorderWidth = 2f};

            if (draww)
            {
                new Circle() { Color = Color.DodgerBlue, Radius = Lucian.W.Radius, BorderWidth = 2f };
            }
            if (drawe)
            {
                new Circle() { Color = Color.LimeGreen, Radius = Lucian.E.Range, BorderWidth = 2f };
            }
        }
    }
}
