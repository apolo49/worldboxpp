using System;
using System.Threading;
using NCMS;
using UnityEngine;
using ReflectionUtility;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Worldboxpp //This should be the same as the namespace in main.cs
{
    class Traits
    {
        public static void init()
        {
         ActorTrait example = new ActorTrait();
         example.action_special_effect = (WorldAction)Delegate.Combine(example.action_special_effect new, WorldAction(exampleP)); //This adds the effect that will add a trait, I add P to represent Power
         example.id = "Trait Name"; //Trait Name is the name of your trait so if your adding a super hero it would be Super Hero
         example.path_icon = "ui/Icons/iconStupid"; //The icon of your trait
         example.baseStats.health = 200;
         AssetManager.traits.add(example);
         addTraitToLocalizedLibrary(example.id, "This a description");
         PlayerConfig.unlockTrait(example.id); // This unlocks your trait Dont forget to add this

         ActorTrait eexample = new ActorTrait();
         eexample.id = "Trait Namee";
         eexample.path_icon = "ui/Icons/iconStupid";
         eexample.baseStats.health = 200;
         AssetManager.traits.add(eexample);
         addTraitToLocalizedLibrary(eexample.id, "This a descriptions");
         PlayerConfig.unlockTrait(eexample.id);

         ActorTrait eexamplee = new ActorTrait();
         eexamplee.id = "Trait Nameee";
         eexamplee.path_icon = "ui/Icons/iconStupid";
         eexamplee.baseStats.health = 200;
         AssetManager.traits.add(eexamplee);
         addTraitToLocalizedLibrary(eexamplee.id, "This a descriptionss");
         PlayerConfig.unlockTrait(eexamplee.id);
         }

        public static void addTraitToLocalizedLibrary(string id, string description)
      	{
            //Everything below down here is the siztem to add the description V
      		string language = Reflection.GetField(LocalizedTextManager.instance.GetType(), LocalizedTextManager.instance, "language") as string;
      		Dictionary<string, string> localizedText = Reflection.GetField(LocalizedTextManager.instance.GetType(), LocalizedTextManager.instance, "localizedText") as Dictionary<string, string>;
      		localizedText.Add("trait_" + id, id);
      		localizedText.Add("trait_" + id + "_info", description);
        }
        public static bool exampleP(BaseSimObject pTarget, WorldTile pTile = null)
        {
        Actor a = Reflection.GetField(pTarget.GetType(), pTarget, "a") as Actor; //This is getting the unit
          a.addTrait("fire_proof");  // This adds the traits
          a.addTrait("madness");
          a.addTrait("strong_minded");
          return true;
        }
        }
        
}