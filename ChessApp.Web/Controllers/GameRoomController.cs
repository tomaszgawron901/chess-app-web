using ChessApp.Web.Exstentions;
using ChessApp.Web.Models;
using ChessApp.Web.Pages;
using ChessApp.Web.Services;
using ChessBoardComponents;
using ChessClassLib.Enums;
using ChessClassLib.Logic.Games;
using ChessClassLib.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessApp.Web.Controllers
{
    public class GameRoomController : IDisposable
    {
        private NavigationManager AppNavigationManager { get; }
        private GameHubService GameHubService { get; }
        private GameRoomService GameRoomService { get; set; }

        private GameOptions gameOptions;
        private GameRoom GameRoom;
        private IClassicGame game;
        private PieceColor? ClientColor;

        public GameRoomController(GameHubService gameHubService, NavigationManager appNavigationManager)
        {
            GameHubService = gameHubService;
            AppNavigationManager = appNavigationManager;
        }

        public async Task Initialize(string roomKey, GameRoom gameRoom)
        {
            try
            {
                GameRoom = gameRoom;
                GameRoom.OnBoardFieldClicked = EventCallback.Factory.Create(this, delegate (Position p) { HandleBoardFieldClicked(p); });
                GameRoom.OnLeaveGameRoomClicked = EventCallback.Factory.Create(this, async delegate() { await HandleLeaveGame(); });
                GameRoomService = GameHubService.CreateGameRoomService(roomKey);
                SetListeners();
                var gameOptions = await GameRoomService.JoinGame();
                HandleOptionsChange(gameOptions);
                GameRoom.NotifySuccessfullyConnected();

                if (gameOptions.Player1 == null || gameOptions.Player2 == null)
                {
                    GameRoom.DisableBoard();
                    GameRoom.SetDefaultBoard();
                }
            }
            catch
            {
                AppNavigationManager.NavigateToMainPage();
            }
        }

        private Task HandleLeaveGame()
        {
            return GameRoomService.LeaveGame()
                .ContinueWith(_ => { AppNavigationManager.NavigateToMainPage(); });
        }

        private void HandleOptionsChange(GameOptions gameOptions)
        {
            this.gameOptions = gameOptions;
            GameRoom.SetGameOptions(this.gameOptions);
            if(gameOptions.Player1 == GameHubService.ConnectionId)
            {
                SetClientColor(PieceColor.White);
            }
            else if (gameOptions.Player2 == GameHubService.ConnectionId)
            {
                SetClientColor(PieceColor.Black);
            }
            else
            {
                SetClientColor(null);
            }
            if (this.gameOptions.Player1 == null || this.gameOptions.Player2 == null)
            {
                GameRoom.DisableBoard();
            }
                if (this.gameOptions.Player1 != null && this.gameOptions.Player2 != null)
            {
                CreateGame();
            }
        }


        private void SetClientColor(PieceColor? color)
        {
            ClientColor = color;
            GameRoom.SetClientColor(color);
        }


        private void HandleBoardFieldClicked(Position position)
        {
            var selectedPosition = GameRoom.GetSelectedPosition();
            if (selectedPosition == null)
            {
                var pieceAtPosition = game.Board.GetPiece(position);
                if (pieceAtPosition != null)
                {
                    if (pieceAtPosition.Color == ClientColor)
                    {
                        GameRoom.SetPieceMoveSet(position, pieceAtPosition.MoveSet.Select(x => position + x.Shift));
                    }
                }
            }
            else
            {
                GameRoom.ClearBoardSelections();
                var moveToPerform = new BoardMove((Position)selectedPosition, position);
                if (CanPerformMove(moveToPerform))
                {
                    try
                    {
                        GameRoomService.PerformMove(moveToPerform);
                    }
                    catch
                    {
                        GameRoom.NotifyErrorWhilePerformingMove(moveToPerform);
                    }
                }
            }
        }

        private void HandlePerformMove(BoardMove move, SharedClock clock1, SharedClock clock2)
        {
            var performingColor = game.CurrentPlayerColor;
            if (game.TryPerformMove(move))
            {
                GameRoom.ClearBoardSelections();
                UpdateBoardComponentPieces();
                GameRoom.NotifyPieceMoved(performingColor, move);
                GameRoom.SetCurrentColor(game.CurrentPlayerColor);
                if (ClientColor == PieceColor.Black)
                {
                    GameRoom.SetTimer1(clock1);
                    GameRoom.SetTimer2(clock2);
                }
                else
                {
                    GameRoom.SetTimer1(clock2);
                    GameRoom.SetTimer2(clock1);
                }
                if (game.GameState == GameState.Ended)
                {
                    GameRoom.DisableBoard();
                }
            }
            else
            {
                GameRoom.NotifyErrorWhilePerformingMove(move);
            }
        }

        private void HandleGameEnded(PieceColor? winner)
        {
            GameRoom.NotifyGameEnded(winner);
            GameRoom.ClearBoardSelections();
            GameRoom.DisableBoard();
        }

        private void HandlePlayerLeft(string player)
        {
            GameRoom.NotifyPlayerLeft();
        }

        private void HandlePlayerJoined(string player)
        {
            GameRoom.NotifyPlayerJoined();
        }

        private void SetListeners()
        {
            GameRoomService.OnOptionsChanged += HandleOptionsChange;
            GameRoomService.OnMovePerformed += HandlePerformMove;
            GameRoomService.OnGameEnded += HandleGameEnded;
            GameRoomService.OnPlayerLeft += HandlePlayerLeft;
            GameRoomService.OnPlayerJoined += HandlePlayerJoined;
        }

        private void CreateGame()
        {
            game = gameOptions.GameVarient.ConvertToGame();
            UpdateBoardComponentPieces();
            GameRoom.SetCurrentColor(game.CurrentPlayerColor);
            GameRoom.EnableBoard();

            GameRoom.SetTimer1(new SharedClock() { Time = gameOptions.MinutesPerSide * 60000, Started = false });
            GameRoom.SetTimer2(new SharedClock() { Time = gameOptions.MinutesPerSide * 60000, Started = false });
        }

        private PieceForView[,] GetPiecesForView()
            => game?.Board.GetPiecesForView() ?? new PieceForView[8, 8];

        private bool CanPerformMove(BoardMove move)
            => ClientColor == game.CurrentPlayerColor && (game?.CanPerformMove(move) ?? false);

        private void UpdateBoardComponentPieces()
        {
            var piecesForView = GetPiecesForView();
            GameRoom.SetBoardPieces(piecesForView);
        }

        private IEnumerable<PieceMove> GetPieceMoveSetAtPosition(Position position)
            => game?.GetPieceMoveSetAtPosition(position) ?? Enumerable.Empty<PieceMove>();


        private PieceForView GetPieceForViewAtPosition(Position position)
        {
            try
            {
                var piece = game.Board.GetPiece(position);
                return new PieceForView() { PieceColor = piece.Color, PieceType = piece.Type };
            }
            catch
            {
                return null;
            }
        }

        public void Dispose()
        {
            GameRoomService?.Dispose();
        }

        ~GameRoomController() { Dispose(); }
    }
}
