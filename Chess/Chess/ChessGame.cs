using System.Text.RegularExpressions;
using Chess.Games;

namespace Chess.Chess
{
    partial class ChessGame : Game<Move>
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
        public static Dictionary<char, int> piece_name_to_int = new()
        {
            ['P'] = WHITE | PAWN,
            ['N'] = WHITE | KNIGHT,
            ['B'] = WHITE | BISHOP,
            ['R'] = WHITE | ROOK,
            ['Q'] = WHITE | QUEEN,
            ['K'] = WHITE | KING,
            ['p'] = BLACK | PAWN,
            ['n'] = BLACK | KNIGHT,
            ['b'] = BLACK | BISHOP,
            ['r'] = BLACK | ROOK,
            ['q'] = BLACK | QUEEN,
            ['k'] = BLACK | KING,
        };
        public static Dictionary<char, int> piece_types = new()
        {
            ['N'] = KNIGHT,
            ['B'] = BISHOP,
            ['R'] = ROOK,
            ['Q'] = QUEEN,
            ['K'] = KING
        };
        public static string[] piece_type_names = [
            /* 0: */      "!",
            /* PAWN: */   "",
            /* KNIGHT: */ "N",
            /* BISHOP: */ "B",
            /* ROOK: */   "R",
            /* QUEEN: */  "Q",
            /* KING: */   "K"
        ];
        public static Dictionary<char, int> colour_name_to_int = new()
        {
            ['w'] = WHITE,
            ['b'] = BLACK
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

        // STARTING POSITION
        public const string STANDARD_STARTING_POSITION_FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        // BOARD PROPERTIES
        public const int RANKS = 8;
        public const int FILES = 8;
        public const int PAWN_START_RANK = 1;
        public const int PAWN_PROMOTION_RANK = 7;

        // GAME STATE
        private int[,] board;
        public int turn;
        private List<Move>? moves;
        private bool can_white_castle_kingside, can_white_castle_queenside,
                     can_black_castle_kingside, can_black_castle_queenside;
        private Stack<Move> history;
        private Stack<int> en_passant_files;
        private int hash;
        private Dictionary<int, int> prev_positions;
        private int rule50counter;
        private int halfmove_counter;



        // PROPERTIES
        public int Hash => hash;
        public int Turn => turn;
        public int HalfmoveCounter => halfmove_counter;
        public int MoveCounter => halfmove_counter / 2;
    }
}