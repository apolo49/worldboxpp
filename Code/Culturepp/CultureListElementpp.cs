using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worldboxpp.Culturepp
{
    internal class CultureListElementpp : CultureListElement
    {
        new public void clickCulture()
        {
            Config.selectedCulture = culture;
            ScrollWindow.showWindow("culturepp");
        }
    }
}
