using UnityEngine;

namespace  UIExtensionPackage.UISystem.Core.Generics
{
 
    /// <summary>
    /// Class for MonoBehaviours that aren't supposed to be destroyed on load and only one instance should exist.
    /// </summary>
    /// <remarks>They are not supposed to be singletons.</remarks>
    public abstract class PersistentGameObject: MonoBehaviour
    {
        protected virtual void Awake()
        {
            // Ensure that object is on the root level as Unity only allows root level objects to be made persistent.
            if (transform.parent)
            {
                transform.SetParent(null);
                Debug.LogWarning($"{gameObject.name} was not a root object, it has been moved to root level.");
            }
            DontDestroyOnLoad(gameObject);
        }
    }
}