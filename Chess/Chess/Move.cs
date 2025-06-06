using System.Numerics;

namespace Chess.Chess
{
    public struct Move
    {
        public int from_x, from_y;
        public int to_x, to_y;
        public int capture;
        public int promotion;
        public int en_passant_file;
        public bool en_passant;
        public bool stops_white_kingside_castle, stops_white_queenside_castle,
                    stops_black_kingside_castle, stops_black_queenside_castle;
        public int rule50counter_update;

        public static bool operator ==(Move a, Move b) => a.Equals(b);
        public static bool operator !=(Move a, Move b) => !a.Equals(b);
    }
}