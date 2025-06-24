using System.Text.RegularExpressions;
using Chess.Games;

namespace Chess.Chess
{
    partial class ChessGame : Game<Move>
    {
        // STATIC CONSTRUCTOR
        static ChessGame()
        {
            Random random = new();

            piece_position_hashes = new int[23, RANKS, FILES];
            for (int x = 0; x < RANKS; x++)
            {
                for (int y = 0; y < FILES; y++)
                {
                    piece_position_hashes[0, x, y] = 0;
                    foreach (int piece in pieces)
                        piece_position_hashes[piece, x, y] = random.Next(int.MinValue, int.MaxValue);
                }
            }

            white_kingside_castle_hash = random.Next(int.MinValue, int.MaxValue);
            white_queenside_castle_hash = random.Next(int.MinValue, int.MaxValue);
            black_kingside_castle_hash = random.Next(int.MinValue, int.MaxValue);
            black_queenside_castle_hash = random.Next(int.MinValue, int.MaxValue);

            black_turn_hash = random.Next(int.MinValue, int.MaxValue);
        }

        // CONSTRUCTORS
        public ChessGame(string starting_position_fen = STANDARD_STARTING_POSITION_FEN)
        {
            board = new int[0, 0];
            history = new();
            en_passant_files = new();
            prev_positions = [];

            LoadFEN(starting_position_fen);
        }
    }
}