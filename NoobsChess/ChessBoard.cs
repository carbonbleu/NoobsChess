using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoobsEngine.Enums;

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

        int castlingPermissions;

        MoveUndo[] history = new MoveUndo[NoobsDefs.MaxGameMoves];

        int[,] pieceList = new int[13,10];

        public UInt64 GeneratePositionKey() {
            UInt64 finalKey = 0UL;

            int piece = (int) Pieces.Empty;

            for (int square = 0; square < NoobsDefs.BoardSquareCount; square++) {
                piece = pieces[square];
                if (piece != (int) BoardSquares.NoSquare && piece != (int) Pieces.Empty) {
                    finalKey ^= NoobsGlobals.PieceKeys[piece,square];
                }
            }

            if (sideToMove == (int) Players.White) {
                finalKey ^= (UInt64) sideToMove;
            }

            if (enPassant != (int) BoardSquares.NoSquare) {
                finalKey ^= NoobsGlobals.PieceKeys[0,enPassant];
            }

            finalKey ^= NoobsGlobals.CastlingKeys[castlingPermissions];

            return finalKey;
        }

        public void Reset() {
            for (int i = 0; i < NoobsDefs.BoardSquareCount; i++) {
                pieces[i] = (int) BoardSquares.Offboard;
            }

            for (int i = 0; i < 64; i++) {
                pieces[NoobsGlobals.Square64To120[i]] = (int) Pieces.Empty;
            }

            for (int i = 0; i < 3; i++) {
                bigPiecesByColour[i] = 0;
                majorPiecesByColour[i] = 0;
                minorPiecesByColour[i] = 0;
                pawns[i] = 0UL;
            }

            for (int i = 0; i < 13; i++) {
                pieceCount[i] = 0;
            }

            kingSquares[(int) Players.White] = kingSquares[(int) Players.Black] = (int) BoardSquares.NoSquare;

            sideToMove = (int) Players.Both;
            enPassant = (int) BoardSquares.NoSquare;
            fiftyMoveCounter = 0;

            ply = 0;
            pliesMade = 0;

            castlingPermissions = 0;

            positionKey = 0UL;

            for (int i = 0; i < NoobsDefs.MaxGameMoves; i++) {
                history[i].Move = 0;
                history[i].CastlingPermission = 0;
                history[i].EnPassantSquare = 0;
                history[i].FiftyMoveRule = 0;
                history[i].PositionKey = 0UL;
            }
        }
    }
}
