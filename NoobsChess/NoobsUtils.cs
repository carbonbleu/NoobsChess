using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using NoobsEngine.Data;
using NoobsEngine.Enums;

using static NoobsEngine.NoobsGlobals;

namespace NoobsEngine
{
    public class NoobsUtils
    {
        public static int FileRankTo120Square(int file, int rank)
        {
            return (21 + file) + (rank * 10);
        }

        public static int FileRankTo120Square(BoardFiles file, BoardRanks rank)
        {
            return (21 + (int) file) + ((int) rank * 10);
        }

        public static void PlaySquareOnBitBoard(BitBoard board, BoardSquares square) {
            board.Value |= 1UL << NoobsGlobals.Square120To64[(int)square];
        }

        public static UInt64 NextUInt64() {
            Random random = new Random();
            var buffer = new byte[sizeof(UInt64)];
            random.NextBytes(buffer);
            return BitConverter.ToUInt64(buffer, 0);
        }

        public static String GetSquareNotation(int square) {
            int file = FileLookup[square];
            int rank = RankLookup[square];

            return String.Format("{0}{1}", (char)('a' + file), (char)('1' + rank));
        }

        public static bool IsSquareOnBoard(int square) {
            return FileLookup[square] != (int) BoardSquares.Offboard;
        }

        public static bool IsValidSide(int side) {
            return side != (int) Players.Both;
        }

        public static bool IsValidFileRank(int file, int rank) {
            return (file >= (int) BoardFiles.A && file <= (int) BoardFiles.H)
                && (rank >= (int) BoardRanks.Rank1 && rank <= (int) BoardRanks.Rank8);
        }

        public static bool IsPieceValidOrEmpty(int piece) {
            return piece >= (int) Pieces.Empty && piece <= (int) Pieces.BlackKing;
        }

        public static bool IsPieceValid(int piece) {
            return piece >= (int) Pieces.WhitePawn && piece <= (int) Pieces.BlackKing;
        }

        public static Move ConstructMove(int from, int to, int capturedPiece, int promotedPiece, int flag) {
            return new Move(from | (to << 7) | (capturedPiece << 14) | (promotedPiece << 20) | flag);
        }

        public static bool IsSquareOffBoard(int square) {
            return FileLookup[square] == (int) BoardSquares.Offboard;
        }

        public static int Mirror(int square) {
            return NoobsGlobals.Mirror[square];
        }
    }
}
