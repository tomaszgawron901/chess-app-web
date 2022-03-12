using ChessApp.Web.Components;
using ChessApp.Web.Controllers;
using ChessApp.Web.Exstentions;
using ChessApp.Web.Models;
using ChessBoardComponents;
using ChessClassLib.Enums;
using ChessClassLib.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChessApp.Web.Pages
{
    public partial class GameRoom: ComponentBase, IDisposable
    {
        [Inject] private NavigationManager AppNavigationManager { get; set; }
        [Inject] private IStringLocalizer<App> Localizer { get; set; }
        [Inject] private GameRoomController GameRoomController { get; set; }

        [Parameter] public string RoomCode { get; set; }
        [Parameter] public EventCallback<Position> OnBoardFieldClicked { get; set; }
        [Parameter] public EventCallback OnLeaveGameRoomClicked { get; set; }

        private ChessBoardComponent ChessBoardComponent;
        private FullGameOptionsForm FullGameOptionsForm;
        private ActivityLog ActivityLog;
        private TimerComponent timer1;
        private TimerComponent timer2;

        private string JoinUrl;

        protected override Task OnInitializedAsync()
        {
            JoinUrl = AppNavigationManager.Uri;
            return base.OnInitializedAsync()
                .Then(() => GameRoomController.Initialize(RoomCode, this));
        }

        public void SetGameOptions(GameOptions options)
        {
            FullGameOptionsForm.SetGameOptions(options);
        }

        public void SetClientColor(PieceColor? color)
        {
            FullGameOptionsForm.SetUserPlayer(color);
            if(color == PieceColor.Black)
            {
                ChessBoardComponent.SetBoardRotation(true);
            }
            else
            {
                ChessBoardComponent.SetBoardRotation(false);
            }
        }

        public void SetCurrentColor(PieceColor? color)
        {
            FullGameOptionsForm.SetCurrentPlayer(color);
        }

        public void SetTimer1(SharedClock clock) => timer1.SetClock(clock);
        public void SetTimer2(SharedClock clock) => timer2.SetClock(clock);

        public void NotifyPieceMoved(PieceColor color, BoardMove move)
        {
            StringBuilder sb = new StringBuilder();
            if (color == PieceColor.White)
            {
                sb.Append($"{Localizer["white"]} ");
            }
            else if (color == PieceColor.Black)
            {
                sb.Append($"{Localizer["black"]} ");
            }
            sb.Append($"{move.Current} {Localizer["to"]} {move.Destination}");
            ActivityLog.AddMessage(sb.ToString());
        }

        public void NotifyPlayerJoined() => ActivityLog.AddMessage(Localizer["player_joined"]);

        public void NotifyPlayerLeft() => ActivityLog.AddMessage(Localizer["player_left"]);

        public void NotifyGameEnded(PieceColor? winner)
        {
            if (winner == null)
            {
                ActivityLog.AddMessage($"{Localizer["game_ended"]}: {Localizer["stalemate"]}");
            }
            else if (winner == PieceColor.White)
            {
                ActivityLog.AddMessage($"{Localizer["game_ended"]}: {Localizer["white_won"]}");
            }
            else if (winner == PieceColor.Black)
            {
                ActivityLog.AddMessage($"{Localizer["game_ended"]}: {Localizer["black_won"]}");
            }
        }

        public void NotifySuccessfullyConnected()
        {
            ActivityLog.AddMessage($"{Localizer["successfully_connected_message"]}");
        }

        public void NotifyErrorWhilePerformingMove(BoardMove move)
        {
            ActivityLog.AddMessage($"{Localizer["error_while_performing_move"]}: {move.Current} {Localizer["to"]} {move.Destination}");
        }

        public void SetBoardRotation(bool rotated)
        {
            ChessBoardComponent.SetBoardRotation(rotated);
        }

        public void SetDefaultBoard()
        {
            SetBoardPieces(new PieceForView[8, 8]);
        }

        public void SetBoardPieces(PieceForView[,] pieces)
        {
            ChessBoardComponent.SetPieces(pieces);
        }

        public void SetPieceMoveSet(Position position, IEnumerable<Position> moveSet)
        {
            ChessBoardComponent.SelectPosition(position);
            ChessBoardComponent.ShowMoves(moveSet);
        }

        public void ClearBoardSelections()
        {
            ChessBoardComponent.UnSelectAll();
        }

        public void DisableBoard()
        {
            ChessBoardComponent.SetBoardDisability(true);
        }

        public void EnableBoard()
        {
            ChessBoardComponent.SetBoardDisability(false);
        }

        public Position? GetSelectedPosition() => ChessBoardComponent.selectedPosition;

        private async Task HandleBoardFieldClicked(Position position)
        {
            await OnBoardFieldClicked.InvokeAsync(position);
        }

        private async Task HandleNewGameClicked()
        {
            await OnLeaveGameRoomClicked.InvokeAsync();
        }

        public void Dispose()
        {
            GameRoomController.Dispose();
        }

        ~GameRoom() { Dispose(); }
    }
}
