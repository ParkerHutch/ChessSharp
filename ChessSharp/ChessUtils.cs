using System;
using static ChessSharp.Board;
using static ChessSharp.Pawn;

namespace ChessSharp;

public enum Color
{
	White,
	Black
}
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


public class Move(Square lastSquare, Square nextSquare)
{
	public readonly Square LastSquare = lastSquare;
	public readonly Square NextSquare = nextSquare;
	public override string ToString()
	{
		return $"{LastSquare.Piece} ({LastSquare.Location.Row}, {LastSquare.Location.Col}) to "
			+ $"({NextSquare.Location.Row}, {NextSquare.Location.Col})";

	}

    public override bool Equals(object? obj)
    {
		var otherMove = obj as Move;
		if (otherMove == null)
		{
			return false;
		}

		if (otherMove.LastSquare == LastSquare && 
			otherMove.NextSquare == NextSquare)
		{
			return true;
		}
		return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(LastSquare, NextSquare);
    }
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
		// TODO check if the indexes are in bounds
        IPiece? pieceAhead = board.BoardArr[
			myLoc.Row + directionOffset, myLoc.Col].Piece;
		if (pieceAhead == null)
		{
			validMoves.Add(new Move(
				board.BoardArr[myLoc.Row, myLoc.Col],
				board.BoardArr[myLoc.Row + directionOffset, myLoc.Col]
			));
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
        
        if (ensureNoCheckOnFriendlyKing) { 
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
            } else
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

        if (ensureNoCheckOnFriendlyKing) { 
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

        foreach((int moveRow, int moveCol) in possibleMovePositions)
        {
            if (board.GetSquareAt(moveRow, moveCol, out Square? moveSquare))
            {
                if (moveSquare!.Piece == null || moveSquare!.Piece.Color != Color) {
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

    private IEnumerable<Move> GetDiagonalMoves(Board board)
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
        return validMoves;
    }

    private IEnumerable<Move> GetRowAndColumnMoves(Board board)
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
        return validMoves;
    }

    private IEnumerable<Move> GetNeighboringSquareMoves(Board board)
    {
        List<Move> validMoves = new List<Move>();
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
        return validMoves;
    }

    public IEnumerable<Move> GetValidMoves(Board board, bool ensureNoCheckOnFriendlyKing)
    {
        List<Move> validMoves = 
            GetDiagonalMoves(board).ToList()
            .Concat(GetRowAndColumnMoves(board)).ToList()
            .Concat(GetNeighboringSquareMoves(board)).ToList();

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

    public IEnumerable<Move> GetValidMoves(Board board, bool ensureNoCheckOnFriendlyKing)
    {
        List<Move> validMoves = new List<Move>();
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

public class Board
{
    public Color CurrentTurn { get; set; } = Color.White;
    // Return true and store the square at (row, col) if that position is in the bounds of the board
    public bool GetSquareAt(int row, int col, out Square? square)
    {
        if (row >= 0 && row < 8 && col >= 0 && col < 8)
        {
            square = BoardArr[row, col];
            return true;
        }
        square = null;
        return false;
    }
    public class Location(int row, int col)
    {
        public int Row { get; set; } = row;
        public int Col { get; set; } = col;
    }

    public class Square(int row, int col, IPiece? piece = null)
	{
		public Location Location { get; set; } = new(row, col);
		public IPiece? Piece { get; set; } = piece;

        public override string ToString()
        {
            return Piece?.ToString() ?? "-";
        }
    }
	public Square[,] BoardArr = new Square[8, 8];

	private void SetBoard(bool empty)
	{
		for (int row = 0; row < 8; ++row)
		{
			for (int col = 0; col < 8; ++col)
			{
				BoardArr[row, col] = new Square(row, col);
			}
		}

		if (!empty)
		{
			BoardArr[0, 0].Piece = new Rook(0, 0, Color.White);
            BoardArr[0, 1].Piece = new Knight(0, 1, Color.White);
            BoardArr[0, 2].Piece = new Bishop(0, 2, Color.White);
            BoardArr[0, 3].Piece = new King(0, 3, Color.White);
            BoardArr[0, 4].Piece = new Queen(0, 4, Color.White);
            BoardArr[0, 5].Piece = new Bishop(0, 5, Color.White);
            BoardArr[0, 6].Piece = new Knight(0, 6, Color.White);
            BoardArr[0, 7].Piece = new Rook(0, 7, Color.White);

            BoardArr[7, 0].Piece = new Rook(7, 0, Color.Black);
            BoardArr[7, 1].Piece = new Knight(7, 1, Color.Black);
            BoardArr[7, 2].Piece = new Bishop(7, 2, Color.Black);
            BoardArr[7, 3].Piece = new King(7, 3, Color.Black);
            BoardArr[7, 4].Piece = new Queen(7, 4, Color.Black);
            BoardArr[7, 5].Piece = new Bishop(7, 5, Color.Black);
            BoardArr[7, 6].Piece = new Knight(7, 6, Color.Black);
            BoardArr[7, 7].Piece = new Rook(7, 7, Color.Black);
            // place pawns
            for (int col = 0; col < 8; ++col)
            {
                BoardArr[1, col].Piece = new Pawn(1, col, Color.White);
            }

            for (int col = 0; col < 8; ++col)
            {
                BoardArr[6, col].Piece = new Pawn(6, col, Color.Black);
            }
        }
    }
	public Board(bool empty)
	{
       SetBoard(empty);
    }

    public bool IsKingInCheck(Color kingColor)
    {
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                IPiece? piece = BoardArr[row, col].Piece;
                if (piece != null)
                {
                    if (piece.Color != kingColor)
                    {
                        var nextSquares = piece.GetValidMoves(this, false).Select(x => x.NextSquare);
                        if (nextSquares.Any(x => x.Piece != null && x.Piece.Type == PieceType.King && x.Piece.Color == kingColor))
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
    public bool MoveDoesNotResultInCheckOnOwnKing(Move move)
    {
        // ideas: 
        // 1. actually make the move and 
        Color movingPieceColor = move.LastSquare.Piece!.Color;
        bool result;
        // 1. Store the pieces that were previously at the move's lastSquare and nextSquare squares
        IPiece oldLastSquarePiece = move.LastSquare.Piece!;
        IPiece? oldNextSquarePiece = move.NextSquare.Piece;
        // 2. Make the move, but don't alternate the turn
        move.NextSquare.Piece = move.LastSquare.Piece;
        move.LastSquare.Piece = null;
        // 3. Check if any piece has a check on the moving piece's king
        result = !IsKingInCheck(movingPieceColor);

        // 4. Revert the move by using the storage from 1
        move.LastSquare.Piece = oldLastSquarePiece;
        move.NextSquare.Piece = oldNextSquarePiece;

        // 5. Return true if the result for 3 is empty, false otherwise
        return result;
    }

    public void ExecuteMove(Move move)
    {
        // Assumption: the move is valid

        Color moveColor = move.LastSquare.Piece.Color;
        move.NextSquare.Piece = move.LastSquare.Piece;
        move.LastSquare.Piece = null;
        move.NextSquare.Piece.Location = move.NextSquare.Location;
        CurrentTurn = moveColor == Color.White ? Color.Black : Color.White;
    }

    public List<IPiece> GetUncapturedPieces(Color color)
    {
        List<IPiece> pieces = new();
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                if (BoardArr[row, col].Piece?.Color == color)
                {
                    pieces.Add(BoardArr[row, col].Piece!);
                }
            }
        }
        return pieces;
    }
    public Move? GetRandomMoveForColor(Color color)
    {
        // start a search from a random square for a piece belonging to the player color, then return a random move for that piece
        Random rnd = new();
        List<IPiece> pieces = GetUncapturedPieces(color);
        if (pieces.Count > 0) {
            int randomIndex = rnd.Next(pieces.Count());

            // find the first index of a piece that has some valid moves
            int pieceWithMovesIndex = Enumerable.Range(randomIndex, pieces.Count - randomIndex)
                              .Concat(Enumerable.Range(0, randomIndex))
                              .FirstOrDefault(i => pieces[i].GetValidMoves(this, true).Any(), -1); // TODO firstOrDefault means that 0 will be the result even if there are no valid moves for any pieces

            if (pieceWithMovesIndex >= 0)
            {
                int randomMoveIndex = rnd.Next(pieces[pieceWithMovesIndex].GetValidMoves(this, true).Count());
                return pieces[pieceWithMovesIndex].GetValidMoves(this, true).ElementAt(randomMoveIndex);
            }
            //do // search for a piece
            //{
            //    var validMoves = pieces.ElementAt(pieceIndex).GetValidMoves(this);
            //    if (validMoves.Any()) // search for a move
            //    {
                    
            //    } else
            //    {
            //        pieceIndex++;
            //        if (pieceIndex == pieces.Count())
            //        {
            //            pieceIndex = 0; // wrap around
            //        }
            //    }
            //} while (pieceIndex != randomIndex);
        }

        return null;
    }

	public void Print()
	{
		string topRowNumbers = "\t" + string.Join("\t", Enumerable.Range(1, 8));
		Console.WriteLine(topRowNumbers);

        for (int row = 0; row < 8; ++row)
        {
			Console.Write($"{row + 1}\t");
            for (int col = 0; col < 8; ++col)
            {
				Console.Write($"{BoardArr[row, col]}\t");
            }
            Console.WriteLine();

        }
    }
}



