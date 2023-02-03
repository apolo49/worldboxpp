using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Worldboxpp.Languages;

namespace Worldboxpp.Zones
{
    class ZoneCalculatorpp : ZoneCalculator
    {

        protected SpriteRenderer spriteRenderer;

        Type mapBoxType = typeof(MapBox);
        Type worldTileZoneBorderType = typeof(WorldTileZoneBorder);
        Type tileZoneType = typeof(TileZone);
        MethodInfo miGetForcedMapMode;
        MethodInfo miShowCultureZones;
        MethodInfo miShowKingdomZones;
        MethodInfo miShowCityZones;
        MethodInfo miIsEnemyKingdom = typeof(Kingdom).GetMethod("isEnemy");
        FieldInfo fiQualityChanger;
        FieldInfo fiLowRes = typeof(QualityChanger).GetField("lowRes");
        FieldInfo fiMainCamera;
        FieldInfo fiZoneUp;
        FieldInfo fiZoneDown;
        FieldInfo fiZoneLeft;
        FieldInfo fiZoneRight;
        FieldInfo fiWorldTileZoneBorder = typeof(WorldTile).GetField("worldTileZoneBorder");
        FieldInfo fiWTZBBorder;
        FieldInfo fiWTZBBorderUp;
        FieldInfo fiWTZBBorderDown;
        FieldInfo fiWTZBBorderLeft;
        FieldInfo fiWTZBBorderRight;
        FieldInfo fiCityZones = typeof(City).GetField("zones");
        FieldInfo fiCityHashcode = typeof(City).GetField("hashcode");
        FieldInfo fiCityKingdom = typeof(City).GetField("kingdom");
        FieldInfo fiKingdomColor = typeof(Kingdom).GetField("kingdomColor");
        MoveCamera moveCamera = (MoveCamera)typeof(MoveCamera).GetField("instance").GetValue(null);


        protected Texture2D texture;

        protected City highlight_city;
        protected Kingdom highlight_kingdom;
        protected Culture highlight_culture;
        protected Language highlight_language;

        protected ZoneDisplayMode _mode;
        protected bool _drawZones_dirty;
        protected float _redraw_timer;
        private bool _dirty;
        protected int _debug_redrawn_last_amount;
        protected Kingdom _lastSelectedKingdom;
        protected HashSetTileZone _currentDrawnZones = new HashSetTileZone();
        protected HashSetTileZone _toCleanUp = new HashSetTileZone();
        protected bool checkKingdom;
        protected Color32[] pixels;

        public ZoneCalculatorpp()
        {
            spriteRenderer = (SpriteRenderer)base.GetType().GetField("sprRnd").GetValue(this);
            texture = (Texture2D)typeof(MapLayer).GetField("texture").GetValue(this);

            miGetForcedMapMode = mapBoxType.GetMethod("getForcedMapMode");
            miShowCultureZones = mapBoxType.GetMethod("showCultureZones");
            miShowKingdomZones = mapBoxType.GetMethod("showKingdomZones");
            miShowCityZones = mapBoxType.GetMethod("showCityZones");
            fiQualityChanger = mapBoxType.GetField("qualityChanger");
            fiWTZBBorder = worldTileZoneBorderType.GetField("Border");
            fiWTZBBorderUp = worldTileZoneBorderType.GetField("borderUp");
            fiWTZBBorderDown = worldTileZoneBorderType.GetField("BorderDown");
            fiWTZBBorderLeft = worldTileZoneBorderType.GetField("BorderLeft");
            fiWTZBBorderRight = worldTileZoneBorderType.GetField("BorderRight");
            fiZoneUp = tileZoneType.GetField("zone_up");
            fiZoneDown = tileZoneType.GetField("zone_down");
            fiZoneLeft = tileZoneType.GetField("zone_left");
            fiZoneRight = tileZoneType.GetField("zone_right");
            fiMainCamera = typeof(MoveCamera).GetField("mainCamera");
        }

