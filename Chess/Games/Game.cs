namespace Chess.Games
{
    abstract class Game<TMove>
    {
        public abstract bool HasEnded();
        public abstract List<TMove> GetMoves();
        public abstract bool MakeMove(TMove move);
        public abstract bool UndoMakeMove();
    }
}