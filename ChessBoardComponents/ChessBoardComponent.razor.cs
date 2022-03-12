using ChessClassLib.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChessBoardComponents
{
    public partial class ChessBoardComponent : ComponentBase
    {
        public Position? selectedPosition;

        [Parameter] public bool IsRotated { get; set; } = false;
        [Parameter] public bool IsDisabled { get; set; } = false;
        [Parameter] public EventCallback<Position> OnFieldClicked { get; set; }

        private int Width = 0;
        private int Height = 0;
        private PieceForView[,] Pieces;
        private FieldComponent[,] Fields;

        public void SetPieces(PieceForView[,] pieces)
        {
            int newWidth, newHeight;
            if (pieces == null)
            {
                newWidth = 0;
                newHeight = 0;
                Pieces = new PieceForView[0, 0];
            }
            else
            {
                newWidth = pieces.GetLength(0);
                newHeight = pieces.GetLength(1);
                Pieces = pieces;
            }
            if(newWidth != Width || newHeight != Height)
            {
                Width = newWidth;
                Height = newHeight;
                Fields = new FieldComponent[newWidth, newHeight];
            }

            StateHasChanged();
        }

        public void SetBoardRotation(bool rotated = true)
        {
            IsRotated = rotated;
            StateHasChanged();
        }

        public void SetBoardDisability(bool disabled = true)
        {
            IsDisabled = disabled;
            StateHasChanged();
        }

        public void UnSelectAll()
        {
            foreach (FieldComponent field in this.Fields)
            {
                field.SetBorderColor(BorderColor.None);
            }
            selectedPosition = null;
            StateHasChanged();
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

        public void SetCheckMated(Position position)
        {
            this.Fields[position.X, position.Y].SetBorderColor(BorderColor.Bad);
        }

        private IEnumerable<int> GetXIndexes()
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

        private IEnumerable<int> GetYIndexes()
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

        private char GetLetter(int w, int h)
        {
            if ((IsRotated && h == Height - 1) || (!IsRotated && h == 0))
            {
                return (char)('a' + w);
            }
            return ' ';
        }

        private char GetNumber(int w, int h)
        {
            if ((IsRotated && w == 0) || (!IsRotated && w == Width - 1))
            {
                return (char)('1' + h);
            }
            return ' ';
        }

        private async void OnFieldClick(int x, int y)
        {
            if (!IsDisabled)
            {
                await OnFieldClicked.InvokeAsync(new Position(x, y));
            }
        }
    }
}
