using RQ.Base.Config;
using UnityEngine;
using RQ.Base.Item;
using System.Collections.Generic;

namespace RQ.Controller.Scene
{
    public class SceneConfig : RQBaseConfig
    {
        /// <summary>
        /// Pointer to the Scene file (.unity)
        /// </summary>
        public UnityEngine.Object Scene;

        [HideInInspector]
        public string SceneName;

        [SerializeField]
        public ItemDesc[] StartingItems;

        [SerializeField]
        public List<SpawnPointConfig> SpawnPoints;
    }
}
