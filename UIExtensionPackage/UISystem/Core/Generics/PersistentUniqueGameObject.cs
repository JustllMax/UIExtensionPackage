using UnityEngine;

namespace UIExtensionPackage.UISystem.Core.Generics
{
    /// <summary>
    /// Represents a persistent game object, that there should exist only 1 instance of, with no access from outside classes.
    /// </summary>
    public class PersistentUniqueGameObject<T> : PersistentGameObject
        where T : PersistentUniqueGameObject<T>
    {
        protected override void Awake()
        {

            if (IsDontDestroyValid())
                base.Awake();
        }

        /// <summary>
        /// Method looks for objects of the same 
        /// </summary>
        /// <returns></returns>
        private bool IsDontDestroyValid()
        {
            var objects = FindObjectsByType<T>(FindObjectsSortMode.None);
            // var objects = FindObjectsOfType<T>(); // Use this if on an older Unity version
            if (objects.Length > 1)
            {
                gameObject.SetActive(false);
                DestroyImmediate(gameObject);
                return false;
            }

            return true;
        }
    }
}