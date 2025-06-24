using System.Text.RegularExpressions;
using Chess.Games;

namespace Chess.Chess
{
    partial class ChessGame : Game<Move>
    {
        // GETTING MOVES
        public override List<Move> GetMoves()
        {
            // Check cache
            if (this.moves is not null)
                return this.moves;

            // Create list
            List<Move> moves = [];

            // Check if game is not a draw
            if (IsDrawByRepetition() || IsDrawBy50MoveRule() || IsDrawByInsufficientMaterial())
                return moves;

            // Add piece moves
            for (int x = 0; x < RANKS; x++)
                for (int y = 0; y < FILES; y++)
                    if ((board[x, y] & COLOUR) == turn)
                        AddPieceMoves(moves, x, y);

            // Add castles
            AddCastlingMoves(moves);

            // Cache result
            this.moves = moves;

            // Return
            return moves;
        }

        private void AddPieceMoves(List<Move> moves, int x, int y)
        {
            switch (board[x, y] & PIECE_TYPE)
            {
                case PAWN:
                    AddPawnMoves(moves, x, y);
                    break;
                case KNIGHT:
                    AddFiniteMoves(moves, x, y, knight_moves_x, knight_moves_y);
                    break;
                case BISHOP:
                    AddMovesInDirections(moves, x, y, bishop_dirs_x, bishop_dirs_y);
                    break;
                case ROOK:
                    AddMovesInDirections(moves, x, y, rook_dirs_x, rook_dirs_y);
                    break;
                case QUEEN:
                    AddMovesInDirections(moves, x, y, queen_dirs_x, queen_dirs_y);
                    break;
                case KING:
                    AddFiniteMoves(moves, x, y, king_moves_x, king_moves_y);
                    break;
            }
        }

        private void AddFiniteMoves(List<Move> moves, int from_x, int from_y, int[] dirs_x, int[] dirs_y)
        {
            for (int i = 0; i < dirs_x.Length; i++)
            {
                // Compute target square
                int to_x = from_x + dirs_x[i],
                    to_y = from_y + dirs_y[i];
                // Check if in bounds
                if (IsSqOutOfBounds(to_x, to_y))
                    continue;
                // Check if not occupied by a piece of the same colour
                if ((board[to_x, to_y] & COLOUR) == turn) continue;
                // Add move
                RegisterMove(moves, from_x, from_y, to_x, to_y);
            }
        }

        private void AddMovesInDirections(List<Move> moves, int from_x, int from_y, int[] dirs_x, int[] dirs_y)
        {
            for (int i = 0; i < dirs_x.Length; i++)
            {
                int to_x = from_x, to_y = from_y;
                while (true)
                {
                    // Compute target square
                    to_x += dirs_x[i];
                    to_y += dirs_y[i];
                    // Check if in bounds
                    if (IsSqOutOfBounds(to_x, to_y))
                        break;
                    // Check if not occupied by a piece of the same colour
                    if ((board[to_x, to_y] & COLOUR) == turn) break;
                    // Add move
                    RegisterMove(moves, from_x, from_y, to_x, to_y);
                    // Check if occupied by a piece
                    if (board[to_x, to_y] != 0) break;
                }
            }
        }

        private void AddPawnMoves(List<Move> moves, int from_x, int from_y)
        {
            // Determine the forward direction
            int fwd = (turn == WHITE) ? 1 : -1;
            int to_x = from_x + fwd;
            int to_x2 = to_x + fwd;
            int start_rank = (turn == WHITE) ? PAWN_START_RANK : RANKS - PAWN_START_RANK - 1;
            int prom_rank = (turn == WHITE) ? PAWN_PROMOTION_RANK :
                            RANKS - PAWN_PROMOTION_RANK - 1;
            int en_passant_rank = RANKS - 1 - (start_rank + fwd);
            // Advance moves
            if (!IsSqOutOfBounds(to_x, from_y) && board[to_x, from_y] == 0)
            {
                AddPawnMove(moves, from_x, from_y, to_x, from_y, prom_rank);
                if (from_x == start_rank && !IsSqOutOfBounds(to_x2, from_y) &&
                    board[to_x2, from_y] == 0)
                {
                    AddPawnMove(moves, from_x, from_y, to_x2, from_y, prom_rank, en_passant_file: from_y);
                }
            }
            // Captures
            foreach (int dir_y in pawn_captures_y)
            {
                int to_y = from_y + dir_y;
                if (!IsSqOutOfBounds(to_x, to_y))
                {
                    if ((board[to_x, to_y] & COLOUR) == (turn ^ COLOUR))
                        AddPawnMove(moves, from_x, from_y, to_x, to_y, prom_rank);
                    else if (to_x == en_passant_rank && to_y == en_passant_files.Peek())
                        AddPawnMove(moves, from_x, from_y, to_x, to_y, prom_rank, en_passant: true);
                }

            }
        }

