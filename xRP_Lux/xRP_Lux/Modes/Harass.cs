using EloBuddy;
using EloBuddy.SDK;
using xRP_Lux.Modes;
using Settings = xRp_Lux.Config.Modes.Harass;

namespace xRp_Lux.Modes
{
    public sealed class Harass : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass);
        }

        public override void Execute()
        {
            // TODO: Add harass logic here

            if (Settings.UseQ && Player.Instance.ManaPercent > Settings.Mana && Q.IsReady())
            {
                var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                if (target != null)
                {
                    Q.Cast(target);
                }
            }
        }
    }
}