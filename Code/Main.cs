using UnityEngine;
using NCMS;
using ReflectionUtility;
using Worldboxpp.Culturepp;
using HarmonyLib;
using Worldboxpp.Helpers;
using System.Collections.Generic;
using ModDeclaration;
using Worldboxpp.Languages;
using Worldboxpp.Configpp;
using System.Reflection;

namespace Worldboxpp
{ //Change example to the name of your mod
    [ModEntry]
    class Main : MonoBehaviour
    {

        public static Main instance;
        public Harmony harmony;
        public Localisation localisation;
        public LanguageManager languages;


        public static string id = "ir.mods.worldbox.worldboxpp";

        void Awake()
        {

            localisation = new Localisation();
            harmony = new Harmony(id);
            MapBox.instance.cultures = new CultureManagerpp(MapBox.instance);
            instance = this;
            harmony.PatchAll();
        }

        public static bool showLanguageZones()
        {
            if (!PlayerConfigpp.optionBoolEnabled("language_culture_zones"))
            {
                MethodInfo mi = typeof(MapBox).GetMethod("isPowerForceMapMode");
                return (bool)mi.Invoke(MapBox.instance, new object[] { (MapMode)4 });
            }
            return true;
        }

        void Update()
        {
            localisation.Process();
        }
    }
}
