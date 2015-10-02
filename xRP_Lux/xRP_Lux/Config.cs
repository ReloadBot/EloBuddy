using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace xRP_Lux
{
    public static class config
    {
        public static Menu Menu, ComboMenu, FarMenu, HarassMenu, DrawMenu, MiscMenu;

        static config()
        {
            Menu = MainMenu.AddMenu("xRP_Lux", "xlux");
            Menu.AddSeparator();

            ComboMenu = Menu.AddSubMenu("Combo Menu", "xcombo");
            ComboMenu.Add("comboq", new CheckBox("Use (Q) in Combo", true));
            ComboMenu.Add("combow", new CheckBox("Use (W) in Combo", true));
            ComboMenu.Add("comboe", new CheckBox("Use (E) in Combo", true));
            ComboMenu.Add("combor", new Slider("Min Life Percent to (R)", 30, 0, 100));

            HarassMenu = Menu.AddSubMenu("Harass Menu", "xharass");
            HarassMenu.Add("harasq", new CheckBox("use (Q) to Harass", true));
            HarassMenu.Add("harase", new CheckBox("use (E) to Harass", true));

            FarMenu = Menu.AddSubMenu("Farm Menu", "xfarm");
            FarMenu.Add("farmq", new CheckBox("Use (Q) to Farm", true));
            FarMenu.Add("farmw", new CheckBox("Use (W) to Farm", true));
            FarMenu.Add("farme", new Slider("Use (E) to Farm", 10,0,30));

            DrawMenu = Menu.AddSubMenu("Farm Menu", "xfarm");
            DrawMenu.Add("drawq", new CheckBox("Draw (Q)", true));
            DrawMenu.Add("draww", new CheckBox("Draw(W)", true));
            DrawMenu.Add("drawr", new CheckBox("Draw(R)", true));

            MiscMenu = Menu.AddSubMenu("Misc Menu", "xmisc");
            MiscMenu.Add("xz", new Slider("Auto Zhonia When Life <=", 15, 0, 100));
            MiscMenu.Add("xs", new CheckBox("Auto Ignite ",true));

            


        }


    }



}
