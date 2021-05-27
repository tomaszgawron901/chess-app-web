using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessApp.Web.Components
{
    public class ChatWindowBase : ComponentBase
    {
        protected List<(DateTime time, string message)> MessagesList { get; private set; }

        public ChatWindowBase()
        {
            this.MessagesList = new List<(DateTime time, string message)>();
        }

        public void AddMessage(string message, DateTime? time = null)
        {
            this.MessagesList.Add((time ?? DateTime.Now, message));
            this.StateHasChanged();
        }

    }
}
