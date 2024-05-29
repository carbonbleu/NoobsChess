using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoobsEngine.Data;
using NoobsEngine.Enums;

namespace NoobsEngine
{
    public static class NoobsGlobals
    {
        public static int[] Square120To64 { get; set; } = new int[NoobsDefs.BoardSquareCount];
        public static int[] Square64To120 { get; set; } = new int[64];

        public static BitBoard[] SetMask { get; set; } = new BitBoard[64];
        public static BitBoard[] ClearMask { get; set; } = new BitBoard[64];

        public static UInt64[,] PieceKeys { get; set; } = new UInt64[13,120];
        public static UInt64 ColourKey { get; set; }
        public static UInt64[] CastlingKeys { get; set; } = new UInt64[16];

        // Pieces are in the order
        // No pieces
        // Pawn, Knight, Bishop, Rook, Queen, King, (White)
        // Pawn, Knight, Bishop, Rook, Queen, King, (Black)
        public static readonly bool[] PieceBig = new bool[13] {
            false, 
            false, true, true, true, true, true,
            false, true, true, true, true, true
        };

        public static readonly bool[] PieceMajor = new bool[13] {
            false, 
            false, false, false, true, true, true,
            false, false, false, true, true, true
        };

        public static readonly bool[] PieceMinor = new bool[13] {
            false, 
            false, true, true, false, false, false,
            false, true, true, false, false, false
        };

        public static readonly int[] PieceValues = new int[13] {
            0,
            100, 325, 325, 550, 1000, 50000,
            100, 325, 325, 550, 1000, 50000
        };

        public static readonly Players[] PieceColours = new Players[13] {
            Players.Both, 
            Players.White, Players.White, Players.White, Players.White, Players.White, Players.White,
            Players.Black, Players.Black, Players.Black, Players.Black, Players.Black, Players.Black
        };

        public static int[] FileLookup = new int[NoobsDefs.BoardSquareCount];
        public static int[] RankLookup = new int[NoobsDefs.BoardSquareCount];
    }
}
