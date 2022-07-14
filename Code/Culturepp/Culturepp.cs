using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NCMS;
using ReflectionUtility;
using UnityEngine;
using HarmonyLib;

namespace Worldboxpp.Culturepp
{
    public class Culturepp : Culture
    {
        public Vector2Int originLoc;
        private int lastDivergence;

        public String divergedfrom { get; set; }

        public Culturepp() { }

        public Culturepp(Culture pCulture)
        {
            base.race = pCulture.race;
            base.list_tech_ids = pCulture.list_tech_ids;
            base.id = pCulture.id;
            base.name = pCulture.name;
            base.village_origin = pCulture.village_origin;
            base.year = pCulture.year;
        }

        public void addOriginLoc(City pCity)
        {
            this.originLoc = pCity.getTile().pos;
        }

        private void prepare()
        {
            this.createSkins();
            base.setDirty();
            if (base.color32 == Color.clear)
            {
                base.createGameColors();
            }
            base.hash = base.GetHashCode();
        }

        private void createSkins()
        {
            if (string.IsNullOrEmpty(this.race))
            {
                this.race = "human";
            }
            if (!string.IsNullOrEmpty(this.skin_citizen_female))
            {
                return;
            }
            Race race = AssetManager.raceLibrary.get(this.race);
            int num = Toolbox.randomInt(0, race.skin_citizen_female.Count);
            this.skin_citizen_female = race.skin_citizen_female[num];
            this.skin_citizen_male = race.skin_citizen_male[num];
            this.skin_warrior = race.skin_warrior[num];
            if (string.IsNullOrEmpty(this.icon_element))
            {
                this.icon_element = race.culture_elements.GetRandom<string>();
                this.icon_decor = race.culture_decors.GetRandom<string>();
                race.culture_colors.Shuffle<string>();
                this.color = string.Empty;
                for (int i = 0; i < race.culture_colors.Count; i++)
                {
                    if (!MapBox.instance.cultures.isColorUsed(race.culture_colors[i]))
                    {
                        this.color = race.culture_colors[i];
                        break;
                    }
                }
                if (string.IsNullOrEmpty(this.color))
                {
                    this.color = race.culture_colors.GetRandom<string>();
                }
            }
        }

        new public void update(float pElapsed)
        {
            Debug.Log("Update Culture: " + this.name);
            if ((bool)ReflectionUtility.Reflection.GetField(typeof(Boolean), this, "_zones_dirty"))
            {
                ReflectionUtility.Reflection.SetField(this, "_zones_dirty", false);
                ReflectionUtility.Reflection.CallMethod(this, "updateTitleCenter");
            }

            if (!(bool)ReflectionUtility.Reflection.CallMethod(MapBox.instance, "isPaused"))
            {
                ReflectionUtility.Reflection.CallMethod(MapBox.instance, "updateSpread", pElapsed);
            }

            ReflectionUtility.Reflection.CallMethod(MapBox.instance, "updateDirty", pElapsed);
            this.split();


        }

        new public void create(Race pRace, City pCity)
        {
            base.create(pRace, pCity);
            this.addOriginLoc(pCity);
            this.lastDivergence = 0;

        }

        public void split()
        {
            if ((this.lastDivergence != 0) && ((MapBox.instance.mapStats.year - this.lastDivergence) <= 20))
            {
                return;
            }
            if (((MapBox.instance.mapStats.year - this.year + 1) > 500) && (base.followers > 200) && (Toolbox.randomChance(0.99f)) && (base._list_cities.Count > 2))
            {
                City city = base._list_cities.GetRandom();
                if (city.leader == null) return;

                if (Toolbox.DistVec2(city.getTile().pos, this.originLoc) > (Math.Sqrt(MapBox.instance.tilesList.Count) / 6))
                {
                    Race cityRace = (Race)ReflectionUtility.Reflection.GetField(typeof(City), city, "race");
                    _list_cities.Remove(city);
                    Culturepp newculture = (Culturepp)CultureManager.instance.newCulture(cityRace, city);
                    ReflectionUtility.Reflection.CallMethod(city.leader, "setCulture", newculture);
                    newculture.divergedfrom = this.name;
                    newculture.list_tech_ids = this.list_tech_ids;
                    foreach (Actor i in MapBox.instance.units.getSimpleList())
                    {
                        ActorStatus actorData = (ActorStatus)ReflectionUtility.Reflection.GetField(typeof(Actor), i, "data");
                        if ((actorData == null) && (i.city == null) && (actorData.culture == null))
                            continue;
                        if ((i.city == city) && (actorData.culture == this.id))
                        {
                            i.CallMethod("setCulture", newculture);
                        }
                    }
                    ((CityData)ReflectionUtility.Reflection.GetField(typeof(City), city, "data")).culture = newculture.id;
                    foreach (var i in (List<TileZone>)ReflectionUtility.Reflection.GetField(typeof(City), city, "zones"))
                        newculture.addZone(i);
                    this.lastDivergence = MapBox.instance.mapStats.year;
                }

            }

        }
    }
}
