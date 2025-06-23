// using Chess.Chess;

// static void PrintMove(Move move)
// {
//     Console.WriteLine(
//         ((char)('a' + move.from_y)).ToString() + ((char)('1' + move.from_x)).ToString() +
//         " -> " +
//         (char)('a' + move.to_y) + (char)('1' + move.to_x) +
//         ((move.capture == 0) ? "" : " (takes " + ChessGame.piece_names[move.capture] + ")") +
//         ((move.promotion == 0) ? "" : " (promotes to " + ChessGame.piece_names[(move.promotion ^ ChessGame.PAWN) | ChessGame.WHITE] + ")") +
//         " " + move.rule50counter_update.ToString()
//     );
// }

// ChessGame game = new();
// game.PrintBoard();

// foreach (Move move in game.GetMoves())
//     PrintMove(move);

// Move e5 = new() { from_x = 1, from_y = 4, to_x = 3, to_y = 4, capture = 0, promotion = 0 };
// Console.WriteLine(game.MakeMove(e5));
// game.PrintBoard();

// foreach (Move move in game.GetMoves())
//     PrintMove(move);

// Move d4 = new() { from_x = 6, from_y = 3, to_x = 4, to_y = 3, capture = 0, promotion = 0 };
// Console.WriteLine(game.MakeMove(d4));
// game.PrintBoard();

// foreach (Move move in game.GetMoves())
//     PrintMove(move);

// Move exd5 = new() { from_x = 3, from_y = 4, to_x = 4, to_y = 3, capture = ChessGame.BLACK | ChessGame.PAWN, promotion = 0 };
// Console.WriteLine(game.MakeMove(exd5));
// game.PrintBoard();

// Move e6 = new() { from_x = 6, from_y = 4, to_x = 5, to_y = 4, capture = 0, promotion = 0 };
// game.MakeMove(e6);

// Move dxe6 = new() { from_x = 4, from_y = 3, to_x = 5, to_y = 4, capture = ChessGame.BLACK | ChessGame.PAWN, promotion = 0 };
// game.MakeMove(dxe6);

// Move a6 = new() { from_x = 6, from_y = 0, to_x = 5, to_y = 0, capture = 0, promotion = 0 };
// game.MakeMove(a6);

// Move exf7 = new() { from_x = 5, from_y = 4, to_x = 6, to_y = 5, capture = ChessGame.BLACK | ChessGame.PAWN, promotion = 0 };
// game.MakeMove(exf7);

// Move Ke7 = new() { from_x = 7, from_y = 4, to_x = 6, to_y = 4, capture = 0, promotion = 0 };
// game.MakeMove(Ke7);

// Move fxg8Q = new() { from_x = 6, from_y = 5, to_x = 7, to_y = 6, capture = ChessGame.BLACK | ChessGame.KNIGHT, promotion = ChessGame.PAWN ^ ChessGame.QUEEN };
// game.MakeMove(fxg8Q);

// Console.WriteLine();
// game.PrintBoard();

