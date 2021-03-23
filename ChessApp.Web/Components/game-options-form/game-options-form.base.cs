using ChessApp.Web.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChessApp.Web.Components
{
    public class GameOptionsFormBase: ComponentBase
    {
        protected GameOptions optionsModel { get; set; }
    }
}
