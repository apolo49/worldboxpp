using System;
using NCMS.Utils;
using UnityEngine;
using NCMS;
using ReflectionUtility;

namespace Worldboxpp{ //Change example to the name of your mod
    [ModEntry]
    class Main : MonoBehaviour{
        void Awake(){
            Traits.init();
        }
    }
}