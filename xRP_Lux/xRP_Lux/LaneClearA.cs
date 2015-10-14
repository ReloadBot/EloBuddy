using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace xRP_Lux
{
    internal class LaneClearA
    {
        public enum AttackSpell
        {
            E
        };

        public static AIHeroClient Lux
        {
            get { return ObjectManager.Player; }
        }

        public static Obj_AI_Base GetEnemy(float range, GameObjectType t)
        {
            switch (t)
            {
                case GameObjectType.AIHeroClient:
                    return EntityManager.Heroes.Enemies.OrderBy(a => a.Health).FirstOrDefault(
                        a => a.Distance(Player.Instance) < range && !a.IsDead && !a.IsInvulnerable);
                default:
                    return EntityManager.MinionsAndMonsters.EnemyMinions.OrderBy(a => a.Health).FirstOrDefault(
                        a => a.Distance(Player.Instance) < range && !a.IsDead && !a.IsInvulnerable);
            }
        }
        public static void LaneClear()
        {
            var echeck = Program.LaneClearMenu["LCE"].Cast<CheckBox>().CurrentValue;
            var eready = Program.E.IsReady();

            if (echeck && eready)
            {
                var eenemy =
                    (Obj_AI_Minion)GetEnemy(Program.E.Range, GameObjectType.obj_AI_Minion);

                if (eenemy != null)
                {
                    Program.E.Cast(eenemy.ServerPosition);
                }
            }
        }
    }
}