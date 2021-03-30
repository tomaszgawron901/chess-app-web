using ChessApp.Web.Enums;
using ChessApp.Web.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessApp.Web.Components
{
    public class FullGameOptionsFormModel
    {
        public string Player1 { get; set; }
        public string Player2 { get; set; }
        public GameVarient GameVarient { get; set; }
        public int SecondsPerSide { get; set; }
        public int IncrementInSeconds { get; set; }
    }

    public class FullGameOptionsFormBase: ComponentBase
    {
        [Parameter] public FullGameOptionsFormModel FullGameOptions { get; set; }
    }
}