        public override void update(float pElapsed)
        {
            checkDrawnZonesDirty();
            if ((bool)miShowCityZones.Invoke(world, new object[] { }))
            {
                setMode(ZoneDisplayMode.CityBorders);
                spriteRenderer.enabled = true;
            }
            else if ((bool)miShowKingdomZones.Invoke(world, new object[] { }))
            {
                setMode(ZoneDisplayMode.KingdomBorders);
                spriteRenderer.enabled = true;
            }
            else if ((bool)miShowCultureZones.Invoke(world, new object[] { }))
            {
                setMode(ZoneDisplayMode.Cultures);
                spriteRenderer.enabled = true;
            }
            else if (Main.showLanguageZones())
            {
                setMode((ZoneDisplayMode)4);
                spriteRenderer.enabled = true;
            }
            else
            {
                setMode(ZoneDisplayMode.None);
                spriteRenderer.enabled = false;
            }
            redrawZones();
            Color white = Color.white;
            QualityChanger qc = (QualityChanger)fiQualityChanger.GetValue(MapBox.instance);
            if ((bool)fiLowRes.GetValue(qc))
            {
                white.a = 0.7f;
            }
            else
            {
                white.a = Mathf.Clamp(getCameraScaleZoom() * 0.3f, 0f, 0.7f);
            }
            spriteRenderer.color = white;
            base.update(pElapsed);
        }

        private float getCameraScaleZoom()
        {
            return Mathf.Clamp(((Camera)fiMainCamera.GetValue(moveCamera)).orthographicSize / 20f, 1f, 30f);
        }

        private void checkDrawnZonesDirty()
        {
            if (_drawZones_dirty)
            {
                _drawZones_dirty = false;
                _redraw_timer = 0f;
            }
        }

        private void setMode(ZoneDisplayMode pMode)
        {
            if (_mode != pMode)
            {
                _redraw_timer = 0f;
            }

            _mode = pMode;
        }

        private void redrawZones()
        {
            if (_lastSelectedKingdom != Config.selectedKingdom)
            {
                _lastSelectedKingdom = Config.selectedKingdom;
                _redraw_timer = 0f;
            }
            if (_redraw_timer > 0f)
            {
                _redraw_timer -= Time.deltaTime;
                return;
            }
            _redraw_timer = 0.3f;
            _dirty = false;
            _debug_redrawn_last_amount = 0;
            if (_currentDrawnZones.Any())
            {
                _toCleanUp.UnionWith(_currentDrawnZones);
            }
            if (spriteRenderer.enabled)
            {
                switch (_mode)
                {
                    case ZoneDisplayMode.KingdomBorders:
                        {
                            for (int j = 0; j < world.kingdoms.list.Count; j++)
                            {
                                Kingdom kingdom = world.kingdoms.list[j];
                                for (int k = 0; k < kingdom.cities.Count; k++)
                                {
                                    City city = kingdom.cities[k];
                                    for (int l = 0; l < ((List<TileZone>)fiCityZones.GetValue(city)).Count; l++)
                                    {
                                        TileZone pZone = ((List<TileZone>)fiCityZones.GetValue(city))[l];
                                        colorZone(pZone);
                                    }
                                }
                            }
                            break;
                        }
                    case ZoneDisplayMode.CityBorders:
                        {
                            for (int m = 0; m < world.citiesList.Count; m++)
                            {
                                City city2 = world.citiesList[m];
                                for (int n = 0; n < ((List<TileZone>)fiCityZones.GetValue(city2)).Count; n++)
                                {
                                    TileZone pZone2 = ((List<TileZone>)fiCityZones.GetValue(city2))[n];
                                    colorZone(pZone2);
                                }
                            }
                            break;
                        }
                    case ZoneDisplayMode.Cultures:
                        {
                            for (int i = 0; i < world.cultures.list.Count; i++)
                            {
                                foreach (TileZone zone in world.cultures.list[i].zones)
                                {
                                    colorZone(zone);
                                }
                            }
                            break;
                        }
                    case (ZoneDisplayMode)4:
                        {
                            foreach (Language lang in Main.instance.languages.list)
                            {
                                foreach (TileZone zone in lang.zones)
                                {
                                    colorZone(zone);
                                }
                            }
                            break;
                        }
                }
            }
            if (_toCleanUp.Any())
            {
                clearDrawnZones();
            }
            if (_dirty)
            {
                updatePixels();
            }
        }

