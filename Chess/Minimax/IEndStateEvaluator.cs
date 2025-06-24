namespace Chess.Minimax
{
    interface IEndStateEvaluator<GameType>
    {
        public int EvaluateEndState(GameType game);
    }
}