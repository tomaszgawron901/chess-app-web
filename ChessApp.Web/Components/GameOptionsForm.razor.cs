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
        [Parameter] public GameOptions GameOptions { get; set; } = new GameOptions();
        [Parameter] public bool IsDisabled { get; set; } = false;
        [Parameter] public EventCallback<GameOptions> OnValidSubmit { get; set; } = new EventCallback<GameOptions>();
        [Parameter] public EventCallback OnCancel { get; set; } = new EventCallback();

        protected void HandleValidSubmit()
        {
            this.OnValidSubmit.InvokeAsync(this.GameOptions);
        }

        protected void HandleCancelClicked()
        {
            this.OnCancel.InvokeAsync();
        }
    }
}
