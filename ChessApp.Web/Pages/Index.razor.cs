using ChessApp.Web.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessClassLibrary.Games.ClassicGame;
using ChessApp.Web.Services;
using ChessClassLibrary.Models;

namespace ChessApp.Web.Pages
{
    public class IndexBase: ComponentBase
    {
        [Inject] protected NavigationManager AppNavigationManager { get; set; }
        [Inject] protected GameService GameService { get; set; }

        protected bool isFormDisabled { get; set; } = false;

        protected async void CreateNewGame(GameOptions gameOptions)
        {
            isFormDisabled = true;
            string gameCode = await GameService.CreateNewGameRoom(gameOptions);
            isFormDisabled = false;
            this.NavigateToGameRoom(gameCode);
        }

        private void NavigateToGameRoom(string gameCode)
        {
            AppNavigationManager.NavigateTo($"/game-room/{gameCode}");
        }
    }
}
