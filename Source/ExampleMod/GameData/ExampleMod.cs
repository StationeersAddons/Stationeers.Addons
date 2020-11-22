// Stationeers.Addons (c) 2018-2020 Damian 'Erdroy' Korczowski & Contributors

using Stationeers.Addons.Loader.Plugins;
using UnityEngine;

namespace ExampleMod
{
    public class ExampleMod : IPlugin
    {
        public void OnLoad()
        {
            Debug.Log("Example Mod: Hello, World!");
        }

        public void OnUnload()
        {
            Debug.Log("Example Mod: Bye!");
        }
    }
}
