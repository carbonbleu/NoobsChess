using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoobsEngine.Enums;

using static NoobsEngine.NoobsGlobals;

namespace NoobsEngine
{
    public class ChessBoard
    {
        public int[] PiecesOnBoard { get; set; } = new int[NoobsDefs.BoardSquareCount];
        UInt64[] pawns = new UInt64[3];

        int[] kingSquares = new int[2];

        public int SideToMove { get; set; }
        public int EnPassant { get; set; }
        public int FiftyMoveCounter { get; set; }

        public int Ply { get; set; }
        public int PliesMade { get; set; }

        public int[] MaterialScores { get; set; } = new int[2];

        UInt64 positionKey;

        int[] pieceCount = new int[13];

        int[] bigPiecesByColour = new int[2];
        int[] majorPiecesByColour = new int[2];
        int[] minorPiecesByColour = new int[2];

        public int CastlingPermission { get; set; }

        MoveUndo[] history = new MoveUndo[NoobsDefs.MaxGameMoves];

        int[,] pieceList = new int[13,10];

        public ChessBoard() {
            Reset();
        }

        public UInt64 GeneratePositionKey() {
            UInt64 finalKey = 0UL;

            int piece = (int) Pieces.Empty;

            for (int square = 0; square < NoobsDefs.BoardSquareCount; square++) {
                piece = PiecesOnBoard[square];
                if (piece != (int) BoardSquares.NoSquare && piece != (int) Pieces.Empty && piece != (int) BoardSquares.Offboard) {
                    finalKey ^= PieceKeys[piece,square];
                }
            }

            if (SideToMove == (int) Players.White) {
                finalKey ^= (UInt64) SideToMove;
            }

            if (EnPassant != (int) BoardSquares.NoSquare && EnPassant != (int) BoardSquares.Offboard) {
                finalKey ^= PieceKeys[0,EnPassant];
            }

            finalKey ^= CastlingKeys[CastlingPermission];

            return finalKey;
        }

        public void SetPositionKey(UInt64 positionKeyIn) {
            positionKey = positionKeyIn;
        }

        public void Reset() {
            for (int i = 0; i < NoobsDefs.BoardSquareCount; i++) {
                PiecesOnBoard[i] = (int) BoardSquares.Offboard;
            }

            for (int i = 0; i < 64; i++) {
                PiecesOnBoard[Square64To120[i]] = (int) Pieces.Empty;
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

            SideToMove = (int) Players.Both;
            EnPassant = (int) BoardSquares.NoSquare;
            FiftyMoveCounter = 0;

            Ply = 0;
            PliesMade = 0;

            CastlingPermission = 0;

            positionKey = 0UL;

            for (int i = 0; i < NoobsDefs.MaxGameMoves; i++) {
                history[i] = new MoveUndo();
                history[i].Move = 0;
                history[i].CastlingPermission = 0;
                history[i].EnPassantSquare = 0;
                history[i].FiftyMoveRule = 0;
                history[i].PositionKey = 0UL;
            }

            MaterialScores[(int) Players.Black] = 0;
            MaterialScores[(int) Players.White] = 0;
        }

        public override String ToString() {
            String pieceChars = ".PNBRQKpnbrqk";
            String colourChars = "wb-";
            String rankChars = "12345678";
            String fileChars = "abcdefgh";

            StringBuilder result = new StringBuilder();

            for (BoardRanks rank = BoardRanks.Rank8; rank >= BoardRanks.Rank1; rank--) {
                result.Append(String.Format("{0}\t", (int) (rank + 1)));
                for (BoardFiles file = BoardFiles.A; file <= BoardFiles.H; file++) {
                    int square = NoobsUtils.FileRankTo120Square((int) file, (int) rank);
                    int piece = PiecesOnBoard[square];
                    
                    result.Append(String.Format("{0:3} ", pieceChars[piece]));
                }

                result.Append("\n");
            }
            result.Append("\t");
            for (BoardFiles file = BoardFiles.A; file <= BoardFiles.H; file++) {
                result.Append(fileChars[(int)file] + " ");
            }
            result.Append("\n");
            result.Append(String.Format("Side to move: {0}\n", colourChars[SideToMove]));
            result.Append(String.Format("En Passant Square: {0}\n", EnPassant));

            result.Append(String.Format(
                "Castling Permissions: {0}{1}{2}{3}\n",
                ((CastlingPermission & (int) CastlingPermissions.WhiteKingSideCastling) > 0) ? 'K' : '-',
                ((CastlingPermission & (int) CastlingPermissions.WhiteQueenSideCastling) > 0) ? 'Q' : '-',
                ((CastlingPermission & (int) CastlingPermissions.BlackKingSideCastling) > 0) ? 'k' : '-',
                ((CastlingPermission & (int) CastlingPermissions.BlackQueenSideCastling) > 0) ? 'q' : '-'
            ));
            result.Append(String.Format("Position Key: {0:X}", positionKey));
            return result.ToString();
        }

        public void UpdateMaterial() {
            for (int i = 0; i < NoobsDefs.BoardSquareCount; i++) {
                int square = i;
                int piece = PiecesOnBoard[square];
                if (piece != (int) BoardSquares.Offboard && piece != (int) Pieces.Empty) {
                    int pieceColour = (int) PieceColours[piece];
                    if (PieceBig[piece]) {
                        bigPiecesByColour[pieceColour]++;
                    }
                    if (PieceMajor[piece]) {
                        majorPiecesByColour[pieceColour]++;
                    }
                    if (PieceMinor[piece]) {
                        minorPiecesByColour[pieceColour]++;
                    }

                    MaterialScores[pieceColour] += PieceValues[piece];

                    pieceList[piece,pieceCount[piece]] = square;
                    pieceCount[piece]++;

                    if (piece == (int) Pieces.WhiteKing) {
                        kingSquares[(int) Players.White] = square;
                    }
                    if (piece == (int) Pieces.BlackKing) {
                        kingSquares[(int) Players.Black] = square;
                    }
                }
                
            }
        }
    }
}
