using Chess.Hexapawn;
using Chess.Games;

namespace Chess.Minimax
{
    // interface IStaticEvaluator
    // {
    //     public float Evaluate(HexapawnGame game);
    // }

    interface IStaticEvaluator<TGame>
    {
        public int Evaluate(TGame game);
    }

    class ZeroStaticEvaluator : IStaticEvaluator<HexapawnGame>
    {
        public int Evaluate(HexapawnGame game) => 0;
    }
}