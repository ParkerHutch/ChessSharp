using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ChessSharp.Board;

namespace ChessSharp;
public enum PieceType
{
    King,
    Queen,
    Rook,
    Bishop,
    Knight,
    Pawn,
    None
}

public interface IPiece // use abstract base class?
{
    Location Location { get; set; }
    PieceType Type { get; }
    public Color Color { get; }
    public IEnumerable<Move> GetValidMoves(Board board, bool ensureNoCheckOnFriendlyKing);

    //public override string ToString; // need to use an abstract base class: https://stackoverflow.com/questions/510341/force-subclasses-of-an-interface-to-implement-tostring
    public string getNotation();
}

public class Pawn(int row, int col, Color color) : IPiece
{
    public PieceType Type => PieceType.Pawn;

    public Color Color => color;

    Location IPiece.Location { get; set; } = new(row, col);

    public IEnumerable<Move> GetValidMoves(Board board, bool ensureNoCheckOnFriendlyKing)
    {
        List<Move> validMoves = new List<Move>();
        int directionOffset = color == Color.White ? 1 : -1;
        Board.Location myLoc = (this as IPiece).Location;

        IPiece? pieceAhead = board.BoardArr[
            myLoc.Row + directionOffset, myLoc.Col].Piece;
        if (pieceAhead == null)
        {
            validMoves.Add(new Move(
                board.BoardArr[myLoc.Row, myLoc.Col],
                board.BoardArr[myLoc.Row + directionOffset, myLoc.Col]
            ));
            bool hasNotMovedOffStartingRow = (color == Color.White && myLoc.Row == 1) || (color == Color.Black && myLoc.Row == 6);
            if (hasNotMovedOffStartingRow)
            {
                IPiece? pieceTwoRowsAhead = board.BoardArr[
                        myLoc.Row + directionOffset * 2, myLoc.Col].Piece;
                if (pieceTwoRowsAhead == null)
                {
                    validMoves.Add(new Move(
                        board.BoardArr[myLoc.Row, myLoc.Col],
                        board.BoardArr[myLoc.Row + directionOffset * 2, myLoc.Col]
                    ));
                }
            }
        }

        // check if the pawn can move diagonally
        if (myLoc.Col + 1 < board.BoardArr.GetLength(1))
        {
            IPiece? pieceDiagonalRight = board.BoardArr[
            myLoc.Row + directionOffset, myLoc.Col + 1].Piece;
            if (pieceDiagonalRight != null && pieceDiagonalRight.Color != Color)
            {
                validMoves.Add(new Move(
                    board.BoardArr[myLoc.Row, myLoc.Col],
                    board.BoardArr[myLoc.Row + directionOffset, myLoc.Col + 1]
                ));
            }
        }

        if (myLoc.Col - 1 >= 0)
        {
            IPiece? pieceDiagonalLeft = board.BoardArr[
            myLoc.Row + directionOffset, myLoc.Col - 1].Piece;
            if (pieceDiagonalLeft != null && pieceDiagonalLeft.Color != Color)
            {
                validMoves.Add(new Move(
                    board.BoardArr[myLoc.Row, myLoc.Col],
                    board.BoardArr[myLoc.Row + directionOffset, myLoc.Col - 1]
                ));
            }
        }

        if (ensureNoCheckOnFriendlyKing)
        {
            validMoves = validMoves.Where(x => board.MoveDoesNotResultInCheckOnOwnKing(x)).ToList();
        }
        // TODO look for en passant
        return validMoves;
    }

    public override string ToString()
    {
        return "P";
    }

    public string getNotation()
    {
        return "";
    }
}

public class Rook(int row, int col, Color color) : IPiece
{
    public PieceType Type => PieceType.Rook;

    public Color Color => color;

    Location IPiece.Location { get; set; } = new(row, col);

    public bool HasMoved { get; set; } = false;

