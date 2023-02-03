using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Worldboxpp.Culturepp;
using UnityEngine;

namespace Worldboxpp.HarmonyPatches
{
    [HarmonyPatch(typeof(MapBox))]
    static internal class Patch
    {

        [HarmonyPostfix]
        [HarmonyPatch("Awake")]
        static void Awake(MapBox __instance)
        {
            Debug.Log("Patched MapBox.Awake()");
            __instance.cultures = new CultureManagerpp(__instance);
        }
    }

    [HarmonyPatch(typeof(MapStats))]
    internal class Patch_MapStats
    {

        public static int id_language;

        [HarmonyPostfix]
        [HarmonyPatch("getNextID")]
        static void getNextID(MapStats __instance, ref string __result, string pType)
        {
            if (__result == "")
            {
                switch (pType)
                {
                    case "language":
                        __result = "l_" + id_language++;
                        break;
                }
            }

        }

    }
}
