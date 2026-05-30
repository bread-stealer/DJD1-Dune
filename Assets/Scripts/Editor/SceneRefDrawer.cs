#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SceneRef))]
public class SceneRefDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty sceneAssetProp = property.FindPropertyRelative("sceneAsset");
        SerializedProperty sceneNameProp = property.FindPropertyRelative("sceneName");

        // Draw the scene asset field
        EditorGUI.BeginChangeCheck();
        Object sceneAsset = EditorGUI.ObjectField(position, label, sceneAssetProp.objectReferenceValue, typeof(SceneAsset), false);

        if (EditorGUI.EndChangeCheck())
        {
            sceneAssetProp.objectReferenceValue = sceneAsset;

            // Keep scene name in sync
            if (sceneAsset != null)
                sceneNameProp.stringValue = sceneAsset.name;
            else
                sceneNameProp.stringValue = string.Empty;
        }

        EditorGUI.EndProperty();
    }
}
#endif
