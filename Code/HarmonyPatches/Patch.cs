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
}
