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
        private readonly HubConnection hubConnection;
        public GameService(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.hubConnection = this.GetHubConnection();
            this.hubConnection.StartAsync();
        }

        public async Task<HubConnection> EnsureIsConnected()
        {
            if(!(this.hubConnection.State == HubConnectionState.Connecting || this.hubConnection.State == HubConnectionState.Connected))
            {
                await this.hubConnection.StartAsync();
            }
            return this.hubConnection;
        }

        public Task<string> CreateNewGameRoom(GameOptions gameOptions)
        {
            return this.EnsureIsConnected()
                .ContinueWith(hubTask =>
                {
                    return hubTask.Result.InvokeAsync<string>("CreateGameRoom", gameOptions);
                }).Unwrap();
        }

        private HubConnection GetHubConnection()
        {
            return new HubConnectionBuilder().WithUrl($"{configuration.GetSection("chess_server_root").Value}gamehub").Build();
        }
    }
}
