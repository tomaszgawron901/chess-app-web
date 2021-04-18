using ChessClassLibrary.Boards;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessBoardComponents
{
    public class ChessBoardComponentBase : ComponentBase
    {
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
    }
}
