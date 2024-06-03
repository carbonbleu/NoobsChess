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

        public static readonly int[] PawnValues = new int[]{
            0	,	0	,	0	,	0	,	0	,	0	,	0	,	0	,
            10	,	10	,	0	,	-10	,	-10	,	0	,	10	,	10	,
            5	,	0	,	0	,	5	,	5	,	0	,	0	,	5	,
            0	,	0	,	10	,	20	,	20	,	10	,	0	,	0	,
            5	,	5	,	5	,	10	,	10	,	5	,	5	,	5	,
            10	,	10	,	10	,	20	,	20	,	10	,	10	,	10	,
            20	,	20	,	20	,	30	,	30	,	20	,	20	,	20	,
            0	,	0	,	0	,	0	,	0	,	0	,	0	,	0	
        };

        public static readonly int[] KnightValues = new int[]{
            0	,	-10	,	0	,	0	,	0	,	0	,	-10	,	0	,
            0	,	0	,	0	,	5	,	5	,	0	,	0	,	0	,
            0	,	0	,	10	,	10	,	10	,	10	,	0	,	0	,
            0	,	0	,	10	,	20	,	20	,	10	,	5	,	0	,
            5	,	10	,	15	,	20	,	20	,	15	,	10	,	5	,
            5	,	10	,	10	,	20	,	20	,	10	,	10	,	5	,
            0	,	0	,	5	,	10	,	10	,	5	,	0	,	0	,
            0	,	0	,	0	,	0	,	0	,	0	,	0	,	0		
        };

        public static readonly int[] BishopValues = new int[]{
            0	,	0	,	-10	,	0	,	0	,	-10	,	0	,	0	,
            0	,	0	,	0	,	10	,	10	,	0	,	0	,	0	,
            0	,	0	,	10	,	15	,	15	,	10	,	0	,	0	,
            0	,	10	,	15	,	20	,	20	,	15	,	10	,	0	,
            0	,	10	,	15	,	20	,	20	,	15	,	10	,	0	,
            0	,	0	,	10	,	15	,	15	,	10	,	0	,	0	,
            0	,	0	,	0	,	10	,	10	,	0	,	0	,	0	,
            0	,	0	,	0	,	0	,	0	,	0	,	0	,	0	
        };

        public static readonly int[] RookValues = new int[] {
            0	,	0	,	5	,	10	,	10	,	5	,	0	,	0	,
            0	,	0	,	5	,	10	,	10	,	5	,	0	,	0	,
            0	,	0	,	5	,	10	,	10	,	5	,	0	,	0	,
            0	,	0	,	5	,	10	,	10	,	5	,	0	,	0	,
            0	,	0	,	5	,	10	,	10	,	5	,	0	,	0	,
            0	,	0	,	5	,	10	,	10	,	5	,	0	,	0	,
            25	,	25	,	25	,	25	,	25	,	25	,	25	,	25	,
            0	,	0	,	5	,	10	,	10	,	5	,	0	,	0	
        };

        public static readonly int[] KingEndgame = new int[]{	
            -50	,	-10	,	0	,	0	,	0	,	0	,	-10	,	-50	,
            -10,	0	,	10	,	10	,	10	,	10	,	0	,	-10	,
            0	,	10	,	20	,	20	,	20	,	20	,	10	,	0	,
            0	,	10	,	20	,	40	,	40	,	20	,	10	,	0	,
            0	,	10	,	20	,	40	,	40	,	20	,	10	,	0	,
            0	,	10	,	20	,	20	,	20	,	20	,	10	,	0	,
            -10,	0	,	10	,	10	,	10	,	10	,	0	,	-10	,
            -50	,	-10	,	0	,	0	,	0	,	0	,	-10	,	-50	
        };

        public static readonly int[] KingOpening = new int[]{	
            0	,	5	,	5	,	-10	,	-10	,	0	,	10	,	5	,
            -30	,	-30	,	-30	,	-30	,	-30	,	-30	,	-30	,	-30	,
            -50	,	-50	,	-50	,	-50	,	-50	,	-50	,	-50	,	-50	,
            -70	,	-70	,	-70	,	-70	,	-70	,	-70	,	-70	,	-70	,
            -70	,	-70	,	-70	,	-70	,	-70	,	-70	,	-70	,	-70	,
            -70	,	-70	,	-70	,	-70	,	-70	,	-70	,	-70	,	-70	,
            -70	,	-70	,	-70	,	-70	,	-70	,	-70	,	-70	,	-70	,
            -70	,	-70	,	-70	,	-70	,	-70	,	-70	,	-70	,	-70		
        };

        public static readonly int[] Mirror = new int[] {
            56	,	57	,	58	,	59	,	60	,	61	,	62	,	63	,
            48	,	49	,	50	,	51	,	52	,	53	,	54	,	55	,
            40	,	41	,	42	,	43	,	44	,	45	,	46	,	47	,
            32	,	33	,	34	,	35	,	36	,	37	,	38	,	39	,
            24	,	25	,	26	,	27	,	28	,	29	,	30	,	31	,
            16	,	17	,	18	,	19	,	20	,	21	,	22	,	23	,
            8	,	9	,	10	,	11	,	12	,	13	,	14	,	15	,
            0	,	1	,	2	,	3	,	4	,	5	,	6	,	7
        };

        public static readonly Dictionary<Pieces, int[]> PieceToValuesMap = new Dictionary<Pieces, int[]> {
            {Pieces.WhitePawn, PawnValues},
            {Pieces.BlackPawn, PawnValues},
            {Pieces.WhiteKnight, KnightValues},
            {Pieces.BlackKnight, KnightValues},
            {Pieces.WhiteRook, RookValues},
            {Pieces.BlackRook, RookValues},
            {Pieces.WhiteBishop, BishopValues},
            {Pieces.BlackBishop, BishopValues},
        };

        public static readonly int[] VictimScores = {0, 100, 200, 300, 400, 500, 600, 100, 200, 300, 400, 500, 600};
        public static int[,] MVVLVAScores = new int[13,13];
    }
}