// Move e4 = new() { from_x = 1, from_y = 4, to_x = 3, to_y = 4, capture = 0, promotion = 0 };
// Move Nd2 = new() { from_x = 2, from_y = 5, to_x = 1, to_y = 3, capture = 0, promotion = 0 };
// Move Bg4 = new() { from_x = 2, from_y = 7, to_x = 3, to_y = 6, capture = 0, promotion = 0 };
// Move Ra2 = new() { from_x = 0, from_y = 0, to_x = 1, to_y = 0, capture = 0, promotion = 0 };
// Move Qc4 = new() { from_x = 2, from_y = 1, to_x = 3, to_y = 2, capture = 0, promotion = 0 };
// Move Kd1 = new() { from_x = 0, from_y = 4, to_x = 0, to_y = 3, capture = 0, promotion = 0 };
// Move Nxe5 = new() { from_x = 2, from_y = 5, to_x = 4, to_y = 4, capture = ChessGame.BLACK | ChessGame.PAWN, promotion = 0 };
// Move dxc6 = new() { from_x = 4, from_y = 3, to_x = 5, to_y = 2, capture = ChessGame.BLACK | ChessGame.PAWN, promotion = 0 };
// Move a8Q = new() { from_x = 6, from_y = 0, to_x = 7, to_y = 0, capture = 0, promotion = ChessGame.PAWN ^ ChessGame.QUEEN };
// Move axb8Q = new() { from_x = 6, from_y = 0, to_x = 7, to_y = 1, capture = ChessGame.BLACK | ChessGame.KNIGHT, promotion = ChessGame.PAWN ^ ChessGame.QUEEN };
// // dxe6 - en passant
// // O-O
// // O-O-O
// Console.WriteLine($"e4 = 'e4':         {game.MoveMatchesNotation(e4, "e4")}");
// Console.WriteLine($"e4 = 'b4':         {!game.MoveMatchesNotation(e4, "b4")}");
// Console.WriteLine($"e4 = 'e3':         {!game.MoveMatchesNotation(e4, "e3")}");
// Console.WriteLine($"Nd2 = 'Nd2':       {game.MoveMatchesNotation(Nd2, "Nd2")}");
// Console.WriteLine($"Nd2 = 'Rd2':       {!game.MoveMatchesNotation(Nd2, "Rd2")}");
// Console.WriteLine($"Nd2 = 'Ne2':       {!game.MoveMatchesNotation(Nd2, "Ne2")}");
// Console.WriteLine($"Nd2 = 'Nd3':       {!game.MoveMatchesNotation(Nd2, "Nd3")}");
// Console.WriteLine($"Bg4 = 'Bg4':       {game.MoveMatchesNotation(Bg4, "Bg4")}");
// Console.WriteLine($"Ra2 = 'Ra2':       {game.MoveMatchesNotation(Ra2, "Ra2")}");
// Console.WriteLine($"Qc4 = 'Qc4':       {game.MoveMatchesNotation(Qc4, "Qc4")}");
// Console.WriteLine($"Kd1 = 'Kd1':       {game.MoveMatchesNotation(Kd1, "Kd1")}");
// Console.WriteLine($"Nd2 = 'Nxd2':      {!game.MoveMatchesNotation(Nd2, "Nxd2")}");
// Console.WriteLine($"Nxe5 = 'Nxe5':     {game.MoveMatchesNotation(Nxe5, "Nxe5")}");
// Console.WriteLine($"Nxe5 = 'Ne5':      {!game.MoveMatchesNotation(Nxe5, "Ne5")}");
// Console.WriteLine($"Nxe5 = 'Nxd5':     {!game.MoveMatchesNotation(Nxe5, "Nxd5")}");
// Console.WriteLine($"Nxe5 = 'Nxe6':     {!game.MoveMatchesNotation(Nxe5, "Nxe6")}");
// Console.WriteLine($"dxc6 = 'dxc6':     {game.MoveMatchesNotation(dxc6, "dxc6")}");
// Console.WriteLine($"Nd2 = 'Nfd2':      {game.MoveMatchesNotation(Nd2, "Nfd2")}");
// Console.WriteLine($"Nd2 = 'N3d2':      {game.MoveMatchesNotation(Nd2, "N3d2")}");
// Console.WriteLine($"Nd2 = 'Nf3d2':     {game.MoveMatchesNotation(Nd2, "Nf3d2")}");
// Console.WriteLine($"Nd2 = 'Nbd2':      {!game.MoveMatchesNotation(Nd2, "Nbd2")}");
// Console.WriteLine($"Nd2 = 'N4d2':      {!game.MoveMatchesNotation(Nd2, "Nbd2")}");
// Console.WriteLine($"e4 = '2e4':        {game.MoveMatchesNotation(e4, "2e4")}");
// Console.WriteLine($"a8=Q = 'a8=Q':     {game.MoveMatchesNotation(a8Q, "a8=Q")}");
// Console.WriteLine($"a8=Q = 'a8':       {!game.MoveMatchesNotation(a8Q, "a8")}");
// Console.WriteLine($"a8=Q = 'a8=N':     {!game.MoveMatchesNotation(a8Q, "a8=N")}");
// Console.WriteLine($"axb8=Q = 'axb8=Q': {game.MoveMatchesNotation(axb8Q, "axb8=Q")}");


