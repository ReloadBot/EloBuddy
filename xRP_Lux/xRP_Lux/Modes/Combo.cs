using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using xRP_Lux.Modes;

namespace xRp_Lux.Modes
{
    public sealed class Combo : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
        }

        public enum AttackSpell
        {
            Q,
            E,
            R
        };
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
        public override void Execute()
        {
            var qcheck = Config.Modes.Combo.UseQ;
            var qready = SpellManager.Q.IsReady();
            var echeck = Config.Modes.Combo.UseE;
            var rcheck = Config.Modes.Combo.UseR;
            var eready = SpellManager.E.IsReady();
            var rready = SpellManager.R.IsReady();

            if (qcheck && qready)
            {
                var enemy = (AIHeroClient)GetEnemy(SpellManager.Q.Range, GameObjectType.AIHeroClient);

                if (enemy != null)
                    if (SpellManager.Q.GetPrediction(enemy).HitChance >= HitChance.High)
                        SpellManager.Q.Cast();
            }
            if (echeck && eready)
            {
                var enemy = (AIHeroClient)GetEnemy(SpellManager.E.Range, GameObjectType.AIHeroClient);

                if (enemy != null)
                    if (SpellManager.E.GetPrediction(enemy).HitChance >= HitChance.High)
                        SpellManager.E.Cast();
            }
            if (rready)
            {
                var target = TargetSelector.GetTarget(2000, DamageType.Magical);
                var predR = SpellManager.R.GetPrediction(target).CastPosition;
                if (target.CountEnemiesInRange(SpellManager.R.Width) >= rcheck)
                    SpellManager.R.Cast(predR);
            }
            if (Orbwalker.CanAutoAttack)
            {
                var cenemy = (AIHeroClient)GetEnemy(Player.Instance.GetAutoAttackRange(), GameObjectType.AIHeroClient);

                if (cenemy != null)
                    Orbwalker.ForcedTarget = cenemy;
            }
        }
        
    }
}