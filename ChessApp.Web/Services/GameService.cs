using ChessClassLibrary.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ChessApp.Web.Services
{
    public class GameService
    {
        private readonly IConfiguration configuration;
        private readonly IHttpClientFactory clientFactory;
        public GameService(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.clientFactory = clientFactory;
        }

        public async Task<string> CreateNewGameRoom(GameOptions gameOptions)
        {
            HttpResponseMessage response = await clientFactory
                .CreateClient("chess_server_client")
                .PostAsJsonAsync("api/Game/CreateGameRoom", gameOptions);
            response.EnsureSuccessStatusCode();
            return response.Headers.Location.OriginalString;
        }

        public async Task<GameOptions> GetGameOptionsByKey(string gameKey)
        {
            HttpResponseMessage response = await clientFactory
                .CreateClient("chess_server_client")
                .GetAsync($"api/Game/GetGameOptionsByKey?gameKey={gameKey}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<GameOptions>();
        }

        public async Task<HubConnection> JoinGame(string roomKey, Action<string, GameOptions> onGameOptionsChange=null)
        {
            var connection = new HubConnectionBuilder().WithUrl($"{configuration.GetSection("chess_server_root").Value}gamehub").Build();
            if (onGameOptionsChange != null)
            {
                connection.On<string, GameOptions>("GameOptionsChanged", onGameOptionsChange);
            }
            await connection.StartAsync();

            await connection.InvokeAsync("JoinGame", roomKey);

            return connection;
        }
    }
}
