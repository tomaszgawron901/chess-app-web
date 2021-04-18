using ChessClassLibrary.enums;
using ChessClassLibrary.Pieces;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessBoardComponents
{
    public class PieceComponentBase : ComponentBase
    {
        [CascadingParameter]
        public FieldComponent FieldComponent { get; set; }

        [Parameter]
        public IPiece Piece { get; set; }

        protected string GetPieceImageUrl()
        {
            StringBuilder sb = new StringBuilder();
            switch (Piece.Color)
            {
                case PieceColor.White:
                    sb.Append("White");
                    break;
                case PieceColor.Black:
                    sb.Append("Black");
                    break;
                default:
                    break;
            }

            switch (Piece.Type)
            {
                case PieceType.Pawn:
                    sb.Append("Pawn");
                    break;
                case PieceType.Rook:
                    sb.Append("Rook");
                    break;
                case PieceType.Knight:
                    sb.Append("Knight");
                    break;
                case PieceType.Bishop:
                    sb.Append("Bishop");
                    break;
                case PieceType.Queen:
                    sb.Append("Queen");
                    break;
                case PieceType.King:
                    sb.Append("King");
                    break;
                default:
                    break;
            }
            sb.Append(".png");
            return sb.ToString();
        }
    }
}
