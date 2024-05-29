using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NoobsEngine;
using NoobsEngine.Enums;

namespace NoobsEngine.Data
{
    public class BitBoard
    {
        public UInt64 Value { get; set; }
        private const UInt64 one = 1UL;

        public readonly int[] BitTable = new int[]{
            63, 30, 3, 32, 25, 41, 22, 33, 15, 50, 42, 13, 11, 53, 19, 34, 61, 29, 2,
            51, 21, 43, 45, 10, 18, 47, 1, 54, 9, 57, 0, 35, 62, 31, 40, 4, 49, 5, 52,
            26, 60, 6, 23, 44, 46, 27, 56, 16, 7, 39, 48, 24, 59, 14, 12, 55, 38, 28,
            58, 20, 37, 17, 36, 8
        };

        public BitBoard(UInt64 board)
        {
            Value = board;
        }

        public void Print()
        {
            for (BoardRanks rank = BoardRanks.Rank8; rank >= BoardRanks.Rank1; rank--)
            {
                for (BoardFiles file = BoardFiles.A; file <= BoardFiles.H; file++)
                {
                    int square = NoobsUtils.FileRankTo120Square((int)file, (int)rank);
                    int square64 = NoobsGlobals.Square120To64[square];

                    if (((one << square64) & Value) != 0)
                    {
                        Console.Write("x");
                    }
                    else
                    {
                        Console.Write("-");
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public int PopBit() {
            UInt64 b = Value ^ (Value - 1);
            uint fold = (uint) ((b & 0xffffffff) ^ (b >> 32));
            Value &= Value - 1;
            return BitTable[(fold * 0x783a9b23) >> 26];
        }

        public int BitCount() {
            int r = 0;
            UInt64 x = Value;
            while (x > 0) {
                r++;
                x &= x - 1;
            }
            return r;
        }

        public void ClearBit(BoardSquares square) {
            Value &= NoobsGlobals.ClearMask[(int) square].Value;
        }

        public void ClearBit(int square) {
            Value &= NoobsGlobals.ClearMask[square].Value;
        }

        public void SetBit(BoardSquares square) {
            Value |= NoobsGlobals.SetMask[(int) square].Value;
        }

        public void SetBit(int square) {
            Value |= NoobsGlobals.SetMask[square].Value;
        }
    }
}
