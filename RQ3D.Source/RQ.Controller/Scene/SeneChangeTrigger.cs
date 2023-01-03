using RQ.Base.Attributes;
using RQ.Base.Global;
using RQ.Base.Manager;
using RQ.Common.Container;
using RQ.Controller.Manager;
using RQ.Controller.Scene;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RQ.Controller
{
    [AddComponentMenu("RQ/Components/Scene Change Trigger")]
    public class SeneChangeTrigger : MonoBehaviour
    {
        /// <summary>
        /// Scene config to change to.
        /// </summary>
        [SerializeField]
        private SceneConfig _sceneConfig;
        public SceneConfig SceneConfig { get => _sceneConfig; set => _sceneConfig = value; }

        [SerializeField]
        private string _spawnPointUniqueId;
        public string SpawnPointUniqueId { get => _spawnPointUniqueId; set => _spawnPointUniqueId = value; }

        [Tag]
        [SerializeField]
        private string _tag;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag != _tag)
                return;
            Debug.Log($"Changing scenes to {_sceneConfig.SceneName} - {_spawnPointUniqueId}");
            GameStateController.Instance.LoadScene(_sceneConfig.SceneName, _spawnPointUniqueId);
        }

    }
}
