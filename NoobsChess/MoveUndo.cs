using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobsEngine
{
    public class MoveUndo
    {
        public int Move { get; set; }
        public int CastlingPermission { get; set; }
        public int EnPassantSquare { get; set; }
        public int FiftyMoveRule { get; set; }
        public UInt64 PositionKey { get; set; }
    }
}
