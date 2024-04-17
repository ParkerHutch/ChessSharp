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
	Square nextSquare = nextSquare;
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
	public PieceType type { get; } = PieceType.Pawn;

	public IEnumerable<Move> GetValidMoves(Board board)
	{
		List<Move> validMoves = new List<Move>();
		int directionOffset = color == Color.White ? 0 : 1;
		Board.Location myLoc = (this as IPiece).Location;
		if (myLoc.Row < 8 && myLoc.Row > 0)
		{
			IPiece? pieceAhead = board.BoardArr[
				myLoc.Row + directionOffset, myLoc.Col].Piece;
			if (pieceAhead == null)
			{
				validMoves.Add(new Move(
					board.BoardArr[myLoc.Row, myLoc.Col],
					board.BoardArr[myLoc.Row + directionOffset, myLoc.Col]
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

	private void SetBoard()
	{
		for (int row = 0; row < 8; ++row)
		{
			for (int col = 0; col < 8; ++col)
			{
				BoardArr[row, col] = new Square(row, col);
			}
		}

        for (int col = 0; col < 8; ++col)
        {
			BoardArr[1, col].Piece = new Pawn(1, col, Color.White);
        }

        for (int col = 0; col < 8; ++col)
        {
            BoardArr[6, col].Piece = new Pawn(1, col, Color.Black);
        }

    }
	public Board()
	{
		SetBoard();
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



