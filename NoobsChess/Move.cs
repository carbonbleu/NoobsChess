using static NoobsEngine.NoobsGlobals;

namespace NoobsEngine {
    public class Move {
        // Moves can be represented in 7 bytes
        public int Value { get; set; }
        public int Score { get; set; }


        public Move(int value) {
            Value = value;
            Score = 0;
        }

        public Move(int value, int score) {
            Value = value;
            Score = score;
        }
        

        public int GetFromPosition() {
            return Value & 0x7F;
        }

        public int GetToPosition() {
            return (Value >> 7) & 0x7F;
        }

        public int GetCapturedPiece() {
            return (Value >> 14) & 0xF;
        }

        public bool IsEnPassantCapture() {
            return (Value & 0x40000) > 0;
        }

        public bool IsPawnStartMove() {
            return (Value & 0x80000) > 0;
        }

        public int GetPromotedPiece() {
            return (Value >> 20) & 0xF;
        }

        public bool IsCastle() {
            return (Value & 0x1000000) > 0;
        }

        public bool IsCapture() {
            return (Value & 0x7C000) > 0;
        }

        public bool IsPromotion() {
            return (Value & 0xF00000) > 0;
        }

        public override string ToString()
        {
            if (Value == 0) return "NoMove";

            int fileFrom = FileLookup[GetFromPosition()];
            int fileTo = FileLookup[GetToPosition()];

            int rankFrom = RankLookup[GetFromPosition()];
            int rankTo = RankLookup[GetToPosition()];

            bool promoted = IsPromotion();
            String promotedChar = "q";
            String result;
            if (promoted) {
                int promotedPiece = GetPromotedPiece();
                if (PieceKnight[promotedPiece]) {
                    promotedChar = "n";
                }
                else if (PieceRookOrQueen[promotedPiece] && !PieceBishopOrQueen[promotedPiece]) {
                    promotedChar = "r";
                }
                else if (!PieceRookOrQueen[promotedPiece] && PieceBishopOrQueen[promotedPiece]) {
                    promotedChar = "b";
                }
                result = String.Format("{0}{1}{2}{3}{4}", (char)('a' + fileFrom), (char)('1' + rankFrom), (char)('a' + fileTo), (char)('1' + rankTo), promotedChar);
            }
            else {
                result = String.Format("{0}{1}{2}{3}", (char)('a' + fileFrom), (char)('1' + rankFrom), (char)('a' + fileTo), (char)('1' + rankTo));
            }

            return result;
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is Move)) return false;
            return Value == ((Move) obj).Value;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}