using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RQ.Base.VariableClasses
{
    [Serializable]
    public class Variables
    {
        // Unity does not recognize dictionaries unfortanately
        [SerializeField]
        private List<StringVariable> StringVariables;
        [SerializeField]
        private List<BoolVariable> BoolVariables;
        [SerializeField]
        private List<FloatVariable> FloatVariables;
        [SerializeField]
        private List<IntVariable> IntVariables;
        [SerializeField]
        private List<Vector2Variable> Vector2Variables;
        [SerializeField]
        private List<Vector3Variable> Vector3Variables;

        public VariableDictOfType<string> StringVariablesDict;
        public VariableDictOfType<bool> BoolVariablesDict;
        public VariableDictOfType<float> FloatVariablesDict;
        public VariableDictOfType<int> IntVariablesDict;
        public VariableDictOfType<Vector2> Vector2VariablesDict;
        public VariableDictOfType<Vector3> Vector3VariablesDict;

        public void Init()
        {
            StringVariablesDict = new VariableDictOfType<string>(StringVariables.Cast<Variable<string>>());
            BoolVariablesDict = new VariableDictOfType<bool>(BoolVariables.Cast<Variable<bool>>());
            FloatVariablesDict = new VariableDictOfType<float>(FloatVariables.Cast<Variable<float>>());
            IntVariablesDict = new VariableDictOfType<int>(IntVariables.Cast<Variable<int>>());
            Vector2VariablesDict = new VariableDictOfType<Vector2>(Vector2Variables.Cast<Variable<Vector2>>());
            Vector3VariablesDict = new VariableDictOfType<Vector3>(Vector3Variables.Cast<Variable<Vector3>>());
        }

        //private Dictionary<string, string> StringVariablesDict;
        //private Dictionary<string, bool> BoolVariablesDict;
        //private Dictionary<string, float> FloatVariablesDict;
        //private Dictionary<string, int> IntVariablesDict;
        //private Dictionary<string, Vector2> Vector2VariablesDict;
        //private Dictionary<string, Vector3> Vector3VariablesDict;

        //public void Init()
        //{
        //    StringVariablesDict = new Dictionary<string, string>();
        //    for (int i = 0; i < StringVariables.Count; i++)
        //    {
        //        StringVariablesDict.Add(StringVariables[i].Name, StringVariables[i].Value);
        //    }

        //    BoolVariablesDict = new Dictionary<string, bool>();
        //    for (int i = 0; i < BoolVariables.Count; i++)
        //    {
        //        BoolVariablesDict.Add(BoolVariables[i].Name, BoolVariables[i].Value);
        //    }

        //    FloatVariablesDict = new Dictionary<string, float>();
        //    for (int i = 0; i < FloatVariables.Count; i++)
        //    {
        //        FloatVariablesDict.Add(FloatVariables[i].Name, FloatVariables[i].Value);
        //    }

        //    IntVariablesDict = new Dictionary<string, int>();
        //    for (int i = 0; i < IntVariables.Count; i++)
        //    {
        //        IntVariablesDict.Add(IntVariables[i].Name, IntVariables[i].Value);
        //    }

        //    Vector2VariablesDict = new Dictionary<string, Vector2>();
        //    for (int i = 0; i < Vector2Variables.Count; i++)
        //    {
        //        Vector2VariablesDict.Add(Vector2Variables[i].Name, Vector2Variables[i].Value);
        //    }

        //    Vector3VariablesDict = new Dictionary<string, Vector3>();
        //    for (int i = 0; i < Vector3Variables.Count; i++)
        //    {
        //        Vector3VariablesDict.Add(Vector3Variables[i].Name, Vector3Variables[i].Value);
        //    }
        //}

        //public string GetString(string name)
        //{
        //    return Get(StringVariablesDict, name);
        //}

        //public void SetString(string name, string value)
        //{
        //    Set(StringVariablesDict, name, value);
        //}

        //public bool GetBool(string name)
        //{
        //    return Get(BoolVariablesDict, name);
        //}

        //public void SetBool(string name, bool value)
        //{
        //    Set(BoolVariablesDict, name, value);
        //}

        //public float GetFloat(string name)
        //{
        //    return Get(FloatVariablesDict, name);
        //}

        //public int GetInt(string name)
        //{
        //    return Get(IntVariablesDict, name);
        //}

        //public void SetFloat(string name, float value)
        //{
        //    Set(FloatVariablesDict, name, value);
        //}

        //public void SetInt(string name, int value)
        //{
        //    Set(IntVariablesDict, name, value);
        //}

        //public Vector2 GetVector2(string name)
        //{
        //    return Get(Vector2VariablesDict, name);
        //}

        //public void SetVector2(string name, Vector2 value)
        //{
        //    Set(Vector2VariablesDict, name, value);
        //}

        ////public T Get<T>(string name)
        ////{

        ////}

        //public T Get<T>(Dictionary<string, T> dict, string name)
        //{
        //    if (!dict.TryGetValue(name, out T variable))
        //        throw new Exception($"Variable {name} not found");
        //    return variable;
        //}

        //public void Set<T>(Dictionary<string, T> dict, string name, T value)
        //{
        //    if (!dict.ContainsKey(name))
        //        dict.Add(name, value);
        //    else
        //        dict[name] = value;
        //}
    }

    public class VariableDictOfType<T>
    {
        //public List<Variable<T>> VariablesOfType;

        private Dictionary<string, T> VariablesDict;

        public VariableDictOfType(IEnumerable<Variable<T>> initVariables)
        {
            VariablesDict = new Dictionary<string, T>();
            if (initVariables == null)
                return;
            //var enumerator = initVariables.GetEnumerator();
            //while (enumerator.)
            //while ()
            foreach (var variable in initVariables)
            {
                VariablesDict.Add(variable.Name, variable.Value);
            }
        }

        //public void Init()
        //{

        //}

        public T Get(string name)
        {
            if (!VariablesDict.TryGetValue(name, out T variable))
                throw new Exception($"Variable {name} not found");
            return variable;
        }

        public void Set(string name, T value)
        {
            if (!VariablesDict.ContainsKey(name))
                VariablesDict.Add(name, value);
            else
                VariablesDict[name] = value;
        }
    }
}
