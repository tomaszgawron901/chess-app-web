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
        [Parameter] public CreateGameOptions GameOptions { get; set; } = new CreateGameOptions();
        [Parameter] public bool IsDisabled { get; set; } = false;
        [Parameter] public EventCallback<CreateGameOptions> OnValidSubmit { get; set; } = new EventCallback<CreateGameOptions>();
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
