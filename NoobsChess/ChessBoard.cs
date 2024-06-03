using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoobsEngine.Data;
using NoobsEngine.Enums;

using static NoobsEngine.NoobsGlobals;
using static NoobsEngine.Enums.Direction;
using static NoobsEngine.NoobsUtils;
using NoobsChess;
using NoobsChess.Search;

namespace NoobsEngine
{
    public class ChessBoard
    {
        public int[] PiecesOnBoard { get; set; } = new int[NoobsDefs.BoardSquareCount];
        public BitBoard[] Pawns { get; set; } = new BitBoard[3];

        int[] kingSquares = new int[2];

        public int SideToMove { get; set; }
        public int EnPassant { get; set; }
        public int FiftyMoveCounter { get; set; }

        public int Ply { get; set; }
        public int PliesMade { get; set; }

        public int[] MaterialScores { get; set; } = new int[2];

        UInt64 positionKey;

        public int[] PieceCount { get; set; } = new int[13];

        int[] bigPiecesByColour = new int[2];
        int[] majorPiecesByColour = new int[2];
        int[] minorPiecesByColour = new int[2];

        public int CastlingPermission { get; set; }

        MoveUndo[] history = new MoveUndo[NoobsDefs.MaxGameMoves];

        public int[,] PieceList { get; set; } = new int[13,10];

        public Dictionary<UInt64, Move> PVTable;
        public Move[] PVList = new Move[64];

        public int[,] SearchHistory = new int[13, NoobsDefs.BoardSquareCount];
        public int[,] SearchKillers = new int[2, NoobsDefs.MaxDepth];

        public ChessBoard() {
            Reset();
            PVTable = new Dictionary<ulong, Move>{};
        }

        public UInt64 GeneratePositionKey() {
            UInt64 finalKey = 0UL;

            int piece = (int) Pieces.Empty;

            for (int square = 0; square < NoobsDefs.BoardSquareCount; square++) {
                piece = PiecesOnBoard[square];
                if (piece != (int) BoardSquares.NoSquare && piece != (int) Pieces.Empty && piece != (int) BoardSquares.Offboard) {
                    finalKey ^= PieceKeys[piece,square];
                }
            }

            if (SideToMove == (int) Players.White) {
                finalKey ^= (UInt64) ColourKey;
            }

            if (EnPassant != (int) BoardSquares.NoSquare && EnPassant != (int) BoardSquares.Offboard) {
                finalKey ^= PieceKeys[0,EnPassant];
            }

            finalKey ^= CastlingKeys[CastlingPermission];

            return finalKey;
        }

        public void SetPositionKey(UInt64 positionKeyIn) {
            positionKey = positionKeyIn;
        }

        public UInt64 GetPositionKey() {
            return positionKey;
        }

        public void Reset() {
            for (int i = 0; i < NoobsDefs.BoardSquareCount; i++) {
                PiecesOnBoard[i] = (int) BoardSquares.Offboard;
            }

            for (int i = 0; i < 64; i++) {
                PiecesOnBoard[Square64To120[i]] = (int) Pieces.Empty;
            }

            for (int i = 0; i < 2; i++) {
                bigPiecesByColour[i] = 0;
                majorPiecesByColour[i] = 0;
                minorPiecesByColour[i] = 0;
                Pawns[i] = new BitBoard(0UL);
            }

            Pawns[(int) Players.Both] = new BitBoard(0UL);

            for (int i = 0; i < 13; i++) {
                PieceCount[i] = 0;
            }

            kingSquares[(int) Players.White] = kingSquares[(int) Players.Black] = (int) BoardSquares.NoSquare;

            SideToMove = (int) Players.Both;
            EnPassant = (int) BoardSquares.NoSquare;
            FiftyMoveCounter = 0;

            Ply = 0;
            PliesMade = 0;

            CastlingPermission = 0;

            positionKey = 0UL;

            for (int i = 0; i < NoobsDefs.MaxGameMoves; i++) {
                history[i] = new MoveUndo();
                history[i].Move = 0;
                history[i].CastlingPermission = 0;
                history[i].EnPassantSquare = 0;
                history[i].FiftyMoveRule = 0;
                history[i].PositionKey = 0UL;
            }

            MaterialScores[(int) Players.Black] = 0;
            MaterialScores[(int) Players.White] = 0;
        }

