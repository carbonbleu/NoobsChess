using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NoobsEngine.Enums;
using NoobsEngine.Data;

namespace NoobsEngine
{
    class NoobsEngineMain
    {
        static void Main(string[] args)
        {
            InitAll();

            BitBoard board = new BitBoard(0UL);
            
            board.SetBit(BoardSquares.A5);
            board.Print();

            board.ClearBit(BoardSquares.A5);
            board.Print();
        }

        static void InitAll()
        {
            Initialize120SquareTo64();
            InitializeBitMasks();
        }

        static void InitializeBitMasks()
        {
            for (int i = 0; i < 64; i++) {
                NoobsGlobals.SetMask[i] = new BitBoard(0UL);
                NoobsGlobals.ClearMask[i] = new BitBoard(0UL);
            }

            for (int i = 0; i < 64; i++) {
                NoobsGlobals.SetMask[i].Value |= 1UL << i;
                NoobsGlobals.ClearMask[i].Value = ~NoobsGlobals.SetMask[i].Value;
            }
        }

        static void PrintSquares()
        {
            for (int i = 0; i < NoobsDefs.BoardSquareCount; i++)
            {
                if (i % 10 == 0)
                {
                    Console.WriteLine();
                }
                Console.Write("{0,5}", NoobsGlobals.Square120To64[i]);

            }

            Console.WriteLine();
            Console.WriteLine();

            for (int i = 0; i < 64; i++)
            {
                if (i % 8 == 0)
                {
                    Console.WriteLine();
                }
                Console.Write("{0,5}", NoobsGlobals.Square64To120[i]);

            }

            Console.WriteLine();
            Console.WriteLine();
        }

        static void Initialize120SquareTo64()
        {
            for (int idx = 0; idx < NoobsDefs.BoardSquareCount; idx++)
            {
                NoobsGlobals.Square120To64[idx] = 65;
            }
            for (int idx = 0; idx < 64; idx++)
            {
                NoobsGlobals.Square64To120[idx] = 120;
            }

            int square64 = 0;

            for (BoardRanks rank = BoardRanks.Rank1; rank <= BoardRanks.Rank8; rank++)
            {
                for (BoardFiles file = BoardFiles.A; file <= BoardFiles.H; file++)
                {
                    int square = NoobsUtils.FileRankTo120Square((int) file, (int) rank);
                    NoobsGlobals.Square64To120[square64] = square;
                    NoobsGlobals.Square120To64[square] = square64;
                    square64++;
                }
            }
        }
    }
}
