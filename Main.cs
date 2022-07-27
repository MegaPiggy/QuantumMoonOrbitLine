using OWML.Common;
using OWML.ModHelper;
using UnityEngine;

namespace QuantumMoonOrbitLine
{
    public class Main : ModBehaviour
    {
        private static Main instance;
        public static Main Instance => instance;

        public static void FireOnNextUpdate(System.Action action) => Instance.ModHelper.Events.Unity.FireOnNextUpdate(action);

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
            {
                if (loadScene != OWScene.SolarSystem) return;

                QuantumMoon quantumMoon = Locator.GetQuantumMoon();

                GameObject orbitGO = new GameObject("Orbit_QM");
                orbitGO.transform.parent = quantumMoon.transform;
                orbitGO.transform.localPosition = Vector3.zero;
                orbitGO.AddComponent<QuantumMoonOrbitLine>();
            };
        }

        public static T FindResourceOfTypeAndName<T>(string name) where T : Object
        {
            T[] firstList = Resources.FindObjectsOfTypeAll<T>();

            for (var i = 0; i < firstList.Length; i++)
            {
                if (firstList[i].name == name)
                {
                    return firstList[i];
                }
            }

            return null;
        }
    }
}
