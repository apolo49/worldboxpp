using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReflectionUtility;
using Worldboxpp.Languages;

namespace Worldboxpp.Zones
{
    public class TileZonepp : TileZone
    {
        protected bool zoneChecked;
        public Language language;

        TileZonepp(TileZone pZone)
        {
            this.culture = pZone.culture;
            this.goodForNewCity = pZone.goodForNewCity;
            this.zoneChecked = (bool)Reflection.GetField(pZone.GetType(), pZone, "zoneChecked");

        }
    }
}
