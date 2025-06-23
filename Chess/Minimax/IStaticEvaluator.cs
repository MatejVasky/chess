using Chess.Hexapawn;

namespace Chess.Minimax
{
    interface IStaticEvaluator
    {
        public float Evaluate(HexapawnGame game);
    }

    class ZeroStaticEvaluator : IStaticEvaluator
    {
        public float Evaluate(HexapawnGame game) => 0;
    }
}