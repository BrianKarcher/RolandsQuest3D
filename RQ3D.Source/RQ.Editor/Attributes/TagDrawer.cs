using RQ.Base.Attributes;
using UnityEditor;
using UnityEngine;

namespace RQ.Editor
{
    // Place this file inside Assets/Editor
    [CustomPropertyDrawer(typeof(TagAttribute))]
    public class TagDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            // Generate a unique ID, defaults to an empty string if nothing has been serialized yet
            //if (prop.stringValue == "")
            //{
            //    Guid guid = Guid.NewGuid();
            //    prop.stringValue = guid.ToString();
            //}

            // Place a label so it can't be edited by accident
            Rect textFieldPosition = position;
            textFieldPosition.height = 16;
            DrawPopupField(textFieldPosition, prop, label);
        }

        private void DrawPopupField(Rect position, SerializedProperty prop, GUIContent label)
        {
            //int mask = 0;
            //int count = 1;
            //foreach (var tag in UnityEditorInternal.InternalEditorUtility.tags)
            //{
            //    mask = mask | mask << count;
            //    count++;
            //}
            //EditorGUI.Property
            prop.stringValue = EditorGUI.TagField(position, label, prop.stringValue);
            //int newMask = EditorGUI.MaskField(position, label, mask, 
            //    UnityEditorInternal.InternalEditorUtility.tags);
            //prop.
            //EditorGUI.LabelField(position, label, new GUIContent(prop.stringValue));
        }
    }
}