    public IEnumerable<Move> GetValidMoves(Board board, bool ensureNoCheckOnFriendlyKing)
    {
        List<Move> validMoves = new List<Move>();

        Location myLoc = (this as IPiece).Location;
        // check squares above the rook
        for (int row = myLoc.Row + 1; row < 8; ++row)
        {
            Board.Square otherSquare = board.BoardArr[row, myLoc.Col];
            if (otherSquare.Piece != null)
            {
                if (otherSquare.Piece.Color != Color)
                {
                    validMoves.Add(new Move(board.BoardArr[myLoc.Row, myLoc.Col], otherSquare));
                }
                break;
            }
            else
            {
                validMoves.Add(new Move(board.BoardArr[myLoc.Row, myLoc.Col], otherSquare));
            }
        }
        // check squares below the rook
        for (int row = myLoc.Row - 1; row >= 0; --row)
        {
            Board.Square otherSquare = board.BoardArr[row, myLoc.Col];
            if (otherSquare.Piece != null)
            {
                if (otherSquare.Piece.Color != Color)
                {
                    validMoves.Add(new Move(board.BoardArr[myLoc.Row, myLoc.Col], otherSquare));
                }
                break;
            }
            else
            {
                validMoves.Add(new Move(board.BoardArr[myLoc.Row, myLoc.Col], otherSquare));
            }
        }

        // check squares to the left of the rook
        for (int col = myLoc.Col - 1; col >= 0; --col)
        {
            Board.Square otherSquare = board.BoardArr[myLoc.Row, col];
            if (otherSquare.Piece != null)
            {
                if (otherSquare.Piece.Color != Color)
                {
                    validMoves.Add(new Move(board.BoardArr[myLoc.Row, myLoc.Col], otherSquare));
                }
                break;
            }
            else
            {
                validMoves.Add(new Move(board.BoardArr[myLoc.Row, myLoc.Col], otherSquare));
            }
        }

        // check squares to the right of the rook
        for (int col = myLoc.Col + 1; col < 8; ++col)
        {
            Board.Square otherSquare = board.BoardArr[myLoc.Row, col];
            if (otherSquare.Piece != null)
            {
                if (otherSquare.Piece.Color != Color)
                {
                    validMoves.Add(new Move(board.BoardArr[myLoc.Row, myLoc.Col], otherSquare));
                }
                break;
            }
            else
            {
                validMoves.Add(new Move(board.BoardArr[myLoc.Row, myLoc.Col], otherSquare));
            }
        }

        if (ensureNoCheckOnFriendlyKing)
        {
            validMoves = validMoves.Where(x => board.MoveDoesNotResultInCheckOnOwnKing(x)).ToList();
        }

        return validMoves;
    }

    public override string ToString()
    {
        return "R";
    }

    public string getNotation()
    {
        return "R";
    }
}

public class Bishop(int row, int col, Color color) : IPiece
{
    public PieceType Type => PieceType.Bishop;

    public Color Color => color;

    Location IPiece.Location { get; set; } = new(row, col);

    public IEnumerable<Move> GetValidMoves(Board board, bool ensureNoCheckOnFriendlyKing)
    {
        List<Move> validMoves = new List<Move>();

        Location myLoc = (this as IPiece).Location;
        bool northwestDiagonalBlocked = false;
        bool northeastDiagonalBlocked = false;
        bool southwestDiagonalBlocked = false;
        bool southeastDiagonalBlocked = false;

        for (int squaresAwayFromBishop = 1; squaresAwayFromBishop < 8; ++squaresAwayFromBishop)
        {
            // candidates are (row + sqA, col + sqA), (row + sqA, col - sqA), (row - sqA, col - sqA), (row - sqA, col + sqA)
            // stop searching a diagonal once it's blocked by a piece
            if (!northwestDiagonalBlocked &&
                board.GetSquareAt(
                    myLoc.Row + squaresAwayFromBishop,
                    myLoc.Col - squaresAwayFromBishop,
                    out Board.Square? northwestSquare
            ))
            {
                IPiece? piece;
                if ((piece = northwestSquare!.Piece) != null)
                {
                    if (piece.Color != Color)
                    {
                        validMoves.Add(new Move(board.BoardArr[myLoc.Row, myLoc.Col], northwestSquare));
                    }
                    northwestDiagonalBlocked = true; // stop exploring this diagonal
                }
                else
                {
                    validMoves.Add(new Move(board.BoardArr[myLoc.Row, myLoc.Col], northwestSquare));
                }
            }

            if (!northeastDiagonalBlocked &&
                board.GetSquareAt(
                    myLoc.Row + squaresAwayFromBishop,
                    myLoc.Col + squaresAwayFromBishop,
                    out Board.Square? northeastSquare
            ))
            {
                IPiece? piece;
                if ((piece = northeastSquare!.Piece) != null)
                {
                    if (piece.Color != Color)
                    {
                        validMoves.Add(new Move(board.BoardArr[myLoc.Row, myLoc.Col], northeastSquare));
                    }
                    northeastDiagonalBlocked = true; // stop exploring this diagonal
                }
                else
                {
                    validMoves.Add(new Move(board.BoardArr[myLoc.Row, myLoc.Col], northeastSquare));
                }
            }

            if (!southwestDiagonalBlocked &&
                board.GetSquareAt(
                    myLoc.Row - squaresAwayFromBishop,
                    myLoc.Col - squaresAwayFromBishop,
                    out Board.Square? southwestSquare
            ))
            {
                IPiece? piece;
                if ((piece = southwestSquare!.Piece) != null)
                {
                    if (piece.Color != Color)
                    {
                        validMoves.Add(new Move(board.BoardArr[myLoc.Row, myLoc.Col], southwestSquare));
                    }
                    southwestDiagonalBlocked = true; // stop exploring this diagonal
                }
                else
                {
                    validMoves.Add(new Move(board.BoardArr[myLoc.Row, myLoc.Col], southwestSquare));
                }
            }

            if (!southeastDiagonalBlocked &&
                board.GetSquareAt(
                    myLoc.Row - squaresAwayFromBishop,
                    myLoc.Col + squaresAwayFromBishop,
                    out Board.Square? southeastSquare
            ))
            {
                IPiece? piece;
                if ((piece = southeastSquare!.Piece) != null)
                {
                    if (piece.Color != Color)
                    {
                        validMoves.Add(new Move(board.BoardArr[myLoc.Row, myLoc.Col], southeastSquare));
                    }
                    southeastDiagonalBlocked = true; // stop exploring this diagonal
                }
                else
                {
                    validMoves.Add(new Move(board.BoardArr[myLoc.Row, myLoc.Col], southeastSquare));
                }
            }

        }

        if (ensureNoCheckOnFriendlyKing)
        {
            validMoves = validMoves.Where(x => board.MoveDoesNotResultInCheckOnOwnKing(x)).ToList();
        }
        return validMoves;
    }

