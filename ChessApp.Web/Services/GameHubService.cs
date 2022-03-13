using ChessClassLib.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using ChessApp.Web.Exstentions;
using ChessApp.Web.Models;
using ChessClassLib.Enums;

namespace ChessApp.Web.Services
{
    public delegate void OptionsChangedEventHandler(string roomKey, GameOptions gameOptions);
    public delegate void MovePerformedEventHandler(string roomKey, BoardMove move, SharedClock clock1, SharedClock clock2);
    public delegate void GameEndedEventHandler(string roomKey, PieceColor? winner);
    public delegate void PlayerLeftEventHandler(string roomName, string player);
    public delegate void PlayerJoinedEventHandler(string roomName, string player);

    public class GameHubService
    {
        private readonly HubConnection hubConnection;
        private string GameHubUrl { get; }

        public OptionsChangedEventHandler OnOptionsChanged { get; set; }
        public MovePerformedEventHandler OnMovePerformed { get; set; }
        public GameEndedEventHandler OnGameEnded { get; set; }
        public PlayerLeftEventHandler OnPlayerLeft { get; set; }
        public PlayerJoinedEventHandler OnPlayerJoined { get; set; }

        public GameHubService(IConfiguration configuration)
        {
            GameHubUrl = $"{configuration.GetSection("chess_server_root").Value}/gamehub";
            hubConnection = BuildHubConnection();
            SetListeners();
            hubConnection.StartAsync();
        }

        public string ConnectionId => hubConnection.ConnectionId;

        public Task<string> CreateNewGameRoom(CreateGameOptions gameOptions) => EnsureIsConnected()
                .Then(c => c.InvokeAsync<string>("CreateGameRoom", gameOptions));

        public Task<GameOptions> JoinGame(string roomKey) => EnsureIsConnected()
                .Then(c => c.InvokeAsync<GameOptions>("JoinGame", roomKey));

        public Task LeaveGame(string roomKey) => EnsureIsConnected()
                .Then(c => c.InvokeAsync<GameOptions>("LeaveGame", roomKey));

        public Task PerformMove(string roomKey, BoardMove move) => EnsureIsConnected()
                .Then(c => c.InvokeAsync("PerformMove", roomKey, move));

        public GameRoomService CreateGameRoomService(string roomCode)
            => new GameRoomService(roomCode, this);

        private HubConnection BuildHubConnection()
        {
            return new HubConnectionBuilder().WithUrl(GameHubUrl)
                .AddMessagePackProtocol()
                .Build();
        }

        private async Task<HubConnection> EnsureIsConnected()
        {
            if (!(hubConnection.State == HubConnectionState.Connecting || hubConnection.State == HubConnectionState.Connected))
            {
                await hubConnection.StartAsync();
            }
            return hubConnection;
        }

        private void SetListeners()
        {
            hubConnection.On("GameOptionsChanged", delegate (string roomKey, GameOptions gameOptions)
            {
                OnOptionsChanged.Invoke(roomKey, gameOptions);
            });

            hubConnection.On("PerformMove", delegate (string roomKey, BoardMove move, SharedClock clock1, SharedClock clock2)
            {
                OnMovePerformed.Invoke(roomKey, move, clock1, clock2);
            });

            hubConnection.On("GameEnded", delegate (string roomKey, PieceColor? winner)
            {
                OnGameEnded.Invoke(roomKey, winner);
            });

            hubConnection.On("PlayerLeft", delegate (string roomKey, string player)
            {
                OnPlayerLeft.Invoke(roomKey, player);
            });

            hubConnection.On("PlayerJoined", delegate (string roomKey, string player)
            {
                OnPlayerJoined.Invoke(roomKey, player);
            });
        }
    }
}
