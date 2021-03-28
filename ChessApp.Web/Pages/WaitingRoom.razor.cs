using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ChessApp.Web.Pages
{
    public class WaitingRoomBase: ComponentBase
    {
        [Parameter] public string GameCode { get; set; }
        [Inject] protected NavigationManager AppNavigationManager { get; set; }
        protected string JoinUrl;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            this.JoinUrl = AppNavigationManager.Uri;

            // TODO check is game with given code exists
            // TODO GET game options
            // TODO connect to signalR server
            // TODO if two users are connected start game
        }

        protected void OnConfirmClicked()
        {
            // TODO Confirm 
        }

        protected void OnCancelClicked()
        {
            AppNavigationManager.NavigateTo("/");
        }
    }
}
