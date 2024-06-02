using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NoobsEngine.Enums;
using NoobsEngine.Data;
using NoobsEngine.Fen;

using static NoobsEngine.NoobsGlobals;
using NoobsChess;

namespace NoobsEngine
{
    public class NoobsEngineMain
    {
        static void Main(string[] args)
        {
            InitAll();

            ChessBoard board = new ChessBoard();
            FenUtils.ParseFen(NoobsDefs.StartingFEN, board);

            PerftTesting.FullTest(board, 4);            
        }       

        static void InitAll()
        {
            Initialize120SquareTo64();
            InitializeBitMasks();
            InitializeHashKeys();
            InitializeBoardLookups();
        }

        private static void InitializeBoardLookups()
        {
            for (int i = 0; i < NoobsDefs.BoardSquareCount; i++)
            {
                FileLookup[i] = (int) BoardSquares.Offboard;
                RankLookup[i] = (int) BoardSquares.Offboard;
            }

            for (BoardRanks rank = BoardRanks.Rank1; rank <= BoardRanks.Rank8; rank++) {
                for (BoardFiles file = BoardFiles.A; file <= BoardFiles.H; file++) {
                    int square = NoobsUtils.FileRankTo120Square(file, rank);
                    FileLookup[square] = (int) file;
                    RankLookup[square] = (int) rank;
                }
            }
        }

        static void InitializeHashKeys()
        {
            for (int i = 0; i < 13; i++) {
                for (int j = 0; j < 120; j++) {
                    PieceKeys[i, j] = NoobsUtils.NextUInt64();
                }
            }

            ColourKey = NoobsUtils.NextUInt64();

            for (int i = 0; i < 16; i++) {
                CastlingKeys[i] = NoobsUtils.NextUInt64();
            }
        }

        static void InitializeBitMasks()
        {
            for (int i = 0; i < 64; i++) {
                SetMask[i] = new BitBoard(0UL);
                ClearMask[i] = new BitBoard(0UL);
            }

            for (int i = 0; i < 64; i++) {
                SetMask[i].Value |= 1UL << i;
                ClearMask[i].Value = ~SetMask[i].Value;
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
                Console.Write("{0,5}", Square120To64[i]);

            }

            Console.WriteLine();
            Console.WriteLine();

            for (int i = 0; i < 64; i++)
            {
                if (i % 8 == 0)
                {
                    Console.WriteLine();
                }
                Console.Write("{0,5}", Square64To120[i]);

            }

            Console.WriteLine();
            Console.WriteLine();
        }

        static void Initialize120SquareTo64()
        {
            for (int idx = 0; idx < NoobsDefs.BoardSquareCount; idx++)
            {
                Square120To64[idx] = 65;
            }
            for (int idx = 0; idx < 64; idx++)
            {
                Square64To120[idx] = 120;
            }

            int square64 = 0;

            for (BoardRanks rank = BoardRanks.Rank1; rank <= BoardRanks.Rank8; rank++)
            {
                for (BoardFiles file = BoardFiles.A; file <= BoardFiles.H; file++)
                {
                    int square = NoobsUtils.FileRankTo120Square((int) file, (int) rank);
                    Square64To120[square64] = square;
                    Square120To64[square] = square64;
                    square64++;
                }
            }
        }
    }
}
