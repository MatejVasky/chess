using System.Text.RegularExpressions;
using Chess.Games;

namespace Chess.Chess
{
    partial class ChessGame : Game<Move>
    {
        // FEN
        private void LoadFEN(string FEN)
        {
            // Split FEN
            string[] parts = FEN.Split(' ');
            if (parts.Length != 6) throw new ArgumentException("Invalid FEN");
            string position = parts[0], turn = parts[1], castling_rights = parts[2],
                en_passant_square = parts[3], halfmove_clock = parts[4], move_counter = parts[5];

            // Load position
            board = new int[RANKS, FILES];
            int x = RANKS - 1, y = 0;
            foreach (char piece_char in position)
            {
                if (piece_char == '/') { x -= 1; y = 0; continue; }
                if (x < 0 || y >= FILES) throw new ArgumentException("Invalid board position");
                if ('1' <= piece_char && piece_char <= '0' + FILES) { y += piece_char - '0'; }
                else
                {
                    if (!piece_name_to_int.TryGetValue(piece_char, out board[x, y]))
                        throw new ArgumentException("Invalid piece in board position");
                    y += 1;
                }
            }

            // Set turn
            if (turn.Length != 1) throw new ArgumentException("Turn must be of length 1");
            if (!colour_name_to_int.TryGetValue(turn[0], out this.turn))
                throw new ArgumentException("Invalid turn");

            // Set moves
            moves = null;

            // Set castling rights
            can_white_castle_kingside = false; can_white_castle_queenside = false;
            can_black_castle_kingside = false; can_black_castle_queenside = false;
            if (castling_rights != "-")
                foreach (char castling_right in castling_rights)
                {
                    switch (castling_right)
                    {
                        case 'K':
                            can_white_castle_kingside = true;
                            break;
                        case 'Q':
                            can_white_castle_queenside = true;
                            break;
                        case 'k':
                            can_black_castle_kingside = true;
                            break;
                        case 'q':
                            can_black_castle_queenside = true;
                            break;
                        default:
                            throw new ArgumentException("Invalid castling right");
                    }
                }

            // Set history
            history = new();

            // Set en passant file
            en_passant_files = new();
            if (en_passant_square == "-")
                en_passant_files.Push(-1);
            else
            {
                if (en_passant_square.Length != 2) throw new ArgumentException("Invalid en passant square");
                if ('a' > en_passant_square[0] || en_passant_square[0] > 'h')
                    throw new ArgumentException("Invalid en passant square");
                if ('1' > en_passant_square[1] || en_passant_square[1] > '8')
                    throw new ArgumentException("Invalid en passant square");
                en_passant_files.Push(en_passant_square[0] - 'a');
            }

            // Set hash
            SetZobristHash();

            // Set previous positions
            prev_positions = [];
            prev_positions[hash] = 1;

            // Set 50-move rule counter
            if (!int.TryParse(halfmove_clock, out rule50counter))
                throw new ArgumentException("Invalid 50-move rule clock");

            // Set half-move counter
            if (!int.TryParse(move_counter, out int move_counter_int))
                throw new ArgumentException("Invalid move counter");
            halfmove_counter = move_counter_int * 2 - 1;
            if (this.turn == BLACK) halfmove_counter++;
        }

        // MOVEMENT ALGEBRAIC NOTATION
        public bool MoveMatchesNotation(Move move, string notation)
        {
            // TODO: Check checks
            // TODO: xf6 is not a valid notation for pawn captures

            // Castles
            if (notation == "O-O" || notation == "O-O+" || notation == "O-O#")
                return move == WHITE_CASTLES_KINGSIDE || move == BLACK_CASTLES_KINGSIDE;
            if (notation == "O-O-O" || notation == "O-O-O+" || notation == "O-O-O#")
                return move == WHITE_CASTLES_QUEENSIDE || move == BLACK_CASTLES_QUEENSIDE;

            // Match pattern
            string pattern = @"^([NBRQK]?)([a-h]?)([1-8]?)(x?)([a-h])([1-8])(?:=([NBRQK]))?([+#])?$";
            Match match = Regex.Match(notation, pattern);
            if (!match.Success)
                return false;

            // Get values
            string piece = match.Groups[1].Value;
            string fy = match.Groups[2].Value;
            string fx = match.Groups[3].Value;
            string cap = match.Groups[4].Value;
            string ty = match.Groups[5].Value;
            string tx = match.Groups[6].Value;
            string prom = match.Groups[7].Value;
            string check = match.Groups[8].Value;

            // Check destination
            int to_x = char.Parse(tx) - '1', to_y = char.Parse(ty) - 'a';
            if (move.to_x != to_x || move.to_y != to_y) return false;

            // Check origin
            if (fx != "")
            {
                int from_x = char.Parse(fx) - '1';
                if (move.from_x != from_x) return false;
            }
            if (fy != "")
            {
                int from_y = char.Parse(fy) - 'a';
                if (move.from_y != from_y) return false;
            }

            // Check piece type
            int piece_type;
            if (piece == "") piece_type = PAWN;
            else if (!piece_types.TryGetValue(char.Parse(piece), out piece_type))
                return false;
            if ((board[move.from_x, move.from_y] & PIECE_TYPE) != piece_type)
                return false;

            // Check capture
            if ((cap == "x" && move.capture == 0) || (cap == "" && move.capture != 0))
                return false;
            if (cap == "x" && piece == "" && fy == "")
                return false;

            // Check promotion
            if (prom == "")
                return move.promotion == 0;
            if (!piece_types.TryGetValue(char.Parse(prom), out int promotion)) return false;
            if (move.promotion != (piece_type ^ promotion)) return false;
            return true;
        }

        public Move FindMove(string notation)
        {
            Move? move = null;
            foreach (Move m in GetMoves())
            {
                if (MoveMatchesNotation(m, notation))
                {
                    if (move is null) move = m;
                    else throw new KeyNotFoundException("Ambiguous notation");
                }
            }
            if (move is null) throw new KeyNotFoundException("No matching move found");
            return (Move)move;
        }

        public string ToAlgebraicNotation(Move move)
        {
            if (!GetMoves().Contains(move))
                throw new ArgumentException("Invalid move");

            int piece_type = board[move.from_x, move.from_y] & PIECE_TYPE;
            string piece_char = piece_type_names[piece_type];
            char from_file = (char)('a' + move.from_y);
            char from_rank = (char)('1' + move.from_x);
            string captures = (move.capture == 0) ? "" : "x";
            char to_file = (char)('a' + move.to_y);
            char to_rank = (char)('1' + move.to_x);
            string promotion = (move.promotion == 0) ? "" : $"={piece_type_names[piece_type ^ move.promotion]}";
            // TODO: Checks & Checkmates

            // Pick the right notation option
            string[] notation_options = [
                piece_char + captures + to_file + to_rank + promotion,
                piece_char + from_file + captures + to_file + to_rank + promotion,
                piece_char + from_rank + captures + to_file + to_rank + promotion
            ];
            foreach (string notation_option in notation_options)
            {
                try
                {
                    FindMove(notation_option);
                    return notation_option;
                }
                catch (KeyNotFoundException) { }
            }

            return piece_char + from_file + from_rank + captures + to_file + to_rank + promotion;
        }
        
        // PRINTING
        public void PrintBoard()
        {
            for (int rank = RANKS - 1; rank >= 0; rank--)
            {
                for (int file = 0; file < FILES; file++)
                    Console.Write(piece_names[board[rank, file]]);
                Console.WriteLine();
            }
        }
    }
}