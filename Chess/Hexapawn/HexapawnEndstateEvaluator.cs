using Chess.Minimax;

namespace Chess.Hexapawn
{
    class HexapawnEndstateEvaluator : IEndStateEvaluator
    {
        public float EvaluateEndState(HexapawnGame game)
        {
            int winner = game.Winner;
            if (winner == game.turn) return 1;
            else if (winner == 0) return 0;
            else return -1;
        }
    }
}