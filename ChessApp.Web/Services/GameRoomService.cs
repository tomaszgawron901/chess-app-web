using ChessApp.Web.Models;
using ChessClassLib.Enums;
using ChessClassLib.Models;
using System;
using System.Threading.Tasks;

namespace ChessApp.Web.Services
{
    public delegate void RoomOptionsChangedEventHandler(GameOptions gameOptions);
    public delegate void RoomMovePerformedEventHandler(BoardMove move, SharedClock clock1, SharedClock clock2);
    public delegate void RoomGameEndedEventHandler(PieceColor? winner);
    public delegate void RoomPlayerLeftEventHandler(string player);
    public delegate void RoomPlayerJoinedEventHandler(string player);

    public class GameRoomService: IDisposable
    {
        public string RoomKey { get; }
        private GameHubService GameHubService { get; }

        public RoomOptionsChangedEventHandler OnOptionsChanged { get; set; }
        public RoomMovePerformedEventHandler OnMovePerformed { get; set; }
        public RoomGameEndedEventHandler OnGameEnded { get; set; }
        public RoomPlayerLeftEventHandler OnPlayerLeft { get; set; }
        public RoomPlayerJoinedEventHandler OnPlayerJoined { get; set; }

        public GameRoomService(string roomKey, GameHubService gameHubService)
        {
            RoomKey = roomKey;
            GameHubService = gameHubService;
            SetListeners();
        }

        private void SetListeners()
        {
            GameHubService.OnOptionsChanged += OptionsChanged;
            GameHubService.OnMovePerformed += MovePerformed;
            GameHubService.OnGameEnded += GameEnded;
            GameHubService.OnPlayerLeft += PlayerLeft;
            GameHubService.OnPlayerJoined += PlayerJoined;
        }

        private void OptionsChanged(string roomKey, GameOptions gameOptions)
        {
            if(roomKey == RoomKey) OnOptionsChanged.Invoke(gameOptions);
        }

        private void MovePerformed(string roomKey, BoardMove move, SharedClock clock1, SharedClock clock2)
        {
            if (roomKey == RoomKey) OnMovePerformed.Invoke(move, clock1, clock2);
        }

        private void GameEnded(string roomKey, PieceColor? winner)
        {
            if (roomKey == RoomKey) OnGameEnded.Invoke(winner);
        }

        private void PlayerLeft(string roomKey, string player)
        {
            if (roomKey == RoomKey) OnPlayerLeft.Invoke(player);
        }

        private void PlayerJoined(string roomKey, string player)
        {
            if (roomKey == RoomKey) OnPlayerJoined.Invoke(player);
        }

        public Task<GameOptions> JoinGame() => GameHubService.JoinGame(RoomKey);

        public Task LeaveGame() => GameHubService.LeaveGame(RoomKey);

        public Task PerformMove(BoardMove move) => GameHubService.PerformMove(RoomKey, move);

        public void Dispose()
        {
            GameHubService.OnOptionsChanged -= OptionsChanged;
            GameHubService.OnMovePerformed -= MovePerformed;
            GameHubService.OnGameEnded -= GameEnded;
            GameHubService.OnPlayerLeft -= PlayerLeft;
            GameHubService.OnPlayerJoined -= PlayerJoined;
        }

        ~GameRoomService()
        {
            Dispose();
        }
    }
}
