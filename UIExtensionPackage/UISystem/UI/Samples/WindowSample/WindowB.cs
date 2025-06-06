using UIExtensionPackage.UISystem.UI.Windows;

namespace UIExtensionPackage.UISystem.UI.Samples.WindowSample
{
    public class WindowB : WindowTestBase
    {
        protected override void OpenAnotherWindow()
        {
            UIWindowManager.Instance.OpenWindow<WindowC>(this);
        }
    }
}
