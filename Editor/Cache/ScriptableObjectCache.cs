using UnityEngine;

namespace ReadyPlayerMe.Editor.Cache
{
    public class ScriptableObjectCache<T> : ObjectCache<T> where T : ScriptableObject
    {
        private readonly string _name;
        private T _scriptableObject;
        
        public ScriptableObjectCache(string name = "") : base(name)
        {
            _name = name;
        }

        public T Init(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                Debug.LogError("Invalid Filename.");
                return null;
            }
      
            if(_scriptableObject == null)
            {
                _scriptableObject = Resources.Load<T>(fileName);
            }
            
            if (_scriptableObject != null)
                return _scriptableObject;

            var cache = new ObjectCache<T>(_name);
            _scriptableObject = ScriptableObject.CreateInstance<T>();
            cache.Save(_scriptableObject, fileName);
            return _scriptableObject;
        }
    }
}