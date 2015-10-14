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
        public static AIHeroClient Player {get { return ObjectManager.Player; }}
        
        static void Main(string[] args)        
       {
            if (args == null) throw new ArgumentNullException("args");
            if (EloBuddy.Player.Instance.ChampionName != "Lucian") return;
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
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                Combo();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                LaneClear();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                Harass();
        }

        public static void Combo()
        {
            var useq = config.ComboMenu["comboq"].Cast<CheckBox>().CurrentValue;
            var usew = config.ComboMenu["combow"].Cast<CheckBox>().CurrentValue;
            var usee = config.ComboMenu["comboe"].Cast<CheckBox>().CurrentValue;
          


            
            if (Lucian.Q.IsReady()&& useq)
            {
                var enemy = TargetSelector.GetTarget(Lucian.Q.Range, DamageType.Physical);

                if (Player.HasBuff("Lightslinger")) return;
                if (enemy.IsValidTarget() && Lucian.Q.IsInRange(enemy))
                {
             Lucian.Q.Cast(enemy);
                }
            }

            if (Lucian.W.IsReady() && usew)

            {
                var enemy = TargetSelector.GetTarget(Lucian.W.Range, DamageType.Physical);

                if (Player.HasBuff("Lightslinger")) return;
                if (enemy.IsValidTarget(Lucian.W.Range))
                {
                    Lucian.W.Cast(enemy.Position);
                }
            }


            if (Lucian.E.IsReady() && usee)
            {

                var enemy = TargetSelector.GetTarget(Lucian.E.Range, DamageType.Physical);

                if (enemy.IsValidTarget(Lucian.E.Range))
                {
                    Lucian.E.Cast(Game.CursorPos);
                }
            }
    }

        private static void LaneClear()
        {
            var farmq = config.ComboMenu["farmq"].Cast<CheckBox>().CurrentValue;
            var farmw = config.ComboMenu["farmw"].Cast<CheckBox>().CurrentValue;
            var farme = config.ComboMenu["farme"].Cast<CheckBox>().CurrentValue;

           var minions = ObjectManager.Get<Obj_AI_Minion>().OrderBy(m => m.Health).Where(m => m.IsEnemy);



            var objAiMinions = minions as Obj_AI_Minion[] ?? minions.ToArray();
            foreach (var minion in objAiMinions)
            if (Lucian.Q.IsReady() && Lucian.Q.IsInRange(minion) && farmq )

            {
                if (Player.HasBuff("Lightslinger")) return;
                Lucian.Q.Cast(minion);
            }

            foreach (var minion in objAiMinions)
            if (Lucian.W.IsReady() && Lucian.W.IsInRange(minion) && farmw)
            {
                if (Player.HasBuff("Lightslinger")) return;
                Lucian.W.Cast(minion);
            }

            foreach (var minion in objAiMinions)
                if (Lucian.E.IsReady() && farme && Lucian.E.IsInRange(minion))
                {
                    if (Player.HasBuff("Lightslinger")) return;
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