        private void colorZone(TileZone pZone)
        {
            checkKingdom = true;
            switch (_mode)
            {
                case ZoneDisplayMode.CityBorders:
                    colorModeCityBorders(pZone);
                    break;
                case ZoneDisplayMode.KingdomBorders:
                    colorModeKingdomBorders(pZone);
                    break;
                case ZoneDisplayMode.Cultures:
                    colorModeCultures(pZone);
                    break;
                case (ZoneDisplayMode)4:
                    colorModeLanguages(pZone);
                    break;
            }
            _currentDrawnZones.Add(pZone);
            _toCleanUp.Remove(pZone);
        }

        private void clearDrawnZones()
        {
            foreach (TileZone item in _toCleanUp)
            {
                colorZone(item, Toolbox.clear);
                item.last_drawn_id = 0;
                item.last_drawn_hashcode = 0;
                _currentDrawnZones.Remove(item);
            }
            _toCleanUp.Clear();
        }

        private void updatePixels()
        {
            texture.SetPixels32(pixels);
            texture.Apply();
        }

        private void colorModeCityBorders(TileZone pZone)
        {
            Kingdom kingdom = (Kingdom)fiCityKingdom.GetValue(pZone.city);
            KingdomColor kingdomColor = (KingdomColor)fiKingdomColor.GetValue(kingdom);
            Color32 color = kingdomColor.colorBorderInsideAlpha;
            Color32 color2 = kingdomColor.colorBorderOut;
            bool flag = isBorderColor_cities((TileZone)fiZoneUp.GetValue(pZone), pZone.city, pCheckFriendly: true);
            bool flag2 = isBorderColor_cities((TileZone)fiZoneDown.GetValue(pZone), pZone.city);
            bool flag3 = isBorderColor_cities((TileZone)fiZoneLeft.GetValue(pZone), pZone.city);
            bool flag4 = isBorderColor_cities((TileZone)fiZoneRight.GetValue(pZone), pZone.city, pCheckFriendly: true);
            int num = (int)_mode * 10000000;
            int hashcode = (int)fiCityHashcode.GetValue(pZone.city);
            if (flag)
            {
                num += 100000;
            }
            if (flag2)
            {
                num += 10000;
            }
            if (flag3)
            {
                num += 1000;
            }
            if (flag4)
            {
                num += 100;
            }
            num += kingdom.hashcode;
            bool flag5 = false;
            if (pZone.city == highlight_city)
            {
                num++;
                flag5 = true;
            }
            if (pZone.last_drawn_id == num && pZone.last_drawn_hashcode == hashcode)
            {
                return;
            }
            pZone.last_drawn_id = num;
            pZone.last_drawn_hashcode = hashcode;
            _dirty = true;
            for (int i = 0; i < pZone.tiles.Count; i++)
            {
                WorldTile worldTile = pZone.tiles[i];
                WorldTileZoneBorder wtzb = (WorldTileZoneBorder)fiWorldTileZoneBorder.GetValue(worldTile);
                if (flag5)
                {
                    pixels[worldTile.data.tile_id] = Toolbox.color_white_32;
                }
                else if (!(bool)fiWTZBBorder.GetValue(wtzb))
                {
                    pixels[worldTile.data.tile_id] = color;
                }
                else if ((bool)fiWTZBBorderUp.GetValue(wtzb) && flag)
                {
                    pixels[worldTile.data.tile_id] = color2;
                }
                else if ((bool)fiWTZBBorderDown.GetValue(wtzb) && flag2)
                {
                    pixels[worldTile.data.tile_id] = color2;
                }
                else if ((bool)fiWTZBBorderLeft.GetValue(wtzb) && flag3)
                {
                    pixels[worldTile.data.tile_id] = color2;
                }
                else if ((bool)fiWTZBBorderRight.GetValue(wtzb) && flag4)
                {
                    pixels[worldTile.data.tile_id] = color2;
                }
                else
                {
                    pixels[worldTile.data.tile_id] = color;
                }
            }
            _debug_redrawn_last_amount++;
        }

