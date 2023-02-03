using NCMS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Worldboxpp.Configpp;

namespace Worldboxpp.Languages
{
    internal class LanguageButton
    {
        static GodPower power;
        static PowerButton button;

        public static void init()
        {
            power = AssetManager.powers.clone("language_zones", "culture_zones");
            power.name = "language_zones";
            power.toggle_name = "map_language_zones";

            power.force_map_text = (MapMode)4;

            button = PowerButtons.CreateButton("language_zones",
                Mod.EmbededResources.LoadSprite($"{Mod.Info.Name}.Resources.icons.languageButton.png"),
                "language_zones_button_name",
                "language_zones_button_description",
                Vector2.zero,
                ButtonType.GodPower
                );
            PowerButtons.AddButtonToTab(button, PowerTab.Main, new Vector2(345.6f + 36, 18));
        }
    }
}
