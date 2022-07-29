using ReflectionUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Worldboxpp.Helpers
{
    internal class Windows
    {

        public static List<object> windows;
        readonly GameObject inspect_unit = GameObject.Find("/Canvas Container Main/Canvas - Windows/windows/inspect_unit");

        public void registerWindow<T>(string name) where T : new()
        {
            Reflection.CallStaticMethod(typeof(ScrollWindow), "checkWindowExist", name);
            var window = GameObject.Find("/Canvas Container Main/Canvas - Windows/windows/" + name);
            window.SetActive(false);

            windows.Add(new T());
        }
    }
}