        private void colorModeKingdomBorders(TileZone pZone)
        {
            Kingdom kingdom = (Kingdom)fiCityKingdom.GetValue(pZone.city);
            KingdomColor kingdomColor = (KingdomColor)fiKingdomColor.GetValue(kingdom);
            Color32 color = kingdomColor.colorBorderInsideAlpha;
            Color32 color2 = kingdomColor.colorBorderOut;
            bool flag = isBorderColor_kingdoms((TileZone)fiZoneUp.GetValue(pZone), pZone.city, pCheckFriendly: true);
            bool flag2 = isBorderColor_kingdoms((TileZone)fiZoneDown.GetValue(pZone), pZone.city);
            bool flag3 = isBorderColor_kingdoms((TileZone)fiZoneLeft.GetValue(pZone), pZone.city);
            bool flag4 = isBorderColor_kingdoms((TileZone)fiZoneRight.GetValue(pZone), pZone.city, pCheckFriendly: true);
            int num = (int)_mode * 10000000;
            int hashcode = kingdom.hashcode;
            if (flag)
            {
                num += 100000;
            }
            if (flag2)
            {
                num += 10000;
            }
            if (flag3)
            {
                num += 1000;
            }
            if (flag4)
            {
                num += 100;
            }
            bool flag5 = false;
            if (kingdom == highlight_kingdom)
            {
                num++;
                flag5 = true;
            }
            else if (highlight_kingdom != null && pZone.city != null && (bool)miIsEnemyKingdom.Invoke(kingdom, new object[] { highlight_kingdom }))
            {
                num += 5;
            }
            if (pZone.last_drawn_id == num && pZone.last_drawn_hashcode == hashcode)
            {
                return;
            }
            pZone.last_drawn_id = num;
            pZone.last_drawn_hashcode = hashcode;
            _dirty = true;
            for (int i = 0; i < pZone.tiles.Count; i++)
            {
                WorldTile worldTile = pZone.tiles[i];
                WorldTileZoneBorder wtzb = (WorldTileZoneBorder)fiWorldTileZoneBorder.GetValue(worldTile);
                if (flag5)
                {
                    pixels[worldTile.data.tile_id] = Toolbox.color_white_32;
                }
                else if (!(bool)fiWTZBBorder.GetValue(wtzb))
                {
                    pixels[worldTile.data.tile_id] = color;
                }
                else if ((bool)fiWTZBBorderUp.GetValue(wtzb) && flag)
                {
                    pixels[worldTile.data.tile_id] = color2;
                }
                else if ((bool)fiWTZBBorderDown.GetValue(wtzb) && flag2)
                {
                    pixels[worldTile.data.tile_id] = color2;
                }
                else if ((bool)fiWTZBBorderLeft.GetValue(wtzb) && flag3)
                {
                    pixels[worldTile.data.tile_id] = color2;
                }
                else if ((bool)fiWTZBBorderRight.GetValue(wtzb) && flag4)
                {
                    pixels[worldTile.data.tile_id] = color2;
                }
                else
                {
                    pixels[worldTile.data.tile_id] = color;
                }
            }
            _debug_redrawn_last_amount++;
        }

