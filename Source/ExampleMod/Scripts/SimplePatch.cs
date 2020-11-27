// Stationeers.Addons (c) 2018-2020 Damian 'Erdroy' Korczowski & Contributors

using Assets.Scripts.UI;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace ExampleMod.Scripts
{
    [HarmonyPatch(typeof(TutorialMenu))]
    [HarmonyPatch("GetMissionList")]
    public class SimplePatch
    {
        [UsedImplicitly]
        private static bool Prefix(TutorialMenu __instance)
        {
            // Write some stuff so we can know, that we've successfully patched the game!
            Debug.Log("We've patched the game! YEY " + __instance.ButtonContainer);
            return true;
        }

        [UsedImplicitly]
        private static void Postfix()
        {
        }
    }
}