    public override string ToString()
    {
        return "B";
    }

    public string getNotation()
    {
        return "B";
    }
}

public class Knight(int row, int col, Color color) : IPiece
{
    public PieceType Type => PieceType.Knight;

    public Color Color => color;

    Location IPiece.Location { get; set; } = new(row, col);

    public IEnumerable<Move> GetValidMoves(Board board, bool ensureNoCheckOnFriendlyKing)
    {
        List<Move> validMoves = new List<Move>();
        Location myLoc = (this as IPiece).Location;

        List<(int moveRow, int moveCol)> possibleMovePositions = [
            (myLoc.Row - 1, myLoc.Col - 2),
            (myLoc.Row - 1, myLoc.Col + 2),
            (myLoc.Row + 1, myLoc.Col - 2),
            (myLoc.Row + 1, myLoc.Col + 2),
            (myLoc.Row - 2, myLoc.Col - 1),
            (myLoc.Row - 2, myLoc.Col + 1),
            (myLoc.Row + 2, myLoc.Col - 1),
            (myLoc.Row + 2, myLoc.Col + 1),
        ];

        foreach ((int moveRow, int moveCol) in possibleMovePositions)
        {
            if (board.GetSquareAt(moveRow, moveCol, out Square? moveSquare))
            {
                if (moveSquare!.Piece == null || moveSquare!.Piece.Color != Color)
                {
                    validMoves.Add(new Move(board.BoardArr[myLoc.Row, myLoc.Col], moveSquare));
                }
            }
        }

        if (ensureNoCheckOnFriendlyKing)
        {
            validMoves = validMoves.Where(x => board.MoveDoesNotResultInCheckOnOwnKing(x)).ToList();
        }

        return validMoves;
    }

    public override string ToString()
    {
        return "N";
    }

    public string getNotation()
    {
        return "N";
    }
}

public class Queen(int row, int col, Color color) : IPiece
{
    public PieceType Type => PieceType.Queen;

    public Color Color => color;

    Location IPiece.Location { get; set; } = new(row, col);

    public IEnumerable<Move> GetValidMoves(Board board, bool ensureNoCheckOnFriendlyKing)
    {
        Location myLoc = (this as IPiece).Location;

        Rook tempRook = new(myLoc.Row, myLoc.Col, Color);
        Bishop tempBishop = new(myLoc.Row, myLoc.Col, Color);

        var validMoves = tempRook.GetValidMoves(board, ensureNoCheckOnFriendlyKing)
            .Concat(tempBishop.GetValidMoves(board, ensureNoCheckOnFriendlyKing));

        if (ensureNoCheckOnFriendlyKing)
        {
            validMoves = validMoves.Where(x => board.MoveDoesNotResultInCheckOnOwnKing(x)).ToList();
        }

        return validMoves;
    }

    public override string ToString()
    {
        return "Q";
    }

    public string getNotation()
    {
        return "Q";
    }
}

public class King(int row, int col, Color color) : IPiece
{
    public PieceType Type => PieceType.King;

    public Color Color => color;

    Location IPiece.Location { get; set; } = new(row, col);

    public bool HasMoved { get; set; } = false;

