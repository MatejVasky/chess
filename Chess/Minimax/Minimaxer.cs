using Chess.Hexapawn;

namespace Chess.Minimax
{
    class Minimaxer
    {
        private IEndStateEvaluator endStateEvaluator;
        private IStaticEvaluator staticEvaluator;

        public Minimaxer(IEndStateEvaluator endStateEvaluator, IStaticEvaluator staticEvaluator)
        {
            this.endStateEvaluator = endStateEvaluator;
            this.staticEvaluator = staticEvaluator;
        }

        public MinimaxResult Evaluate(HexapawnGame game, int depth)
        {
            if (game.HasEnded())
                return new MinimaxResult(endStateEvaluator.EvaluateEndState(game));
            if (depth == 0)
                return new MinimaxResult(staticEvaluator.Evaluate(game));

            MinimaxResult best_move = new(float.MinValue);
            foreach (Move move in game.GetMoves())
            {
                game.MakeMove(move);
                MinimaxResult result = Evaluate(game, depth - 1);
                result.AddMove(move);
                if (result.Eval > best_move.Eval)
                    best_move = result;
                game.UndoMakeMove();
            }

            return best_move;
        }
    }
}