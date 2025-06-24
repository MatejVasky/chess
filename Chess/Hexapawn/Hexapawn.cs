using Chess.Games;

namespace Chess.Hexapawn
{
    class HexapawnGame : Game<Move>
    {
        public const int WHITE = 1;
        public const int BLACK = 2;

        public const int FILES = 3;

        private int[,] board;
        public int turn;
        private Stack<Move> history;

        public HexapawnGame()
        {
            board = new int[3, FILES];
            for (int y = 0; y < FILES; y++)
            {
                board[0, y] = WHITE;
                board[1, y] = 0;
                board[2, y] = BLACK;
            }
            turn = WHITE;
            history = new();
        }

        public override List<Move> GetMoves()
        {
            if (HasPawnOnTheLastRank() != 0) return [];

            int fwd = (turn == WHITE) ? 1 : -1;

            List<Move> moves = [];

            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < FILES; y++)
                {
                    if (board[x, y] == turn)
                    {
                        if (board[x + fwd, y] == 0)
                            moves.Add(new Move { from_x = x, from_y = y, to_x = x + fwd, to_y = y, capture = 0 });
                        if (y > 0 && board[x + fwd, y - 1] == 3 - turn)
                            moves.Add(new Move { from_x = x, from_y = y, to_x = x + fwd, to_y = y - 1, capture = board[x + fwd, y - 1] });
                        if (y < FILES - 1 && board[x + fwd, y + 1] == 3 - turn)
                            moves.Add(new Move { from_x = x, from_y = y, to_x = x + fwd, to_y = y + 1, capture = board[x + fwd, y + 1] });
                    }
                }
            }

            return moves;
        }

        public override bool MakeMove(Move move)
        {
            if (!GetMoves().Contains(move))
                return false;

            board[move.to_x, move.to_y] = board[move.from_x, move.from_y];
            board[move.from_x, move.from_y] = 0;

            turn = 3 - turn;
            history.Push(move);

            return true;
        }

        public override bool UndoMakeMove()
        {
            if (!history.TryPop(out Move move))
                return false;

            board[move.from_x, move.from_y] = board[move.to_x, move.to_y];
            board[move.to_x, move.to_y] = move.capture;

            turn = 3 - turn;

            return true;
        }

        public override bool HasEnded()
        {
            return HasPawnOnTheLastRank() != 0 || GetMoves().Count == 0;
        }

        private int HasPawnOnTheLastRank()
        {
            for (int y = 0; y < FILES; y++)
            {
                if (board[2, y] == WHITE) return WHITE;
                if (board[0, y] == BLACK) return BLACK;
            }
            return 0;
        }

        public int Winner
        {
            get
            {
                if (HasPawnOnTheLastRank() != 0) return HasPawnOnTheLastRank();
                if (GetMoves().Count == 0) return 3 - turn;
                return -1;
            }
        }

        public void PrintBoard()
        {
            for (int x = 2; x >= 0; x--)
            {
                for (int y = 0; y < FILES; y++)
                {
                    Console.Write(board[x, y]);
                }
                Console.WriteLine();
            }
        }
    }
}