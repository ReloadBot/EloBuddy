using EloBuddy;
using EloBuddy.SDK;
using xRP_Lux.Modes;
using Settings = xRp_Lux.Config.Misc;

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
            var player = ObjectManager.Player;

             Item zhonias = new Item((int)ItemId.Zhonyas_Hourglass);

            if (player.HealthPercent <= Settings._AZ)
                    
            {
                zhonias.Cast();
            }

        }
    }
}