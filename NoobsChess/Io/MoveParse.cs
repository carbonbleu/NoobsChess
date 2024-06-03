namespace NoobsChess.Io;

using static NoobsEngine.NoobsGlobals;
using static NoobsEngine.NoobsDefs;
using static NoobsEngine.NoobsUtils;

using NoobsEngine;
using NoobsEngine.Enums;

public class MoveParse {
    public static Move ParseMove(String move, ChessBoard board) {
        if (move.Length < 4) {
            throw new Exception("Invalid move format");
        }

        if (move[0] < 'a' || move[0] > 'h') return NoobsDefs.NoMove;
        if (move[2] < 'a' || move[2] > 'h') return NoobsDefs.NoMove;
        if (move[1] < '1' || move[1] > '8') return NoobsDefs.NoMove;
        if (move[3] < '1' || move[3] > '8') return NoobsDefs.NoMove;

        int from = FileRankTo120Square(move[0] - 'a', move[1] - '1');
        int to = FileRankTo120Square(move[2] - 'a', move[3] - '1');

        if (!IsSquareOnBoard(from) || !IsSquareOnBoard(to)) {
            throw new Exception("Invalid move locations");
        }

        MoveGen moveGen = new MoveGen();
        moveGen.GenerateAllMoves(board);

        for (int i = 0; i < moveGen.Moves.Count; i++) {
            Move mvI = moveGen.Moves[i];
            if (mvI.GetFromPosition() == from && mvI.GetToPosition() == to) {
                int promotedPiece = mvI.GetPromotedPiece();
                if (promotedPiece != (int) Pieces.Empty) {
                    if (PieceRookOrQueen[promotedPiece] && !PieceBishopOrQueen[promotedPiece] && move[4] == 'r') {
                        return mvI;
                    }
                    else if (PieceBishopOrQueen[promotedPiece] && !PieceRookOrQueen[promotedPiece] && move[4] == 'b') {
                        return mvI;
                    }
                    else if (PieceBishopOrQueen[promotedPiece] && PieceRookOrQueen[promotedPiece] && move[4] == 'q') {
                        return mvI;
                    }
                    else if (PieceKnight[promotedPiece] && move[4] == 'n') {
                        return mvI;
                    }
                    continue;
                }
                return mvI;
            }
        }

        return NoobsDefs.NoMove;
    }
}