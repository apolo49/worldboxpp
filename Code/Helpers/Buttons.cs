using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Worldboxpp.Sliders;

namespace Worldboxpp.Helpers
{

    internal class Buttons
    {

        public void RegisterButton<T>(string name) where T : new()
        {
            //Override timer function
            //Create timer slider
            //Use slider to set time scale in config using Config.timeScale and Time.timeScale
        }

        public static GameObject RegisterSlider(string name, Transform parent, string localName, UnityAction call = null, string localDescription = null, GameObject instantinateFrom = null, Sprite sprite = null)
        {

            Main.instance.localisation.Add(name, localName);
            if (localDescription != null)
                NCMS.Utils.Localization.Add(name + " Description", localDescription);

            GameObject newSlider;
            if (instantinateFrom != null)
            {
                instantinateFrom.SetActive(false);
                newSlider = GameObject.Instantiate(instantinateFrom, parent);
                instantinateFrom.SetActive(true);
            }
            else
            {
                var tempObject = NCMS.Utils.GameObjects.FindEvenInactive("WorldLaws");
                tempObject.SetActive(false);
                newSlider = GameObject.Instantiate(tempObject);
                tempObject.SetActive(true);
            }
            var powerButtonComponent = newSlider.GetComponent<PowerButton>();
            GameObject.DestroyImmediate(powerButtonComponent);
            GameObject.DestroyImmediate(newSlider.GetComponent<Button>());
            newSlider.AddComponent<Sliderpp>();
            newSlider.GetComponent<Sliderpp>().gameObject.AddComponent<Slider>();

            newSlider.SetActive(true);

            newSlider.transform.SetParent(parent, false);

            return newSlider;
        }
    }
}
