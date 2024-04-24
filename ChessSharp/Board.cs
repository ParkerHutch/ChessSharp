using System;
using System.Drawing;
using static ChessSharp.Board;
using static ChessSharp.Pawn;

namespace ChessSharp;

public enum Color
{
	White,
	Black
}

public enum GameState
{
    Checkmate,
    Stalemate,
    Ongoing
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
    public List<Move> GetAllMovesForCurrentPlayer()
    {
        List<Move> moves = new List<Move>();
        List<IPiece> pieces = GetUncapturedPieces(CurrentTurn);
        foreach (IPiece piece in pieces)
        {
            foreach (Move move in piece.GetValidMoves(this, true))
            {
                moves.Add(move);
            }
        }
        //moves = pieces.SelectMany(x => x.GetValidMoves(this, true))
        return moves;
    }

    public GameState GetGameState()
    {
        if (GetAllMovesForCurrentPlayer().Count == 0)
        {
            if (IsKingInCheck(CurrentTurn))
            {
                return GameState.Checkmate;
            } else
            {
                return GameState.Stalemate;
            }
        }
        return GameState.Ongoing;
    }

    public bool MoveDoesNotResultInCheckOnOwnKing(Move move)
    {
        // 1. Store the pieces that were previously at the move's lastSquare and nextSquare squares
        IPiece oldLastSquarePiece = move.LastSquare.Piece!;
        IPiece? oldNextSquarePiece = move.NextSquare.Piece;
        Color movingPieceColor = oldLastSquarePiece.Color;
        // 2. Simulate the move without actually alternating the turn
        ExecuteMove(move, alternateTurn: false);
        // 3. Check if any piece has a check on the moving piece's king
        bool result = !IsKingInCheck(movingPieceColor);

        // 4. Revert the move by using the storage from 1
        move.LastSquare.Piece = oldLastSquarePiece;
        oldLastSquarePiece.Location = move.LastSquare.Location;

        move.NextSquare.Piece = oldNextSquarePiece;
        if (oldNextSquarePiece != null)
        {
            oldNextSquarePiece.Location = move.NextSquare.Location;
        }

        // 5. Return true if the result for step 3 is empty, false otherwise
        return result;
    }

    public void ExecuteMove(Move move, bool alternateTurn = true)
    {
        // Assumption: the move is valid and LastSquare.Piece is not null
        Color moveColor = move.LastSquare.Piece!.Color;
        bool moveIsPawnPromotion =
            (
                (moveColor == Color.White && move.NextSquare.Location.Row == 7) ||
                (moveColor == Color.Black && move.NextSquare.Location.Row == 0)
            ) && move.LastSquare.Piece!.Type == PieceType.Pawn;
        
        if (moveIsPawnPromotion)
        {
            move.NextSquare.Piece = new Queen(move.NextSquare.Location.Row, move.NextSquare.Location.Col, moveColor);
        } else
        {
            move.NextSquare.Piece = move.LastSquare.Piece;
        }
        move.LastSquare.Piece = null;
        move.NextSquare.Piece.Location = move.NextSquare.Location;
        if (alternateTurn)
        {
            CurrentTurn = moveColor == Color.White ? Color.Black : Color.White;
        }
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
        Random rnd = new();
        List<IPiece> pieces = GetUncapturedPieces(color);
        if (pieces.Count > 0) {
            int randomIndex = rnd.Next(pieces.Count());

            // find the first index of a piece that has some valid moves
            int pieceWithMovesIndex = Enumerable.Range(randomIndex, pieces.Count - randomIndex)
                              .Concat(Enumerable.Range(0, randomIndex))
                              .FirstOrDefault(i => pieces[i].GetValidMoves(this, true).Any(), -1);

            if (pieceWithMovesIndex >= 0)
            {
                int randomMoveIndex = rnd.Next(pieces[pieceWithMovesIndex].GetValidMoves(this, true).Count());
                return pieces[pieceWithMovesIndex].GetValidMoves(this, true).ElementAt(randomMoveIndex);
            }
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