    public IEnumerable<CastleMove> GetCastleMoves(Board board)
    {
        if (board.IsKingInCheck(Color))
        {
            return [];
        }

        List<CastleMove> castleMoves = [];
        Location myLoc = (this as IPiece).Location;

        if (!HasMoved)
        {
            
            int backRow = Color == Color.White ? 0 : 7;
            Rook? cornerRook1 = board.BoardArr[backRow, 0].Piece as Rook;
            Rook? cornerRook2 = board.BoardArr[backRow, 7].Piece as Rook;

            bool noPiecesInPathToCornerRook1 = true;
            bool noPiecesInPathToCornerRook2 = true;

            for (int col = 1; col < myLoc.Col; col++)
            {
                if (board.BoardArr[backRow, col].Piece != null)
                {
                    noPiecesInPathToCornerRook1 = false;
                    break;
                }
            }

            for (int col = myLoc.Col + 1; col < 7; col++)
            {
                if (board.BoardArr[backRow, col].Piece != null)
                {
                    noPiecesInPathToCornerRook2 = false;
                    break;
                }
            }

            bool noEnemyPiecesAttackPathToCornerRook1 = true;
            bool noEnemyPiecesAttackPathToCornerRook2 = true;

            
            List<IPiece> enemyPieces = board.GetUncapturedPieces(Color == Color.White ? Color.Black : Color.White);
            foreach (IPiece piece in enemyPieces)
            {
                // the enemy king will never block this castle if it hasn't moved
                if (piece is King enemyKing && !enemyKing.HasMoved)
                {
                    continue;
                }
                else
                {
                    foreach (Move move in piece.GetValidMoves(board, false))
                    {
                        // can castle if the move attacks column 0 or 7 on the back row, but not in between
                        if (move.NextSquare.Location.Row == backRow && move.NextSquare.Location.Col > 0 && move.NextSquare.Location.Col < 7)
                        {
                            if (move.NextSquare.Location.Col < myLoc.Col)
                            {
                                noEnemyPiecesAttackPathToCornerRook1 = false;
                            }
                            else
                            {
                                noEnemyPiecesAttackPathToCornerRook2 = false;
                            }
                        }
                    }
                }
            }

            bool canCastleToCornerRook1 = cornerRook1 != null && !cornerRook1.HasMoved && noPiecesInPathToCornerRook1 && noEnemyPiecesAttackPathToCornerRook1;
            bool canCastleToCornerRook2 = cornerRook2 != null && !cornerRook2.HasMoved && noPiecesInPathToCornerRook2 && noEnemyPiecesAttackPathToCornerRook2;
            if (canCastleToCornerRook1)
            {
                CastleMove castleMove = new(
                    kingLastSquare: board.BoardArr[backRow, myLoc.Col],
                    kingNextSquare: board.BoardArr[backRow, 1],
                    rookLastSquare: board.BoardArr[backRow, 0],
                    rookNextSquare: board.BoardArr[backRow, 2]
                    ); 
                castleMoves.Add(castleMove);
            }
            if (canCastleToCornerRook2)
            {
                CastleMove castleMove = new(
                    kingLastSquare: board.BoardArr[backRow, myLoc.Col],
                    kingNextSquare: board.BoardArr[backRow, 5],
                    rookLastSquare: board.BoardArr[backRow, 7],
                    rookNextSquare: board.BoardArr[backRow, 4]
                    );
                castleMoves.Add(castleMove);
            }
        }
        return castleMoves;
    }

    public IEnumerable<Move> GetValidMoves(Board board, bool ensureNoCheckOnFriendlyKing)
    {
        List<Move> validMoves = [];
        Location myLoc = (this as IPiece).Location;

        for (int row = myLoc.Row - 1; row <= myLoc.Row + 1; row++)
        {
            for (int col = myLoc.Col - 1; col <= myLoc.Col + 1; col++)
            {
                if (row == myLoc.Row && col == myLoc.Col)
                {
                    continue;
                }
                if (board.GetSquareAt(row, col, out Board.Square? neighborSquare))
                {
                    if (neighborSquare!.Piece == null || neighborSquare.Piece.Color != Color)
                    {
                        validMoves.Add(new Move(
                            board.BoardArr[myLoc.Row, myLoc.Col], neighborSquare
                        ));
                    }
                }
            }
        }

        

        if (ensureNoCheckOnFriendlyKing)
        {
            validMoves = validMoves.Where(x => board.MoveDoesNotResultInCheckOnOwnKing(x)).ToList();
        }

        validMoves = validMoves.Concat(GetCastleMoves(board)).ToList();

        return validMoves;
    }

    public override string ToString()
    {
        return "K";
    }

    public string getNotation()
    {
        return "K";
    }
}
