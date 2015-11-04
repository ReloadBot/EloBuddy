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

namespace xRP_Lucian
{
    class Program
    {

        public static Spell.Skillshot Q;
        public static Spell.Skillshot W;
        public static Spell.Active E;
        public static Spell.Skillshot R;

        public static Menu Menu, ComboMenu, FarMenu, HarassMenu, DrawMenu;
        public static AIHeroClient _player {get { return ObjectManager.Player; }}
        
        static void Main(string[] args)        
       {
            if (EloBuddy.Player.Instance.ChampionName != "Lucian") return;
           
            Loading.OnLoadingComplete += Game_OnStart;
         
        }

        private static void Game_OnStart(EventArgs args)
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 1100, SkillShotType.Linear);
            W = new Spell.Skillshot(SpellSlot.W, 1000, SkillShotType.Circular);
            E = new Spell.Active(SpellSlot.E, 425);
            R = new Spell.Skillshot(SpellSlot.R, 1400, SkillShotType.Linear);



            Menu = MainMenu.AddMenu("xRP_Lucian", "xlucian");
            Menu.AddSeparator();

            ComboMenu = Menu.AddSubMenu("Combo Menu", "xcombo");
            ComboMenu.Add("comboq", new CheckBox("Use (Q) in Combo"));
            ComboMenu.Add("combow", new CheckBox("Use (W) in Combo"));
            ComboMenu.Add("comboe", new CheckBox("Use (E) in Combo", true));
            ComboMenu.Add("combor", new Slider("Min Life to (R)", 30, 0, 100));

            FarMenu = Menu.AddSubMenu("Farm Menu", "xfarm");
            FarMenu.Add("farmq", new CheckBox("Use (Q) to Farm", true));
            FarMenu.Add("farmw", new CheckBox("Use (W) to Farm", true));
            FarMenu.Add("farme", new CheckBox("Use (E) to Farm", true));

            DrawMenu = Menu.AddSubMenu("Farm Menu", "xfarm");
            DrawMenu.Add("drawq", new CheckBox("Draw (Q)", true));
            DrawMenu.Add("draww", new CheckBox("Draw(W)", true));
            DrawMenu.Add("drawe", new CheckBox("Draw(E)", true));


            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += OnDraw;

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
            var useq = ComboMenu["comboq"].Cast<CheckBox>().CurrentValue;
            var usew = ComboMenu["combow"].Cast<CheckBox>().CurrentValue;
            var usee = ComboMenu["comboe"].Cast<CheckBox>().CurrentValue;

            if (Player.HasBuff("Lightslinger")) return;


            
            if (Q.IsReady()&& useq)
            {
                var enemy = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                var predQ = Q.GetPrediction(enemy).CastPosition;

                
                if (enemy.IsValidTarget() && Q.IsInRange(enemy))
                {
                     Q.Cast(predQ);
                }
            }

            if (W.IsReady() && usew)

            {
                var enemy = TargetSelector.GetTarget(W.Range, DamageType.Physical);
                var predW = W.GetPrediction(enemy).CastPosition;

                
                if (enemy.IsValidTarget(W.Range))
                {
                    W.Cast(predW);
                }
            }


            if (E.IsReady() && usee)
            {

                var enemy = TargetSelector.GetTarget(E.Range, DamageType.Physical);

                if (enemy.IsValidTarget(E.Range))
                {
                    E.Cast(Game.CursorPos);
                }
            }
    }

        private static void LaneClear()
        {
            var farmq = ComboMenu["farmq"].Cast<CheckBox>().CurrentValue;
            var farmw = ComboMenu["farmw"].Cast<CheckBox>().CurrentValue;
            var farme = ComboMenu["farme"].Cast<CheckBox>().CurrentValue;

            var minion = EntityManager.MinionsAndMonsters.EnemyMinions.OrderBy(a => a.Health).FirstOrDefault(
                        a => a.Distance(Player.Instance) < _player.GetAutoAttackRange() && !a.IsDead && !a.IsInvulnerable);

            
            if (Q.IsReady() && Q.IsInRange(minion) && farmq )
            {
                if (Player.HasBuff("Lightslinger")) return;
                Q.Cast(minion);
            }

            if (W.IsReady() && W.IsInRange(minion) && farmw)
            {
                if (Player.HasBuff("Lightslinger")) return;
                W.Cast(minion);
            }

                if (E.IsReady() && farme && E.IsInRange(minion))
                {
                    if (Player.HasBuff("Lightslinger")) return;
                    E.Cast(Game.CursorPos);

                }
}

        private static void Harass()
        {
            
        }

        private static void OnDraw(EventArgs args)
        {
            var drawq = ComboMenu["drawq"].Cast<CheckBox>().CurrentValue;
            var draww = ComboMenu["draww"].Cast<CheckBox>().CurrentValue;
            var drawe = ComboMenu["drawe"].Cast<CheckBox>().CurrentValue;

            if (drawq)
                new Circle() {Color = Color.Red, Radius = Q.Range, BorderWidth = 2f};

            if (draww)
            {
                new Circle() { Color = Color.DodgerBlue, Radius = W.Range, BorderWidth = 2f };
            }
            if (drawe)
            {
                new Circle() { Color = Color.LimeGreen, Radius = E.Range, BorderWidth = 2f };
            }
        }
    }
}
