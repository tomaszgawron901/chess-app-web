using ChessBoardComponents;
using ChessClassLibrary;
using ChessClassLibrary.Games.ClassicGame;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessApp.Web.Pages
{
    public class GameRoomBase: ComponentBase
    {
        [Inject] protected NavigationManager AppNavigationManager { get; set; }

        protected ChessBoardComponent ChessBoardComponent;

        [Parameter] public string GameCode { get; set; }
        protected string JoinUrl;
        protected ClassicGame Game;

        protected override async Task OnInitializedAsync()
        {
            await base.OnParametersSetAsync();
            this.JoinUrl = AppNavigationManager.Uri;
            // TODO check is game with given code exists
            // TODO GET game options
            // TODO connect to signalR server
            // TODO if two users are connected start game
            Game = new ClassicGame();
            await OnGameCreate();

        }

        protected async Task OnGameCreate()
        {
        }

        public void AfterBoardReady()
        {
            UpdateBoardComponentPieces();
        }

        protected void OnBoardFieldClicked(Position position)
        {
            Console.WriteLine(position.x + " " + position.y);
            if (ChessBoardComponent.selectedPosition == null)
            {
                
                var pieceAtPosition = Game.Board.GetPiece(position);
                if (pieceAtPosition != null)
                {
                    if (pieceAtPosition.Color == Game.CurrentPlayerColor)
                    {
                        ChessBoardComponent.SelectPosition(position);
                        ChessBoardComponent.ShowMoves(pieceAtPosition.MoveSet.Select(x => position + x.Shift));
                    }
                }
            }
            else
            {
                var move = new BoardMove((Position)ChessBoardComponent.selectedPosition, position);
                if (Game.CanPerformMove(move))
                {
                    Game.PerformMove(move);
                    UpdateBoardComponentPieces();
                }
                ChessBoardComponent.UnSelectAll();
            }
        }

        protected void UpdateBoardComponentPieces()
        {
            for (int x = 0; x < Game.Board.Width; x++)
            {
                for (int y = 0; y < Game.Board.Height; y++)
                {
                    var gamePiece = Game.Board.GetPiece(new Position(x, y));
                    ChessBoardComponent.Fields[x, y].Piece = gamePiece == null ? null : new PieceForView() { PieceColor = gamePiece.Color, PieceType = gamePiece.Type };
                }
            }
        }
    }
}
