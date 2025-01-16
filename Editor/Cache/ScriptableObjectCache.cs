using UnityEngine;

namespace PlayerZero.Editor.Cache
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
            if (!string.IsNullOrEmpty(fileName))
            {
                var scriptableObject = Resources.Load<T>(fileName);

                if (scriptableObject != null)
                {
                    return scriptableObject;
                }
            }

            var cache = new ObjectCache<T>(_name);
            var newObject = ScriptableObject.CreateInstance<T>();
            cache.Save(newObject, fileName);
            return newObject;
        }
    }
}