        private void AddPawnMove(List<Move> moves, int from_x, int from_y, int to_x, int to_y, int prom_rank, int en_passant_file = -1, bool en_passant = false)
        {
            if (to_x == prom_rank)
                foreach (int piece in pawn_promotions)
                    RegisterMove(moves, from_x, from_y, to_x, to_y, PAWN ^ piece, en_passant_file,
                        en_passant);
            else
                RegisterMove(moves, from_x, from_y, to_x, to_y, en_passant_file: en_passant_file,
                    en_passant: en_passant);
        }

        private void AddCastlingMoves(List<Move> moves)
        {
            if (turn == WHITE)
            {
                if (can_white_castle_kingside && board[0, 5] == 0 && board[0, 6] == 0)
                    RegisterMove(moves, WHITE_CASTLES_KINGSIDE);
                if (can_white_castle_queenside && board[0, 3] == 0 && board[0, 2] == 0 && board[0, 1] == 0)
                    RegisterMove(moves, WHITE_CASTLES_QUEENSIDE);
            }
            if (turn == BLACK)
            {
                if (can_black_castle_kingside && board[7, 5] == 0 && board[7, 6] == 0)
                    RegisterMove(moves, BLACK_CASTLES_KINGSIDE);
                if (can_black_castle_queenside && board[7, 3] == 0 && board[7, 2] == 0 && board[7, 1] == 0)
                    RegisterMove(moves, BLACK_CASTLES_QUEENSIDE);
            }
        }

        private void RegisterMove(List<Move> moves, int from_x, int from_y, int to_x, int to_y, int promotion = 0, int en_passant_file = -1, bool en_passant = false)
        {
            // Check castling rights changes
            bool stops_white_kingside_castle = can_white_castle_kingside && (
                (from_x == WHITE_CASTLES_KINGSIDE.from_x && from_y == WHITE_CASTLES_KINGSIDE.from_y) ||
                (from_x == WHITE_CASTLES_KINGSIDE_ROOK.from_x && from_y == WHITE_CASTLES_KINGSIDE_ROOK.from_y) ||
                (to_x == WHITE_CASTLES_KINGSIDE_ROOK.from_x && to_y == WHITE_CASTLES_KINGSIDE_ROOK.from_y)
            );
            bool stops_white_queenside_castle = can_white_castle_queenside && (
                (from_x == WHITE_CASTLES_QUEENSIDE.from_x && from_y == WHITE_CASTLES_QUEENSIDE.from_y) ||
                (from_x == WHITE_CASTLES_QUEENSIDE_ROOK.from_x && from_y == WHITE_CASTLES_QUEENSIDE_ROOK.from_y) ||
                (to_x == WHITE_CASTLES_QUEENSIDE_ROOK.from_x && to_y == WHITE_CASTLES_QUEENSIDE_ROOK.from_y)
            );
            bool stops_black_kingside_castle = can_black_castle_kingside && (
                (from_x == BLACK_CASTLES_KINGSIDE.from_x && from_y == BLACK_CASTLES_KINGSIDE.from_y) ||
                (from_x == BLACK_CASTLES_KINGSIDE_ROOK.from_x && from_y == BLACK_CASTLES_KINGSIDE_ROOK.from_y) ||
                (to_x == BLACK_CASTLES_KINGSIDE_ROOK.from_x && to_y == BLACK_CASTLES_KINGSIDE_ROOK.from_y)
            );
            bool stops_black_queenside_castle = can_black_castle_queenside && (
                (from_x == BLACK_CASTLES_QUEENSIDE.from_x && from_y == BLACK_CASTLES_QUEENSIDE.from_y) ||
                (from_x == BLACK_CASTLES_QUEENSIDE_ROOK.from_x && from_y == BLACK_CASTLES_QUEENSIDE_ROOK.from_y) ||
                (to_x == BLACK_CASTLES_QUEENSIDE_ROOK.from_x && to_y == BLACK_CASTLES_QUEENSIDE_ROOK.from_y)
            );
            int rule50update = (board[to_x, to_y] != 0 ||
                                    (board[from_x, from_y] & PIECE_TYPE) == PAWN) ?
                               -rule50counter : 1;

            // Add move
            RegisterMove(moves, new Move()
            {
                from_x = from_x,
                from_y = from_y,
                to_x = to_x,
                to_y = to_y,
                capture = en_passant ? board[from_x, to_y] : board[to_x, to_y],
                promotion = promotion,
                en_passant_file = en_passant_file,
                en_passant = en_passant,
                stops_white_kingside_castle = stops_white_kingside_castle,
                stops_white_queenside_castle = stops_white_queenside_castle,
                stops_black_kingside_castle = stops_black_kingside_castle,
                stops_black_queenside_castle = stops_black_queenside_castle,
                rule50counter_update = rule50update
            });
        }

        private void RegisterMove(List<Move> moves, Move move)
        {
            // Check if doesn't lead to a check
            ForceMakeMove(move);
            bool leadsToCheck = IsInCheck(turn ^ COLOUR);
            UndoMakeMove();

            // Add move
            if (!leadsToCheck)
                moves.Add(move);
        }
    }
}