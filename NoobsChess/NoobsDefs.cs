using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobsEngine
{
    class NoobsDefs
    {
        public const String EngineName = "NoobsEngine";
        public const int BoardSquareCount = 120;

        public const int MaxGameMoves = 2048;
        public const int MaxMovesPerPosition = 256;
        public const int MaxDepth = 64;

        public const String StartingFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        public const String AnotherPosition = "n1n5/PPPk4/8/8/8/8/4Kppp/5N1N w - - 0 1";
        public const String KiwipetePosition = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1";

        public const String WAC1 = "2rr3k/pp3pp1/1nnqbN1p/3pN3/2pP4/2P3Q1/PPB4P/R4RK1 w - -";
        public const int PawnStartFlag = 0x80000;
        public const int EnPassantCaptureFlag = 0x40000;
        public const int CastlingMoveFlag = 0x1000000;

        public static readonly Move NoMove = new(0);

        public const int Mate = 29000;
    }
}
