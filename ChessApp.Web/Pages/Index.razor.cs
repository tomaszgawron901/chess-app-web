using ChessApp.Web.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessClassLibrary.Games.ClassicGame;

namespace ChessApp.Web.Pages
{
    public class IndexBase: ComponentBase
    {
        [Inject] protected NavigationManager AppNavigationManager { get; set; }
        protected void CreateNewGame(CreateGameOptions gameOptions)
        {
            // TODO create new game and GET game code;
            string gameCode = "EXAMPLE_GAME_CODE";
            this.NavigateToWaitingRoom(gameCode);
        }

        private void NavigateToWaitingRoom(string gameCode)
        {
            AppNavigationManager.NavigateTo($"/game-room/{gameCode}");
        }
    }
}
