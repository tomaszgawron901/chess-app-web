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
    public class GameManager : IClassicGame
    {
        private HubConnection hubConnection;
        public GameManager(HubConnection hubConnection)
        {
            this.hubConnection = hubConnection;
            hubConnection.On<BoardMove>("PerformMove", (boardMove) => {
                if (InnerGame != null)
                {
                    InnerGame.TryPerformMove(boardMove);
                }
            });
        }

        public void CreateGame(GameOptions gameOptions)
        {
            switch (gameOptions.GameVarient)
            {
                case GameVarient.Standard:
                    InnerGame = new ClassicGame();
                    break;
                default:
                    InnerGame = null;
                    throw new NotSupportedException("Game type not supported");
            }

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

        public IClassicGame InnerGame { get; set; }
        public PieceColor ClientColor { get; set; }

        public PieceColor CurrentPlayerColor => InnerGame != null ? InnerGame.CurrentPlayerColor : throw new NullReferenceException("Inner game does not exist.");

        public GameState GameState => InnerGame != null ? InnerGame.GameState : GameState.NotStarted;

        public ClassicBoard Board => InnerGame != null ? InnerGame.Board : null;

        public bool CanPerformMove(BoardMove move)
        {
            try
            {
                return  InnerGame != null && ClientColor == CurrentPlayerColor  && InnerGame.CanPerformMove(move);
            }
            catch
            {
                return false;
            }
            
        }

        public IEnumerable<PieceMove> GetPieceMoveSetAtPosition(Position position)
        {
            Console.WriteLine("ashdkjadsk");
            if (InnerGame != null && CurrentPlayerColor == ClientColor)
            {
                return InnerGame.GetPieceMoveSetAtPosition(position);
            }
            return Enumerable.Empty<PieceMove>();
        }

        public void PerformMove(BoardMove move)
        {
            this.hubConnection.SendAsync("PerformMove", move);
        }

        public void TryPerformMove(BoardMove move)
        {
            if (CanPerformMove(move))
            {
                PerformMove(move);
            }
        }
    }
}
