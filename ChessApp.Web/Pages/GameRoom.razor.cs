using ChessApp.Web.Components;
using ChessApp.Web.helpers;
using ChessApp.Web.Models;
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
    public partial class GameRoom: ComponentBase
    {
        [Inject] protected NavigationManager AppNavigationManager { get; set; }
        [Inject] IConfiguration Configuration { get; set; }
        [Inject] protected GameManager GameManager { get; set; }

        protected ChessBoardComponentBase ChessBoardComponent;
        public ChatWindow ChatWindow;

        [Parameter] public string GameCode { get; set; }
        protected string JoinUrl;
        protected GameOptions GameOptions;
        protected PieceForView[,] pieces;
        protected TimerComponent timer1;
        protected TimerComponent timer2;

        protected bool IsBoardRotated => this.GameManager.ClientColor == PieceColor.Black;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            this.JoinUrl = AppNavigationManager.Uri;

            try
            {
                await this.GameManager.PrepareGameRoom(this.GameCode, this);
            }
            catch
            {
                AppNavigationManager.NavigateTo($"/");
            }
            
        }

        protected void AfterBoardReady(ChessBoardComponentBase board)
        {
            this.ChessBoardComponent = board;
        }

        public void SetTimer1(SharedClock clock)
        {
            this.timer1.SetClock(clock);
        }

        public void SetTimer2(SharedClock clock)
        {
            this.timer2.SetClock(clock);
        }


        protected async Task OnBoardFieldClicked(Position position)
        {
            if (this.GameManager.IsGameReadyToPlay)
            {
                if (ChessBoardComponent.selectedPosition == null)
                {
                    var pieceAtPosition = GameManager.GetPieceForViewAtPosition(position);
                    if (pieceAtPosition != null)
                    {
                        if (pieceAtPosition.PieceColor == GameManager.ClientColor)
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

        }

        public void SetBoardPieces(PieceForView[,] pieces)
        {
            this.pieces = pieces;
            this.StateHasChanged();
        }

        public void UnSelectBoard()
        {
            if (ChessBoardComponent != null)
            {
                ChessBoardComponent.UnSelectAll();
            }
        }

        public void SetGameOptions(GameOptions gameOptions)
        {
            this.GameOptions = gameOptions;
            this.StateHasChanged();
        }
    }
}
