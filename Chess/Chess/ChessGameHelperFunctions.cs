using System.Text.RegularExpressions;
using Chess.Games;

namespace Chess.Chess
{
    partial class ChessGame : Game<Move>
    {
        // HELPER FUNCTIONS
        private static bool IsSqOutOfBounds(int x, int y) => x < 0 || x >= RANKS || y < 0 || y >= FILES;

        private void FindKing(int colour, out int king_x, out int king_y)
        {
            for (int x = 0; x < RANKS; x++)
                for (int y = 0; y < FILES; y++)
                    if (board[x, y] == (colour | KING))
                    {
                        king_x = x; king_y = y;
                        return;
                    }
            king_x = -1; king_y = -1;
        }

        private int CountPiecesOfType(int piece_type)
        {
            int count = 0;
            for (int x = 0; x < RANKS; x++)
                for (int y = 0; y < FILES; y++)
                    if ((board[x, y] & PIECE_TYPE) == piece_type)
                        count++;
            return count;
        }

        private int CountPieces(int piece)
        {
            int count = 0;
            for (int x = 0; x < RANKS; x++)
                for (int y = 0; y < FILES; y++)
                    if (board[x, y] == piece)
                        count++;
            return count;
        }

        private void SetZobristHash()
        {
            // Initialise
            hash = 0;
            // Add piece positions
            for (int x = 0; x < RANKS; x++)
                for (int y = 0; y < FILES; y++)
                    hash ^= piece_position_hashes[board[x, y], x, y];
            // Add castling rights
            if (can_white_castle_kingside) hash ^= white_kingside_castle_hash;
            if (can_white_castle_queenside) hash ^= white_queenside_castle_hash;
            if (can_black_castle_kingside) hash ^= black_kingside_castle_hash;
            if (can_black_castle_queenside) hash ^= black_queenside_castle_hash;
            // Add turn
            if (turn == BLACK) hash ^= black_turn_hash;
        }
    }
}