        public override String ToString() {
            String pieceChars = ".PNBRQKpnbrqk";
            String colourChars = "wb-";
            String fileChars = "abcdefgh";

            StringBuilder result = new StringBuilder();

            for (BoardRanks rank = BoardRanks.Rank8; rank >= BoardRanks.Rank1; rank--) {
                result.Append(String.Format("{0}\t", (int) (rank + 1)));
                for (BoardFiles file = BoardFiles.A; file <= BoardFiles.H; file++) {
                    int square = FileRankTo120Square((int) file, (int) rank);
                    int piece = PiecesOnBoard[square];
                    
                    result.Append(String.Format("{0:3} ", pieceChars[piece]));
                }

                result.Append("\n");
            }
            result.Append("\t");
            for (BoardFiles file = BoardFiles.A; file <= BoardFiles.H; file++) {
                result.Append(fileChars[(int)file] + " ");
            }
            result.Append("\n");
            result.Append(String.Format("Side to move: {0}\n", colourChars[SideToMove]));
            result.Append(String.Format("En Passant Square: {0}\n", EnPassant));

            result.Append(String.Format(
                "Castling Permissions: {0}{1}{2}{3}\n",
                ((CastlingPermission & (int) CastlingPermissions.WhiteKingSideCastling) > 0) ? 'K' : '-',
                ((CastlingPermission & (int) CastlingPermissions.WhiteQueenSideCastling) > 0) ? 'Q' : '-',
                ((CastlingPermission & (int) CastlingPermissions.BlackKingSideCastling) > 0) ? 'k' : '-',
                ((CastlingPermission & (int) CastlingPermissions.BlackQueenSideCastling) > 0) ? 'q' : '-'
            ));
            result.Append(String.Format("Position Key: {0:X}", positionKey));
            return result.ToString();
        }

        public void UpdateMaterial() {
            for (int i = 0; i < NoobsDefs.BoardSquareCount; i++) {
                int square = i;
                int piece = PiecesOnBoard[square];
                if (piece != (int) BoardSquares.Offboard && piece != (int) Pieces.Empty) {
                    int pieceColour = (int) PieceColours[piece];
                    if (PieceBig[piece]) {
                        bigPiecesByColour[pieceColour]++;
                    }
                    if (PieceMajor[piece]) {
                        majorPiecesByColour[pieceColour]++;
                    }
                    if (PieceMinor[piece]) {
                        minorPiecesByColour[pieceColour]++;
                    }

                    MaterialScores[pieceColour] += PieceValues[piece];

                    PieceList[piece,PieceCount[piece]] = square;
                    PieceCount[piece]++;

                    if (piece == (int) Pieces.WhiteKing) {
                        kingSquares[(int) Players.White] = square;
                    }
                    if (piece == (int) Pieces.BlackKing) {
                        kingSquares[(int) Players.Black] = square;
                    }

                    if (piece == (int) Pieces.WhitePawn) {
                        Pawns[(int) Players.White].SetBit(Square120To64[square]);
                        Pawns[(int) Players.Both].SetBit(Square120To64[square]);
                    }
                    else if (piece == (int) Pieces.BlackPawn) {
                        Pawns[(int) Players.Black].SetBit(Square120To64[square]);
                        Pawns[(int) Players.Both].SetBit(Square120To64[square]);
                    }
                }
                
            }
        }

