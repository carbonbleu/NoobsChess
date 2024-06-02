using NoobsEngine;

namespace NoobsChess;

public class PerftTesting
{
    private static long leafNodes;
    public static void Perft(ChessBoard position, int depth) {
        if (depth == 0) {
            leafNodes++;
            return;
        }

        MoveGen moveGen = new MoveGen();
        moveGen.GenerateAllMoves(position);

        for (int i = 0; i < moveGen.Moves.Count; i++) {
            // Console.WriteLine("Performing move: {0}", moveGen.Moves[i]);
            if (!position.MakeMove(moveGen.Moves[i])) {
                continue;
            }
            Perft(position, depth - 1);
            position.UndoMove();
        }
    }

    public static void FullTest(ChessBoard position, int depth) {
        Console.WriteLine(position);

        Console.WriteLine("Initiating Perft to depth {0}", depth);

        leafNodes = 0;

        MoveGen moveGen = new MoveGen();
        moveGen.GenerateAllMoves(position);

        for (int i = 0; i < moveGen.Moves.Count; i++) {
            Move move = moveGen.Moves[i];
            if (!position.MakeMove(move)) {
                continue;
            }
            // if (!move.ToString().Equals("e2a6")) {
            //     continue;
            // }
                
            long cumulativeNodes = leafNodes;
            Perft(position, depth - 1);
            position.UndoMove();

            long oldNodes = leafNodes - cumulativeNodes;
            // if (move.ToString().Equals("e2a6")) {
                Console.WriteLine("Move {0} - {1}: {2}", i + 1, move, oldNodes);
            // }
            
        }

        Console.WriteLine("{0} leaf nodes visited", leafNodes);
    }
}
