using Chess.Games;
using Chess.Minimax;

namespace Chess.Chess.Bot
{
    class ChessBot : Player<ChessGame, Move>
    {
        private int depth;
        private Minimaxer<ChessGame, Move> minimaxer;

        public ChessBot(int depth)
        {
            this.depth = depth;
            minimaxer = new(new ChessEndStateEvaluator(), new ChessStaticEvaluator());
        }

        public override bool PlayMove(ChessGame game)
        {
            MinimaxResult<Move> minimaxResult = minimaxer.Evaluate(game, depth);
            game.MakeMove(minimaxResult.Line[0]);
            return true;
        }
    }
}