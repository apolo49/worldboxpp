using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace worldboxpp.Language
{
    internal class LanguageListWindow : MonoBehaviour
    {
        public LanguageListElement elementPrefab;

        private List<LanguageListElement> elements = new List<LanguageListElement>();

        private MapBox world;

        public Transform transformContent;

        public GameObject noItems;

        private void OnEnable()
        {
            showList();
        }

        internal void showList()
        {
            if (!Config.gameLoaded)
            {
                return;
            }

            world = MapBox.instance;
            while (elements.Count > 0)
            {
                LanguageListElement languageListElement = elements[elements.Count - 1];
                elements.RemoveAt(elements.Count - 1);
                Object.Destroy(languageListElement.gameObject);
            }

            if (world.languages.list.Count == 0)
            {
                noItems.SetActive(value: true);
                transformContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 232f);
                return;
            }

            noItems.SetActive(value: false);
            world.languages.list.Sort(sorter);
            foreach (Language item in world.languages.list)
            {
                showElement(item);
            }

            transformContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, elements.Count * 44 + 30);
        }

        private void showElement(Language pObject)
        {
            LanguageListElement languageListElement = Object.Instantiate(elementPrefab, transformContent);
            languageListElement.GetComponent<RectTransform>().anchoredPosition = new Vector3(-6f, -(elements.Count * 44 + 30));
            elements.Add(languageListElement);
            languageListElement.language = pObject;
            Race race = AssetManager.raceLibrary.get(pObject.origin_race);
            languageListElement.iconRace.sprite = race.getRaceIconSprite();
            languageListElement.name.text = pObject.name;
            languageListElement.name.color = pObject.color32_text;
            languageListElement.languageElement.sprite = (Sprite)Resources.Load(languageListElement.language.icon_element, typeof(Sprite));
            languageListElement.languageDecor.sprite = (Sprite)Resources.Load(languageListElement.language.icon_decor, typeof(Sprite));
            languageListElement.languageElement.color = Toolbox.makeColor(languageListElement.language.color);
            languageListElement.languageDecor.color = Toolbox.makeColor(languageListElement.language.color);
            languageListElement.textFollowers.text = pObject.followers.ToString() ?? "";
            languageListElement.textCities.text = pObject.cities.ToString() ?? "";
            languageListElement.textZones.text = pObject.zones.Count.ToString() ?? "";
        }

        public int sorter(Language k1, Language k2)
        {
            return k2.followers.CompareTo(k1.followers);
        }
    }
}
