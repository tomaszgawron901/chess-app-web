using Microsoft.AspNetCore.Components;

namespace ChessApp.Web.Exstentions
{
    public static class AppNavigationExtensions
    {
        public static void NavigateToMainPage(this NavigationManager nav)
        {
            nav.NavigateTo($"/");
        }

        public static void NavigateToGameRoom(this NavigationManager nav, string roomKey)
        {
            nav.NavigateTo($"/game-room/{roomKey}");
        }
    }
}
