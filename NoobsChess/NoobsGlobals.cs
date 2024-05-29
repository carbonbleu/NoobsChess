using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoobsEngine.Data;

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
    }
}
