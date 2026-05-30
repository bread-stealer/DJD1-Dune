using UnityEngine;

[System.Serializable]
public class SceneRef
{
#if UNITY_EDITOR
    // Only used in the Editor to store the scene asset reference
    [SerializeField] private UnityEditor.SceneAsset sceneAsset;
#endif

    [SerializeField] private string sceneName;

    public string SceneName => sceneName;

#if UNITY_EDITOR
    public void OnValidate()
    {
        // Keep the scene name in sync with the assigned scene asset
        if (sceneAsset != null)
            sceneName = sceneAsset.name;
        else
            sceneName = string.Empty;
    }
#endif
}
