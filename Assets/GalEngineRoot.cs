using GalEngine.Core;
using UnityEditor;
using UnityEngine;

public class GalEngineRoot : MonoBehaviour
{
    private GalEngineCore core;
    public TextAsset script;

    private void Awake()
    {
        core = new GalEngineCore();
        core.Initialize(AssetDatabase.GetAssetPath(script));
    }

    private void Update()
    {
        if (core != null && core.IsInitialized)
            core.Update();
    }
}
