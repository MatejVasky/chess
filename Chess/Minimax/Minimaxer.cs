using Chess.Games;

namespace Chess.Minimax
{
    class Minimaxer<TGame, TMove> where TGame : Game<TMove>
    {
        private IEndStateEvaluator<TGame> endStateEvaluator;
        private IStaticEvaluator<TGame> staticEvaluator;

        public Minimaxer(IEndStateEvaluator<TGame> endStateEvaluator, IStaticEvaluator<TGame> staticEvaluator)
        {
            this.endStateEvaluator = endStateEvaluator;
            this.staticEvaluator = staticEvaluator;
        }

        public MinimaxResult<TMove> Evaluate(TGame game, int depth, int alpha = int.MinValue, int beta = int.MaxValue)
        {
            if (game.HasEnded())
                return new(endStateEvaluator.EvaluateEndState(game));
            if (depth == 0)
                return new(staticEvaluator.Evaluate(game));

            MinimaxResult<TMove> best_move = new(int.MinValue);
            foreach (TMove move in game.GetMoves())
            {
                game.MakeMove(move);
                MinimaxResult<TMove> result = Evaluate(game, depth - 1, -beta, -alpha);
                result.AddMove(move);
                if (result.Eval > best_move.Eval)
                    best_move = result;
                game.UndoMakeMove();

                if (best_move.Eval >= beta)
                    break;
                alpha = int.Max(alpha, best_move.Eval);
            }

            return best_move;
        }

        // public MinimaxResult Evaluate(HexapawnGame game, int depth)
        // {
        //     if (game.HasEnded())
        //         return new MinimaxResult(endStateEvaluator.EvaluateEndState(game));
        //     if (depth == 0)
        //         return new MinimaxResult(staticEvaluator.Evaluate(game));

        //     MinimaxResult best_move = new(float.MinValue);
        //     foreach (Move move in game.GetMoves())
        //     {
        //         game.MakeMove(move);
        //         MinimaxResult result = Evaluate(game, depth - 1);
        //         result.AddMove(move);
        //         if (result.Eval > best_move.Eval)
        //             best_move = result;
        //         game.UndoMakeMove();
        //     }

        //     return best_move;
        // }
    }
}