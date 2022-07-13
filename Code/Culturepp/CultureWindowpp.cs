using System;
using HarmonyLib;

namespace Worldboxpp.Culturepp
{
    [HarmonyPatch(typeof(CultureWindow))]
    internal class CultureWindowpp : CultureWindow
    {
        [HarmonyPostfix]
        [HarmonyPatch("showInfo")]
        static public void showInfo(CultureWindow __instance)
        {
            String loc = ((Culturepp)Config.selectedCulture).originLoc.ToString();
            if (!string.IsNullOrEmpty(loc))
            {
                ReflectionUtility.Reflection.CallMethod(__instance, "showStat", "culture_founded_at", loc);
            } else
            {
                ReflectionUtility.Reflection.CallMethod(__instance, "showStat", "culture_founded_at", "??");
            }
            String divergedFrom = ((Culturepp)Config.selectedCulture).divergedfrom;
            if (!string.IsNullOrEmpty(divergedFrom))
            {
                ReflectionUtility.Reflection.CallMethod(__instance, "showStat", "culture_diverged_from", divergedFrom);
            }
        }
    }
}
