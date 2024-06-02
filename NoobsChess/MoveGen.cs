using NoobsEngine;
using NoobsEngine.Enums;
using static NoobsEngine.NoobsGlobals;
using static NoobsEngine.NoobsUtils;

namespace NoobsChess;

public class MoveGen
{
    public List<Move> Moves { get; private set; } = new List<Move>();

    public MoveGen() {

    }

    public void AddQuietMove(ChessBoard position, Move move) {
        Moves.Add(move);
    }

    public void AddQuietMove(ChessBoard position, int move) {
        AddQuietMove(position, new Move(move));
    }

    public void AddCaptureMove(ChessBoard position, Move move) {
        Moves.Add(move);
    }

    public void AddCaptureMove(ChessBoard position, int move) {
        AddCaptureMove(position, new Move(move));
    }

    public void AddEnPassantMove(ChessBoard position, Move move) {
        Moves.Add(move);
    }

    public void AddEnPassantMove(ChessBoard position, int move) {
        AddEnPassantMove(position, new Move(move));
    }

    void AddWhitePawnCapture(ChessBoard position, int from, int to, int capturedPiece) {
        if (!IsPieceValidOrEmpty(capturedPiece) || !IsSquareOnBoard(from) || !IsSquareOnBoard(to)) {
            return;
        }

        if (RankLookup[from] == (int) BoardRanks.Rank7) {
            AddCaptureMove(position, ConstructMove(from, to, capturedPiece, (int) Pieces.WhiteQueen, 0));
            AddCaptureMove(position, ConstructMove(from, to, capturedPiece, (int) Pieces.WhiteRook, 0));
            AddCaptureMove(position, ConstructMove(from, to, capturedPiece, (int) Pieces.WhiteBishop, 0));
            AddCaptureMove(position, ConstructMove(from, to, capturedPiece, (int) Pieces.WhiteKnight, 0));
        }
        else {
            AddCaptureMove(position, ConstructMove(from, to, capturedPiece, (int) Pieces.Empty, 0));
        }
    }

    void AddWhitePawnMove(ChessBoard position, int from, int to) {
        if (!IsSquareOnBoard(from) || !IsSquareOnBoard(to)) {
            return;
        }

        if (RankLookup[from] == (int) BoardRanks.Rank7) {
            AddQuietMove(position, ConstructMove(from, to, (int) Pieces.Empty, (int) Pieces.WhiteQueen, 0));
            AddQuietMove(position, ConstructMove(from, to, (int) Pieces.Empty, (int) Pieces.WhiteRook, 0));
            AddQuietMove(position, ConstructMove(from, to, (int) Pieces.Empty, (int) Pieces.WhiteBishop, 0));
            AddQuietMove(position, ConstructMove(from, to, (int) Pieces.Empty, (int) Pieces.WhiteKnight, 0));
        }
        else {
            AddQuietMove(position, ConstructMove(from, to, (int) Pieces.Empty, (int) Pieces.Empty, 0));
        }
    }


    void AddBlackPawnCapture(ChessBoard position, int from, int to, int capturedPiece) {
        if (!IsPieceValidOrEmpty(capturedPiece) || !IsSquareOnBoard(from) || !IsSquareOnBoard(to)) {
            return;
        }

        if (RankLookup[from] == (int) BoardRanks.Rank2) {
            AddCaptureMove(position, ConstructMove(from, to, capturedPiece, (int) Pieces.BlackQueen, 0));
            AddCaptureMove(position, ConstructMove(from, to, capturedPiece, (int) Pieces.BlackRook, 0));
            AddCaptureMove(position, ConstructMove(from, to, capturedPiece, (int) Pieces.BlackBishop, 0));
            AddCaptureMove(position, ConstructMove(from, to, capturedPiece, (int) Pieces.BlackKnight, 0));
        }
        else {
            AddCaptureMove(position, ConstructMove(from, to, capturedPiece, (int) Pieces.Empty, 0));
        }
    }

