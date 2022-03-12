using ChessBoardComponents;
using ChessClassLib.Logic.Boards;
using ChessClassLib.Models;

namespace ChessApp.Web.Exstentions
{
    public static class ClassicBoardExtensions
    {
        public static PieceForView[,] GetPiecesForView(this ClassicBoard board)
        {
            int width = board.Width;
            int height = board.Height;
            var pieces = new PieceForView[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    pieces[x, y] = board.GetPieceForViewAtPosition(new Position(x, y));
                }
            }
            return pieces;
        }

        public static PieceForView GetPieceForViewAtPosition(this ClassicBoard board, Position position)
        {
            var piece = board.GetPiece(position);
            if(piece == null) { return null; }
            return new PieceForView() { PieceColor = piece.Color, PieceType = piece.Type };

        }
    }
}
