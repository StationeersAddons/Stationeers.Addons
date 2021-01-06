using Stationeers.Addons;
using Stationeers.Addons.API;
using UnityEngine;

namespace ExampleMod.Scripts
{
    public class ExampleMod : IPlugin
    {
        public void OnLoad()
        {
            Debug.Log("Example Mod: Hello, World!");

            var monkey = BundleManager.GetAssetBundle("monkey").LoadAsset<Mesh>("monkey");

            var basicModel = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var basicModelTransform = basicModel.transform;
            basicModel.GetComponent<MeshFilter>().sharedMesh = monkey;
            basicModelTransform.position = Vector3.zero;
            basicModelTransform.localScale = Vector3.one * 4.0f;
            basicModelTransform.rotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
        }

        public void OnUnload()
        {
            Debug.Log("Example Mod: Bye!");
        }
    }
}
