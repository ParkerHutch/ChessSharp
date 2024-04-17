using System;

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

public class BoardLocation
{
	public int Row { get; set; }
	public int Col { get; set; }
}
public class Move
{

}

public interface IPiece
{
	BoardLocation Location { get; set; }
	PieceType Type { get; }
	IEnumerable<Move> GetValidMoves(Board board);
}
public class Board
{
	private class Square(int row, int col, IPiece? piece = null)
	{
		public int Row { get; } = row;
		public int Col { get; } = col;
		public IPiece? Piece { get; set; } = piece;
	}
	private static Square[,] _boardArr = new Square[8, 8];

	public Board()
	{

	}
}
