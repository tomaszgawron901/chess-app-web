using ChessClassLib.Enums;
using ChessClassLib.Logic.Games;
using hessClassLibrary.Logic.Games;

namespace ChessApp.Web.Exstentions
{
    public static class GameVarientExtensions
    {
        public static IClassicGame ConvertToGame(this GameVarient gameVarient)
        {
            switch (gameVarient)
            {
                case GameVarient.Standard:
                    return new ClassicGame();
                case GameVarient.Knightmate:
                    return new KnightmateGame();
                default:
                    return new ClassicGame();
            }
        }
    }
}
