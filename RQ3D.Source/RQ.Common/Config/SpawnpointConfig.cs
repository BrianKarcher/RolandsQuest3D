using RQ.Base.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RQ.Base.Config
{
    [Serializable]
    public class SpawnPointConfig
    {
        [SerializeField]
        public string Name;
        //private string _name;
        //public string Name { get { return _name; } set { _name = value; } }

        [SerializeField]
        [UniqueIdentifier]
        //[HideInInspector]
        public string UniqueId;

        //public SceneConfig SceneCameFrom;
        //public int SpawnPointInsanceId;
        ////public Vector3 Point;
        public bool IsInitialSpawnPoint;
        //public string Name;
        public string ExtraInfo;
    }
}
