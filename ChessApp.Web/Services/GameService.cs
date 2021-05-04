using ChessClassLibrary.Models;
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
    }
}
