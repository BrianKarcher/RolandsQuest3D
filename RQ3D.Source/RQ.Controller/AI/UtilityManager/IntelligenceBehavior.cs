﻿using RQ.Common.Container;
using System;
using System.Collections;
using UnityEngine;

namespace UtilityManager
{
    [AddComponentMenu("RQ/Components/AI/Intelligence Behavior")]
    public class IntelligenceBehavior : MonoBehaviour
    {
        [SerializeField]
        private DecisionMakerAsset _dm;
        [SerializeField]
        private IntelligenceDef _intelligenceDef;
        [SerializeField]
        private float _tickDelay = 1f;
        [SerializeField]
        private string _currentDse;
        private bool _initialized = false;

        //public void Awake()
        //{
        //    _intelligenceDef.Init();
        //}

        // What?
        public void Init(IEntity repo)
        {
            _initialized = true;
            _intelligenceDef.Init(repo, EntityContainer.Instance, _dm.GetDecisionMaker());
        }

        public void RemoveTarget(int uniqueId)
        {
            _intelligenceDef.RemoveTarget(uniqueId);
        }

        public void OnEnable()
        {
            StartCoroutine(IntelligenceLoop());
        }

        public void OnDisable()
        {
            StopCoroutine(IntelligenceLoop());
        }

        IEnumerator IntelligenceLoop()
        {
            while (true)
            {
                try
                {
                    var isLocked = _intelligenceDef.IsCurrentDSELocked();
                    if (!isLocked && _initialized)
                    {
                        var dse = _intelligenceDef.ScoreAllDecisions();
                        if (dse != null)
                        {
                            var newDecision =_intelligenceDef.RunDecision(dse);
                            if (newDecision)
                                _currentDse = dse.GetDSE().Name;
                        }
                        else
                            Debug.LogWarning("Could not determine decision.");
                    }
                }
                catch(Exception ex)
                {
                    Debug.LogError(ex);
                }
                yield return new WaitForSeconds(_tickDelay);
            }
        }


        //IEnumerator Fade()
        //{
        //    for (float f = 1f; f >= 0; f -= 0.1f)
        //    {
        //        //Color c = renderer.material.color;
        //        //c.a = f;
        //        //renderer.material.color = c;
        //        yield return new WaitForSeconds(.1f);
        //    }
        //}
    }
}
