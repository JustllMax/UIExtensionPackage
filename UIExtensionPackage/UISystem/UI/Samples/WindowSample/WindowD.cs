using UIExtensionPackage.UISystem.UI.Windows;

namespace UIExtensionPackage.UISystem.UI.Samples.WindowSample
{
    public class WindowD : WindowTestBase
    {
        protected override void OpenAnotherWindow()
        {
            UIWindowManager.Instance.OpenWindow<WindowE>(this);
        }
    }
}
