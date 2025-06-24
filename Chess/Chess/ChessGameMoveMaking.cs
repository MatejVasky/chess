using System.Text.RegularExpressions;
using Chess.Games;

namespace Chess.Chess
{
    partial class ChessGame : Game<Move>
    {
        // MOVEMENT
        public override bool MakeMove(Move move)
        {
            // Check if move is valid
            if (!GetMoves().Contains(move))
                return false;

            // Make the move
            ForceMakeMove(move);
            return true;
        }

        private void ForceMakeMove(Move move)
        {
            // Move piece
            MovePiece(move);

            // Finish castling (if applicable)
            if (move == WHITE_CASTLES_KINGSIDE)
                MovePiece(WHITE_CASTLES_KINGSIDE_ROOK);
            if (move == WHITE_CASTLES_QUEENSIDE)
                MovePiece(WHITE_CASTLES_QUEENSIDE_ROOK);
            if (move == BLACK_CASTLES_KINGSIDE)
                MovePiece(BLACK_CASTLES_KINGSIDE_ROOK);
            if (move == BLACK_CASTLES_QUEENSIDE)
                MovePiece(BLACK_CASTLES_QUEENSIDE_ROOK);

            // Finish en passant (if applicable)
            if (move.en_passant)
                board[move.from_x, move.to_y] = 0;

            // Change castling rights
            can_white_castle_kingside ^= move.stops_white_kingside_castle;
            can_white_castle_queenside ^= move.stops_white_queenside_castle;
            can_black_castle_kingside ^= move.stops_black_kingside_castle;
            can_black_castle_queenside ^= move.stops_black_queenside_castle;

            // Change player's turn & remove cached moves
            turn ^= COLOUR;
            moves = null;
            history.Push(move);
            en_passant_files.Push(move.en_passant_file);
            SetZobristHash();
            if (prev_positions.ContainsKey(hash)) prev_positions[hash] += 1;
            else prev_positions[hash] = 1;
            rule50counter += move.rule50counter_update;
            halfmove_counter++;
        }

        private void MovePiece(Move move)
        {
            // Place piece on the target square
            board[move.to_x, move.to_y] = board[move.from_x, move.from_y] ^ move.promotion;
            // Remove piece from the original square
            board[move.from_x, move.from_y] = 0;
        }

        public override bool UndoMakeMove()
        {
            // Check if history is non-empty
            if (!history.TryPop(out Move move))
                return false;

            // Remove position from previous positions
            prev_positions[hash] -= 1;

            // Move piece
            UndoMovePiece(move);

            // Finish castling (if applicable)
            if (move == WHITE_CASTLES_KINGSIDE)
                UndoMovePiece(WHITE_CASTLES_KINGSIDE_ROOK);
            if (move == WHITE_CASTLES_QUEENSIDE)
                UndoMovePiece(WHITE_CASTLES_QUEENSIDE_ROOK);
            if (move == BLACK_CASTLES_KINGSIDE)
                UndoMovePiece(BLACK_CASTLES_KINGSIDE_ROOK);
            if (move == BLACK_CASTLES_QUEENSIDE)
                UndoMovePiece(BLACK_CASTLES_QUEENSIDE_ROOK);

            // Change castling rights
            can_white_castle_kingside ^= move.stops_white_kingside_castle;
            can_white_castle_queenside ^= move.stops_white_queenside_castle;
            can_black_castle_kingside ^= move.stops_black_kingside_castle;
            can_black_castle_queenside ^= move.stops_black_queenside_castle;

            // Change player's turn & remove cached moves
            turn ^= COLOUR;
            moves = null;
            en_passant_files.Pop();
            SetZobristHash();
            rule50counter -= move.rule50counter_update;
            halfmove_counter--;

            // Return
            return true;
        }

        private void UndoMovePiece(Move move)
        {
            // Place piece back on the original square
            board[move.from_x, move.from_y] = board[move.to_x, move.to_y] ^ move.promotion;
            // Replace piece on the target square
            if (move.en_passant)
            {
                board[move.to_x, move.to_y] = 0;
                board[move.from_x, move.to_y] = move.capture;
            }
            else board[move.to_x, move.to_y] = move.capture;
        }
    }
}