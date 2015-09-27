using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using Color = System.Drawing.Color;

namespace MasterYi
{
    class Program
    {


        public static Menu Drawnigs, ComboMenu, menu;
        public static Spell.Active Q;
        public static Spell.Active W;
        public static Spell.Active E;
        public static Spell.Active R;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }


        private static void Loading_OnLoadingComplete(EventArgs arg)
        {
            //Checar campeao
            if (Player.Instance.ChampionName != "Master Yi")
            {
                return;
            }

            Bootstrap.Init(null);

            Q = new Spell.Active(SpellSlot.Q, 600);
            W = new Spell.Active(SpellSlot.W, 0);
            E = new Spell.Active(SpellSlot.E, 0);
            R = new Spell.Active(SpellSlot.R, 0);

            var menu = MainMenu.AddMenu("Master Yi", "HoYi");

            menu.AddGroupLabel("Combo");


            menu.AddSeparator();
            menu.AddGroupLabel("Drawings");
            var qRange = menu.Add("qDraw", new CheckBox("Q range"));


            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += Drawing_OnDraw;

        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Q.IsReady() && Drawnigs["qDraw"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(Player.Instance.Position, Q.Range, Color.DarkRed);
            }
        }
        private static void Game_OnTick(EventArgs args)
        {

            var target = TargetSelector.GetTarget(600, DamageType.Mixed);






            Orbwalker.GetTarget();


            //Combo

            if ((Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)))
            {
                if (Q.IsReady() && ComboMenu["qCombo"].Cast<CheckBox>().CurrentValue &&
                    (ObjectManager.Player.Distance(target) < Q.Range))
                {
                    Q.Cast();
                }

            }

            if (E.IsReady() && ComboMenu["eCombo"].Cast<CheckBox>().CurrentValue)
            {
                E.Cast();
            }
            //combo
        }
    }
}
