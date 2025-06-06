using UIExtensionPackage.UISystem.UI.Windows;
using UnityEngine;

namespace UIExtensionPackage.UISystem.UI.Samples.WindowSample
{
    public class OpenWindowButton : MonoBehaviour
    {
        public void OpenWindowA()
        {
            UIWindowManager.Instance.OpenWindow<WindowA>();
        }
    
        public void OpenWindowD()
        {
            UIWindowManager.Instance.OpenWindow<WindowD>();
        }
    }
}
