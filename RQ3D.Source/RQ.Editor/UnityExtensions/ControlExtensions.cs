using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace RQ.Editor.UnityExtensions
{
    public static class ControlExtensions
    {
        // TODO Refactor these too functions, too much copied code
        public static string Popup(string label, string value, IList<KeyValuePair<string, string>> items, params GUILayoutOption[] options)
        {
            //var SpawnPoints = new List<KeyValuePair<string, string>>();
            int searchIndex = 0;

            if (!String.IsNullOrEmpty(value))
            {
                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    if (value == item.Key)
                    {
                        searchIndex = i;
                        break;
                    }
                }
            }

            int newIndex;
            string[] searchItems = new string[items.Count];
            for (int i = 0; i < items.Count; i++)
            {
                searchItems[i] = items[i].Value;
            }
            //var searchItems = items.Select(i => i.Value).ToArray();
            if (String.IsNullOrEmpty(label))
                newIndex = EditorGUILayout.Popup(searchIndex, searchItems, options);
            else
                newIndex = EditorGUILayout.Popup(label, searchIndex, searchItems, options);

            var selectedKey = string.Empty;

            if (items.Count != 0)
            {
                selectedKey = items[newIndex].Key;
            }
            //string uniqueId = key == "-1" ? null : key;

            //agent.spawnPointUniqueId = uniqueId;
            return selectedKey;
        }

        public static string EditorGUIPopup(Rect position, string label, string value, IList<KeyValuePair<string, string>> items)
        {
            //var SpawnPoints = new List<KeyValuePair<string, string>>();
            int searchIndex = 0;

            if (!String.IsNullOrEmpty(value))
            {
                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    if (value == item.Key)
                    {
                        searchIndex = i;
                        break;
                    }
                }
            }

            int newIndex;
            string[] searchItems = new string[items.Count];
            for (int i = 0; i < items.Count; i++)
            {
                searchItems[i] = items[i].Value;
            }
            if (String.IsNullOrEmpty(label))
                newIndex = EditorGUI.Popup(position, searchIndex, searchItems);
            //newIndex = EditorGUI.Popup(position, searchIndex, items.Select(i => i.Value).ToArray());
            //newIndex = EditorGUI.Popup(searchIndex, items.Select(i => i.Value).ToArray(), options);
            else
                newIndex = EditorGUI.Popup(position, label, searchIndex, searchItems);
            //newIndex = EditorGUI.Popup(position, label, searchIndex, items.Select(i => i.Value).ToArray());

            var selectedKey = string.Empty;

            if (items.Count != 0)
            {
                selectedKey = items[newIndex].Key;
            }
            //string uniqueId = key == "-1" ? null : key;

            //agent.spawnPointUniqueId = uniqueId;
            return selectedKey;
        }

        public static TKey EditorGUIPopup<TKey>(Rect position, string label, TKey value, IList<KeyValuePair<TKey, string>> items)
        {
            //var SpawnPoints = new List<KeyValuePair<string, string>>();
            int searchIndex = 0;

            if (value != null)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    if (value.Equals(item.Key))
                    {
                        searchIndex = i;
                        break;
                    }
                }
            }

            int newIndex;
            string[] searchItems = new string[items.Count];
            for (int i = 0; i < items.Count; i++)
            {
                searchItems[i] = items[i].Value;
            }
            if (String.IsNullOrEmpty(label))
                newIndex = EditorGUI.Popup(position, searchIndex, searchItems);
            //newIndex = EditorGUI.Popup(position, searchIndex, items.Select(i => i.Value).ToArray());
            //newIndex = EditorGUI.Popup(searchIndex, items.Select(i => i.Value).ToArray(), options);
            else
                newIndex = EditorGUI.Popup(position, label, searchIndex, searchItems);
            //newIndex = EditorGUI.Popup(position, label, searchIndex, items.Select(i => i.Value).ToArray());

            TKey selectedKey = default(TKey);

            if (items.Count != 0)
            {
                selectedKey = items[newIndex].Key;
            }
            //string uniqueId = key == "-1" ? null : key;

            //agent.spawnPointUniqueId = uniqueId;
            return selectedKey;
        }
    }
}
