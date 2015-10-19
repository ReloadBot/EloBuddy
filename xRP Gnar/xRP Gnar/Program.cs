using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK.Events;

namespace xRP_Gnar
{
     class Program
    {
        public static AIHeroClient _player = ObjectManager.Player;
       

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoaded;
        }

        private static void OnLoaded(EventArgs args)
        {
            if (_player.BaseSkinName == "gnarbig")
            { xRP_MegaGnar.MegaGnar.Initialize(); }

        }

    }
}
