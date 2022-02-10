using ChessBoardComponents.Interops;
using ChessClassLibrary;
using ChessClassLibrary.Boards;
using ChessClassLibrary.Models;
using ChessClassLibrary.Pieces;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessBoardComponents
{
    public partial class ChessBoardComponent : ComponentBase
    {
        public Position? selectedPosition;

        [Parameter] public int Width { get; set; } = 8;
        [Parameter] public int Height { get; set; } = 8;
        [Parameter] public bool IsRotated { get; set; } = false;
        [Parameter] public PieceForView[,] Pieces { get; set; }

        [Parameter] public EventCallback<Position> OnFieldClicked { get; set; }
        [Parameter] public EventCallback<ChessBoardComponent> IsReady { get; set; }

        protected FieldComponent[,] Fields;

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
                await this.IsReady.InvokeAsync(this);
            }
        }

        protected PieceForView GetPiece(int w, int h)
        {
            try
            {
                return this.Pieces[w, h];
            }
            catch
            {
                return null;
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

        protected char GetLetter(int w, int h)
        {
            if ((IsRotated && h == Height - 1) || (!IsRotated && h == 0))
            {
                return (char)('a' + w);
            }
            return ' ';
        }

        protected char GetNumber(int w, int h)
        {
            if ((IsRotated && w == 0) || (!IsRotated && w == Width - 1))
            {
                return (char)('1' + h);
            }
            return ' ';
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
            this.Fields[position.X, position.Y].SetBorderColor(BorderColor.Select);
            this.selectedPosition = position;
        }

        public void ShowMoves(IEnumerable<Position> positions)
        {
            foreach (Position position in positions)
            {
                this.Fields[position.X, position.Y].SetBorderColor(BorderColor.Ok);
            }
        }
    }
}
