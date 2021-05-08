using ChessApp.Web.Pages;
using ChessApp.Web.Services;
using ChessBoardComponents;
using ChessClassLibrary.Boards;
using ChessClassLibrary.enums;
using ChessClassLibrary.Games;
using ChessClassLibrary.Games.ClassicGame;
using ChessClassLibrary.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessApp.Web.helpers
{
    public class GameManager: IAsyncDisposable
    {
        private HubConnection hubConnection;
        private GameService gameService;

        private string gameCode;
        private GameOptions gameOptions;
        private GameRoomBase gameRoom;
        private IClassicGame game;

        public bool IsGameCreated { get; private set; }

        public GameManager(GameService gameService)
        {
            this.gameService = gameService;
        }


        private async Task Connect()
        {
            this.hubConnection = gameService.GetHubConnection();
            await this.hubConnection.StartAsync();
        }

        private void SetListeners()
        {
            this.hubConnection.On<string, GameOptions>("GameOptionsChanged", (gameKey, gameOptions) =>
            {
                if (gameKey == this.gameCode)
                {
                    this.gameOptions = gameOptions;
                    this.gameRoom.SetGameOptions(this.gameOptions);
                }
            });

            this.hubConnection.On<string, BoardMove>("PerformMove", (gameKey, boardMove) => {
                if (gameKey == this.gameCode)
                {
                    this.game.TryPerformMove(boardMove);
                    this.gameRoom.SetBoardPieces(GetPiecesForView());
                }
            });
        }

        private async Task JoinGame(string gameCode)
        {
            this.gameCode = gameCode;
            await this.hubConnection.InvokeAsync("JoinGame", this.gameCode);

            var gameOptions = await this.gameService.GetGameOptionsByKey(gameCode);
            this.SetGameOptions(gameOptions);

        }

        private void SetGameOptions(GameOptions gameOptions)
        {
            this.gameOptions = gameOptions;
            this.gameRoom.SetGameOptions(gameOptions);
            if (gameOptions.Player1 == hubConnection.ConnectionId)
            {
                this.ClientColor = PieceColor.White;
            }
            else if (gameOptions.Player2 == hubConnection.ConnectionId)
            {
                this.ClientColor = PieceColor.Black;
            }
            else
            {
                throw new NotImplementedException("view mode not implemented");
            }
        }

        public async Task PrepareGameRoom(string gameCode, GameRoomBase gameRoom)
        {
            if (gameCode == null || gameRoom == null)
            {
                throw new ArgumentNullException("gameCode or gameRoom not provided");
            }
            this.gameRoom = gameRoom;

            await this.Connect();
            await this.JoinGame(gameCode);
            this.SetListeners();
            this.CreateGame();
        }


        private void CreateGame()
        {
            switch (gameOptions.GameVarient)
            {
                case GameVarient.Standard:
                    this.game = new ClassicGame();
                    break;
                default:
                    this.game = null;
                    throw new NotSupportedException("Game type not supported");
            }
            this.gameRoom.SetBoardPieces(GetPiecesForView());
            this.IsGameCreated = true;
        }

        private async Task SetAsReady()
        {
            await this.hubConnection.InvokeAsync("SetAsReady"); // -----------
        }

        public PieceColor ClientColor { get; private set; }

        public PieceColor CurrentPlayerColor => game != null ? game.CurrentPlayerColor : throw new NullReferenceException("Inner game does not exist.");

        public GameState GameState => game != null ? game.GameState : GameState.NotStarted;


        private PieceForView[,] GetPiecesForView()
        {
            if (this.game == null)
            {
                return new PieceForView[8, 8];
            }

            var pieces = new PieceForView[this.game.Board.Width, this.game.Board.Height];
            for (int x = 0; x < this.game.Board.Width; x++)
            {
                for (int y = 0; y < this.game.Board.Height; y++)
                {
                    pieces[x, y] = this.GetPieceForViewAtPosition(new Position(x, y));
                }
            }
            return pieces;
        }

        public bool CanPerformMove(BoardMove move)
        {
            try
            {
                return  game != null && ClientColor == CurrentPlayerColor  && game.CanPerformMove(move);
            }
            catch
            {
                return false;
            }
            
        }

        public IEnumerable<PieceMove> GetPieceMoveSetAtPosition(Position position)
        {
            if (game != null && CurrentPlayerColor == ClientColor)
            {
                return game.GetPieceMoveSetAtPosition(position);
            }
            return Enumerable.Empty<PieceMove>();
        }

        private async Task PerformMove(BoardMove move)
        {
            await this.hubConnection.InvokeAsync("PerformMove", this.gameCode, move);
        }

        public async Task TryPerformMove(BoardMove move)
        {
            if (CanPerformMove(move))
            {
                await PerformMove(move);
            }
        }

        public PieceForView GetPieceForViewAtPosition(Position position)
        {
            try
            {
                var piece = this.game.Board.GetPiece(position);
                return new PieceForView() { PieceColor = piece.Color, PieceType = piece.Type };
            }
            catch
            {
                return null;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (this.hubConnection != null)
            {
                await this.hubConnection.DisposeAsync();
            }
        }
    }
}
