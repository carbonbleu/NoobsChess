using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobsEngine
{
    class NoobsDefs
    {
        public const String Name = "NoobsEngine";
        public const int BoardSquareCount = 120;

        public const int MaxGameMoves = 2048;
        public const int MaxMovesPerPosition = 256;

        public const String StartingFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        public const String AnotherPosition = "k7/8/8/1q1Pp1K1/8/8/8/8 w - e6 0 2";
        public const String KiwipetePosition = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1";

        public const int PawnStartFlag = 0x80000;
        public const int EnPassantCaptureFlag = 0x40000;
        public const int CastlingMoveFlag = 0x1000000;
    }
}