        public bool CheckBoard() {
            int[] tempPieceCount = new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
            int[] tempBigPiecesByColour = new int[] {0, 0};
            int[] tempMajorPiecesByColour = new int[] {0, 0};
            int[] tempMinorPiecesByColour = new int[] {0, 0};
            int[] tempMaterialScores = new int[] {0, 0};

            BitBoard[] tempPawns = new BitBoard[] {
                new BitBoard(Pawns[(int) Players.White].Value), 
                new BitBoard(Pawns[(int) Players.Black].Value), 
                new BitBoard(Pawns[(int) Players.Both].Value)
            };
            
            for (Pieces piece = Pieces.WhitePawn; piece <= Pieces.BlackKing; piece++) {
                for (int tempI = 0; tempI < PieceCount[(int) piece]; tempI++) {
                    int square120 = PieceList[(int)piece, tempI];
                    if (PiecesOnBoard[square120] != (int) piece) {
                        return false;
                    }
                }
            }

            for (int square64 = 0; square64 < 64; square64++) {
                int square120 = Square64To120[square64];
                int tempPiece = PiecesOnBoard[square120];
                tempPieceCount[tempPiece]++;
                int colour = (int)PieceColours[tempPiece];
                
                if (PieceBig[tempPiece]) {
                    tempBigPiecesByColour[colour]++;
                }
                if (PieceMajor[tempPiece]) {
                    tempMajorPiecesByColour[colour]++;
                }
                if (PieceMinor[tempPiece]) {
                    tempMinorPiecesByColour[colour]++;
                }
                if (colour != (int) Players.Both)
                    tempMaterialScores[colour] += PieceValues[tempPiece];
            }

            for (Pieces tempPiece = Pieces.WhitePawn; tempPiece <= Pieces.BlackKing; tempPiece++) {
                if (tempPieceCount[(int) tempPiece] != PieceCount[(int) tempPiece]) {
                    return false;
                }
            }
            
            if (tempPawns[(int) Players.White].BitCount() != PieceCount[(int) Pieces.WhitePawn]) {
                return false;
            }

            if (tempPawns[(int) Players.Black].BitCount() != PieceCount[(int) Pieces.BlackPawn]) {
                return false;
            }

            if (tempPawns[(int) Players.Both].BitCount() != PieceCount[(int) Pieces.WhitePawn] + PieceCount[(int) Pieces.BlackPawn]) {
                return false;
            }
            
            while (tempPawns[(int) Players.White].Value > 0) {
                int square64 = tempPawns[(int) Players.White].PopBit();
                if (PiecesOnBoard[Square64To120[square64]] != (int) Pieces.WhitePawn) {
                    return false;
                }
            }

            while (tempPawns[(int) Players.Black].Value > 0) {
                int square64 = tempPawns[(int) Players.Black].PopBit();
                if (PiecesOnBoard[Square64To120[square64]] != (int) Pieces.BlackPawn) {
                    return false;
                }
            }

            while (tempPawns[(int) Players.Both].Value > 0) {
                int square64 = tempPawns[(int) Players.Both].PopBit();
                if (!((PiecesOnBoard[Square64To120[square64]] == (int) Pieces.BlackPawn) || (PiecesOnBoard[Square64To120[square64]] == (int) Pieces.WhitePawn))) {
                    return false;
                }
            }

            
            if (tempMaterialScores[(int) Players.White] != MaterialScores[(int) Players.White]) {
                return false;
            }
            
            if (tempMaterialScores[(int) Players.Black] != MaterialScores[(int) Players.Black]) {
                return false;
            }
            
            if (tempBigPiecesByColour[(int) Players.White] != bigPiecesByColour[(int) Players.White]) {
                return false;
            }
            
            if (tempBigPiecesByColour[(int) Players.Black] != bigPiecesByColour[(int) Players.Black]) {
                return false;
            }
            
            if (tempMajorPiecesByColour[(int) Players.White] != majorPiecesByColour[(int) Players.White]) {
                return false;
            }
            
            if (tempMajorPiecesByColour[(int) Players.Black] != majorPiecesByColour[(int) Players.Black]) {
                return false;
            }
            
            if (tempMinorPiecesByColour[(int) Players.White] != minorPiecesByColour[(int) Players.White]) {
                return false;
            }
            
            if (tempMinorPiecesByColour[(int) Players.Black] != minorPiecesByColour[(int) Players.Black]) {
                return false;
            }
            
            if (SideToMove != (int) Players.White && SideToMove != (int) Players.Black) {
                return false;
            }
            
            // if (GeneratePositionKey() != positionKey) {
            //     return false;
            // }
            
            if (!((EnPassant == (int) BoardSquares.NoSquare) 
                || ((RankLookup[EnPassant] == (int) BoardRanks.Rank6) && (SideToMove == (int) Players.White))
                || ((RankLookup[EnPassant] == (int) BoardRanks.Rank3) && (SideToMove == (int) Players.Black))
            )) {
                return false;
            }
            
            if (!(PiecesOnBoard[kingSquares[(int) Players.White]] == (int) Pieces.WhiteKing)) {
                return false;
            }
            
            if (!(PiecesOnBoard[kingSquares[(int) Players.Black]] == (int) Pieces.BlackKing)) {
                return false;
            }
        
            return true;
        }

