using System.Text.RegularExpressions;
using Chess.Games;

namespace Chess.Chess
{
    partial class ChessGame : Game<Move>
    {
        // CHECKS
        public bool FiniteMovesSeesSquare(int from_x, int from_y, int to_x, int to_y, int[] dirs_x, int[] dirs_y)
        {
            for (int i = 0; i < dirs_x.Length; i++)
                if (to_x == from_x + dirs_x[i] && to_y == from_y + dirs_y[i])
                    return true;
            return false;
        }

        public bool DirectionMovesSeesSquare(int from_x, int from_y, int to_x, int to_y, int[] dirs_x, int[] dirs_y)
        {
            for (int i = 0; i < dirs_x.Length; i++)
            {
                int sq_x = from_x + dirs_x[i], sq_y = from_y + dirs_y[i];
                while (!IsSqOutOfBounds(sq_x, sq_y))
                {
                    if (to_x == sq_x && to_y == sq_y)
                        return true;
                    if (board[sq_x, sq_y] != 0)
                        break;
                    sq_x += dirs_x[i]; sq_y += dirs_y[i];
                }
            }

            return false;
        }

        public bool PawnSeesSquare(int from_x, int from_y, int to_x, int to_y, int colour)
        {
            int fwd = (colour == WHITE) ? 1 : -1;
            return to_x == from_x + fwd && (to_y == from_y + 1 || to_y == from_y - 1);
        }

        public bool PieceSeesSquare(int from_x, int from_y, int to_x, int to_y, int piece)
        {
            switch (piece & PIECE_TYPE)
            {
                case PAWN:
                    return PawnSeesSquare(from_x, from_y, to_x, to_y, piece & COLOUR);
                case KNIGHT:
                    return FiniteMovesSeesSquare(from_x, from_y, to_x, to_y, knight_moves_x, knight_moves_y);
                case BISHOP:
                    return DirectionMovesSeesSquare(from_x, from_y, to_x, to_y, bishop_dirs_x, bishop_dirs_y); ;
                case ROOK:
                    return DirectionMovesSeesSquare(from_x, from_y, to_x, to_y, rook_dirs_x, rook_dirs_y);
                case QUEEN:
                    return DirectionMovesSeesSquare(from_x, from_y, to_x, to_y, queen_dirs_x, queen_dirs_y);
                case KING:
                    return FiniteMovesSeesSquare(from_x, from_y, to_x, to_y, king_moves_x, king_moves_y);
                default:
                    throw new ArgumentException("Invalid piece");
            }
        }

        public bool IsInCheck(int colour)
        {
            // Find king
            FindKing(colour, out int king_x, out int king_y);

            // Check if some piece sees the king
            for (int x = 0; x < RANKS; x++)
            {
                for (int y = 0; y < FILES; y++)
                {
                    if ((board[x, y] & COLOUR) == (colour ^ COLOUR) &&
                        PieceSeesSquare(x, y, king_x, king_y, board[x, y]))
                    {
                        return true;
                    }

                }
            }

            return false;
        }

        // GAME RESULTS
        public bool IsCheckmate() => IsInCheck(turn) && GetMoves().Count == 0;
        public bool IsStalemate() => !IsInCheck(turn) && GetMoves().Count == 0;
        public bool IsDrawByRepetition() => prev_positions[hash] >= 3;
        public bool IsDrawBy50MoveRule() => rule50counter >= 100;
        public bool IsDrawByInsufficientMaterial() => CountPiecesOfType(QUEEN) == 0 &&
            CountPiecesOfType(ROOK) == 0 && CountPiecesOfType(PAWN) == 0 &&
            (CountPieces(WHITE | BISHOP) <= 1 && CountPieces(WHITE | KNIGHT) == 0 ||
            CountPieces(WHITE | BISHOP) == 0 && CountPieces(WHITE | KNIGHT) <= 1) &&
            (CountPieces(BLACK | BISHOP) <= 1 && CountPieces(BLACK | KNIGHT) == 0 ||
            CountPieces(BLACK | BISHOP) == 0 && CountPieces(BLACK | KNIGHT) <= 1);
        public override bool HasEnded()
        {
            return IsCheckmate() || IsStalemate() || IsDrawByRepetition() || IsDrawBy50MoveRule() ||
                IsDrawByInsufficientMaterial();
        }

        public int Winner
        {
            get
            {
                if (IsCheckmate()) return turn ^ COLOUR;
                if (HasEnded()) return 0;
                return -1;
            }
        }
    }
}