    void AddBlackPawnMove(ChessBoard position, int from, int to) {
        if (!IsSquareOnBoard(from) || !IsSquareOnBoard(to)) {
            return;
        }

        if (RankLookup[from] == (int) BoardRanks.Rank2) {
            AddQuietMove(position, ConstructMove(from, to, (int) Pieces.Empty, (int) Pieces.BlackQueen, 0));
            AddQuietMove(position, ConstructMove(from, to, (int) Pieces.Empty, (int) Pieces.BlackRook, 0));
            AddQuietMove(position, ConstructMove(from, to, (int) Pieces.Empty, (int) Pieces.BlackBishop, 0));
            AddQuietMove(position, ConstructMove(from, to, (int) Pieces.Empty, (int) Pieces.BlackKnight, 0));
        }
        else {
            AddQuietMove(position, ConstructMove(from, to, (int) Pieces.Empty, (int) Pieces.Empty, 0));
        }
    }

    public void GenerateAllMoves(ChessBoard position) {
        if (!position.CheckBoard()) {
            throw new Exception("Invalid Board");
        }

        Moves = new List<Move>();

        if (position.SideToMove == (int) Players.White) {
            GenerateWhitePawnMoves(position);
        }
        else {
            GenerateBlackPawnMoves(position);
        }        
        GenerateCastlingMoves(position);
        GenerateSliderMoves(position);
        GenerateNonSliderMoves(position);
    }

    private void GenerateCastlingMoves(ChessBoard position)
    {
        if (position.SideToMove == (int) Players.White) {
            if ((position.CastlingPermission & (int) CastlingPermissions.WhiteKingSideCastling) > 0) {
                if ((position.PiecesOnBoard[(int) BoardSquares.F1] == (int) Pieces.Empty) && (position.PiecesOnBoard[(int) BoardSquares.G1] == (int) Pieces.Empty)) {
                    if (!position.IsSquareUnderAttackBy((int) BoardSquares.E1, (int) Players.Black) 
                        && !position.IsSquareUnderAttackBy((int) BoardSquares.F1, (int) Players.Black)) {
                        AddQuietMove(position, ConstructMove((int) BoardSquares.E1, (int) BoardSquares.G1, (int) Pieces.Empty, (int) Pieces.Empty, NoobsDefs.CastlingMoveFlag));
                    }
                }
            }

            if ((position.CastlingPermission & (int) CastlingPermissions.WhiteQueenSideCastling) > 0) {
                if ((position.PiecesOnBoard[(int) BoardSquares.D1] == (int) Pieces.Empty) 
                    && (position.PiecesOnBoard[(int) BoardSquares.C1] == (int) Pieces.Empty)
                    && (position.PiecesOnBoard[(int) BoardSquares.B1] == (int) Pieces.Empty)) {
                    if (!position.IsSquareUnderAttackBy((int) BoardSquares.E1, (int) Players.Black) 
                        && !position.IsSquareUnderAttackBy((int) BoardSquares.D1, (int) Players.Black)) {
                        AddQuietMove(position, ConstructMove((int) BoardSquares.E1, (int) BoardSquares.C1, (int) Pieces.Empty, (int) Pieces.Empty, NoobsDefs.CastlingMoveFlag));
                    }
                }
            }
        }
        else {
            if ((position.CastlingPermission & (int) CastlingPermissions.BlackKingSideCastling) > 0) {
                if ((position.PiecesOnBoard[(int) BoardSquares.F8] == (int) Pieces.Empty) && (position.PiecesOnBoard[(int) BoardSquares.G8] == (int) Pieces.Empty)) {
                    if (!position.IsSquareUnderAttackBy((int) BoardSquares.E8, (int) Players.White) 
                        && !position.IsSquareUnderAttackBy((int) BoardSquares.F8, (int) Players.White)) {
                        AddQuietMove(position, ConstructMove((int) BoardSquares.E8, (int) BoardSquares.G8, (int) Pieces.Empty, (int) Pieces.Empty, NoobsDefs.CastlingMoveFlag));
                    }
                }
            }

            if ((position.CastlingPermission & (int) CastlingPermissions.BlackQueenSideCastling) > 0) {
                if ((position.PiecesOnBoard[(int) BoardSquares.D8] == (int) Pieces.Empty) 
                    && (position.PiecesOnBoard[(int) BoardSquares.C8] == (int) Pieces.Empty)
                    && (position.PiecesOnBoard[(int) BoardSquares.B8] == (int) Pieces.Empty)) {
                    if (!position.IsSquareUnderAttackBy((int) BoardSquares.E8, (int) Players.White) 
                        && !position.IsSquareUnderAttackBy((int) BoardSquares.D8, (int) Players.White)) {
                        AddQuietMove(position, ConstructMove((int) BoardSquares.E8, (int) BoardSquares.C8, (int) Pieces.Empty, (int) Pieces.Empty, NoobsDefs.CastlingMoveFlag));
                    }
                }
            }
        }
        
    }

