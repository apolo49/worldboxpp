using System;
using NCMS.Utils;
using UnityEngine;
using NCMS;
using ReflectionUtility;
using Worldboxpp.Culturepp;
using HarmonyLib;
using Worldboxpp.Helpers;

namespace Worldboxpp{ //Change example to the name of your mod
    [ModEntry]
    class Main : MonoBehaviour{

        public static Main instance;
        public MapBoxpp mapboxpp;
        public Harmony harmony;
        public Localisation localisation;

        public static string id = "ir.mods.worldbox.worldboxpp";

        void Awake(){
            localisation = new Localisation();
            harmony = new Harmony(id);
            MapBox.instance.cultures = new CultureManagerpp(MapBox.instance);
            harmony.PatchAll();
            instance = this;
        }
    }
}