using ChessApp.Web.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessApp.Web.Components
{
    public class GameOptionsFormBase: ComponentBase
    {
        public GameOptions GameOptions  = new GameOptions();
        [Parameter] public EventCallback<GameOptions> OnValidSubmit { get; set; } = new EventCallback<GameOptions>();

        protected void HandleValidSubmit()
        {
            Console.WriteLine(GameOptions.SecondsPerSide);
            this.OnValidSubmit.InvokeAsync(this.GameOptions);
        }

        protected void HandleCancelClicked()
        {
            Console.WriteLine("can");
        }
    }
}
