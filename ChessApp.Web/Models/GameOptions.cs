using ChessClassLib.Enums;
using System.ComponentModel.DataAnnotations;

namespace ChessApp.Web.Models
{
    public class GameOptions
    {
        public string Player1 { get; set; }
        public string Player2 { get; set; }
        public GameVarient GameVarient { get; set; }
        public int MinutesPerSide { get; set; }
        public int IncrementInSeconds { get; set; }
        public PieceColor Side { get; set; }
    }
}
