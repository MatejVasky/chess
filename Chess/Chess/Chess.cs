using System.Text.RegularExpressions;

namespace Chess.Chess
{
    class ChessGame
    {
        // PIECE TYPES
        public const int PIECE_TYPE = 0b111;
        public const int PAWN = 0b001;
        public const int KNIGHT = 0b010;
        public const int BISHOP = 0b011;
        public const int ROOK = 0b100;
        public const int QUEEN = 0b101;
        public const int KING = 0b110;

        // PIECE COLOURS
        public const int COLOUR = 0b11000;
        public const int WHITE = 0b01000;
        public const int BLACK = 0b10000;

        // PIECE NAMES
        public static int[] pieces = [
            WHITE | PAWN, WHITE | KNIGHT, WHITE | BISHOP, WHITE | ROOK, WHITE | QUEEN, WHITE | KING,
            BLACK | PAWN, BLACK | KNIGHT, BLACK | BISHOP, BLACK | ROOK, BLACK | QUEEN, BLACK | KING,
        ];
        public static char[] piece_names = [
            /* 0: */            '.',
            '!', '!', '!', '!', '!', '!', '!', '!',
            /* WHITE PAWN: */   'P',
            /* WHITE KNIGHT: */ 'N',
            /* WHITE BISHOP: */ 'B',
            /* WHITE ROOK: */   'R',
            /* WHITE QUEEN: */  'Q',
            /* WHITE KING: */   'K',
            '!', '!',
            /* BLACK PAWN: */   'p',
            /* BLACK KNIGHT: */ 'n',
            /* BLACK BISHOP: */ 'b',
            /* BLACK ROOK: */   'r',
            /* BLACK QUEEN: */  'q',
            /* BLACK KING: */   'k',
        ];
        public static Dictionary<char, int> piece_types = new()
        {
            ['N'] = KNIGHT,
            ['B'] = BISHOP,
            ['R'] = ROOK,
            ['Q'] = QUEEN,
            ['K'] = KING
        };

        // KNIGHT MOVEMENT
        public static int[] knight_moves_x = [-2, -2, -1, -1, 1, 1, 2, 2];
        public static int[] knight_moves_y = [-1, 1, -2, 2, -2, 2, -1, 1];
        // BISHOP MOVEMENT
        public static int[] bishop_dirs_x = [-1, -1, 1, 1];
        public static int[] bishop_dirs_y = [-1, 1, -1, 1];
        // ROOK MOVEMENT
        public static int[] rook_dirs_x = [-1, 0, 1, 0];
        public static int[] rook_dirs_y = [0, -1, 0, 1];
        // QUEEN MOVEMENT
        public static int[] queen_dirs_x = [-1, -1, 1, 1, -1, 0, 1, 0];
        public static int[] queen_dirs_y = [-1, 1, -1, 1, 0, -1, 0, 1];
        // KING MOVEMENT
        public static int[] king_moves_x = [-1, -1, -1, 0, 0, 1, 1, 1];
        public static int[] king_moves_y = [-1, 0, 1, -1, 1, -1, 0, 1];
        // PAWN MOVEMENT
        public static int[] pawn_captures_y = [-1, 1];
        public static int[] pawn_promotions = [KNIGHT, BISHOP, ROOK, QUEEN];
        // CASTLES
        public static Move WHITE_CASTLES_KINGSIDE = new() { from_x = 0, from_y = 4, to_x = 0, to_y = 6, capture = 0, promotion = 0, en_passant_file = -1, en_passant = false, stops_white_kingside_castle = true, stops_white_queenside_castle = true, stops_black_kingside_castle = false, stops_black_queenside_castle = false, rule50counter_update = 1 };
        public static Move WHITE_CASTLES_KINGSIDE_ROOK = new() { from_x = 0, from_y = 7, to_x = 0, to_y = 5, capture = 0, promotion = 0, en_passant = false };
        public static Move WHITE_CASTLES_QUEENSIDE = new() { from_x = 0, from_y = 4, to_x = 0, to_y = 2, capture = 0, promotion = 0, en_passant_file = -1, en_passant = false, stops_white_kingside_castle = true, stops_white_queenside_castle = true, stops_black_kingside_castle = false, stops_black_queenside_castle = false, rule50counter_update = 1 };
        public static Move WHITE_CASTLES_QUEENSIDE_ROOK = new() { from_x = 0, from_y = 0, to_x = 0, to_y = 3, capture = 0, promotion = 0, en_passant = false };
        public static Move BLACK_CASTLES_KINGSIDE = new() { from_x = 7, from_y = 4, to_x = 7, to_y = 6, capture = 0, promotion = 0, en_passant_file = -1, en_passant = false, stops_white_kingside_castle = false, stops_white_queenside_castle = false, stops_black_kingside_castle = true, stops_black_queenside_castle = true, rule50counter_update = 1 };
        public static Move BLACK_CASTLES_KINGSIDE_ROOK = new() { from_x = 7, from_y = 7, to_x = 7, to_y = 5, capture = 0, promotion = 0, en_passant = false };
        public static Move BLACK_CASTLES_QUEENSIDE = new() { from_x = 7, from_y = 4, to_x = 7, to_y = 2, capture = 0, promotion = 0, en_passant_file = -1, en_passant = false, stops_white_kingside_castle = false, stops_white_queenside_castle = false, stops_black_kingside_castle = true, stops_black_queenside_castle = true, rule50counter_update = 1 };
        public static Move BLACK_CASTLES_QUEENSIDE_ROOK = new() { from_x = 7, from_y = 0, to_x = 7, to_y = 3, capture = 0, promotion = 0, en_passant = false };

