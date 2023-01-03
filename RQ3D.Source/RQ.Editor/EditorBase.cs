using RQ.Base.Config;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace RQ.Editor
{
    public class EditorBase<T> : UnityEditor.Editor where T : class
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
    }
}
