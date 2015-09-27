using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
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
        public static Menu Drawnigs, ComboMenu, KSMenu, menu;
        public static Spell.Active Q;
        public static Spell.Active W;
        public static Spell.Active E;
        public static Spell.Targeted R;



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


            Hacks.AntiAFK = true;
            Bootstrap.Init(null);
            Q = new Spell.Active(SpellSlot.Q, 500);
            W = new Spell.Active(SpellSlot.W, 280);
            E = new Spell.Active(SpellSlot.E, 300);
            R = new Spell.Targeted(SpellSlot.R, 400);


            menu = MainMenu.AddMenu("HariGaren", "harigaren");
            ComboMenu = menu.AddSubMenu("Combo", "comboMenu");
            KSMenu = menu.AddSubMenu("KillSteal", "KSMenu");
            Drawnigs = menu.AddSubMenu("Drawnigs", "drawnigs");

            ComboMenu.AddGroupLabel("CONFIGURAR COMBO:");
            ComboMenu.Add("qCombo", new CheckBox("Usar (Q) no combo"));
            ComboMenu.Add("wCombo", new CheckBox("Usar (W) no combo"));
            ComboMenu.Add("eCombo", new CheckBox("Usar (E) no combo"));
            ComboMenu.Add("rCombo", new CheckBox("Usar (R) no combo"));

            KSMenu.AddGroupLabel("CONFIGURAR KS:");
            KSMenu.Add("rKS", new CheckBox("Usar (R) no KS"));

            Drawnigs.AddGroupLabel("Drawnings ON/OFF");
            Drawnigs.Add("wDraw", new CheckBox("Draw (W)"));
            Drawnigs.Add("eDraw", new CheckBox("Draw (E)"));
            Drawnigs.Add("rDraw", new CheckBox("Draw (R)"));


            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (W.IsReady() && Drawnigs["wDraw"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(Player.Instance.Position, W.Range, Color.CornflowerBlue);
            }

            if (E.IsReady() && Drawnigs["eDraw"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(Player.Instance.Position, E.Range, Color.DarkRed);
            }

            if (R.IsReady() && Drawnigs["rDraw"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(Player.Instance.Position, R.Range, Color.Goldenrod);
            }

        }

        private static void Game_OnTick(EventArgs args)
        {
            var target = TargetSelector.GetTarget(700, DamageType.Mixed);

            /*foreach (var buff in ObjectManager.Player.Buffs)
            {
                Console.WriteLine(buff.Name);
                Chat.Print(buff.Name);
            }*/

            //CALCULOS DA ULT.
            var LevelDoR = R.Level;
            var PorcentagemDoR = 0;
            var DanoPorLevel = 0;
            if (LevelDoR == 1)
            {
                PorcentagemDoR = 28;
                DanoPorLevel = 170;
            }
            else if (LevelDoR == 2)
            {
                PorcentagemDoR = 33;
                DanoPorLevel = 340;
            }
            else if (LevelDoR == 3)
            {
                PorcentagemDoR = 40;
                DanoPorLevel = 510;
            }



            Orbwalker.GetTarget();
            var magicResist = target.SpellBlock;
            var vidamaxima = TargetSelector.GetTarget(500, DamageType.Mixed).MaxHealth;
            var vidaatual = TargetSelector.GetTarget(500, DamageType.Mixed).Health;
            var vidaperdida = vidamaxima - vidaatual;
            var danodoR = (PorcentagemDoR * vidaperdida / 100) + DanoPorLevel;
            var porcentagemMr = magicResist / (100 + magicResist) * 100;
            var reducaoDoDano = porcentagemMr * danodoR / 100;
            var danoNoInimigo = danodoR - reducaoDoDano;
            //CALCULOS DA ULT FIIIIM.

            //CONFIGURAÇAO DO KS
            if (R.IsReady() && (KSMenu["rKS"].Cast<CheckBox>().CurrentValue))
            {
                if (danoNoInimigo > vidaatual)
                {
                    R.Cast(target);
                }
            }
            //MODO COMBO


            if ((Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)))
            {
                if (Q.IsReady() && ComboMenu["qCombo"].Cast<CheckBox>().CurrentValue &&
                    (ObjectManager.Player.Distance(target) < Q.Range))
                {
                    Q.Cast();
                }

                if (W.IsReady() && ComboMenu["wCombo"].Cast<CheckBox>().CurrentValue &&
                    (ObjectManager.Player.Distance(target) < W.Range))
                {
                    W.Cast();
                }

                if (E.IsReady() && ComboMenu["eCombo"].Cast<CheckBox>().CurrentValue && !Player.HasBuff("GarenQ") &&
                    !Player.HasBuff("GarenE") && (ObjectManager.Player.Distance(target) < E.Range))
                {
                    E.Cast();
                }

            }
            //MODO COMBO

        }

    }
}