        // HASHING
        private static int[,,] piece_position_hashes;
        private static int white_kingside_castle_hash;
        private static int white_queenside_castle_hash;
        private static int black_kingside_castle_hash;
        private static int black_queenside_castle_hash;
        private static int black_turn_hash;

        // BOARD PROPERTIES
        public const int RANKS = 8;
        public const int FILES = 8;
        public const int PAWN_START_RANK = 1;
        public const int PAWN_PROMOTION_RANK = 7;

        // GAME STATE
        private int[,] board;
        private int turn;
        private List<Move>? moves;
        private bool can_white_castle_kingside, can_white_castle_queenside,
                     can_black_castle_kingside, can_black_castle_queenside;
        private Stack<Move> history;
        private Stack<int> en_passant_files;
        private int hash;
        private Dictionary<int, int> prev_positions;
        private int rule50counter;

        // PROPERTIES
        public int Hash => hash;

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
        public ChessGame()
        {
            board = new int[RANKS, FILES];
            board[0, 0] = 12; board[0, 1] = 10; board[0, 2] = 11; board[0, 3] = 13;
            board[0, 4] = 14; board[0, 5] = 11; board[0, 6] = 10; board[0, 7] = 12;
            for (int file = 0; file < FILES; file++) board[1, file] = 9;
            for (int rank = 2; rank < RANKS - 2; rank++)
                for (int file = 0; file < FILES; file++)
                    board[rank, file] = 0;
            for (int file = 0; file < FILES; file++) board[RANKS - 2, file] = 17;
            board[RANKS - 1, 0] = 20; board[RANKS - 1, 1] = 18;
            board[RANKS - 1, 2] = 19; board[RANKS - 1, 3] = 21;
            board[RANKS - 1, 4] = 22; board[RANKS - 1, 5] = 19;
            board[RANKS - 1, 6] = 18; board[RANKS - 1, 7] = 20;

            can_white_castle_kingside = true;
            can_white_castle_queenside = true;
            can_black_castle_kingside = true;
            can_black_castle_queenside = true;

            // .n..k...
            // P.......
            // ..p.....
            // ...Pp...
            // ........
            // .Q...N.B
            // ....P...
            // R...K..R
            // for (int rank = 0; rank < ranks; rank++)
            //     for (int file = 0; file < files; file++)
            //         board[rank, file] = 0;  
            // board[0, 0] = WHITE | ROOK;
            // board[0, 4] = WHITE | KING;
            // board[0, 7] = WHITE | ROOK;
            // board[1, 4] = WHITE | PAWN;
            // board[2, 1] = WHITE | QUEEN;
            // board[2, 5] = WHITE | KNIGHT;
            // board[2, 7] = WHITE | BISHOP;
            // board[4, 3] = WHITE | PAWN;
            // board[4, 4] = BLACK | PAWN;
            // board[5, 2] = BLACK | PAWN;
            // board[6, 0] = WHITE | PAWN;
            // board[7, 1] = BLACK | KNIGHT;
            // board[7, 4] = BLACK | KING;

            turn = WHITE;
            moves = null;
            en_passant_files = new();
            en_passant_files.Push(-1);

            history = new();
            SetZobristHash();
            prev_positions = [];
            prev_positions[hash] = 1;
            rule50counter = 0;
        }

        // GETTING MOVES
        public List<Move> GetMoves()
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

        // MOVEMENT
        public bool MakeMove(Move move)
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
        }

        private void MovePiece(Move move)
        {
            // Place piece on the target square
            board[move.to_x, move.to_y] = board[move.from_x, move.from_y] ^ move.promotion;
            // Remove piece from the original square
            board[move.from_x, move.from_y] = 0;
        }

        public bool UndoMakeMove()
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

        // MOVEMENT ALGEBRAIC NOTATION
        public bool MoveMatchesNotation(Move move, string notation)
        {
            // TODO: Check checks

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
            CountPiecesOfType(WHITE | BISHOP) == 0 && CountPieces(WHITE | KNIGHT) <= 1) &&
            (CountPieces(BLACK | BISHOP) <= 1 && CountPieces(BLACK | KNIGHT) == 0 ||
            CountPiecesOfType(BLACK | BISHOP) == 0 && CountPieces(BLACK | KNIGHT) <= 1);
        public bool HasEnded()
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