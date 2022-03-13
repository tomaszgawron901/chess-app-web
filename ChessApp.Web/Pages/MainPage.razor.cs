using Microsoft.AspNetCore.Components;
using ChessApp.Web.Services;
using ChessApp.Web.Exstentions;
using ChessApp.Web.Models;

namespace ChessApp.Web.Pages
{
    public partial class MainPage: ComponentBase
    {
        [Inject] protected NavigationManager AppNavigationManager { get; set; }
        [Inject] protected GameHubService GameService { get; set; }

        protected bool isLoading { get; set; } = false;
        protected bool isError { get; set; } = false;

        protected async void CreateNewGame(CreateGameOptions gameOptions)
        {
            isLoading = true;
            isError = false;
            try
            {
                string roomCode = await GameService.CreateNewGameRoom(gameOptions);
                AppNavigationManager.NavigateToGameRoom(roomCode);
            }
            catch
            {
                isError = true;
            }
            isLoading = false;
            StateHasChanged();
        }
    }
}
