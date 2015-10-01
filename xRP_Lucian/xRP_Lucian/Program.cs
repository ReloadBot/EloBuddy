using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;

using EloBuddy.SDK.Menu.Values;

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

            if (Lucian.Q.IsReady() && enemy.IsValid && Lucian.Q.IsInRange(enemy) && useq)
            {
                Lucian.Q.Cast(enemy);
            }

            if (Lucian.W.IsReady() && enemy.IsValid && Lucian.W.IsInRange(enemy) && usew )
            {
                Lucian.W.Cast(enemy.Position);
            }

            if (Lucian.E.IsReady() && enemy.IsValid && usee)
            {
                Lucian.E.Cast(Game.CursorPos);
            }
    }

        private static void LaneClear()
        {
            
        }

        private static void Harass()
        {
            
        }
    }
}
