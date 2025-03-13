using UnityEngine;
using UnityEngine.UI;

namespace UIExtensionPackage.UISystem.UI.Windows
{
    /// <summary>
    /// Class for windows that aren't supposed to be closed by anything except its own exit button
    /// or call to Close method from inside the window.
    /// </summary>
    public abstract class IgnoreCloseWindow : UIWindow
    {
        [SerializeField] private Button closeButton;
        public override bool AllowMultiple { get; } = false;

        public override void SetUp()
        {
            base.SetUp();
            
            if (closeButton)
                closeButton.onClick.AddListener(Close);
            else
                Debug.LogWarning($"{gameObject.name} does not have a close button reference.");
        }
    }
}