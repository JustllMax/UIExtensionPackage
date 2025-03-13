using UnityEngine;

namespace UIExtensionPackage.UISystem.Core.Generics
{
    /// <summary>
    /// Class for MonoBehaviours that should be accessed from other classes
    /// </summary>
    public abstract class SingletonMonobehaviour<T> : MonoBehaviour 
        where T : Component
    {
        
        private static T _instance = null;

        public static T Instance
        {
            get
            {
                if (!_instance) _instance = FindFirstObjectByType<T>();
                // if (!_instance) _instance = FindObjectOfType<T>(); // Use this if on an older Unity version
                return _instance;
            }
        }

        protected virtual void Awake()
        {
           
            if (_instance == null)
            {
                _instance = this as T;
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        protected virtual void OnDestroyObject()
        {
            
        }
        protected void OnDestroy()
        {
            OnDestroyObject();
            
            if (_instance == this)
                _instance = null;
        }
    }
}