using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Worldboxpp.Culturepp;

namespace Worldboxpp.Languages
{
    public class LanguageManager
    {
        public static LanguageManager instance;

        private readonly MapBox world = MapBox.instance;

        public readonly List<Language> list;

        public Dictionary<string, Language> dict;

        private float _timer_recalc_values;

        private List<Language> _to_remove = new List<Language>();

        public LanguageManager(MapBox pWorld)
        {
            list = new List<Language>();
            instance = this;
            dict = new Dictionary<string, Language>();
        }

        public void update(float pElapsed)
        {
            Toolbox.bench("bench_lang");
            foreach (Language lang in list)
            {
                lang.update(pElapsed);
            }

            updateRecalcValues(pElapsed);
            Toolbox.benchEnd("bench_lang");
        }

        private void updateRecalcValues(float pElapsed)
        {
            if (_timer_recalc_values > 0f)
            {
                _timer_recalc_values -= pElapsed;
            }
            else
            {
                recalcLanguageValues();
            }
        }

        public void recalcLanguageValues()
        {
            _timer_recalc_values = 3f;
            foreach (Language lang in list)
            {
                lang.reset();
            }

            foreach (Kingdom kingdom in world.kingdoms.list)
            {
                if (!string.IsNullOrEmpty(kingdom.courtLanguageID))
                {
                    countKingdom(kingdom);
                }

                foreach (City city in kingdom.cities)
                {
                    countCity(city);
                    foreach (ActorData popPoint in city.data.popPoints)
                    {
                        countUnit(popPoint.status.language);
                    }
                }
            }

            foreach (Actor unit in world.units)
            {
                countUnit(unit.data.language);
            }

            removeDeadLanguages();
        }

        private void countKingdom(Kingdom pKingdom)
        {
            Language language = get(pKingdom.courtLanguageID);
            if (language != null)
            {
                language.kingdoms++;
            }
        }

        private void countCity(City pCity)
        {
            Language language = get(pCity.data.commonLanguageID);
            if (language != null)
            {
                language.cities++;
                language._list_cities.Add(pCity);
            }
        }

        private void countUnit(string pID)
        {
            Language language = get(pID);
            if (language != null)
            {
                language.followers++;
            }
        }

        public Language get(string pID)
        {
            if (string.IsNullOrEmpty(pID))
            {
                return null;
            }

            Language value = null;
            dict.TryGetValue(pID, out value);
            return value;
        }

        private void removeDeadLanguages()
        {
            _to_remove.Clear();
            foreach (Language lang in list)
            {
                if (lang.followers == 0 && lang.cities == 0 && lang.kingdoms == 0)
                {
                    _to_remove.Add(lang);
                }
            }

            foreach (Language lang in _to_remove)
            {
                lang.clearZones();
                list.Remove(lang);
                dict.Remove(lang.id);
            }
        }

        public string getLanguageName(string pID)
        {
            string empty = string.Empty;
            if (string.IsNullOrEmpty(pID))
            {
                return empty;
            }

            return get(pID).name;
        }

        public List<Language> save()
        {
            foreach (Language lang in list)
            {
                lang.prepareForSave();
            }

            return list;
        }

        public Language newLanguage(Race pRace, City pCity)
        {
            Language culture = new Language();
            culture.create(pRace, pCity);
            add(culture);
            return culture;
        }

        public Language newLanguage(Race pRace, City pCity, Culturepp pCulture)
        {
            Language culture = new Language();
            culture.create(pRace, pCity, pCulture);
            add(culture);
            return culture;
        }

        public void loadLanguage(Language pLanguage)
        {
            pLanguage.load();
            add(pLanguage);
        }

        public void clear()
        {
            dict.Clear();
            list.Clear();
        }

        private void add(Language pLanguage)
        {
            list.Add(pLanguage);
            dict.Add(pLanguage.id, pLanguage);
        }

        public bool isColorUsed(string pColor)
        {
            foreach (Language lang in list)
            {
                if (lang.color == pColor)
                {
                    return true;
                }
            }

            return false;
        }

        public void testLanguage()
        {
            Language language = new Language();
            Race pRace = AssetManager.raceLibrary.get("elf");
            language.create(pRace, null);
        }
    }
}
