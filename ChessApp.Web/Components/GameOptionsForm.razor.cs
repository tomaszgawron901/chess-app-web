using ChessApp.Web.Enums;
using ChessApp.Web.Models;
using ChessClassLib.Enums;
using Microsoft.AspNetCore.Components;
using System;

namespace ChessApp.Web.Components
{
    public partial class GameOptionsForm: ComponentBase
    {
        [Parameter] public CreateGameOptions GameOptions { get; set; } = new CreateGameOptions();
        [Parameter] public bool IsDisabled { get; set; } = false;
        [Parameter] public EventCallback<CreateGameOptions> OnValidSubmit { get; set; } = new EventCallback<CreateGameOptions>();
        [Parameter] public EventCallback OnCancel { get; set; } = new EventCallback();

        protected Side Side { get; set; }

        protected void HandleValidSubmit()
        {
            if (Side == Side.White)
            {
                GameOptions.Side = PieceColor.White;
            }
            else if(Side == Side.Black)
            {
                GameOptions.Side = PieceColor.Black;
            }
            else
            {
                if(new Random().Next(2) == 0)
                {
                    GameOptions.Side = PieceColor.White;
                } else
                {
                    GameOptions.Side = PieceColor.Black;
                }
            }
            OnValidSubmit.InvokeAsync(GameOptions);
        }
    }
}
