namespace Chess.Games
{
    abstract class Player<TGame, TMove> where TGame : Game<TMove>
    {
        public abstract bool PlayMove(TGame game);
    }
}