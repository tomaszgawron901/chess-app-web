using ChessClassLib.Enums;
using ChessClassLib.Models;
using Microsoft.AspNetCore.Components;


namespace ChessApp.Web.Components
{
    public partial class FullGameOptionsForm: ComponentBase
    {
        private PieceColor? UserPlayer { get; set; } = null;
        private GameOptions GameOptions { get; set; }
        private PieceColor? CurrentPlayer { get; set; } = null;

        public void SetGameOptions(GameOptions gameOptions)
        {
            GameOptions = gameOptions;
            StateHasChanged();
        }

        public void SetCurrentPlayer(PieceColor? color)
        {
            CurrentPlayer = color;
            StateHasChanged();
        }

        public void SetUserPlayer(PieceColor? color)
        {
            UserPlayer = color;
            StateHasChanged();
        }

        private string Player1 => GameOptions?.Player1 ?? L["waiting"];
        private string Player2 => GameOptions?.Player2 ?? L["waiting"];

        private string GameVarient {
            get {
                switch (GameOptions?.GameVarient)
                {
                    case null:
                        return @L["not_provided"];
                    case ChessClassLib.Enums.GameVarient.Standard:
                        return @L["game_varient.standard"];
                    case ChessClassLib.Enums.GameVarient.Knightmate:
                        return @L["game_varient.knightmate"];
                    default:
                        return @L["unknown_type"];
                }
            }
        }

        private string MinutesPerSide => GameOptions?.MinutesPerSide.ToString()+" min" ?? @L["not_provided"];
        private string IncrementInSeconds => GameOptions?.IncrementInSeconds.ToString()+" sec" ?? @L["not_provided"];
    }
}
