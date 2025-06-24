using Chess.Minimax;

namespace Chess.Chess.Bot
{
    class ChessEndStateEvaluator : IEndStateEvaluator<ChessGame>
    {
        public int EvaluateEndState(ChessGame game)
        {
            if (game.Winner == game.turn) return int.MaxValue - game.HalfmoveCounter;
            if (game.Winner == 0) return 0;
            return -int.MaxValue + game.HalfmoveCounter;
        }
    }
}