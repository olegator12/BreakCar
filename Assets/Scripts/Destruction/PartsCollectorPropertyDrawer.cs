#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Destruction
{
    [CustomPropertyDrawer(typeof(PartsCollector))]
    public class PartsCollectorPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUILayout.PropertyField(property);
            PartsCollector collector = (PartsCollector)property.objectReferenceValue;

            if (GUILayout.Button("Generate", GUILayout.ExpandWidth(true), GUILayout.Height(32)) == true)
            {
                collector.Generate();
                return;
            }

            if (GUILayout.Button("Clear", GUILayout.ExpandWidth(true), GUILayout.Height(32)) == false)
                return;

            collector.Clear();
        }
    }
}
#endif