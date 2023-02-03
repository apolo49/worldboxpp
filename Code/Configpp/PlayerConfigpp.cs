using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Worldboxpp.Configpp
{
    public class PlayerConfigpp : PlayerConfig
    {
        private PlayerConfigData data;
        public PlayerConfigData Data { get; }

        private static MethodInfo miOptionBoolEnabled = typeof(PlayerConfigpp).GetMethod("optionBoolEnabled");

        private static MethodInfo miSwitchOption = typeof(PlayerConfigpp).GetMethod("switchOption");

        void Awake()
        {
            FieldInfo fi = typeof(PlayerConfig).GetField("data");
            PlayerConfigData data = (PlayerConfigData)fi.GetValue(PlayerConfig.instance);
            PlayerConfig.instance = this;
            data.add(new PlayerOptionData("map_language_zones")
            {
                boolVal = false
            });
        }

        public static bool optionBoolEnabled(string name)
        {
            return (bool)miOptionBoolEnabled.Invoke(typeof(PlayerConfig), new object[] { name });
        }

        public static void switchOption(string gameOptionName, OptionType pType)
        {
            miSwitchOption.Invoke(typeof(PlayerConfig), new object[] { gameOptionName, pType });
        }
    }
}
