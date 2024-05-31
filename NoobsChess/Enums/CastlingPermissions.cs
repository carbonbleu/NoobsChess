using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobsEngine.Enums
{
    public enum CastlingPermissions
    {
        WhiteKingSideCastling = 1,
        WhiteQueenSideCastling = 2,
        BlackKingSideCastling = 4,
        BlackQueenSideCastling = 8,
        None = 0
    }
}
