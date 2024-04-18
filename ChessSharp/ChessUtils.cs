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
	Square LastSquare = lastSquare;
	Square NextSquare = nextSquare;
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

}

public interface IPiece // use abstract base class?
{
    Location Location { get; set; }
    PieceType Type { get; }
	public Color Color { get; }
    public IEnumerable<Move> GetValidMoves(Board board);

	//public override string ToString; // need to use an abstract base class: https://stackoverflow.com/questions/510341/force-subclasses-of-an-interface-to-implement-tostring
	public string getNotation();
}

public class Pawn(int row, int col, Color color) : IPiece
{
	public PieceType Type => PieceType.Pawn;

	public Color Color => color;

	Location IPiece.Location { get; set; } = new(row, col);

	public IEnumerable<Move> GetValidMoves(Board board)
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
            IPiece? pieceDiagonalLeft = board.BoardArr[
            myLoc.Row + directionOffset, myLoc.Col + 1].Piece;
			if (pieceDiagonalLeft != null)
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
            if (pieceDiagonalLeft != null)
            {
                validMoves.Add(new Move(
                    board.BoardArr[myLoc.Row, myLoc.Col],
                    board.BoardArr[myLoc.Row + directionOffset, myLoc.Col - 1]
                ));
            }
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

    public IEnumerable<Move> GetValidMoves(Board board)
    {
        List<Move> validMoves = new List<Move>();

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
    public PieceType Type => PieceType.Knight;

    public Color Color => color;

    Location IPiece.Location { get; set; } = new(row, col);

    public IEnumerable<Move> GetValidMoves(Board board)
    {
        List<Move> validMoves = new List<Move>();

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

    public IEnumerable<Move> GetValidMoves(Board board)
    {
        List<Move> validMoves = new List<Move>();

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
    public PieceType Type => PieceType.Knight;

    public Color Color => color;

    Location IPiece.Location { get; set; } = new(row, col);

    public IEnumerable<Move> GetValidMoves(Board board)
    {
        List<Move> validMoves = new List<Move>();

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
    public PieceType Type => PieceType.Knight;

    public Color Color => color;

    Location IPiece.Location { get; set; } = new(row, col);

    public IEnumerable<Move> GetValidMoves(Board board)
    {
        List<Move> validMoves = new List<Move>();

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
			// place rooks
			BoardArr[0, 0].Piece = new Rook(0, 0, Color.White);
            BoardArr[0, 7].Piece = new Rook(0, 7, Color.White);
            BoardArr[7, 0].Piece = new Rook(7, 0, Color.Black);
            BoardArr[7, 7].Piece = new Rook(7, 7, Color.Black);
            // place pawns
            for (int col = 0; col < 8; ++col)
            {
                BoardArr[1, col].Piece = new Pawn(1, col, Color.White);
            }

            for (int col = 0; col < 8; ++col)
            {
                BoardArr[6, col].Piece = new Pawn(1, col, Color.Black);
            }
        }
    }
	public Board(bool empty)
	{
       SetBoard(empty);
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



