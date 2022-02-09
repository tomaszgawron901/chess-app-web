using ChessApp.Web.Models;
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
        private GameRoom gameRoom;
        private IClassicGame game;

        public bool IsGameCreated { get; private set; }
        public bool IsGameReadyToPlay { get; private set; }

        public GameManager(GameService gameService)
        {
            this.gameService = gameService;
        }


        private async Task Connect()
        {
            this.hubConnection = await gameService.EnsureIsConnected();
        }

        private void SetListeners()
        {
            this.hubConnection.On<string, GameOptions>("GameOptionsChanged", (gameKey, gameOptions) =>
            {
                if (gameKey == this.gameCode)
                {
                    this.gameOptions = gameOptions;
                    this.gameRoom.SetGameOptions(this.gameOptions);
                    this.CreateGame();
                }
            });

            this.hubConnection.On<string, BoardMove, SharedClock, SharedClock> ("PerformMove", (gameKey, boardMove, clock1, clock2) => {
                if (gameKey == this.gameCode)
                {
                    string message = "";
                    if(this.CurrentPlayerColor == PieceColor.White)
                    {
                        message += "White ";
                    }
                    else if (this.CurrentPlayerColor == PieceColor.Black)
                    {
                        message += "Black ";
                    }
                    this.game.TryPerformMove(boardMove);
                    gameRoom.UnSelectBoard();
                    this.gameRoom.SetBoardPieces(GetPiecesForView());
                    this.gameRoom.ChatWindow.AddMessage($"{message}{boardMove}");
                    if (ClientColor == PieceColor.Black)
                    {
                        this.gameRoom.SetTimer1(clock1);
                        this.gameRoom.SetTimer2(clock2);
                    }
                    else
                    {
                        this.gameRoom.SetTimer1(clock2);
                        this.gameRoom.SetTimer2(clock1);
                    }
                }
            });

            this.hubConnection.On<string, PieceColor?>("GameEnded", (gameKey, winner) => {
                if (gameKey == this.gameCode)
                {
                    if (winner == null)
                    {
                        this.gameRoom.ChatWindow.AddMessage("Game ended: Stalemate");
                    }
                    else if(winner == PieceColor.White)
                    {
                        this.gameRoom.ChatWindow.AddMessage("Game ended: White won");
                    }
                    else if (winner == PieceColor.Black)
                    {
                        this.gameRoom.ChatWindow.AddMessage("Game ended: Black won");
                    }

                }
            });

            this.hubConnection.On<string, string>("ServerMessage", (gameKey, message) => {
                if (gameKey == this.gameCode)
                {
                }
            });

            this.hubConnection.On<string, string>("UserMessage", (gameKey, message) => {
                if (gameKey == this.gameCode)
                {
                }
            });
        }

        private async Task JoinGame(string gameCode)
        {
            this.gameCode = gameCode;
            var gameOptions = await this.hubConnection.InvokeAsync<GameOptions>("JoinGame", this.gameCode);

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

        public async Task PrepareGameRoom(string gameCode, GameRoom gameRoom)
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
            this.gameRoom.ChatWindow.AddMessage("You have successfully connected to the game.");
        }


        private void CreateGame()
        {
            switch (gameOptions.GameVarient)
            {
                case GameVarient.Standard:
                    this.game = new ClassicGame();
                    break;
                case GameVarient.Knightmate:
                    this.game = new KnightmateGame();
                    break;
                default:
                    this.game = null;
                    throw new NotSupportedException("Game type not supported");
            }
            this.gameRoom.SetBoardPieces(GetPiecesForView());
            this.IsGameCreated = true;

            if (this.gameOptions.Player1 != null && this.gameOptions.Player2 != null)
            {
                this.IsGameReadyToPlay = true;
            }
            else
            {
                this.IsGameReadyToPlay = false;
            }
            this.gameRoom.UnSelectBoard();
            this.gameRoom.Update();

            this.gameRoom.SetTimer1(new SharedClock() { Time = gameOptions.MinutesPerSide * 60000, Started = false });
            this.gameRoom.SetTimer2(new SharedClock() { Time = gameOptions.MinutesPerSide * 60000, Started = false });
        }

        public PieceColor ClientColor { get; private set; }

        public PieceColor? CurrentPlayerColor => game != null ? game.CurrentPlayerColor : null;

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
            if (game != null)
            {
                return game.GetPieceMoveSetAtPosition(position);
            }
            return Enumerable.Empty<PieceMove>();
        }

        private async Task PerformMove(BoardMove move)
        {
            await this.hubConnection.InvokeAsync("PerformMove", this.gameCode, move);
            gameRoom.UnSelectBoard();
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
