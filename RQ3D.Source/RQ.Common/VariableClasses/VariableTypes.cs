using System;
using System.Collections.Generic;
using UnityEngine;

namespace RQ.Base.VariableClasses
{
    [Serializable]
    public class StringVariable : Variable<string> { }
    [Serializable]
    public class BoolVariable : Variable<bool> { }
    [Serializable]
    public class FloatVariable : Variable<float> { }
    [Serializable]
    public class IntVariable : Variable<int> { }
    [Serializable]
    public class Vector2Variable : Variable<Vector2> { }
    [Serializable]
    public class Vector3Variable : Variable<Vector3> { }

    [Serializable]
    public class Variable<T>
    {
        public string Name;
        public T Value;
        //public string UniqueId;
        //public StatusPersistenceLength Persistence;

        public Variable<T> Clone()
        {
            return new Variable<T>()
            {
                Name = Name,
                Value = Value
                //UniqueId = UniqueId
                //Persistence = Persistence
            };
        }
    }
}
