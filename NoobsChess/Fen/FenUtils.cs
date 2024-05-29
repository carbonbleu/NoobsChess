using NoobsEngine;
using NoobsEngine.Enums;

namespace NoobsChess.Fen {
    public class FenUtils {
        public static int ParseFen(String fen, ChessBoard board) {
            board.Reset();

            BoardRanks rank = BoardRanks.Rank8;
            BoardFiles file = BoardFiles.A;

            int piece = 0;
            int emptySquares = 0;
            
            int square64 = 0;
            int square120 = 0;

            int i = 0;

            while ((rank >= BoardRanks.Rank1) && i < fen.Length) {
                emptySquares = 1;
                switch (fen[i]) {
                    case 'p':
                        piece = (int) Pieces.BlackPawn;
                        break;
                    case 'n':
                        piece = (int) Pieces.BlackKnight;
                        break;
                    case 'b':
                        piece = (int) Pieces.BlackBishop;
                        break;
                    case 'r':
                        piece = (int) Pieces.BlackRook;
                        break;
                    case 'q':
                        piece = (int) Pieces.BlackQueen;
                        break;
                    case 'k':
                        piece = (int) Pieces.BlackKing;
                        break;
                    case 'P':
                        piece = (int) Pieces.WhitePawn;
                        break;
                    case 'N':
                        piece = (int) Pieces.WhiteKnight;
                        break;
                    case 'B':
                        piece = (int) Pieces.WhiteBishop;
                        break;
                    case 'R':
                        piece = (int) Pieces.WhiteRook;
                        break;
                    case 'Q':
                        piece = (int) Pieces.WhiteQueen;
                        break;
                    case 'K':
                        piece = (int) Pieces.WhiteKing;
                        break;

                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                        piece = (int) Pieces.Empty;
                        emptySquares = fen[i] - '0';
                        break;

                    case '/':
                    case ' ':
                        rank--;
                        file = BoardFiles.A;
                        i++;
                        continue;

                    default:
                        Console.WriteLine("Failed to parse FEN");
                        return -1;                    
                }

                for (int j = 0; j < emptySquares; j++) {
                    square64 = (int) rank * 8 + (int) file;
                    square120 = NoobsGlobals.Square64To120[square64];

                    if (piece != (int) Pieces.Empty) {
                        board.PiecesOnBoard[square120] = piece;
                    }

                    file++;
                }
                i++;
            }

            board.SideToMove = (int) ((fen[i] == 'w') ? Players.White : Players.Black);
            i += 2;

            for (int x = 0; x < 4; x++) {
                if (fen[i] == ' ') break;

                CastlingPermissions perm = fen[i] switch {
                    'K' => CastlingPermissions.WhiteKingSideCastling,
                    'Q' => CastlingPermissions.WhiteQueenSideCastling,
                    'k' => CastlingPermissions.BlackKingSideCastling,
                    'q' => CastlingPermissions.BlackQueenSideCastling,
                    _ => CastlingPermissions.Invalid
                };

                if (perm == CastlingPermissions.Invalid) {
                    return -1;
                }
                else {
                    board.CastlingPermission |= (int) perm;
                }
                i++;
            }           

            i++;

            if (fen[i] != '-') {
                int fileNum = fen[i] - 'a';
                int rankNum = fen[i + 1] - '1';

                board.EnPassant = NoobsUtils.FileRankTo120Square(fileNum, rankNum);
            }
           
            board.SetPositionKey(board.GeneratePositionKey());

            return 0;
        }
    }
}