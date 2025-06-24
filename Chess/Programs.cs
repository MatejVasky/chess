using Chess.Chess;
using Chess.Chess.Bot;
using Chess.Games;
using Chess.Minimax;
using Chess.Benchmarking;

static class Programs
{
    public static int PlayChess(string? starting_position_fen = null, bool isWhiteBot = false, bool isBlackBot = false, bool showEval = false)
    {
        ChessGame game;
        if (starting_position_fen is null) game = new();
        else game = new(starting_position_fen);

        ConsoleChessPlayer consoleChessPlayer = new();
        ChessBot bot = new(3);
        Player<ChessGame, Move>[] players = new Player<ChessGame, Move>[ChessGame.BLACK + 1];
        players[ChessGame.WHITE] = isWhiteBot ? bot : consoleChessPlayer;
        players[ChessGame.BLACK] = isBlackBot ? bot : consoleChessPlayer;

        Minimaxer<ChessGame, Move> minimaxer = new(new ChessEndStateEvaluator(), new ChessStaticEvaluator());

        while (!game.HasEnded())
        {
            game.PrintBoard();
            Console.WriteLine();

            if (showEval)
            {
                MinimaxResult<Move> evalResult = minimaxer.Evaluate(game, 1);
                Console.WriteLine($"Evaluation: {evalResult.Eval}");
                Console.WriteLine($"Best move: {game.ToAlgebraicNotation(evalResult.Line[0])}");
                Console.WriteLine();
            }

            if (!players[game.turn].PlayMove(game))
                return -1;
            Console.WriteLine();
        }

        game.PrintBoard();
        Console.WriteLine();
        switch (game.Winner)
        {
            case ChessGame.WHITE:
                Console.WriteLine("White wins!");
                break;
            case ChessGame.BLACK:
                Console.WriteLine("Black wins!");
                break;
            default:
                Console.WriteLine("Draw");
                break;
        }

        return 0;
    }

    public static int BenchmarkPuzzles()
    {
        return MatePuzzlesBenchmark.BenchmarkPuzzles(depth: 3, iters: 2,
            MatePuzzlesBenchmark.mate_in_1_easy,
            MatePuzzlesBenchmark.mate_in_1_hard,
            MatePuzzlesBenchmark.mate_in_2_easy,
            MatePuzzlesBenchmark.mate_in_2_hard);
    }
}