        public bool IsSquareUnderAttackBy(int square, int side) {
            // Check pawns attacking
            if (side == (int) Players.White) {
                if ((PiecesOnBoard[square - SouthWest] == (int) Pieces.WhitePawn) || (PiecesOnBoard[square - SouthEast] == (int) Pieces.WhitePawn)) {
                    return true;
                } 
            }
            if (side == (int) Players.Black) {
                if ((PiecesOnBoard[square - NorthEast] == (int) Pieces.BlackPawn) || (PiecesOnBoard[square - NorthWest] == (int) Pieces.BlackPawn)) {
                    return true;
                } 
            }

            // Check knights attacking
            for (int i = 0; i < KnightDirection.Length; i++) {
                int piece = PiecesOnBoard[square + KnightDirection[i]];
                if (piece != (int) BoardSquares.Offboard && PieceKnight[piece] && (int) PieceColours[piece] == side) {
                    return true;
                }
            }

            // Check rooks attacking / queens attacking horizontally-vertically
            for (int i = 0; i < RookDirection.Length; i++) {
                int rookDir = RookDirection[i];
                int temp = square + rookDir;
                int piece = PiecesOnBoard[temp];

                while (piece != (int) BoardSquares.Offboard) {
                    if (piece != (int) Pieces.Empty) {
                        if (PieceRookOrQueen[piece] && ((int) PieceColours[piece] == side)) {
                            return true;
                        }
                        break;
                    }

                    temp += rookDir;
                    piece = PiecesOnBoard[temp];
                }
            }

            // Check bishops attacking / queens attacking diagonally
            for (int i = 0; i < BishopDirection.Length; i++) {
                int bishopDir = BishopDirection[i];
                int temp = square + bishopDir;
                int piece = PiecesOnBoard[temp];

                while (piece != (int) BoardSquares.Offboard) {
                    if (piece != (int) Pieces.Empty) {
                        if (PieceBishopOrQueen[piece] && (int) PieceColours[piece] == side) {
                            return true;
                        }
                        break;
                    }

                    temp += bishopDir;
                    piece = PiecesOnBoard[temp];
                }
            }

            // Check kings attacking
            for (int i = 0; i < KingDirection.Length; i++) {
                int piece = PiecesOnBoard[square + KingDirection[i]];
                if (piece != (int) BoardSquares.Offboard && PieceKing[piece] && (int) PieceColours[piece] == side) {
                    return true;
                }
            }

            return false;
        }

