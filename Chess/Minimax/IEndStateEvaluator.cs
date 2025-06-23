using Chess.Hexapawn;

namespace Chess.Minimax
{
    interface IEndStateEvaluator
    {
        public float EvaluateEndState(HexapawnGame game);
    }
}