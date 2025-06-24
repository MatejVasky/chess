using Chess.Games;

namespace Chess.Chess
{
    class ConsoleChessPlayer : Player<ChessGame, Move>
    {
        public override bool PlayMove(ChessGame game)
        {
            while (true)
            {
                // Read move
                Console.Write("Move: ");
                string? notation = Console.ReadLine();
                if (notation is null) return false;
                
                // Undo
                if (notation == "undo")
                {
                    if (game.UndoMakeMove())
                        return true;
                    continue;
                }

                // Play move
                try
                {
                    if (game.MakeMove(game.FindMove(notation)))
                        return true;
                }
                catch (KeyNotFoundException) { }
            }
        }
    }
}