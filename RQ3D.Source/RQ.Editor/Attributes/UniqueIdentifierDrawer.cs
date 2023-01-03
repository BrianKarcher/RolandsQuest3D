using RQ.Base.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace RQ.Editor.Attributes
{
    // Place this file inside Assets/Editor
    [CustomPropertyDrawer(typeof(UniqueIdentifierAttribute))]
    public class UniqueIdentifierDrawer : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            // Generate a unique ID, defaults to an empty string if nothing has been serialized yet
            if (prop.stringValue == "")
            {
                Guid guid = Guid.NewGuid();
                prop.stringValue = guid.ToString();
            }

            // Place a label so it can't be edited by accident
            Rect textFieldPosition = position;
            textFieldPosition.height = 16;
            DrawLabelField(textFieldPosition, prop, label);
        }

        void DrawLabelField(Rect position, SerializedProperty prop, GUIContent label)
        {
            //GUI.enabled = false;
            EditorGUI.TextField(position, "UniqueId", prop.stringValue);
            //EditorGUI.LabelField(position, label, new GUIContent(prop.stringValue));
            //GUI.enabled = true;
        }
    }
}
