#if ODIN_INSPECTOR
using Hlight.Serialization.Serializer;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Hlight.Serialization.InspectorSerializationTool.Editor
{
    public class ShowInspectorSerializationToolAttributeOdinDrawer<T>
        : OdinAttributeDrawer<ShowInspectorSerializationToolAttribute, T>
    {
        private PropertyTree _propertyTree;

        protected override void Initialize()
        {
            base.Initialize();
            _propertyTree ??= PropertyTree.Create(new Tool(ValueEntry));
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            CallNextDrawer(label);
            if (!Property.State.Expanded) return;

            EditorGUI.indentLevel++;
            _propertyTree.Draw(false);
            EditorGUI.indentLevel--;
        }

        private class Tool
        {
            private readonly IPropertyValueEntry<T> _valueEntry;

            public Tool(IPropertyValueEntry<T> valueEntry)
            {
                _valueEntry = valueEntry;
            }

            [HorizontalGroup]
            [Button(ButtonSizes.Medium, ButtonStyle.FoldoutButton)]
            private string Serialize(ASerializationAsset serializer)
            {
                if (serializer == null)
                {
                    Debug.LogWarning($"[{nameof(ShowInspectorSerializationToolAttribute)}] Assign a serializer asset first.");
                    return string.Empty;
                }

                var value = serializer.Serialize(_valueEntry.SmartValue);
                EditorGUIUtility.systemCopyBuffer = value;
                Debug.Log($"[{nameof(ShowInspectorSerializationToolAttribute)}] Copied to clipboard.");
                return value;
            }

            [HorizontalGroup]
            [Button(ButtonSizes.Medium, ButtonStyle.FoldoutButton)]
            private void Deserialize(ASerializationAsset serializer, string json)
            {
                if (serializer == null)
                {
                    Debug.LogWarning($"[{nameof(ShowInspectorSerializationToolAttribute)}] Assign a serializer asset first.");
                    return;
                }

                _valueEntry.SmartValue = serializer.Deserialize<T>(json);
                _valueEntry.ApplyChanges();
            }
        }
    }
}
#endif
