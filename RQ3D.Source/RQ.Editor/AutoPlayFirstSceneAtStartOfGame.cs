using RQ.Base.Global;
using RQ.Controller.Manager;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RQ.Editor
{
    [InitializeOnLoad]
    public class AutoPlayFirstSceneAtStartOfGame
    {
        static AutoPlayFirstSceneAtStartOfGame()
        {
            Debug.Log($"(AutoPlayFirstSceneAtStartOfGame) constructor called, scene count: {EditorBuildSettings.scenes.Length}");
            // Ensure at least one build scene exist.
            if (EditorBuildSettings.scenes.Length == 0)
                return;
            //var sceneSetup = GameObject.FindObjectOfType<RQ.SceneSetup>();
            //var activeScene = EditorSceneManager.GetActiveScene();
            //var activeSceneName = activeScene.name;
            //Debug.Log($"Active scene name: {sceneSetup?.SceneConfig?.name}");
            //GameDataController.Instance.AppStartScene = sceneSetup.SceneConfig;
            // Set Play Mode scene to first scene defined in build settings.
            var oldScene = EditorSceneManager.GetActiveScene();
            var oldScenePath = oldScene.path;
            //var sceneSetup = GameObject.FindObjectOfType<RQ.Controller.Manager.SceneSetup>();
            //Debug.Log($"Scene Config {sceneSetup.SceneConfig?.name}");
            //EditorSceneManager.LoadScene("Game Controller");
            //EditorSceneManager.GetAllScenes
            int countLoaded = SceneManager.sceneCount;
            //Scene[] loadedScenes = new Scene[countLoaded];
            bool IsGameControllerOpen = false;

            //for (int i = 0; i < countLoaded; i++)
            //{
            //    //loadedScenes[i] = SceneManager.GetSceneAt(i);
            //    var openScene = SceneManager.GetSceneAt(i);
            //    if (openScene.name == "Game Controller")
            //    {
            //        IsGameControllerOpen = true;
            //        break;
            //    }
            //}
            //if (!IsGameControllerOpen)
            //    EditorSceneManager.OpenScene("Game Controller", OpenSceneMode.Additive);
            //GlobalStatic.NextScene = sceneSetup.SceneConfig?.name;
            GlobalStatic.NextScene = oldScenePath;
            Debug.Log($"(AutoPlayFirstSceneAtStartOfGame) Setting next scene to {GlobalStatic.NextScene}");
            Debug.Log($"(AutoPlayFirstSceneAtStartOfGame) playModeStartScene = {EditorBuildSettings.scenes[0].path}");
            EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorBuildSettings.scenes[0].path);
        }
    }
}
