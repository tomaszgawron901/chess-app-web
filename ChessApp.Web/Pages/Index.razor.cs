using Microsoft.AspNetCore.Components;
using ChessApp.Web.Services;
using ChessClassLib.Models;

namespace ChessApp.Web.Pages
{
    public partial class Index: ComponentBase
    {
        [Inject] protected NavigationManager AppNavigationManager { get; set; }
        [Inject] protected GameService GameService { get; set; }

        protected bool isLoading { get; set; } = false;
        protected bool isError { get; set; } = false;

        protected async void CreateNewGame(GameOptions gameOptions)
        {
            isLoading = true;
            isError = false;
            try
            {
                string gameCode = await GameService.CreateNewGameRoom(gameOptions);
                this.NavigateToGameRoom(gameCode);
            }
            catch
            {
                isError = true;
            }
            finally
            {
                isLoading = false;
                this.StateHasChanged();
            }
        }

        private void NavigateToGameRoom(string gameCode)
        {
            AppNavigationManager.NavigateTo($"/game-room/{gameCode}");
        }
    }
}
