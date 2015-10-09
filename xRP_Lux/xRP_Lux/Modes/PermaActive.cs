using EloBuddy;
using EloBuddy.SDK;
using Settings = xRp_Lux.Config.Modes.Misc;

namespace xRp_Lux.Modes
{
    public sealed class PermaActive : ModeBase
    {
        public override bool ShouldBeExecuted()
        {

            return true;
        }

        public override void Execute()
        {
            // TODO: Add permaactive logic here, good for spells like Ignite or Smite

             Item zhonias = new Item((int)ItemId.Zhonyas_Hourglass, 0);

            if (ObjectManager.Player.HealthPercent <= Settings.Az)
                    
            {
                zhonias.Cast();
            }

        }
    }
}