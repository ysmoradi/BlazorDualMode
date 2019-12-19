namespace BlazorDualMode.Web.Shared
{
    public partial class NavMenu
    {
        bool collapseNavMenu = true;

        string NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }
    }
}