        private void colorModeCultures(TileZone pZone)
        {
            Culture culture = pZone.culture;
            if (culture == null)
            {
                return;
            }
            bool flag = isBorderColor_cultures((TileZone)fiZoneUp.GetValue(pZone), culture);
            bool flag2 = isBorderColor_cultures((TileZone)fiZoneDown.GetValue(pZone), culture);
            bool flag3 = isBorderColor_cultures((TileZone)fiZoneLeft.GetValue(pZone), culture);
            bool flag4 = isBorderColor_cultures((TileZone)fiZoneRight.GetValue(pZone), culture);
            int num = (int)_mode * 10000000;
            int hash = pZone.culture.hash;
            if (flag)
            {
                num += 100000;
            }
            if (flag2)
            {
                num += 10000;
            }
            if (flag3)
            {
                num += 1000;
            }
            if (flag4)
            {
                num += 100;
            }
            bool flag5 = false;
            if (culture == highlight_culture)
            {
                num++;
                flag5 = true;
            }
            if (pZone.last_drawn_id == num && pZone.last_drawn_hashcode == hash)
            {
                return;
            }
            pZone.last_drawn_id = num;
            pZone.last_drawn_hashcode = hash;
            _dirty = true;
            for (int i = 0; i < pZone.tiles.Count; i++)
            {
                WorldTile worldTile = pZone.tiles[i];
                WorldTileZoneBorder wtzb = (WorldTileZoneBorder)fiWorldTileZoneBorder.GetValue(worldTile);
                if (flag5)
                {
                    pixels[worldTile.data.tile_id] = Toolbox.color_white_32;
                }
                else if (!(bool)fiWTZBBorder.GetValue(wtzb))
                {
                    pixels[worldTile.data.tile_id] = culture.color32;
                }
                else if ((bool)fiWTZBBorderUp.GetValue(wtzb) && flag)
                {
                    pixels[worldTile.data.tile_id] = culture.color32_border;
                }
                else if ((bool)fiWTZBBorderDown.GetValue(wtzb) && flag2)
                {
                    pixels[worldTile.data.tile_id] = culture.color32_border;
                }
                else if ((bool)fiWTZBBorderLeft.GetValue(wtzb) && flag3)
                {
                    pixels[worldTile.data.tile_id] = culture.color32_border;
                }
                else if ((bool)fiWTZBBorderRight.GetValue(wtzb) && flag4)
                {
                    pixels[worldTile.data.tile_id] = culture.color32_border;
                }
                else
                {
                    pixels[worldTile.data.tile_id] = culture.color32;
                }
            }
        }

        private void colorModeLanguages(TileZone pZone)
        {
            Language language = pZone.language;
            if (language == null)
            {
                return;
            }
            bool flag = isBorderColor_languages((TileZone)fiZoneUp.GetValue(pZone), language);
            bool flag2 = isBorderColor_languages((TileZone)fiZoneDown.GetValue(pZone), language);
            bool flag3 = isBorderColor_languages((TileZone)fiZoneLeft.GetValue(pZone), language);
            bool flag4 = isBorderColor_languages((TileZone)fiZoneRight.GetValue(pZone), language);
            int num = (int)_mode * 10000000;
            int hash = pZone.culture.hash;
            if (flag)
            {
                num += 100000;
            }
            if (flag2)
            {
                num += 10000;
            }
            if (flag3)
            {
                num += 1000;
            }
            if (flag4)
            {
                num += 100;
            }
            bool flag5 = false;
            if (language == highlight_language)
            {
                num++;
                flag5 = true;
            }
            if (pZone.last_drawn_id == num && pZone.last_drawn_hashcode == hash)
            {
                return;
            }
            pZone.last_drawn_id = num;
            pZone.last_drawn_hashcode = hash;
            _dirty = true;
            for (int i = 0; i < pZone.tiles.Count; i++)
            {
                WorldTile worldTile = pZone.tiles[i];
                WorldTileZoneBorder wtzb = (WorldTileZoneBorder)fiWorldTileZoneBorder.GetValue(worldTile);
                if (flag5)
                {
                    pixels[worldTile.data.tile_id] = Toolbox.color_white_32;
                }
                else if (!(bool)fiWTZBBorder.GetValue(wtzb))
                {
                    pixels[worldTile.data.tile_id] = language.color32;
                }
                else if ((bool)fiWTZBBorderUp.GetValue(wtzb) && flag)
                {
                    pixels[worldTile.data.tile_id] = language.color32_border;
                }
                else if ((bool)fiWTZBBorderDown.GetValue(wtzb) && flag2)
                {
                    pixels[worldTile.data.tile_id] = language.color32_border;
                }
                else if ((bool)fiWTZBBorderLeft.GetValue(wtzb) && flag3)
                {
                    pixels[worldTile.data.tile_id] = language.color32_border;
                }
                else if ((bool)fiWTZBBorderRight.GetValue(wtzb) && flag4)
                {
                    pixels[worldTile.data.tile_id] = language.color32_border;
                }
                else
                {
                    pixels[worldTile.data.tile_id] = language.color32;
                }
            }
        }

