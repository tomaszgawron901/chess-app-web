using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;

namespace ChessApp.Web.Components
{
    public partial class ActivityLog : ComponentBase
    {
        protected List<(DateTime time, string message)> MessagesList { get; private set; }

        public ActivityLog()
        {
            MessagesList = new List<(DateTime time, string message)>();
        }

        public void AddMessage(string message, DateTime? time = null)
        {
            MessagesList.Add((time ?? DateTime.Now, message));
            StateHasChanged();
        }

    }
}
