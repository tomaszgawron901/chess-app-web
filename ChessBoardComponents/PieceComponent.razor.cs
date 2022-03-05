using ChessClassLib.Enums;
using Microsoft.AspNetCore.Components;
using System.Text;

namespace ChessBoardComponents
{
    public partial class PieceComponent : ComponentBase
    {
        [Parameter] public PieceColor PieceColor { get; set; }
        [Parameter] public PieceType PieceType { get; set; }

        protected string GetPieceImageUrl()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("_content/ChessBoardComponents/Images/");
            switch (PieceColor)
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

            switch (PieceType)
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
                case PieceType.Commoner:
                    sb.Append("Commoner");
                    break;
                case PieceType.Centaur:
                    sb.Append("Centaur");
                    break;
                default:
                    break;
            }
            sb.Append(".svg");
            return sb.ToString();
        }
    }
}
