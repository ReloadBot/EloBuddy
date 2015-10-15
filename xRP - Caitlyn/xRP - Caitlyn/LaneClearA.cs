using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;


namespace xRP_Caitlyn
{
    internal class LaneClearA
    {
        public enum AttackSpell
        {
            Q
        };

        public static AIHeroClient Caitlyn
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
            var qcheck = Program.FarmMenu["useQfarm"].Cast<CheckBox>().CurrentValue;
            var qready = Program.Q.IsReady();

            if (qcheck && qready)
            {
                var qenemy =
                    (Obj_AI_Minion)GetEnemy(Program.Q.Range, GameObjectType.obj_AI_Minion);

                if (qenemy != null)
                {
                    Program.Q.Cast(qenemy.ServerPosition);
                }
            }
        }
    }
}