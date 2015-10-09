using xRp_Lux.Modes;
using EloBuddy;
using EloBuddy.SDK;


using Settings = xRp_Lux.Config.Modes.Combo;

namespace xRp_Lux.Modes
{
    public sealed class Combo : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
        }

        public override void Execute()
        {
            if (Settings.UseQ && Q.IsReady())
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