    private void GenerateSliderMoves(ChessBoard position)
    {
        foreach (Pieces piece_ in SlidingPieces[(Players) position.SideToMove]) {
            int piece = (int) piece_;
            // For every piece of the piece type
            for (int i = 0; i < position.PieceCount[piece]; i++) {
                int square = position.PieceList[piece, i];
                // If it is outside the board, there is some error
                if (IsSquareOffBoard(square)) {
                    return;
                }
                
                for (int idx = 0; idx < PieceDirs[piece_].Length; idx++){
                    int dir = PieceDirs[piece_][idx];
                    int tempSquare = square + dir;

                    while (!IsSquareOffBoard(tempSquare)) {
                        if (position.PiecesOnBoard[tempSquare] != (int) Pieces.Empty) {
                            if (PieceColours[position.PiecesOnBoard[tempSquare]] == (Players)(position.SideToMove ^ 1)) {
                                AddCaptureMove(position, ConstructMove(square, tempSquare, position.PiecesOnBoard[tempSquare], (int) Pieces.Empty, 0));
                            }
                            break;
                        }
                        AddQuietMove(position, ConstructMove(square, tempSquare, (int) Pieces.Empty, (int) Pieces.Empty, 0)); 
                        tempSquare += dir;
                    }                   
                }
            }
        }
    }

    private void GenerateBlackPawnMoves(ChessBoard position)
    {
        for (int pI = 0; pI < position.PieceCount[(int) Pieces.BlackPawn]; pI++) {
            int square = position.PieceList[(int) Pieces.BlackPawn, pI];
            if (IsSquareOffBoard(square)) {
                throw new Exception("Invalid Board");
            }

            if (position.PiecesOnBoard[square + Direction.North] == (int) Pieces.Empty) {
                // Single move forward
                AddBlackPawnMove(position, square, square + Direction.North);
                if ((RankLookup[square] == (int) BoardRanks.Rank7) && (position.PiecesOnBoard[square + 2 * Direction.North] == (int) Pieces.Empty)) {
                    // Initial optional 2-square move for pawn
                    AddQuietMove(position, ConstructMove(square, square + 2 * Direction.North, (int) Pieces.Empty, (int) Pieces.Empty, NoobsDefs.PawnStartFlag));
                }
            }
            
            if (!IsSquareOffBoard(square + Direction.NorthEast) && (PieceColours[position.PiecesOnBoard[square + Direction.NorthEast]] == Players.White)) {
                // Add pawn capture on left side
                AddBlackPawnCapture(position, square, square + Direction.NorthEast, position.PiecesOnBoard[square + Direction.NorthEast]);
            }
            if (!IsSquareOffBoard(square + Direction.NorthWest) && (PieceColours[position.PiecesOnBoard[square + Direction.NorthWest]] == Players.White)) {
                // Add pawn capture on right side
                AddBlackPawnCapture(position, square, square + Direction.NorthWest, position.PiecesOnBoard[square + Direction.NorthWest]);
            }
            if (position.EnPassant != (int) BoardSquares.NoSquare) {
                if (square + Direction.NorthEast == position.EnPassant) {
                    // Add en passant capture on left side
                    AddEnPassantMove(position, ConstructMove(square, square + Direction.NorthEast, (int) Pieces.Empty, (int) Pieces.Empty, NoobsDefs.EnPassantCaptureFlag));
                }
                if (square + Direction.NorthWest == position.EnPassant) {
                    // Add en passant capture on right side
                    AddEnPassantMove(position, ConstructMove(square, square + Direction.NorthWest, (int) Pieces.Empty, (int) Pieces.Empty, NoobsDefs.EnPassantCaptureFlag));
                }
            }
            
        }
    }