        private void colorZone(TileZone pZone, Color32 pColor)
        {
            _dirty = true;
            for (int i = 0; i < pZone.tiles.Count; i++)
            {
                WorldTile worldTile = pZone.tiles[i];
                pixels[worldTile.data.tile_id] = pColor;
            }
        }

        private bool isBorderColor_cities(TileZone pZone, City pCity, bool pCheckFriendly = false)
        {
            Kingdom zoneKingdom = (Kingdom)fiCityKingdom.GetValue(pZone.city);
            Kingdom cityKingdom = (Kingdom)(fiCityKingdom.GetValue(pCity));
            if (pCheckFriendly && pZone != null && pZone.city != pCity && pZone.city != null && checkKingdom && zoneKingdom == cityKingdom)
            {
                return false;
            }
            if (checkKingdom)
            {
                if (pZone != null && pZone.city != null && zoneKingdom == cityKingdom)
                {
                    return pZone.city != pCity;
                }
                return true;
            }
            if (pZone != null)
            {
                return pZone.city != pCity;
            }
            return true;
        }

        private bool isBorderColor_kingdoms(TileZone pZone, City pCity, bool pCheckFriendly = false)
        {
            Kingdom zoneKingdom = (Kingdom)fiCityKingdom.GetValue(pZone.city);
            Kingdom cityKingdom = (Kingdom)(fiCityKingdom.GetValue(pCity));
            if (pCheckFriendly && pZone != null && pZone.city != pCity && pZone.city != null && checkKingdom && zoneKingdom == cityKingdom)
            {
                return false;
            }
            if (checkKingdom)
            {
                if (pZone != null && pZone.city != null)
                {
                    return zoneKingdom != cityKingdom;
                }
                return true;
            }
            if (pZone != null)
            {
                return pZone.city != pCity;
            }
            return true;
        }

        private bool isBorderColor_cultures(TileZone pZone, Culture pCulture, bool pCheckFriendly = false)
        {
            if (pZone.culture != pCulture)
            {
                return true;
            }
            return false;
        }

        private bool isBorderColor_languages(TileZone pZone, Language pLanguage, bool pCheckFriendly = false)
        {
            if (pZone.language != pLanguage)
            {
                return true;
            }
            return false;
        }

        new public MapMode getCurrentMode()
        {
            MapMode mapMode = MapMode.None;
            mapMode = (MapMode)miGetForcedMapMode.Invoke(world, new object[] { });
            if (mapMode == MapMode.None)
            {
                if ((bool)miShowCultureZones.Invoke(world, new object[] { }))
                {
                    mapMode = MapMode.Cultures;
                }
                else if ((bool)miShowKingdomZones.Invoke(world, new object[] { }))
                {
                    mapMode = MapMode.Kingdoms;
                }
                else if ((bool)miShowCityZones.Invoke(world, new object[] { }))
                {
                    mapMode = MapMode.Villages;
                }
                else if (Main.showLanguageZones())
                {
                    mapMode = (MapMode)4;
                }
            }
            return mapMode;
        }
    }
}
