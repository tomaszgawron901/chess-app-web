using ChessApp.Web.Models;
using ChessApp.Web.Pages;
using ChessApp.Web.Services;
using ChessBoardComponents;
using ChessClassLib.Enums;
using ChessClassLib.Logic.Games;
using ChessClassLib.Models;
using hessClassLibrary.Logic.Games;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessApp.Web.helpers
{
    public class GameManager : IAsyncDisposable
    {
        private HubConnection hubConnection;
        private GameService gameService;

        private IStringLocalizer<App> Localizer { get; }

        private string gameCode;
        private GameOptions gameOptions;
        private GameRoom gameRoom;
        private IClassicGame game;

        public bool IsGameCreated { get; private set; }
        public bool IsGameReadyToPlay { get; private set; }

        public GameManager(GameService gameService, IStringLocalizer<App> localizer)
        {
            this.gameService = gameService;
            this.Localizer = localizer;
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

            this.hubConnection.On<string, BoardMove, SharedClock, SharedClock>("PerformMove", (gameKey, boardMove, clock1, clock2) => {
                if (gameKey == this.gameCode)
                {
                    StringBuilder st = new StringBuilder();
                    if (this.CurrentPlayerColor == PieceColor.White)
                    {
                        st.Append($"{Localizer["white"]} ");
                    }
                    else if (this.CurrentPlayerColor == PieceColor.Black)
                    {
                        st.Append($"{Localizer["black"]} ");
                    }
                    st.Append($"{boardMove.Current} {Localizer["to"]} {boardMove.Destination}");
                    this.game.TryPerformMove(boardMove);
                    gameRoom.UnSelectBoard();
                    this.gameRoom.SetBoardPieces(GetPiecesForView());
                    this.gameRoom.ChatWindow.AddMessage(st.ToString());
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
                        this.gameRoom.ChatWindow.AddMessage($"{Localizer["game_ended"]}: {Localizer["stalemate"]}");
                    }
                    else if (winner == PieceColor.White)
                    {
                        this.gameRoom.ChatWindow.AddMessage($"{Localizer["game_ended"]}: {Localizer["white_won"]}");
                    }
                    else if (winner == PieceColor.Black)
                    {
                        this.gameRoom.ChatWindow.AddMessage($"{Localizer["game_ended"]}: {Localizer["black_won"]}");
                    }

                }
            });

            this.hubConnection.On<string, string>("PlayerLeft", (gameKey, user) => {
                if (gameKey == this.gameCode)
                {
                    this.gameRoom.ChatWindow.AddMessage(Localizer["player_left"]);
                }
            });

            this.hubConnection.On<string, string>("PlayerJoined", (gameKey, user) => {
                if (gameKey == this.gameCode)
                {
                    this.gameRoom.ChatWindow.AddMessage(Localizer["player_joined"]);
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
            this.gameRoom.ChatWindow.AddMessage(Localizer["successfuly_connected_message"]);
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
                return game != null && ClientColor == CurrentPlayerColor && game.CanPerformMove(move);
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
