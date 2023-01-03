using RQ.Base.Config;
using RQ.Base.Skill;
using RQ.Base.SpawnPoint;
using RQ.Controller.Manager;
using RQ.Controller.Scene;
using RQ.Editor.GUIStyles;
using RQ.Editor.UnityExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RQ.Editor.Skill
{
    [CustomEditor(typeof(SceneConfig), true)]
    public class SceneConfigEditor : ConfigEditorBase<SceneConfig>
    {
        private SceneSetup _sceneSetup;
        private bool _showSpawnPoints = false;

        [MenuItem("Assets/Create/RQ/Scene Config")]
        public static void CreateNewAsset()
        {
            var newAsset = CreateAsset("NewScene.asset");
            //var gameConfig = Utils.FindAssetsByType<GameConfig>()[0];
            //if (!gameConfig.SceneConfigs.Contains(baseAgent as SceneConfig))
            //{
            //    gameConfig.SceneConfigs.Add(baseAgent as SceneConfig);
            //    EditorUtility.SetDirty(gameConfig);
            //}
            //gameConfig.Assets.Add(newAsset);
            //EditorUtility.SetDirty(gameConfig);

            //SceneConfig sceneData = ScriptableObject.CreateInstance<SceneConfig>();
            //AssetDatabase.CreateAsset(sceneData, "Assets/Areas/NewScene.asset");
            //EditorUtility.FocusProjectWindow();
            //Selection.activeObject = sceneData;
        }

        //public void OnSceneGUI()
        //{
        //    var ev = Event.current;
        //}

        public override void OnInspectorGUI()
        {
            GUI.changed = false;
            //var areaConfigGUIDs = AssetDatabase.FindAssets("t:AreaConfig");
            //bool isFoundInArea = false;
            //foreach (var guid in areaConfigGUIDs)
            //{
            //    var path = AssetDatabase.GUIDToAssetPath(guid);
            //    var areaConfig = AssetDatabase.LoadAssetAtPath<AreaConfig>(path);
            //    if (areaConfig.Scenes.Contains(agent))
            //    {
            //        isFoundInArea = true;
            //        break;
            //    }
            //}

            //if (!isFoundInArea)
            //    EditorGUILayout.HelpBox("Not in an Area Config. Must be added to an Area Config.", MessageType.Error);

            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();
            var oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(agent.SceneName);
            var newScene = EditorGUILayout.ObjectField("Scene", oldScene, typeof(SceneAsset), false) as SceneAsset;

            //agent.SceneName = newScene.

            if (GUILayout.Button("Add/Enable in Build Settings"))
            {
                // Find valid Scene paths and make a list of EditorBuildSettingsScene
                List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
                var editorBuildScene = editorBuildSettingsScenes.FirstOrDefault(i => i.path == agent.SceneName);
                if (editorBuildScene == null)
                {
                    editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(agent.SceneName, true));

                }
                else
                {
                    editorBuildScene.enabled = true;
                }
                EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
                //foreach (var sceneAsset in editorBuildSettingsScenes)
                //{
                //    string scenePath = AssetDatabase.GetAssetPath(sceneAsset);
                //    if (!string.IsNullOrEmpty(scenePath))

                //}

                // Set the Build Settings window Scene list

                //SceneManager.
            }

            if (GUILayout.Button("Disable in Build Settings"))
            {
                // Find valid Scene paths and make a list of EditorBuildSettingsScene
                List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
                var editorBuildScene = editorBuildSettingsScenes.FirstOrDefault(i => i.path == agent.SceneName);
                if (editorBuildScene != null)
                {
                    editorBuildScene.enabled = false;
                }
                EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
            }

            agent.SceneName = EditorGUILayout.TextField("Scene Name", agent.SceneName ?? string.Empty);

            //_variablesEditor.OnInspectorGUI(agent.Variables);

            _showSpawnPoints = GUILayout.Toggle(_showSpawnPoints, "Spawn Points", "Button");

            var spawnPoints = agent.SpawnPoints;

            if (_showSpawnPoints)
            {
                if (GUILayout.Button("Add Spawn Point", GUILayout.Width(130)))
                {
                    //spriteAnimations.Add(new Animation.Common.SpriteAnimation());
                    var newSpawnPoint = new SpawnPointConfig(); //KeyValuePair<string, List<SpriteAnimation>>(string.Empty, new List<SpriteAnimation>());
                    newSpawnPoint.UniqueId = Guid.NewGuid().ToString();
                    //newType.SpriteAnimations = new List<SpriteAnimation>();
                    //spriteAnimationTypes.Add(newType);
                    spawnPoints.Add(newSpawnPoint);
                }

                int removeSpawnpointIndex = -1;

                for (int i = 0; i < spawnPoints.Count; i++)
                {
                    var spawnPoint = spawnPoints[i];
                    GUILayout.Space(4.0f);
                    GUILayout.BeginVertical(Styles.CreateInspectorStyle());
                    EditorGUILayout.BeginHorizontal();
                    spawnPoint.Name = EditorGUILayout.TextField("Name", spawnPoint.Name);
                    if (GUILayout.Button("", Styles.CreateTilemapDeleteItemStyle()))
                    {
                        if (EditorUtility.DisplayDialog("Remove spawn point?", "Remove spawn point " + spawnPoint.Name + "?", "Yes", "No"))
                        {
                            removeSpawnpointIndex = i;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.LabelField("Unique Id", spawnPoint.UniqueId);
                    //spawnPoint.SceneCameFrom = EditorGUILayout.ObjectField("Scene came from", spawnPoint.SceneCameFrom, typeof(SceneConfig)) as SceneConfig;
                    spawnPoint.IsInitialSpawnPoint = EditorGUILayout.Toggle("Initial Spawn Point", spawnPoint.IsInitialSpawnPoint);
                    spawnPoint.ExtraInfo = EditorGUILayout.TextField("Extra Info", spawnPoint.ExtraInfo);

                    GUILayout.EndVertical();
                }

                if (removeSpawnpointIndex != -1)
                    spawnPoints.RemoveAt(removeSpawnpointIndex);
            }

            if (EditorGUI.EndChangeCheck())
            {
                var newPath = AssetDatabase.GetAssetPath(newScene);
                //var scenePathProperty = serializedObject.FindProperty("scenePath");
                //scenePathProperty.stringValue = newPath;
                agent.SceneName = newPath;
            }

            if (GUI.changed)
            {
                Dirty(false);
            }
        }
    }
}
