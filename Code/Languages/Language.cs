using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Worldboxpp.Culturepp;

namespace Worldboxpp.Languages
{
    public class Language
    {
        public static int next_id = 0;
        [NonSerialized]
        public int hash;
        public string id;
        public string name;

        [NonSerialized]
        public Color32 color32 = Color.clear;

        [NonSerialized]
        public Color32 color32_border = Color.clear;

        [NonSerialized]
        public Color32 color32_text = Color.clear;

        public string color = string.Empty;

        public int year;

        public string village_origin = string.Empty;

        public string origin_race = string.Empty;

        public string origin_culture = string.Empty;

        public List<int> list_zone_ids;

        [NonSerialized]
        public List<City> _list_cities = new List<City>();

        [NonSerialized]
        public KingdomStats stats = new KingdomStats();

        [NonSerialized]
        public int followers;

        [NonSerialized]
        public int cities;

        [NonSerialized]
        public int kingdoms;

        [NonSerialized]
        public HashSetTileZone zones = new HashSetTileZone();

        [NonSerialized]
        private bool _zones_dirty;

        private float timer_spread;

        private static List<TileZone> _list_zones = new List<TileZone>();

        private static List<TileZone> _list_zones_to_spread = new List<TileZone>();

        internal Vector3 titleCenter = Globals.POINT_IN_VOID;
        private bool dirty;

        public String divergedfrom { get; set; }

        public void create(Race pRace, City pCity, Culturepp.Culturepp pCulturepp)
        {
            origin_race = pRace.id;
            id = get_next_id();
            name = pCulturepp.name;
            if (pCity != null)
            {
                village_origin = getCityData(pCity).cityName;
            }
            else
            {
                village_origin = "??";
            }

            origin_culture = pCulturepp.id;

            year = MapBox.instance.mapStats.year;
            prepare();
        }

        public void create(Race pRace, City pCity)
        {
            origin_race = pRace.id;
            id = get_next_id();
            NameGeneratorAsset pAsset = AssetManager.nameGenerator.get(pRace.name_template_culture);
            name = NameGenerator.generateNameFromTemplate(pAsset);
            if (pCity != null)
            {
                village_origin = getCityData(pCity).cityName;
            }
            else
            {
                village_origin = "??";
            }

            origin_culture = null;

            year = MapBox.instance.mapStats.year;
            prepare();
        }

        public void create(City pCity, Language pLanguage)
        {
            origin_race = pLanguage.origin_race;
            id = get_next_id();
            if (pCity != null)
            {
                CityData cd = getCityData(pCity);
                name = MapBox.instance.kingdoms.getKingdomByID(cd.kingdomID).name;
                village_origin = cd.cityName;
            }
            else
            {
                name = "New " + pLanguage.name;
                village_origin = "??";
            }

            origin_culture = pLanguage.origin_culture;
            year = MapBox.instance.mapStats.year;
        }

        private CityData getCityData(City pCity)
        {
            return (CityData)ReflectionUtility.Reflection.GetField(typeof(City), pCity, "data");
        }

        private void prepare()
        {
            setDirty();
            if (color32 == Color.clear)
            {
                createGameColors();
            }

            hash = GetHashCode();
        }

        public void setDirty()
        {
            dirty = true;
        }

        public void createGameColors()
        {
            Culture culture = CultureManagerpp.instance.get(origin_culture);
            color32 = culture.color32;
            color32_border = culture.color32_border;
            color32_text = culture.color32_text;
        }

        public void addZone(TileZone pZone)
        {
            if (pZone.language != null && pZone.language != this)
            {
                pZone.culture.removeZone(pZone);
            }

            zones.Add(pZone);
            pZone.setLanguage(this);
            _zones_dirty = true;
        }

        public void removeZone(TileZone pZone)
        {
            zones.Remove(pZone);
            pZone.removeLanguage();
            _zones_dirty = true;
        }

        public void reset()
        {
            kingdoms = 0;
            cities = 0;
            followers = 0;
            _list_cities.Clear();
        }

        public void prepareForSave()
        {
            list_zone_ids = new List<int>();
            foreach (TileZone zone in zones)
            {
                list_zone_ids.Add(zone.id);
            }
        }

        public void load()
        {
            prepare();
            if (list_zone_ids == null)
            {
                return;
            }

            foreach (int list_zone_id in list_zone_ids)
            {
                ZoneCalculator zoneCalculator = (ZoneCalculator)ReflectionUtility.Reflection.GetField(typeof(ZoneCalculator), MapBox.instance, "zoneCalculator");
                addZone(zoneCalculator.getZoneByID(list_zone_id));
            }
        }

        private void updateSpread(float pElapsed)
        {
            if (timer_spread > 0f)
            {
                timer_spread -= pElapsed;
                return;
            }

            timer_spread = stats.culture_spread_speed.value;
            if (followers == 0 || !zones.Any())
            {
                return;
            }

            _list_zones.Clear();
            foreach (TileZone zone in zones)
            {
                _list_zones.Add(zone);
            }

            TileZone random = _list_zones.GetRandom();
            spreadAround(random);
        }

