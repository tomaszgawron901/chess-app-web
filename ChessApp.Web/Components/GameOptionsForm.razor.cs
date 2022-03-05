using ChessApp.Web.Enums;
using ChessClassLib.Enums;
using ChessClassLib.Models;
using Microsoft.AspNetCore.Components;
using System;

namespace ChessApp.Web.Components
{
    public partial class GameOptionsForm: ComponentBase
    {
        [Parameter] public GameOptions GameOptions { get; set; } = new GameOptions();
        [Parameter] public bool IsDisabled { get; set; } = false;
        [Parameter] public EventCallback<GameOptions> OnValidSubmit { get; set; } = new EventCallback<GameOptions>();
        [Parameter] public EventCallback OnCancel { get; set; } = new EventCallback();

        protected Side Side { get; set; }

        protected void HandleValidSubmit()
        {
            if (this.Side == Side.White)
            {
                this.GameOptions.Side = PieceColor.White;
            }
            else if(this.Side == Side.Black)
            {
                this.GameOptions.Side = PieceColor.Black;
            }
            else
            {
                if(new Random().Next(2) == 0)
                {
                    this.GameOptions.Side = PieceColor.White;
                } else
                {
                    this.GameOptions.Side = PieceColor.Black;
                }
            }
            this.OnValidSubmit.InvokeAsync(this.GameOptions);
        }
    }
}
