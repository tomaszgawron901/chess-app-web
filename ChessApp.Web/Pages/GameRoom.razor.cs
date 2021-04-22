using ChessClassLibrary.Games.ClassicGame;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessApp.Web.Pages
{
    public class GameRoomBase: ComponentBase
    {
        [Inject] protected NavigationManager AppNavigationManager { get; set; }

        [Parameter] public string GameCode { get; set; }
        protected string JoinUrl;
        public ClassicGame Game = new ClassicGame();

        protected override void OnInitialized()
        {
            base.OnInitialized();
            this.JoinUrl = AppNavigationManager.Uri;

            // TODO check is game with given code exists
            // TODO GET game options
            // TODO connect to signalR server
            // TODO if two users are connected start game
        }
    }
}
