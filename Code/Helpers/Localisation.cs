using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worldboxpp.Helpers
{
    public class Localisation
    {

        public Dictionary<string, string> Locals { get; }

        public Localisation()
        {
            NCMS.Utils.Localization.Add("culture_founded_at", "Founded at");
            NCMS.Utils.Localization.Add("culture_diverged_from", "Diverged From");
        }

        public void Add(string key, string local)
        {
            Locals.Add(key, local);
        }

        public void Process()
        {
            foreach(var i in Locals.Keys)
            {
                if (string.IsNullOrEmpty(NCMS.Utils.Localization.Get(i)))
                {
                    NCMS.Utils.Localization.Add(i, Locals[i]);
                }
            }
        }
    }
}
