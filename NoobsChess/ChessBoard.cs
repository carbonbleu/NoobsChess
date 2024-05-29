using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobsEngine
{
    public class ChessBoard
    {
        int[] pieces = new int[NoobsDefs.BoardSquareCount];
        UInt64[] pawns = new UInt64[3];

        int[] kingSquares = new int[2];

        int sideToMove;
        int enPassant;
        int fiftyMoveCounter;

        int ply;
        int pliesMade;

        UInt64 positionKey;

        int[] pieceCount = new int[13];

        int[] bigPiecesByColour = new int[3];
        int[] majorPiecesByColour = new int[3];
        int[] minorPiecesByColour = new int[3];

        int[] castlingPermissions = new int[3];

        MoveUndo[] history = new MoveUndo[NoobsDefs.MaxGameMoves];

        int[,] pieceList = new int[13,10];
    }
}
