using ChessApp.Web.helpers;
using ChessApp.Web.Services;
using ChessBoardComponents;
using ChessClassLibrary;
using ChessClassLibrary.enums;
using ChessClassLibrary.Games.ClassicGame;
using ChessClassLibrary.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessApp.Web.Pages
{
    public class GameRoomBase: ComponentBase
    {
        [Inject] protected NavigationManager AppNavigationManager { get; set; }
        [Inject] IConfiguration Configuration { get; set; }
        [Inject] protected GameManager GameManager { get; set; }

        protected ChessBoardComponentBase ChessBoardComponent;

        [Parameter] public string GameCode { get; set; }
        protected string JoinUrl;
        protected GameOptions GameOptions;
        protected PieceForView[,] pieces;

        protected bool IsBoardRotated => this.GameManager.ClientColor == PieceColor.Black;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            this.JoinUrl = AppNavigationManager.Uri;

            await this.GameManager.PrepareGameRoom(this.GameCode, this);
        }

        protected void AfterBoardReady(ChessBoardComponentBase board)
        {
            this.ChessBoardComponent = board;
        }


        protected async Task OnBoardFieldClicked(Position position)
        {
            Console.WriteLine(position.X + " " + position.Y);
            if (ChessBoardComponent.selectedPosition == null)
            {
                
                var pieceAtPosition = GameManager.GetPieceForViewAtPosition(position);
                if (pieceAtPosition != null)
                {
                    if (pieceAtPosition.PieceColor == GameManager.CurrentPlayerColor)
                    {
                        ChessBoardComponent.SelectPosition(position);
                        ChessBoardComponent.ShowMoves(GameManager.GetPieceMoveSetAtPosition(position).Select(x => position + x.Shift));
                    }
                }
            }
            else
            {
                var move = new BoardMove((Position)ChessBoardComponent.selectedPosition, position);
                await GameManager.TryPerformMove(move);
                ChessBoardComponent.UnSelectAll();
            }
        }

        public void SetBoardPieces(PieceForView[,] pieces)
        {
            this.pieces = pieces;
            this.StateHasChanged();
        }

        public void SetGameOptions(GameOptions gameOptions)
        {
            this.GameOptions = gameOptions;
            this.StateHasChanged();
        }
    }
}
