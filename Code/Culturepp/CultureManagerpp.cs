using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using HarmonyLib;

namespace Worldboxpp.Culturepp
{
    [HarmonyPatch(typeof(CultureManager))]
    public class CultureManagerpp : CultureManager
    {
        new public readonly List<Culturepp> list;
        new public Dictionary<string, Culturepp> dict;
        new static public CultureManagerpp instance;

        public CultureManagerpp(MapBox pWorld) : base(pWorld)
        {
            foreach (Culture i in base.list)
            {
                this.list.Add(new Culturepp(i));
            }
            this.dict = new Dictionary<string, Culturepp>();
        }

        [HarmonyPatch("update")]
        [HarmonyPostfix]
        public static void update(CultureManager __instance, float pElapsed)
        {
            for (int i = 0; i < __instance.list.Count; i++)
            {
                ((Culturepp)__instance.list[i]).split();
            }
        }

        new public void update(float pElapsed)
        {
            Toolbox.bench("bench_cultures");
            for (int i = 0; i < list.Count; i++)
            {
                list[i].update(pElapsed);
            }
            ReflectionUtility.Reflection.CallMethod(this, "updateRecalcValues", pElapsed);
            Toolbox.benchEnd("bench_cultures");
        }

        [HarmonyPatch(MethodType.Constructor, typeof(MapBox))]
        [HarmonyPrefix]
        public static bool CultureManagerPatch(CultureManager __instance, MapBox pWorld)
        {
            foreach (Culture i in __instance.list)
            {
                __instance.list.Add(new Culturepp(i));
            }
            return false;
        }

        private void add(Culture pCulture)
        {
            this.list.Add(new Culturepp(pCulture));
            this.dict.Add(pCulture.id, new Culturepp(pCulture));
        }

        [HarmonyPatch("add")]
        [HarmonyPrefix]
        static private bool add(CultureManager __instance, Culture pCulture)
        {
            __instance.list.Add(new Culturepp(pCulture));
            __instance.dict.Add(pCulture.id, new Culturepp(pCulture));
            return false;
        }

        new public Culture newCulture(Race pRace, City pCity)
        {
            return this.newCulturepp(pRace, pCity);
        }

        [HarmonyPatch("newCulture")]
        [HarmonyPrefix]
        static public bool newCulture(CultureManager __instance, Race pRace, City pCity, ref Culture __result)
        {
            Culturepp culture = new Culturepp();
            culture.create(pRace, pCity);
            __instance.list.Add(culture);
            __instance.dict.Add(culture.id, culture);
            Debug.Log("New Culture Created: " + culture.name + "\n\tLocation: " + culture.originLoc.ToString());
            __result = culture;
            return false;
        }

        public Culturepp newCulturepp(Race pRace, City pCity)
        {
            Culturepp culture = new Culturepp();
            culture.create(pRace, pCity);
            add(culture);
            Debug.Log("New Culture Created: " + culture.name);
            return culture;
        }
    }
}
