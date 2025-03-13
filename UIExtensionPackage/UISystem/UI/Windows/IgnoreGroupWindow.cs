namespace UIExtensionPackage.UISystem.UI.Windows
{
    /// <summary>
    /// Represents UI Windows that aren't affected by main Canvas Group in the hierarchy.
    /// </summary>
    public abstract class IgnoreGroupWindow : UIWindow
    {
        public override bool AllowMultiple { get; } = false;

        public override void OpenWindow()
        {
            base.OpenWindow();
            CanvasGroup.ignoreParentGroups = true;
        }
    }
}