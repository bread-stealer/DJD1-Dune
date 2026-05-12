using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SortingLayerSelectorAttribute))]
public class SortingLayerDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.Integer)
        {
            EditorGUI.HelpBox(position, "SortingLayerSelector attribute requires an int field.", MessageType.Error);
            return;
        }

        string[] layerNames = new string[SortingLayer.layers.Length];
        int[] layerIDs = new int[SortingLayer.layers.Length];

        for (int i = 0; i < SortingLayer.layers.Length; i++)
        {
            layerNames[i] = SortingLayer.layers[i].name;
            layerIDs[i] = SortingLayer.layers[i].id;
        }

        int currentIndex = System.Array.IndexOf(layerIDs, property.intValue);
        if (currentIndex < 0) currentIndex = 0;

        EditorGUI.BeginChangeCheck();
        int selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, layerNames);
        if (EditorGUI.EndChangeCheck())
            property.intValue = layerIDs[selectedIndex];
    }
}
