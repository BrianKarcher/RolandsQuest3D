using RQ.Base.Config;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace RQ.Editor
{
    public class ConfigEditorBase<T> : UnityEditor.Editor where T : RQBaseConfig
    {
        protected T agent;

        protected virtual void OnEnable()
        {
            agent = target as T;
        }

        public void Dirty()
        {
            Dirty(true);
        }

        public void Dirty(bool makeSceneDirty)
        {
            EditorUtility.SetDirty(target);
            if (makeSceneDirty && !Application.isPlaying)
            {
                EditorApplication.MarkSceneDirty();
            }
        }

        protected static T CreateAsset(string fileName)
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            //string path = "Assets/Items/SpriteAnimations.asset";
            //if (Selection.activeObject != null)
            //{
            //    path = EditorUtility.GetAssetPath(Selection.activeGameObject) + "/SpriteAnimations.asset";
            //}
            T newAsset = ScriptableObject.CreateInstance<T>();
            //PopulateNewAsset(newAsset);
            //AssetDatabase.CreateAsset(sceneData, path);
            newAsset.UniqueId = Guid.NewGuid().ToString();
            AssetDatabase.CreateAsset(newAsset, AssetDatabase.GenerateUniqueAssetPath(path + "/" + fileName));
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = newAsset;
            return newAsset;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Generate UniqueId"))
            {
                agent.UniqueId = Guid.NewGuid().ToString();
            }
        }
    }
}
