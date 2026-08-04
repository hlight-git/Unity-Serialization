#if !ODIN_INSPECTOR
using Hlight.Serialization.Serializer;
using UnityEditor;
using UnityEngine;

namespace Hlight.Serialization.InspectorSerializationTool.Editor
{
    /// <summary>
    /// IMGUI fallback used when Odin Inspector is absent. Draws the property normally, then a
    /// serializer slot plus Serialize/Deserialize controls underneath.
    /// </summary>
    [CustomPropertyDrawer(typeof(ShowInspectorSerializationToolAttribute))]
    public class ShowInspectorSerializationToolAttributeDrawer : PropertyDrawer
    {
        private const float Pad = 2f;

        // ponytail: one drawer instance is reused across array elements, so the serializer slot
        // and the text buffer are shared between them. Move to a per-propertyPath dictionary if
        // that ever matters.
        private ASerializationAsset _serializer;
        private string _text = string.Empty;

        private static float Line => EditorGUIUtility.singleLineHeight;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = EditorGUI.GetPropertyHeight(property, label, true);
            if (property.isExpanded) height += (Line + Pad) * 3;
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var propertyHeight = EditorGUI.GetPropertyHeight(property, label, true);
            var rect = new Rect(position.x, position.y, position.width, propertyHeight);
            EditorGUI.PropertyField(rect, property, label, true);

            if (!property.isExpanded) return;

            EditorGUI.indentLevel++;
            rect.y += propertyHeight + Pad;
            rect.height = Line;
            _serializer = (ASerializationAsset)EditorGUI.ObjectField(
                rect, "Serializer", _serializer, typeof(ASerializationAsset), false);

            rect.y += Line + Pad;
            _text = EditorGUI.TextField(rect, "Value", _text);

            rect.y += Line + Pad;
            var buttons = EditorGUI.IndentedRect(rect);
            var half = (buttons.width - Pad) / 2f;

            if (GUI.Button(new Rect(buttons.x, buttons.y, half, Line), "Serialize"))
                Serialize(property);

            using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(_text)))
            {
                if (GUI.Button(new Rect(buttons.x + half + Pad, buttons.y, half, Line), "Deserialize"))
                    Deserialize(property);
            }

            EditorGUI.indentLevel--;
        }

        private void Serialize(SerializedProperty property)
        {
            var value = fieldInfo.GetValue(property.serializedObject.targetObject);
            _text = _serializer ? _serializer.Serialize(value) : JsonUtility.ToJson(value);
            EditorGUIUtility.systemCopyBuffer = _text;
            Debug.Log($"[{nameof(ShowInspectorSerializationToolAttribute)}] Copied to clipboard.");
        }

        private void Deserialize(SerializedProperty property)
        {
            var target = property.serializedObject.targetObject;
            var value = _serializer
                ? _serializer.Deserialize(_text, fieldInfo.FieldType)
                : JsonUtility.FromJson(_text, fieldInfo.FieldType);

            Undo.RecordObject(target, "Deserialize Inspector Value");
            fieldInfo.SetValue(target, value);
            EditorUtility.SetDirty(target);
            property.serializedObject.Update();
        }
    }
}
#endif
