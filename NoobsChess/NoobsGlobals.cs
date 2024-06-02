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

        public static readonly int[] KnightDirection = new int[] {
            -8, -19, -21, -12, 8, 19, 21, 12
        };

        public static readonly int[] BishopDirection = new int[] {
            -9, -11, 11, 9
        };

        public static readonly int[] RookDirection = new int[] {
            -1, -10, 1, 10
        };

        public static readonly int[] KingDirection = new int[] {
            -1, -10, 1, 10, -9, -11, 11, 9
        };

        public static readonly bool[] PiecePawn = { false, true, false, false, false, false, false, true, false, false, false, false, false };	
        public static readonly bool[] PieceKnight = { false, false, true, false, false, false, false, false, true, false, false, false, false };
        public static readonly bool[] PieceKing = { false, false, false, false, false, false, true, false, false, false, false, false, true };
        public static readonly bool[] PieceRookOrQueen = { false, false, false, false, true, true, false, false, false, false, true, true, false };
        public static readonly bool[] PieceBishopOrQueen = { false, false, false, true, false, true, false, false, false, true, false, true, false };
        public static readonly bool[] PieceSlides = { false, false, false, true, true, true, false, false, false, true, true, true, false };

        public static readonly Dictionary<Players, Pieces[]> SlidingPieces = new Dictionary<Players, Pieces[]> {
            {Players.White, new[]{Pieces.WhiteBishop, Pieces.WhiteRook, Pieces.WhiteQueen}},
            {Players.Black, new[]{Pieces.BlackBishop, Pieces.BlackRook, Pieces.BlackQueen}},
        };

        public static readonly Dictionary<Players, Pieces[]> NonSlidingPieces = new Dictionary<Players, Pieces[]> {
            {Players.White, new[]{Pieces.WhiteKnight, Pieces.WhiteKing}},
            {Players.Black, new[]{Pieces.BlackKnight, Pieces.BlackKing}},
        };

        public static readonly Dictionary<Pieces, int[]> PieceDirs = new Dictionary<Pieces, int[]> {
            {Pieces.BlackBishop, BishopDirection},
            {Pieces.BlackKnight, KnightDirection},
            {Pieces.BlackRook, RookDirection},
            {Pieces.BlackQueen, KingDirection},
            {Pieces.BlackKing, KingDirection},
            {Pieces.WhiteBishop, BishopDirection},
            {Pieces.WhiteKnight, KnightDirection},
            {Pieces.WhiteRook, RookDirection},
            {Pieces.WhiteQueen, KingDirection},
            {Pieces.WhiteKing, KingDirection},
        };

        public static readonly int[] CastlePermNums = new int[] {
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 13, 15, 15, 15, 12, 15, 15, 14, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15,  7, 15, 15, 15,  3, 15, 15, 11, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15
        };
    }
}
