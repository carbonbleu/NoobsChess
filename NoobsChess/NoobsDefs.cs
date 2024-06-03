using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobsEngine
{
    class NoobsDefs
    {
        public const String EngineName = "NoobsEngine v1";
        public const int BoardSquareCount = 120;

        public const int MaxGameMoves = 2048;
        public const int MaxMovesPerPosition = 256;
        public const int MaxDepth = 64;

        public const String StartingFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        public const String AnotherPosition = "n1n5/PPPk4/8/8/8/8/4Kppp/5N1N w - - 0 1";
        public const String KiwipetePosition = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1";

        public const String WAC1 = "r1b1k2r/ppppnppp/2n2q2/2b5/3NP3/2P1B3/PP3PPP/RN1QKB1R w KQkq - -";
        public const int PawnStartFlag = 0x80000;
        public const int EnPassantCaptureFlag = 0x40000;
        public const int CastlingMoveFlag = 0x1000000;

        public static readonly Move NoMove = new(0);

        public const int Mate = 29000;

        public const int InputBuffer = 400 * 6;
    }
}
