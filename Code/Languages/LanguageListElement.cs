using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace worldboxpp.Language
{
    public class LanguageListElement : MonoBehaviour
    {
        public new Text name;

        public Language language;

        public Text textFollowers;

        public Text textZones;

        public Text textCities;

        public Image languageElement;

        public Image languageDecor;

        public Image iconRace;

        public void clickCulture()
        {
            Config.selectedLanguage = language;
            ScrollWindow.showWindow("language");
        }
    }
}
