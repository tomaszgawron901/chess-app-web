using ChessBoardComponents.Interops;
using ChessClassLibrary;
using ChessClassLibrary.Boards;
using ChessClassLibrary.Pieces;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessBoardComponents
{
    public class ChessBoardComponentBase : ComponentBase
    {
        public Position? selectedPosition;

        [Parameter] public int Width { get; set; } = 8;
        [Parameter] public int Height { get; set; } = 8;
        [Parameter] public bool IsRotated { get; set; } = false;

        [Parameter] public EventCallback<Position> OnFieldClicked { get; set; }
        [Parameter] public EventCallback IsReady { get; set; }

        public FieldComponent[,] Fields;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            Fields = new FieldComponent[Width, Height];
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {

                await this.IsReady.InvokeAsync();
            }
        }

        protected IEnumerable<int> GetXIndexes()
        {
            if (IsRotated)
            {
                for (int i = Width - 1; i >= 0; i--)
                {
                    yield return i;
                }
            }
            else
            {
                for (int i = 0; i < Width; i++)
                {
                    yield return i;
                }
            }
        }

        protected IEnumerable<int> GetYIndexes()
        {
            if (IsRotated)
            {
                for (int i = 0; i < Height; i++)
                {
                    yield return i;
                }
            }
            else
            {
                for (int i = Height - 1; i >= 0; i--)
                {
                    yield return i;
                }
            }
        }

        protected async void OnFieldClick(int x, int y)
        {
            await OnFieldClicked.InvokeAsync(new Position(x, y));
        }

        public void UnSelectAll()
        {
            foreach (FieldComponent field in this.Fields)
            {
                field.SetBorderColor(BorderColor.None);
            }
            this.selectedPosition = null;
        }

        public void SelectPosition(Position position)
        {
            this.Fields[position.x, position.y].SetBorderColor(BorderColor.Select);
            this.selectedPosition = position;
        }

        public void ShowMoves(IEnumerable<Position> positions)
        {
            foreach (Position position in positions)
            {
                this.Fields[position.x, position.y].SetBorderColor(BorderColor.Ok);
            }
        }
    }
}
