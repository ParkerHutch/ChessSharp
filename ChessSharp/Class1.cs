using System;
using static ChessSharp.Pawn;

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

public class BoardLocation(int row, int col)
{
	public int Row { get; set; } = row;
	public int Col { get; set; } = col;
}
public class Move
{

}

public interface IPiece // use abstract base class?
{
    BoardLocation Location { get; set; }
    PieceType Type { get; }
    public IEnumerable<Move> GetValidMoves(Board board);

	//public override string ToString; // need to use an abstract base class: https://stackoverflow.com/questions/510341/force-subclasses-of-an-interface-to-implement-tostring
	public string getNotation();
}

public class Pawn(int row, int col) : IPiece
{
	public PieceType Type => PieceType.Pawn;

	BoardLocation IPiece.Location { get; set; } = new(row, col);
	public PieceType type { get; } = PieceType.Pawn;

	public IEnumerable<Move> GetValidMoves(Board board)
	{
		List<Move> validMoves = new List<Move>();
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
	public class Square(int row, int col, IPiece? piece = null)
	{
		public BoardLocation Location { get; set; } = new(row, col);
		public IPiece? Piece { get; set; } = piece;

        public override string ToString()
        {
            return Piece?.ToString() ?? "-";
        }
    }
	public static Square[,] BoardArr = new Square[8, 8];

	//private static Square[,] _boardArr = new Square[8, 8]
	//{
	//	new Square[8] {},
	//	new Square[8] { },
	//       new Square[8] { },
	//       new Square[8] { },
	//       new Square[8] { },
	//       new Square[8] { },
	//       new Square[8] { },
	//       new Square[8] { }
	//   }

	private static void SetBoard()
	{
		for (int row = 0; row < 8; ++row)
		{
			for (int col = 0; col < 8; ++col)
			{
				BoardArr[row, col] = new Square(row, col);
			}
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



