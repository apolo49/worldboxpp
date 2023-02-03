using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using ReflectionUtility;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;
using NCMS.Utils;

namespace Worldboxpp.HarmonyPatches
{
    [HarmonyPatch(typeof(PowerButton))]
    internal class PowerButtonpp
    {

        [HarmonyPostfix]
        [HarmonyPatch("init")]
        static void init()
        {
            PowerButtonSelector powerButtonSelector = (PowerButtonSelector)(Reflection.GetField(typeof(PowerButtonSelector), null, "instance"));
            UiMover clockButtMover = powerButtonSelector.clockButtMover;
            PowerButton clockButton = clockButtMover.GetComponentInChildren<PowerButton>();
            if (clockButton != null)
            {
                clockButtMover.GetComponent<RectTransform>().localScale = new Vector3(5, 1, 1);
                clockButtMover.GetComponent<RectTransform>().ForceUpdateRectTransforms();
                clockButton.transform.SetParent(null, false);
                var slider = Helpers.Buttons.RegisterSlider("time_scale", clockButtMover.transform, "Time Scale", null, null, clockButton.gameObject);
                PowerButton.powerButtons.Remove(clockButton);
            }

        }

        [HarmonyPrefix]
        [HarmonyPatch("clickTimeScaleTool")]
        public static bool clickTimeScaleTool(PowerButton __instance)
        {
            __instance.sizeButtons.SetActive(value: false);
            __instance.mainSizeButton.icon.sprite = __instance.icon.sprite;
            if (__instance.transform.name == "x10")
            {
                Config.timeScale = 10f;
            }
            else if (__instance.transform.name == "x5")
            {
                Config.timeScale = 5f;
            }
            else if (__instance.transform.name == "x3")
            {
                Config.timeScale = 3f;
            }
            else if (__instance.transform.name == "x2")
            {
                Config.timeScale = 2f;
            }
            else
            {
                Config.timeScale = 1f;
            }

            __instance.mainSizeButton.newClickAnimation();
            ReflectionUtility.Reflection.SetField(MapBox.instance, "inspectTimerClick", 1f);

            Dictionary<string, ScrollWindow> windows = (Dictionary<string, ScrollWindow>)Reflection.GetField(typeof(ScrollWindow), null, "allWindows");
            foreach (var i in windows.Keys)
            {
                Debug.Log(i);
            }

            return false;
        }
    }
}
