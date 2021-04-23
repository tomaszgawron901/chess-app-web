using ChessClassLibrary;
using ChessClassLibrary.enums;
using ChessClassLibrary.Pieces;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessBoardComponents
{
    public class PieceForView
    {
        public PieceColor PieceColor;
        public PieceType PieceType;
    }

    public enum BorderColor
    {
        None,
        Ok,
        Bad,
        Select
    }

    public enum BackgroudColor
    {
        Light,
        Dark,
        Red,
    }

    public class FieldComponentBase : ComponentBase
    {
        [Parameter]  public BackgroudColor BackgroudColor { get; set; }
        [Parameter] public BorderColor BorderColor { get; set; }
        [Parameter] public PieceForView Piece { get; set; }

        [Parameter] public EventCallback OnFieldClicked { get; set; }

        public void SetBorderColor(BorderColor color)
        {
            this.BorderColor = color;
        }


        protected string GetBackgroundColor()
        {
            switch (this.BackgroudColor)
            {
                case BackgroudColor.Light: return "papayawhip";
                case BackgroudColor.Dark: return "saddlebrown";
                case BackgroudColor.Red: return "red";
                default: return "white";
            }
        }

        protected string GetBorderColor()
        {
            switch (this.BorderColor)
            {
                case BorderColor.None: return "lightgrey";
                case BorderColor.Ok: return "limegreen";
                case BorderColor.Bad: return "red";
                case BorderColor.Select: return "black";
                default: return "white";
            }
        }

        protected async void OnFieldClick()
        {
            await this.OnFieldClicked.InvokeAsync();
        }
    }
}
