using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace xRP_Lucian
{
   public static class config
    {
       public static Menu Menu, ComboMenu, HarassMenu;

       static config()
       {
           Menu = MainMenu.AddMenu("xRP_Lucian", "xlucian");
           Menu.AddSeparator();

           ComboMenu = Menu.AddSubMenu("Combo Menu", "xcombo");
           ComboMenu.Add("comboq", new CheckBox("Use (Q) in Combo", true));
           ComboMenu.Add("combow", new CheckBox("Use (W) in Combo", true));
           ComboMenu.Add("comboe", new CheckBox("Use (E) in Combo", true));
           ComboMenu.Add("combor", new Slider("Min Life to (R)", 30, 0, 100));

       }


    }

    
    
}
