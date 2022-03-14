using ChessClassLib.Enums;
using System.ComponentModel.DataAnnotations;

namespace ChessApp.Web.Models
{
    public class CreateGameOptions
    {
        [Required]
        public GameVarient GameVarient { get; set; }

        [Range(1, 60), Required]
        public int MinutesPerSide { get; set; } = 10;

        [Range(0, 600), Required]
        public int IncrementInSeconds { get; set; } = 10;

        [Required]
        public PieceColor Side { get; set; }
    }
}
