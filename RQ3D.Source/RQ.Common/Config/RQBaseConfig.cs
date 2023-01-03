using RQ.Base.Attributes;
using System;
using UnityEngine;

namespace RQ.Base.Config
{
    public class RQBaseConfig : ScriptableObject, IRQConfig
    {
        [UniqueIdentifier]
        public string UniqueId;

        public string GetUniqueId()
        {
            return UniqueId;
        }
        //protected StateMachine _stateMachine;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}
