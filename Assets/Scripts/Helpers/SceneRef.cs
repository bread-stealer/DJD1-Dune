using UnityEngine;

[System.Serializable]
public class SceneRef
{
#if UNITY_EDITOR
    [SerializeField] private UnityEditor.SceneAsset sceneAsset;
#endif

    [SerializeField] private string sceneName;

    public string SceneName => sceneName;

#if UNITY_EDITOR
    public void OnValidate()
    {
        if (sceneAsset != null)
            sceneName = sceneAsset.name;
        else
            sceneName = string.Empty;
    }
#endif
}
