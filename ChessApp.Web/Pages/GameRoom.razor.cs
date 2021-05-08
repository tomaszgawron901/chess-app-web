using ChessApp.Web.helpers;
using ChessApp.Web.Services;
using ChessBoardComponents;
using ChessClassLibrary;
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
    public class GameRoomBase: ComponentBase, IAsyncDisposable
    {
        [Inject] protected NavigationManager AppNavigationManager { get; set; }
        [Inject] IConfiguration Configuration { get; set; }
        [Inject] GameService GameService { get; set; }

        protected ChessBoardComponentBase ChessBoardComponent;

        [Parameter] public string GameCode { get; set; }
        protected string JoinUrl;
        protected GameManager GameManager;
        protected GameOptions GameOptions;
        protected HubConnection HubConnection;

        private bool IsGameReady = false;
        private bool IsBoardReady = false;

        protected bool IsBoardRotated => this.GameOptions?.Player2 == HubConnection.ConnectionId;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            this.JoinUrl = AppNavigationManager.Uri;

            await ConnectToGame();
            await GetGameOptions();
            CreateNewGame();
        }

        protected async Task GetGameOptions()
        {
            this.GameOptions = await this.GameService.GetGameOptionsByKey(GameCode);
        }

        protected async Task ConnectToGame()
        {
            this.HubConnection = await this.GameService.JoinGame(GameCode, (gameKey, gameOptions) =>
            {
                if (gameKey == this.GameCode)
                {
                    this.GameOptions = gameOptions;
                    this.StateHasChanged();
                }
            });
            GameManager = new GameManager(this.HubConnection);
        }

        protected void CreateNewGame()
        {
            try
            {
                if (this.GameOptions?.GameVarient == null) throw new NullReferenceException();
                this.GameManager.CreateGame(this.GameOptions); // throws
                IsGameReady = true;

                if (IsBoardReady)
                {
                    UpdateBoardComponentPieces();
                }
            }
            catch
            {
                // TODO  inform about
            }

        }

        protected void AfterBoardReady(ChessBoardComponentBase board)
        {
            this.ChessBoardComponent = board;
            this.IsBoardReady = true;
            if (IsGameReady)
            {
                UpdateBoardComponentPieces();
            }
        }


        protected void OnBoardFieldClicked(Position position)
        {
            Console.WriteLine(position.x + " " + position.y);
            if (ChessBoardComponent.selectedPosition == null)
            {
                
                var pieceAtPosition = GameManager.Board.GetPiece(position);
                if (pieceAtPosition != null)
                {
                    if (pieceAtPosition.Color == GameManager.CurrentPlayerColor)
                    {
                        ChessBoardComponent.SelectPosition(position);
                        ChessBoardComponent.ShowMoves(pieceAtPosition.MoveSet.Select(x => position + x.Shift));
                    }
                }
            }
            else
            {
                var move = new BoardMove((Position)ChessBoardComponent.selectedPosition, position);
                if (GameManager.CanPerformMove(move))
                {
                    GameManager.PerformMove(move);
                    UpdateBoardComponentPieces();
                }
                ChessBoardComponent.UnSelectAll();
            }
        }

        protected void UpdateBoardComponentPieces()
        {
            for (int x = 0; x < GameManager.Board.Width; x++)
            {
                for (int y = 0; y < GameManager.Board.Height; y++)
                {
                    var gamePiece = GameManager.Board.GetPiece(new Position(x, y));
                    ChessBoardComponent.Fields[x, y].Piece = gamePiece == null ? null : new PieceForView() { PieceColor = gamePiece.Color, PieceType = gamePiece.Type };
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (this.HubConnection != null)
            {
                await this.HubConnection.DisposeAsync();
            }
        }
    }
}
