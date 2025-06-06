using UIExtensionPackage.UISystem.UI.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace UIExtensionPackage.UISystem.UI.Samples.WindowSample
{
    public abstract class WindowTestBase : UIWindow
    {
        [SerializeField] Button _closeButton;
        [SerializeField] Button _openButton;

        public override bool AllowMultiple { get; } = false;
        public override void SetUp()
        {
            base.SetUp();
            _closeButton?.onClick.AddListener(Close); 
            _openButton?.onClick.AddListener(OpenAnotherWindow);
        }

        protected abstract void OpenAnotherWindow();
    }
}
