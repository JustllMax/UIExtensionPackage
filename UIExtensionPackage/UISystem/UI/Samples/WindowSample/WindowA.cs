using UIExtensionPackage.UISystem.UI.Windows;

namespace UIExtensionPackage.UISystem.UI.Samples.WindowSample
{
    public class WindowA : WindowTestBase
    {
        protected override void OpenAnotherWindow()
        {
            UIWindowManager.Instance.OpenWindow<WindowB>(this);
        }
    }
}
