using UnityEngine;

namespace UIExtensionPackage.UISystem.Core.Generics
{
    /// <summary>
    /// Represents a class that is not supposed get destroyed on load, as well as be accessed from other classes.
    /// </summary>
    public class PersistentSingleton<T> : SingletonMonobehaviour<T>
        where T : PersistentSingleton<T>
    {
        protected override void Awake()
        {
            // Ensure that object is on the root level as Unity only allows root level objects to be made persistent.
            if (transform.parent)
            {
                transform.SetParent(null);
                Debug.LogWarning($"{gameObject.name} was not a root object, it has been moved to root level.");
            }
            // Mark this object as DontDestroyOnLoad
            DontDestroyOnLoad(gameObject);
            // In case of a duplicate, it will get destroyed in base.Awake()
            base.Awake();

        }
    }
}