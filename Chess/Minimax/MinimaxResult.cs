namespace Chess.Minimax
{
    class MinimaxResult<TMove>
    {
        public int Eval { get; private set; }
        public List<TMove> Line { get; private set; }

        public MinimaxResult(int eval)
        {
            Eval = eval;
            Line = [];
        }

        public void AddMove(TMove move)
        {
            Eval = -Eval;
            Line.Insert(0, move);
        }
    }
}