        public void ShowSquaresAttackedBy(int side) {
            for (BoardRanks rank = BoardRanks.Rank8; rank >= BoardRanks.Rank1; rank--) {
                for (BoardFiles file = BoardFiles.A; file <= BoardFiles.H; file++) {
                    int square = FileRankTo120Square(file, rank);
                    if (IsSquareUnderAttackBy(square, side)) {
                        Console.Write("X ");
                    }
                    else {
                        Console.Write("- ");
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public void ShowSquaresAttackedBy(Players side) {
            ShowSquaresAttackedBy((int) side);
        }

        public void HashPiece(Pieces piece, int square) {
            positionKey ^= PieceKeys[(int) piece, square];
        }

        public void HashCastle() {
            positionKey ^= CastlingKeys[CastlingPermission];
        }

        public void HashPlayer() {
            positionKey ^= (UInt64) ColourKey;
        }

        public void HashEnPassant() {
            positionKey ^= (UInt64) EnPassant;
        }

        public void ClearPiece(int square) {
            if (!IsSquareOnBoard(square)) {
                return;
            }

            int piece = PiecesOnBoard[square];
            int colour = (int)PieceColours[piece];

            if (!IsPieceValid(piece)) {
                return;
            }

            HashPiece((Pieces) piece, square);

            PiecesOnBoard[square] = (int) Pieces.Empty;
            MaterialScores[colour] -= PieceValues[piece];

            if (PieceBig[piece]) {
                bigPiecesByColour[colour]--;
                if (PieceMajor[piece]) {
                    majorPiecesByColour[colour]--;
                }
                if (PieceMinor[piece]) {
                    minorPiecesByColour[colour]--;
                }
            }
            else {
                Pawns[colour].ClearBit(Square120To64[square]);
                Pawns[(int) Players.Both].ClearBit(Square120To64[square]);
            }

            int pieceIndex = -1;

            for (int i = 0; i < PieceCount[piece]; i++) {
                if (PieceList[piece, i] == square) {
                    pieceIndex = i;
                    break;
                }
            }

            if (pieceIndex == -1) return;
            
            PieceCount[piece]--;
            PieceList[piece, pieceIndex] = PieceList[piece, PieceCount[piece]];
        }

        public void AddPiece(int square, int piece) {
            if (!IsSquareOnBoard(square)) {
                return;
            }

            int colour = (int)PieceColours[piece];

            if (!IsPieceValid(piece)) {
                return;
            }

            HashPiece((Pieces) piece, square);

            PiecesOnBoard[square] = piece;
            
            if (PieceBig[piece]) {
                bigPiecesByColour[colour]++;
                if (PieceMajor[piece]) {
                    majorPiecesByColour[colour]++;
                }
                if (PieceMinor[piece]) {
                    minorPiecesByColour[colour]++;
                }
            }
            else {
                Pawns[colour].SetBit(Square120To64[square]);
                Pawns[(int) Players.Both].SetBit(Square120To64[square]);
            }

            MaterialScores[colour] += PieceValues[piece];
            PieceList[piece, PieceCount[piece]++] = square;            
        }

        public void AddPiece(int square, Pieces piece) {
            AddPiece(square, (int) piece);
        }

        public void MovePiece(int from, int to) {
            if (!IsSquareOnBoard(from) || !IsSquareOnBoard(to)) {
                return;
            }

            int piece = PiecesOnBoard[from];
            int colour = (int) PieceColours[piece];

            HashPiece((Pieces) piece, from);
            PiecesOnBoard[from] = (int) Pieces.Empty;

            HashPiece((Pieces) piece, to);
            PiecesOnBoard[to] = piece;

            if (!PieceBig[piece]) {
                Pawns[colour].ClearBit(Square120To64[from]);
                Pawns[(int) Players.Both].ClearBit(Square120To64[from]);
                Pawns[colour].SetBit(Square120To64[to]);
                Pawns[(int) Players.Both].SetBit(Square120To64[to]);
            }

            for (int i = 0; i < PieceCount[piece]; i++) {
                if (PieceList[piece, i] == from) {
                    PieceList[piece, i] = to;
                    break;
                }
            }
        }

        /*
            Returns false if the king is in check/move is illegal
        */
        public bool MakeMove(Move move) {
            CheckBoard();

            int from = move.GetFromPosition();
            int to = move.GetToPosition();

            int side = SideToMove;

            if (!IsSquareOnBoard(from) || !IsSquareOnBoard(to)) {
                throw new Exception("Invalid move");
            }

            if (!IsValidSide(side)) {
                throw new Exception("Invalid side");
            }

            if (!IsPieceValid(PiecesOnBoard[from])) {
                throw new Exception("Invalid piece");
            }

            history[PliesMade].PositionKey = positionKey;

            if (move.IsEnPassantCapture()) {
                if (side == (int) Players.White) {
                    ClearPiece(to - 10);
                }
                else {
                    ClearPiece(to + 10);
                }
            }
            else if (move.IsCastle()) {
                switch ((BoardSquares) to) {
                    case BoardSquares.C1:
                        MovePiece((int) BoardSquares.A1, (int) BoardSquares.D1);
                        break;
                    case BoardSquares.C8:
                        MovePiece((int) BoardSquares.A8, (int) BoardSquares.D8);
                        break;
                    case BoardSquares.G1:
                        MovePiece((int) BoardSquares.H1, (int) BoardSquares.F1);
                        break;
                    case BoardSquares.G8:
                        MovePiece((int) BoardSquares.H8, (int) BoardSquares.F8);
                        break;
                    default:
                        throw new Exception("Invalid castling move");
                }
            }

            if (EnPassant != (int) BoardSquares.NoSquare) {
                HashEnPassant();
            }
            
            HashCastle();

            history[PliesMade].Move = move.Value;
            history[PliesMade].CastlingPermission = CastlingPermission;
            history[PliesMade].FiftyMoveRule = FiftyMoveCounter;
            history[PliesMade].EnPassantSquare = EnPassant;

            CastlingPermission &= CastlePermNums[from];
            CastlingPermission &= CastlePermNums[to];

            EnPassant = (int) BoardSquares.NoSquare;

            HashCastle();

            int capturedPiece = move.GetCapturedPiece();
            FiftyMoveCounter++;

            if (capturedPiece != (int) Pieces.Empty) {
                if (!IsPieceValid(capturedPiece)) {
                    throw new Exception("Invalid Piece");
                }

                ClearPiece(to);
                FiftyMoveCounter = 0;
            }

            PliesMade++;
            Ply++;

            if (PiecePawn[PiecesOnBoard[from]]) {
                FiftyMoveCounter = 0;
                if (move.IsPawnStartMove()) {
                    if (SideToMove == (int) Players.White) {
                        EnPassant = from + 10;
                        if (RankLookup[EnPassant] != (int) BoardRanks.Rank3) {
                            throw new Exception("Invalid EnPassant Square");
                        }
                    }
                    else {
                        EnPassant = from - 10;
                        if (RankLookup[EnPassant] != (int) BoardRanks.Rank6) {
                            throw new Exception("Invalid EnPassant Square");
                        }
                    }
                    HashEnPassant();    
                }
            }

            MovePiece(from, to);

            int promotedPiece = move.GetPromotedPiece();
            if (IsPieceValid(promotedPiece) && !PiecePawn[promotedPiece]) {
                ClearPiece(to);
                AddPiece(to, promotedPiece);
            }

            if (PieceKing[PiecesOnBoard[to]]) {
                kingSquares[SideToMove] = to;
            }

            SideToMove ^= 1;
            HashPlayer();

            CheckBoard();

            if (IsSquareUnderAttackBy(kingSquares[side], SideToMove)) {
                UndoMove();
                return false;
            }

            return true;
        }

        public void UndoMove()
        {
            CheckBoard();

            PliesMade--;
            Ply--;

            Move move = new Move(history[PliesMade].Move);
            int from = move.GetFromPosition();
            int to = move.GetToPosition();

            if (!IsSquareOnBoard(from) || !IsSquareOnBoard(to)) {
                throw new Exception("Invalid board");
            }

            if (EnPassant != (int) BoardSquares.NoSquare) {
                HashEnPassant();
            }
            
            HashCastle();

            CastlingPermission = history[PliesMade].CastlingPermission;
            FiftyMoveCounter = history[PliesMade].FiftyMoveRule;
            EnPassant = history[PliesMade].EnPassantSquare;

            if (EnPassant != (int) BoardSquares.NoSquare) {
                HashEnPassant();
            }

            HashCastle();

            SideToMove ^= 1;
            HashPlayer();

            if (move.IsEnPassantCapture()) {
                if (SideToMove == (int) Players.White) {
                    AddPiece(to - 10, (int) Pieces.BlackPawn);
                }
                else {
                    AddPiece(to + 10, (int) Pieces.WhitePawn);
                }
            }
            else if (move.IsCastle()) {
                switch ((BoardSquares) to) {
                    case BoardSquares.C1:
                        MovePiece((int) BoardSquares.D1, (int) BoardSquares.A1);
                        break;
                    case BoardSquares.C8:
                        MovePiece((int) BoardSquares.D8, (int) BoardSquares.A8);
                        break;
                    case BoardSquares.G1:
                        MovePiece((int) BoardSquares.F1, (int) BoardSquares.H1);
                        break;
                    case BoardSquares.G8:
                        MovePiece((int) BoardSquares.F8, (int) BoardSquares.H8);
                        break;
                    default:
                        throw new Exception("Invalid castling move");
                }
            }

            MovePiece(to, from);

            if (PieceKing[PiecesOnBoard[from]]) {
                kingSquares[SideToMove] = from;
            }

            int capturedPiece = move.GetCapturedPiece();
            if (capturedPiece != (int) Pieces.Empty) {
                if (!IsPieceValid(capturedPiece)) {
                    throw new Exception("Invalid Captured Piece");
                }
                AddPiece(to, capturedPiece);
            }            

            int promotedPiece = move.GetPromotedPiece();
            if (promotedPiece != (int) Pieces.Empty) {
                if (!IsPieceValid(promotedPiece) && !PiecePawn[promotedPiece]) {
                    throw new Exception("Invalid Captured Piece");
                }
                ClearPiece(from);
                AddPiece(from, (PieceColours[promotedPiece] == Players.White ? Pieces.WhitePawn : Pieces.BlackPawn));
            }

            CheckBoard();  
        }

        public bool IsRepetition() {
            for (int i = PliesMade - FiftyMoveCounter; i < PliesMade - 1; i++) {
                if (positionKey == history[i].PositionKey) {
                    return true;
                }
            }

            return false;
        }

        public void StoreMove(Move move) {
            PVTable[positionKey] = move;
        }

        public Move GetStoredMove(UInt64 positionKey) {
            if (!PVTable.ContainsKey(positionKey)) {
                return NoobsDefs.NoMove;
            }
            return PVTable[positionKey];
        }

        public bool DoesMoveExist(Move move) {
            MoveGen moveGen = new MoveGen();
            moveGen.GenerateAllMoves(this);

            for (int i = 0; i < moveGen.Moves.Count; i++) {
                Move mvI = moveGen.Moves[i];
                if (!MakeMove(mvI)) {
                    continue;
                }
                UndoMove();
                if (mvI.Value == move.Value) {
                    return true;
                }
            }

            return false;
        }

        public void FillPVLine(int depth) {
            Move move = GetStoredMove(positionKey);
            int count = 0;
            
            while (!move.Equals(NoobsDefs.NoMove) && count < depth) {
                if (DoesMoveExist(move)) {
                    MakeMove(move);
                    PVList[count] = move;
                    count++;
                }
                else {
                    break;
                }

                move = GetStoredMove(positionKey);
            }

            while (Ply > 0) {
                UndoMove();
            }
        } 

        public int Evaluate() {
            int score = MaterialScores[(int) Players.White] - MaterialScores[(int) Players.Black];

            score += GetCumulativeScoresForPiece(Pieces.WhitePawn, Players.White);
            score -= GetCumulativeScoresForPiece(Pieces.BlackPawn, Players.Black);
            score += GetCumulativeScoresForPiece(Pieces.WhiteKnight, Players.White);
            score -= GetCumulativeScoresForPiece(Pieces.BlackKnight, Players.Black);
            score += GetCumulativeScoresForPiece(Pieces.WhiteBishop, Players.White);
            score -= GetCumulativeScoresForPiece(Pieces.BlackBishop, Players.Black);
            score += GetCumulativeScoresForPiece(Pieces.WhiteKing, Players.White);
            score -= GetCumulativeScoresForPiece(Pieces.BlackKing, Players.Black);

            score += GetCumulativeScoresForKing(Players.White);
            score -= GetCumulativeScoresForKing(Players.Black);

            if (SideToMove == (int) Players.White) {
                return score;
            } 
            else {
                return -score;
            }
        }

        private int GetCumulativeScoresForKing(Players colour)
        {
            Pieces piece;
            int score = 0;
            if (colour == Players.White) {
                piece = Pieces.WhiteKing;
            }
            else {
                piece = Pieces.BlackKing;
            }

            for (int i = 0; i < PieceCount[(int) piece]; i++) {
                int square = Square120To64[PieceList[(int) piece, i]];
                if (colour == Players.Black) {
                    square = Mirror(square);
                }
                score += NoobsGlobals.KingOpening[square];                
            }

            return score;
        }

        private int GetCumulativeScoresForPiece(Pieces piece, Players colour)
        {
            int score = 0;

            for (int i = 0; i < PieceCount[(int) piece]; i++) {
                int square = Square120To64[PieceList[(int) piece, i]];
                if (colour == Players.Black) {
                    square = Mirror(square);
                }
                if (piece != Pieces.WhiteKing && piece != Pieces.BlackKing) {
                    score += NoobsGlobals.PieceToValuesMap[piece][square];
                }                
            }

            return score;
        }

        public void SearchPosition(SearchInfo info) {
            Move bestMove = NoobsDefs.NoMove;
            
            ClearForSearch(info);

            for (int depth = 1; depth <= info.DepthLimit; depth++) {
                int bestScore = AlphaBetaPrune(-30000, 30000, depth, info, true);
                FillPVLine(depth);
                bestMove = PVList[0];

                Console.WriteLine("Depth: {0} Score: {1}, Move: {2}, Nodes: {3}", depth, bestScore, bestMove, info.Nodes);

                Console.WriteLine("Line of {0} moves", depth);

                foreach (Move mvI in PVList) {
                    Console.Write("{0} ", mvI);
                }
                Console.WriteLine();

                Console.WriteLine("Ordering: {0}", info.FailHigh / info.FailHighFirst);
            }
        }

        public void PollTimeOver() {

        }

        public void ClearForSearch(SearchInfo info) {
            for (int i = 0; i < 13; i++) {
                for (int j = 0; j < NoobsDefs.BoardSquareCount; j++) {
                    SearchHistory[i, j] = 0;
                }
            }

            for (int i = 0; i < 2; i++) {
                for (int j = 0; j < NoobsDefs.MaxDepth; j++) {
                    SearchKillers[i, j] = 0;
                }
            }

            PVTable.Clear();

            Ply = 0;

            info.StartTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            info.StopTime = 0;
            info.Nodes = 0;
            info.FailHigh = 0;
            info.FailHighFirst = 0;
        }

        public int AlphaBetaPrune(int alpha, int beta, int depth, SearchInfo info, bool nullMovesAllowed) {
            if (depth == 0) {
                info.Nodes++;
                return Evaluate();
            }

            info.Nodes++;

            if (IsRepetition() || FiftyMoveCounter >= 100) {
                return 0;
            }

            if (Ply > NoobsDefs.MaxDepth - 1) {
                return Evaluate();
            }

            MoveGen moveGen = new MoveGen();
            moveGen.GenerateAllMoves(this);

            int legalMoves = 0;
            int oldAlpha = alpha;
            Move bestMove = NoobsDefs.NoMove;
            
            for (int i = 0; i < moveGen.Moves.Count; i++) {
                Move move = moveGen.Moves[i];

                if (!MakeMove(move)) {
                    continue;
                }

                legalMoves++;
                int score = -AlphaBetaPrune(-beta, -alpha, depth - 1, info, nullMovesAllowed);
                UndoMove();

                if (score > alpha) {
                    if (score >= beta) {
                        if (legalMoves == 1) {
                            info.FailHighFirst++;
                        }
                        info.FailHigh++;
                        return beta;
                    }

                    alpha = score;
                    bestMove = move;
                }
            }

            if (legalMoves == 0) {
                if (IsSquareUnderAttackBy(kingSquares[SideToMove], SideToMove ^ 1)) {
                    return -NoobsDefs.Mate + Ply;
                }
                else {
                    return 0;
                }
            }

            if (alpha != oldAlpha) {
                StoreMove(bestMove);
            }

            return alpha;
        }

        public int QuiescenceSearch(int alpha, int beta, SearchInfo info) {
            return 0;
        }

    }
}
