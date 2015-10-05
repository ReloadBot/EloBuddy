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

namespace xRP_Tristana
{
    class Program
    {
        public static Menu Menu,
            ComboMenu,
            MiscMenu,
            LaneMenu,
            DrawMenu;

        public static Spell.Active Q;
        public static Spell.Skillshot W;
        public static Spell.Targeted E;
        public static Spell.Targeted R;

        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }



        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Game_OnStart;
            Drawing.OnDraw += Game_OnDraw;
          
            Game.OnTick += Game_OnTick;

        }

        private static void Game_OnStart(EventArgs args)
        {
            Chat.Print("xRP Tristana LOADED \n Have Fun.");

            Q = new Spell.Active(SpellSlot.Q, 550 );
            W = new Spell.Skillshot(SpellSlot.W, 825, SkillShotType.Circular, 250, Int32.MaxValue, 80);
            E = new Spell.Targeted(SpellSlot.E, 550);
            R = new Spell.Targeted(SpellSlot.R, 550);



            Menu = MainMenu.AddMenu("xRP Tristana", "xrptristana");
            Menu.AddSeparator();
            Menu.AddLabel("Made by: XrP for Elobuddy plataform");

            DrawMenu = Menu.AddSubMenu("Drawlings", "xdraw");
            DrawMenu.Add("drawDisable", new CheckBox("Disable all Draw", true));

            ComboMenu = Menu.AddSubMenu("Combo Menu", "xcombo");
            ComboMenu.Add("comboq", new CheckBox("Use (Q) in Combo", true));
            ComboMenu.Add("combow", new CheckBox("Use (W) in Combo", true));
            ComboMenu.Add("comboe", new CheckBox("Use (E) in Combo", true));
            ComboMenu.Add("combor", new CheckBox("Use (R) to Finish", true));

            LaneMenu = Menu.AddSubMenu("Lane Clean", "xlane");
            LaneMenu.Add("eLane", new CheckBox("Use (E) in LaneClear", true));
            LaneMenu.Add("Etower", new CheckBox("Use E on Towers", true));
            




        }

        private static void Game_OnTick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags == (Orbwalker.ActiveModes.LaneClear))
            {
                LaneClear();
            }
            if (Orbwalker.ActiveModesFlags == (Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }
        }

        public static void Game_OnDraw(EventArgs args)
        {

            if (!DrawMenu["drawDisable"].Cast<CheckBox>().CurrentValue)
            {
                new Circle()
                {
                    Color = Color.LawnGreen,
                    Radius = ObjectManager.Player.GetAutoAttackRange(),
                    BorderWidth = 2f

                }.Draw(ObjectManager.Player.Position);

            }

        }


        private static void Combo()
        {

            var enemy = TargetSelector.GetTarget(1000, DamageType.Physical);
            var useR = ComboMenu["combor"].Cast<CheckBox>().CurrentValue;



            if (!enemy.IsValid()) return;

            if (Orbwalker.ActiveModesFlags == Orbwalker.ActiveModes.Combo)
            {
                if (enemy.IsInAutoAttackRange(ObjectManager.Player) && Q.IsReady() && ComboMenu["comboq"].Cast<CheckBox>().CurrentValue)
                {
                    Q.Cast();
                }

                if (W.IsReady() && _Player.Distance(enemy) <= W.Range + _Player.GetAutoAttackRange() && ComboMenu["combow"].Cast<CheckBox>().CurrentValue)
                {

                    W.Cast(enemy);
                }

                if (E.IsInRange(enemy) && ComboMenu["comboe"].Cast<CheckBox>().CurrentValue)
                {
                    E.Cast(enemy);
                }

                if (R.IsReady() && useR && enemy.IsValidTarget(R.Range) && !enemy.IsDead && !enemy.IsZombie && enemy.Health <= Player.Instance.GetSpellDamage(enemy, SpellSlot.R))
                {
                    R.Cast(enemy);
                }
            }

        }

            private static void LaneClear()
        {
            var minion = ObjectManager.Get<Obj_AI_Minion>().FirstOrDefault(a => a.IsEnemy && !a.IsDead && a.Distance(_Player) < _Player.AttackRange);
            var tower = ObjectManager.Get<Obj_AI_Turret>().FirstOrDefault(a => a.IsEnemy && !a.IsDead && a.Distance(_Player) < _Player.AttackRange);
            if (minion == null)
                if (tower == null)
                    return;

            var useE = LaneMenu["eLane"].Cast<CheckBox>().CurrentValue;
            var useETower = LaneMenu["Etower"].Cast<CheckBox>().CurrentValue;


            if (useE && E.IsReady() && (tower == null))
            {
                E.Cast(minion);
            }

            if (useETower && E.IsReady())
            {
                E.Cast(tower);
            }
        


    }
      

        }
    }