        private void spreadAround(TileZone pZone)
        {
            TileZone bestZoneToSpreadFrom = getBestZoneToSpreadFrom(pZone);
            Language language = pZone.language;
            if (bestZoneToSpreadFrom == null)
            {
                return;
            }

            if (bestZoneToSpreadFrom.language == null)
            {
                spreadOn(bestZoneToSpreadFrom);
                return;
            }

            int neighbour_count = 0;
            for (int i = 0; i < bestZoneToSpreadFrom.neighbours.Count; i++)
            {
                if (bestZoneToSpreadFrom.neighbours[i].language == language)
                {
                    neighbour_count++;
                }
            }

            float external_pressure = 0f;
            List<Actor> units = new List<Actor>();
            units.Clear();
            MethodInfo mf = typeof(Toolbox).GetMethod("fillListWithUnitsFromChunk");
            mf.Invoke(null, new object[] { bestZoneToSpreadFrom.centerTile.chunk, units });
            FieldInfo fi = typeof(Actor).GetField("data");
            foreach (
                Language lang in units.Select(
                    unit => ((ActorStatus)fi.GetValue(unit)).languages.Exists(
                        pID => pID == id
                    )
                )
            )
            {
                external_pressure += 0.05f;
            }

            float overall_pressure = stats.culture_spread_convert_chance.value * (float)neighbour_count + external_pressure;
            if (bestZoneToSpreadFrom.culture.followers > followers)
            {
                float prevelence = (followers + 1) / (bestZoneToSpreadFrom.culture.followers + 1);
                overall_pressure *= prevelence;
            }

            if (Toolbox.randomChance(overall_pressure))
            {
                spreadOn(bestZoneToSpreadFrom);
            }
        }

        private TileZone getBestZoneToSpreadFrom(TileZone pZone)
        {
            _list_zones_to_spread.Clear();
            _list_zones_to_spread.AddRange(pZone.neighbours);
            TileZone tileZone = null;
            TileZone tileZone2 = null;
            Language language = pZone.language;
            for (int i = 0; i < _list_zones_to_spread.Count; i++)
            {
                _list_zones_to_spread.ShuffleOne(i);
                TileZone tileZone4 = _list_zones_to_spread[i];
                if (tileZone4.city != null && tileZone4.language != this)
                {
                    if (tileZone2 == null)
                    {
                        tileZone2 = tileZone4;
                    }

                    if (tileZone == null && tileZone4.tilesWithGround == 64)
                    {
                        tileZone = tileZone4;
                        break;
                    }
                }
            }

            if (tileZone != null)
            {
                return tileZone;
            }

            return tileZone2;
        }

        private void spreadOn(TileZone pZone)
        {
            addZone(pZone);
        }

        private void updateDirty(float pElapsed)
        {
            if (!dirty)
            {
                return;
            }

            dirty = false;
            stats.clear();

            foreach (City list_city in _list_cities)
            {
                list_city.setStatusDirty();
            }
        }

        public void update(float pElapsed)
        {
            if (_zones_dirty)
            {
                _zones_dirty = false;
                updateTitleCenter();
            }

            if (!(bool)ReflectionUtility.Reflection.GetField(typeof(bool), MapBox.instance, "_isPaused"))
            {
                updateSpread(pElapsed);
            }

            updateDirty(pElapsed);
        }

        public void debug(DebugTool pTool)
        {
            MethodInfo mf = typeof(DebugTool).GetMethod("setText");
            mf.Invoke(pTool, new object[] { "id: " + id });
            mf.Invoke(pTool, new object[] { "name: " + name });
            mf.Invoke(pTool, new object[] { "followers: " + followers });
            mf.Invoke(pTool, new object[] { "cities: " + cities });
        }

        private void updateTitleCenter()
        {
            if (zones.Count == 0)
            {
                titleCenter = Globals.POINT_IN_VOID;
                return;
            }

            float num = 0f;
            float num2 = 0f;
            float num3;
            float num4 = 0f;
            TileZone tileZone = null;
            foreach (Vector3 zonePosV3 in zones.Select(zone => zone.centerTile.posV3))
            {
                num += zonePosV3.x;
                num2 += zonePosV3.y;
            }

            titleCenter.x = num / (float)zones.Count;
            titleCenter.y = num2 / (float)zones.Count;
            foreach (TileZone zone2 in zones)
            {
                num3 = Toolbox.Dist(zone2.centerTile.x, zone2.centerTile.y, titleCenter.x, titleCenter.y);
                if (tileZone == null || num3 < num4)
                {
                    tileZone = zone2;
                    num4 = num3;
                }
            }

            titleCenter.x = tileZone.centerTile.posV3.x;
            titleCenter.y = tileZone.centerTile.posV3.y + 2f;
        }

        public void clearZones()
        {
            foreach (TileZone zone in zones)
            {
                zone.removeCulture();
            }

            zones.Clear();
            _list_cities.Clear();
        }


        public static string get_next_id()
        {
            return "l_" + next_id++;
        }
    }
}
