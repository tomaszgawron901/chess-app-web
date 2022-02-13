using ChessClassLibrary.enums;
using ChessClassLibrary.Models;
using Microsoft.AspNetCore.Components;


namespace ChessApp.Web.Components
{
    public partial class FullGameOptionsForm: ComponentBase
    {
        [Parameter] public GameOptions GameOptions { get; set; }
        [Parameter] public PieceColor? CurrentPlayer { get; set; } = null;
        [Parameter] public PieceColor? UserPlayer { get; set; } = null;

        public string Player1 => GameOptions?.Player1 ?? L["waiting"];
        public string Player2 => GameOptions?.Player2 ?? L["waiting"];

        public string GameVarient {
            get {
                switch (GameOptions?.GameVarient)
                {
                    case null:
                        return @L["not_provided"];
                    case ChessClassLibrary.enums.GameVarient.Standard:
                        return @L["game_varient.standard"];
                    case ChessClassLibrary.enums.GameVarient.Knightmate:
                        return @L["game_varient.knightmate"];
                    default:
                        return @L["unknown_type"];
                }
            }
        }

        public string MinutesPerSide => GameOptions?.MinutesPerSide.ToString()+" min" ?? @L["not_provided"];
        public string IncrementInSeconds => GameOptions?.IncrementInSeconds.ToString()+" sec" ?? @L["not_provided"];
    }
}
