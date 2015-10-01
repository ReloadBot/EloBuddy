using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace xRP_Lucian
{
   public static class config
    {
       public static Menu Menu, ComboMenu, FarMenu, HarassMenu, DrawMenu;

       static config()
       {
           Menu = MainMenu.AddMenu("xRP_Lucian", "xlucian");
           Menu.AddSeparator();

           ComboMenu = Menu.AddSubMenu("Combo Menu", "xcombo");
           ComboMenu.Add("comboq", new CheckBox("Use (Q) in Combo", true));
           ComboMenu.Add("combow", new CheckBox("Use (W) in Combo", true));
           ComboMenu.Add("comboe", new CheckBox("Use (E) in Combo", true));
           ComboMenu.Add("combor", new Slider("Min Life to (R)", 30, 0, 100));

           FarMenu = Menu.AddSubMenu("Farm Menu", "xfarm");
           FarMenu.Add("farmq", new CheckBox("Use (Q) to Farm", true));
           FarMenu.Add("farmw", new CheckBox("Use (W) to Farm", true));
           FarMenu.Add("farme", new CheckBox("Use (E) to Farm", true));

           DrawMenu = Menu.AddSubMenu("Farm Menu", "xfarm");
           DrawMenu.Add("drawq", new CheckBox("Draw (Q)", true));
           DrawMenu.Add("draww", new CheckBox("Draw(W)", true));
           DrawMenu.Add("drawe", new CheckBox("Draw(E)", true));

       }


    }

    
    
}
