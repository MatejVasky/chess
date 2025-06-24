using Chess.Games;
using Chess.Chess;
using Chess.Chess.Bot;
using System.Collections;
using System.Data;
using System.Diagnostics;

namespace Chess.Benchmarking
{
    static class MatePuzzlesBenchmark
    {
        // public static string[] mate_in_1 = ["1rb5/4r3/3p1npb/3kp1P1/1P3P1P/5nR1/2Q1BK2/bN4NR w - - 3 61", "rn1q2n1/b3k1pr/pp1pB1Qp/2p1p1P1/2P1PP2/5R1P/P2P4/RNB1K3 w - - 1 24", "8/3r3k/NP1p4/p2QP1P1/1BB3Pp/1R4n1/6K1/5R2 w - - 5 82", "1nr1r3/n4Q2/P1kp2N1/2p3B1/1pp3P1/6P1/1R2P2R/K5N1 w - - 3 43"];
        // public static string[] mate_in_2 = ["r1bq2r1/b4pk1/p1pp1p2/1p2pP2/1P2P1PB/3P4/1PPQ2P1/R3K2R w KQ - 0 1", "kbK5/pp6/1P6/8/8/8/8/R7 w - - 0 1"];
        public static MatePuzzleSet mate_in_1_easy = new("Easy Mate in 1",
            new MatePuzzle("7k/R7/1R6/8/8/8/8/4K3 w - - 3 61", 1),
            new MatePuzzle("8/8/8/4K3/8/8/RQ6/5k2 w - - 1 24", 1),
            new MatePuzzle("2k5/R7/2K5/8/8/8/8/8 w - - 5 82", 1),
            new MatePuzzle("7k/8/7K/8/1BB5/8/8/8 w - - 0 1", 1));
        public static MatePuzzleSet mate_in_1_hard = new("Hard Mate in 1",
            new MatePuzzle("1rb5/4r3/3p1npb/3kp1P1/1P3P1P/5nR1/2Q1BK2/bN4NR w - - 3 61", 1),
            new MatePuzzle("rn1q2n1/b3k1pr/pp1pB1Qp/2p1p1P1/2P1PP2/5R1P/P2P4/RNB1K3 w - - 1 24", 1),
            new MatePuzzle("8/3r3k/NP1p4/p2QP1P1/1BB3Pp/1R4n1/6K1/5R2 w - - 5 82", 1),
            new MatePuzzle("1nr1r3/n4Q2/P1kp2N1/2p3B1/1pp3P1/6P1/1R2P2R/K5N1 w - - 3 43", 1));
        public static MatePuzzleSet mate_in_2_easy = new("Easy Mate in 2",
            new MatePuzzle("kbK5/pp6/1P6/8/8/8/8/R7 w - - 0 1", 3),
            new MatePuzzle("8/7k/1R6/R7/8/8/8/3K4 w - - 0 1", 3),
            new MatePuzzle("5k2/2Q5/3K4/8/8/8/8/8 w - - 0 1", 3),
            new MatePuzzle("6k1/8/7K/1BB5/8/8/8/8 w - - 0 1", 3));
        public static MatePuzzleSet mate_in_2_hard = new("Hard Mate in 2",
            new MatePuzzle("r1bq2r1/b4pk1/p1pp1p2/1p2pP2/1P2P1PB/3P4/1PPQ2P1/R3K2R w KQ - 0 1", 3),
            new MatePuzzle("2r3k1/3b1rpp/Q2R4/4p3/2B3q1/6P1/PPP4P/1K6 w - - 0 1", 3),
            new MatePuzzle("6rk/p2nbR1p/2b1q2B/1p1nP2p/3P4/6Q1/PP4P1/5R1K w - - 0 1", 3),
            new MatePuzzle("1r3Q2/1q1k2P1/3p4/p1pPp3/P1P1Rp1P/3r4/6K1/8 w - - 0 1", 3));

        public static int BenchmarkPuzzles(int depth, int iters, params MatePuzzleSet[] puzzle_sets)
        {
            ChessBot bot = new(depth);
            Stopwatch stopwatch = new();

            foreach (MatePuzzleSet puzzle_set in puzzle_sets)
            {
                Console.WriteLine($"Running puzzle set \"{puzzle_set.Name}\"");
                bool set_success = true;
                long set_time = 0;

                for (int i = 0; i < puzzle_set.Count; i++)
                {
                    bool success = true;
                    stopwatch.Start();

                    for (int j = 0; j < iters; j++)
                    {
                        MatePuzzle puzzle = puzzle_set[i];
                        ChessGame game = new(puzzle.position);

                        int moves = 0;
                        while (!game.HasEnded() && moves < puzzle.move_limit)
                        {
                            if (!bot.PlayMove(game))
                                return -1;

                            moves++;
                        }

                        if (game.Winner != ChessGame.WHITE)
                        {
                            success = false; set_success = false;
                            break;
                        }
                    }

                    stopwatch.Stop();

                    if (success)
                        Console.WriteLine($"     Puzzle {i} solved successfully. Time elapsed: {stopwatch.ElapsedMilliseconds / iters}ms");
                    else Console.WriteLine($"  !! Puzzle {i} failed");

                    set_time += stopwatch.ElapsedMilliseconds;

                    stopwatch.Reset();
                }

                if (set_success)
                    Console.WriteLine($"Puzzle set solved successfully. Average time to solve set: {set_time / iters}ms ({set_time / (puzzle_set.Count * iters)}ms per puzzle).");
            }

            return 0;
        }
    }

    class MatePuzzle
    {
        public string position;
        public int move_limit;

        public MatePuzzle(string position, int move_limit)
        {
            this.position = position;
            this.move_limit = move_limit;
        }
    }

    class MatePuzzleSet : IEnumerable<MatePuzzle>
    {
        public string Name { get; private set; }
        private MatePuzzle[] puzzles;

        public MatePuzzleSet(string name, params MatePuzzle[] puzzles)
        {
            Name = name; this.puzzles = puzzles;
        }

        public MatePuzzle this[int index] => puzzles[index];

        public int Count => puzzles.Length;

        public IEnumerator<MatePuzzle> GetEnumerator()
        {
            foreach (MatePuzzle puzzle in puzzles)
                yield return puzzle;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}