using Chess.Hexapawn;

namespace Chess.Minimax
{
    class MinimaxResult
    {
        public float Eval { get; private set; }
        public List<Move> Line { get; private set; }

        public MinimaxResult(float eval)
        {
            Eval = eval;
            Line = [];
        }

        public void AddMove(Move move)
        {
            Eval = -Eval;
            Line.Insert(0, move);
        }
    }
}