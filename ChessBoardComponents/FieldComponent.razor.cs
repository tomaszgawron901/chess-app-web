using ChessClassLibrary;
using ChessClassLibrary.Pieces;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessBoardComponents
{
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
        [CascadingParameter]
        public ChessBoardComponent BoardComponent { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public Position Position { get; set; }


        [Parameter]
        public BackgroudColor BackgroudColor { get; set; }

        [Parameter]
        public BorderColor BorderColor { get; set; }

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
                case BorderColor.Select: return "dimgrey";
                default: return "white";
            }
        }
    }
}