    private void GenerateWhitePawnMoves(ChessBoard position)
    {
        for (int pI = 0; pI < position.PieceCount[(int) Pieces.WhitePawn]; pI++) {
            int square = position.PieceList[(int) Pieces.WhitePawn, pI];
            if (IsSquareOffBoard(square)) {
                throw new Exception("Invalid Board");
            }

            if (position.PiecesOnBoard[square + Direction.South] == (int) Pieces.Empty) {
                // Single move forward
                AddWhitePawnMove(position, square, square + Direction.South);
                if ((RankLookup[square] == (int) BoardRanks.Rank2) && (position.PiecesOnBoard[square + 2 * Direction.South] == (int) Pieces.Empty)) {
                    // Initial optional 2-square move for pawn
                    AddQuietMove(position, ConstructMove(square, square + 2 * Direction.South, (int) Pieces.Empty, (int) Pieces.Empty, NoobsDefs.PawnStartFlag));
                }
            }
            
            if (!IsSquareOffBoard(square + Direction.SouthEast) && (PieceColours[position.PiecesOnBoard[square + Direction.SouthEast]] == Players.Black)) {
                // Add pawn capture on left side
                AddWhitePawnCapture(position, square, square + Direction.SouthEast, position.PiecesOnBoard[square + Direction.SouthEast]);
            }
            if (!IsSquareOffBoard(square + Direction.SouthWest) && (PieceColours[position.PiecesOnBoard[square + Direction.SouthWest]] == Players.Black)) {
                // Add pawn capture on right side
                AddWhitePawnCapture(position, square, square + Direction.SouthWest, position.PiecesOnBoard[square + Direction.SouthWest]);
            }

            if (position.EnPassant != (int) BoardSquares.NoSquare) {
                if (square + Direction.SouthEast == position.EnPassant) {
                    // Add en passant capture on left side
                    AddEnPassantMove(position, ConstructMove(square, square + Direction.SouthEast, (int) Pieces.Empty, (int) Pieces.Empty, NoobsDefs.EnPassantCaptureFlag));
                }
                if (square + Direction.SouthWest == position.EnPassant) {
                    // Add en passant capture on right side
                    AddEnPassantMove(position, ConstructMove(square, square + Direction.SouthWest, (int) Pieces.Empty, (int) Pieces.Empty, NoobsDefs.EnPassantCaptureFlag));
                }
            }
        }
    }

    private void GenerateNonSliderMoves(ChessBoard position) {
        // For every non-sliding piece type
        foreach (Pieces piece_ in NonSlidingPieces[(Players) position.SideToMove]) {
            int piece = (int) piece_;
            // For every piece of the piece type
            for (int i = 0; i < position.PieceCount[piece]; i++) {
                int square = position.PieceList[piece, i];
                // If it is outside the board, there is some error
                if (IsSquareOffBoard(square)) {
                    return;
                }
                
                for (int idx = 0; idx < PieceDirs[piece_].Length; idx++){
                    int dir = PieceDirs[piece_][idx];
                    int tempSquare = square + dir;

                    if (IsSquareOffBoard(tempSquare)) {
                        continue;
                    }

                    if (position.PiecesOnBoard[tempSquare] != (int) Pieces.Empty) {
                        if (PieceColours[position.PiecesOnBoard[tempSquare]] == (Players)(position.SideToMove ^ 1)) {
                            AddCaptureMove(position, ConstructMove(square, tempSquare, position.PiecesOnBoard[tempSquare], (int) Pieces.Empty, 0));
                        }
                        continue;
                    }

                    AddQuietMove(position, ConstructMove(square, tempSquare, (int) Pieces.Empty, (int) Pieces.Empty, 0));                    
                }
            }
        }
    }

    public override String ToString() {
        return String.Join(", ", Moves);
    }
}
