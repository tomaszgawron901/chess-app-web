using ChessBoardComponents.Interops;
using ChessClassLibrary;
using ChessClassLibrary.Boards;
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
        protected Position? selectedPosition;

        private ClassicBoard _board;
        [Parameter]
        public ClassicBoard Board
        {
            get
            {
                return _board;
            }
            set
            {
                _board = value;
                Fields = new FieldComponent[Board.Width, Board.Height];
            }
        }

        protected FieldComponent[,] Fields { get; set; }

        [Parameter]
        public bool IsRotated { get; set; } = false;

        protected IEnumerable<int> GetXIndexes()
        {
            if (IsRotated)
            {
                for (int i = Board.Width - 1; i >= 0; i--)
                {
                    yield return i;
                }
            }
            else
            {
                for (int i = 0; i < Board.Width; i++)
                {
                    yield return i;
                }
            }
        }

        protected IEnumerable<int> GetYIndexes()
        {
            if (IsRotated)
            {
                for (int i = 0; i < Board.Height; i++)
                {
                    yield return i;
                }
            }
            else
            {
                for (int i = Board.Height - 1; i >= 0; i--)
                {
                    yield return i;
                }
            }
        }

        public void OnFieldClicked(Position position)
        {
            Console.WriteLine(position.x + " " + position.y);
            if (this.selectedPosition == null)
            {
                var piece = this.Board.GetPiece(position);
                if (piece != null)
                {
                    this.ShowMoves(piece.MoveSet.Select(move => position + move.Shift));
                    this.SelectPosition(position);
                }
            }
            else
            {
                this.UnSelectAll();
            }
        }

        private void UnSelectAll()
        {
            foreach (FieldComponent field in this.Fields)
            {
                field.SetBorderColor(BorderColor.None);
            }
            this.selectedPosition = null;
        }

        private void SelectPosition(Position position)
        {
            this.Fields[position.x, position.y].SetBorderColor(BorderColor.Select);
            this.selectedPosition = position;
        }

        private void ShowMoves(IEnumerable<Position> positions)
        {
            foreach (Position position in positions)
            {
                this.Fields[position.x, position.y].SetBorderColor(BorderColor.Ok);
            }
        }
    }
}
