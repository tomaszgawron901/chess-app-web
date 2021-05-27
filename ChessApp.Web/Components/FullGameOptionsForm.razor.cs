using ChessApp.Web.Enums;
using ChessApp.Web.Models;
using ChessClassLibrary.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ChessApp.Web.Components
{
    public class FullGameOptionsFormBase: ComponentBase
    {
        [Parameter] public GameOptions GameOptions { get; set; }
        [Inject] public IStringLocalizer<App> L{ get; set; }

        public string Player1 => GameOptions?.Player1 ?? L["waiting"];
        public string Player2 => GameOptions?.Player2 ?? L["waiting"];

        public string GameVarient {
            get {
                switch (GameOptions?.GameVarient)
                {
                    case null:
                        return @L["not_provided"];
                    case ChessClassLibrary.enums.GameVarient:
                        return @L["game_varient.standard"];
                    default:
                        return @L["unknown_type"];
                }
            }
        }

        public string MinutesPerSide => GameOptions?.MinutesPerSide.ToString()+" min" ?? @L["not_provided"];
        public string IncrementInSeconds => GameOptions?.IncrementInSeconds.ToString()+" sec" ?? @L["not_provided"];

    }
}