// while (true)
// {
//     game.PrintBoard();
//     Console.WriteLine();

//     Move? move;
//     while (true)
//     {
//         Console.Write("Move: ");
//         string? notation = Console.ReadLine();
//         if (notation is null) return -1;

//         if (notation == "undo")
//         {
//             try
//             {
//                 move = null;
//                 break;
//             }
//             catch
//             { }
//         }

//         try
//             {
//                 move = game.FindMove(notation);
//                 break;
//             }
//             catch (KeyNotFoundException) { }
//     }

//     if (move is null)
//     {
//         try
//         {
//             game.UndoMakeMove();
//             Console.WriteLine();
//         }
//         catch { }
//         continue;
//     }

//     game.MakeMove((Move)move);
//     Console.WriteLine();

//     if (game.HasEnded())
//     {
//         game.PrintBoard();
//         Console.WriteLine();
//         switch (game.Winner)
//         {
//             case ChessGame.WHITE:
//                 Console.WriteLine("White wins!");
//                 break;
//             case ChessGame.BLACK:
//                 Console.WriteLine("Black wins!");
//                 break;
//             default:
//                 Console.WriteLine("Draw");
//                 break;
//         }
//         return 0;
//     }
// }


using Chess.Hexapawn;
using Chess.Minimax;

HexapawnGame game = new();
Minimaxer minimaxer = new(new HexapawnEndstateEvaluator(), new ZeroStaticEvaluator());

// game.PrintBoard(); Console.WriteLine();
// game.MakeMove(new Move { from_x = 0, from_y = 1, to_x = 1, to_y = 1, capture = 0 });
// game.PrintBoard(); Console.WriteLine();
// // foreach (Move move in game.GetMoves())
// //     Console.WriteLine($"{move.from_x} {move.from_y} {move.to_x} {move.to_y} {move.capture}");
// game.MakeMove(new Move { from_x = 2, from_y = 0, to_x = 1, to_y = 1, capture = 1 });
// game.PrintBoard(); Console.WriteLine();
// game.MakeMove(new Move { from_x = 0, from_y = 0, to_x = 1, to_y = 1, capture = 2 });
// game.PrintBoard(); Console.WriteLine();
// // game.MakeMove(new Move { from_x = 2, from_y = 2, to_x = 1, to_y = 1, capture = 1 });
// // game.PrintBoard(); Console.WriteLine();
// // game.MakeMove(new Move { from_x = 0, from_y = 2, to_x = 1, to_y = 1, capture = 2 });
// // game.PrintBoard(); Console.WriteLine();
// game.MakeMove(new Move { from_x = 2, from_y = 2, to_x = 1, to_y = 2, capture = 0 });
// game.PrintBoard(); Console.WriteLine();
// Console.WriteLine(game.Winner);

while (true)
{
    game.PrintBoard();
    Console.WriteLine();
    MinimaxResult minimaxResult = minimaxer.Evaluate(game, 1);
    Console.WriteLine($"Evaluation: {minimaxResult.Eval}");
    Move best_move = minimaxResult.Line[0];
    Console.WriteLine($"Best move: {best_move.from_x} {best_move.from_y} {best_move.to_x} {best_move.to_y} {best_move.capture}");

    Move? move;
    while (true)
    {
        Console.Write("Move: ");
        string? notation = Console.ReadLine();
        if (notation is null) return -1;

        if (notation == "undo")
        {
            move = null;
            break;
        }

        string[] split = notation.Split(' ');
        int from_x = int.Parse(split[0]);
        int from_y = int.Parse(split[1]);
        int to_x = int.Parse(split[2]);
        int to_y = int.Parse(split[3]);
        int capture = int.Parse(split[4]);
        move = new Move { from_x = from_x, from_y = from_y, to_x = to_x, to_y = to_y, capture = capture };
        break;
    }

    if (move is null)
    {
        game.UndoMakeMove();
        Console.WriteLine();
        continue;
    }

    game.MakeMove((Move)move);
    Console.WriteLine();

    if (game.HasEnded())
    {
        game.PrintBoard();
        Console.WriteLine();
        Console.WriteLine($"{game.Winner} wins!");
        return 0;
    }
}