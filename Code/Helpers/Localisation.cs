using ReflectionUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worldboxpp.Helpers
{
    public class Localisation
    {
        private Dictionary<string, string> locals = new Dictionary<string, string>();
        public Dictionary<string, string> Locals { get => locals; }

        public Localisation()
        {
            NCMS.Utils.Localization.Add("culture_founded_at", "Founded at");
            NCMS.Utils.Localization.Add("culture_diverged_from", "Diverged From");
            NCMS.Utils.Localization.Add("language_zones_button_name", "Language Map");
            NCMS.Utils.Localization.Add("language_zones_button_description", "");
            NCMS.Utils.Localization.Add("language_button_name", "Language list");
            NCMS.Utils.Localization.Add("language_button_description", "View the world's languages");
        }

        public void Add(string key, string local)
        {
            if (!Locals.ContainsKey(key))
                Locals.Add(key, local);
        }

        public void Process()
        {
            var Worldboxlocal = (Dictionary<string, string>)Reflection.GetField(LocalizedTextManager.instance.GetType(), LocalizedTextManager.instance, "localizedText");
            var KeysToBeAdded = from key in Locals.Keys
                                where Worldboxlocal.ContainsKey(key) == false
                                select key;
            foreach (var key in KeysToBeAdded)
            {
                NCMS.Utils.Localization.Add(key, Locals[key]);
            }
        }
    }
}
