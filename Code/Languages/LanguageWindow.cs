using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Worldboxpp.Languages
{
    internal class LanguageWindow : MonoBehaviour
    {
        public NameInput nameInput;

        public Text text_description;

        public Text text_values;

        public Text text_rare_knowledges;

        public CityIcon population;

        public CityIcon cities;

        public CityIcon kingdoms;

        public CityIcon knowledge_gain;

        public CityIcon level;

        public CityIcon zones;

        public CityIcon spreadSpeed;

        public CityIcon convertChance;

        public Transform tech_common;

        public Transform tech_rare;

        private TechElement tech_prefab;

        public StatBar researchBar;

        public Image iconCurrentTech;

        public Image languageElement;

        public Image languageDecor;

        private void Awake()
        {
            nameInput.addListener(applyInputName);
            Worldboxpp.Helpers.Windows.registerWindow("languages");
        }

        private void applyInputName(string input)
        {
            Config.selectedLanguage.name = input;
        }

        public void showInfo()
        {
            if (Config.selectedLanguage != null)
            {
                MapBox.instance.languages.recalcLanguageValues();
                languageElement.sprite = (Sprite)Resources.Load(Config.selectedLanguage.icon_element, typeof(Sprite));
                languageDecor.sprite = (Sprite)Resources.Load(Config.selectedLanguage.icon_decor, typeof(Sprite));
                languageElement.color = Toolbox.makeColor(Config.selectedLanguage.color);
                languageDecor.color = Toolbox.makeColor(Config.selectedLanguage.color);
                if (tech_prefab == null)
                {
                    tech_prefab = Resources.Load<TechElement>("ui/PrefabTechElement");
                }

                nameInput.setText(Config.selectedLanguage.name);
                text_description.text = string.Empty;
                text_values.text = string.Empty;
                population.setValue(Config.selectedLanguage.followers);
                cities.setValue(Config.selectedLanguage.cities);
                kingdoms.setValue(Config.selectedLanguage.kingdoms);
                zones.setValue(Config.selectedLanguage.zones.Count);
                spreadSpeed.setValue(Config.selectedLanguage.stats.language_spread_speed.value, "", "", pFloat: true);
                int num = (int)(Config.selectedLanguage.stats.language_spread_convert_chance.value * 100f);
                convertChance.setValue(num, "%");
                int num2 = MapBox.instance.mapStats.year - Config.selectedLanguage.year + 1;
                if (!string.IsNullOrEmpty(Config.selectedLanguage.village_origin))
                {
                    showStat("language_founded_in", Config.selectedLanguage.village_origin);
                }

                showStat("age", num2);
                Race race = AssetManager.raceLibrary.get(Config.selectedLanguage.origin_race);

                MethodInfo mfCheckTextFont = typeof(LocalizedText).GetMethod("checkTextFont");
                MethodInfo mfCheckSpecialLanguages = typeof(LocalizedText).GetMethod("checkSpecialLanguages");
                mfCheckTextFont.Invoke(text_description.GetComponent<LocalizedText>(), new object[] { });
                mfCheckTextFont.Invoke(text_values.GetComponent<LocalizedText>(), new object[] { });
                mfCheckSpecialLanguages.Invoke(text_description.GetComponent<LocalizedText>(), new object[] { });
                mfCheckSpecialLanguages.Invoke(text_values.GetComponent<LocalizedText>(), new object[] { });
                if (LocalizedTextManager.isRTLLang())
                {
                    text_description.alignment = TextAnchor.UpperRight;
                    text_values.alignment = TextAnchor.UpperLeft;
                }
                else
                {
                    text_description.alignment = TextAnchor.UpperLeft;
                    text_values.alignment = TextAnchor.UpperRight;
                }
            }
        }

        private void OnEnable()
        {
            showInfo();
        }

        private void showStat(string pID, object pValue)
        {
            Text text = text_description;
            text.text = text.text + LocalizedTextManager.getText(pID) + "\n";
            Text text2 = text_values;
            text2.text = text2.text + pValue?.ToString() + "\n";
        }

        private void OnDisable()
        {
            if (Config.selectedLanguage != null)
            {
                nameInput.inputField.DeactivateInputField();
                Config.selectedLanguage = null;
            }
        }
    }
}
