using UnityEngine;

namespace ReadyPlayerMe.Editor.Cache
{
    public class ScriptableObjectCache<T> : ObjectCache<T> where T : ScriptableObject
    {
        private readonly string _name;

        public ScriptableObjectCache(string name = "") : base(name)
        {
            _name = name;
        }

        public T Init(string fileName)
        {
            var scriptableObject = Resources.Load<T>(fileName);

            if (scriptableObject != null)
                return scriptableObject;

            var cache = new ObjectCache<T>(_name);
            scriptableObject = ScriptableObject.CreateInstance<T>();
            cache.Save(scriptableObject, fileName);
            return scriptableObject;
        }
    }
}