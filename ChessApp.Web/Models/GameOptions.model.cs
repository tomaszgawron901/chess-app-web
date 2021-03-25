using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ChessApp.Web.Enums;

namespace ChessApp.Web.Models
{
    public class GameOptions
    {
        [Required]
        public GameVarient GameVarient { get; set; }
        [Required]
        [Range(1, 600, ErrorMessage = "Seconds per side not in range.")]
        public int SecondsPerSide { get; set; }
        [Required]
        [Range(1, 600, ErrorMessage = "Increment in seconds not in range.")]
        public int IncrementInSeconds { get; set; }
        [Required]
        public Side Side { get; set; }

        public GameOptions()
        {
            this.GameVarient = GameVarient.Standard;
            this.SecondsPerSide = 300;
            this.IncrementInSeconds = 8;
            this.Side = Side.White;
        }
    }
}
