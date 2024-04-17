using ChessSharp;

namespace ChessUtilsTests
{
    [TestClass]
    public class TestPawnMoves
    {
        Board board = new(true);

        [TestInitialize]
        public void setupBoard()
        {
            board = new(true);
        }

        [TestMethod]
        public void TestCanMoveForwardIfNotOccupiedByPiece()
        {
            for (int col = 0; col < 8; ++col)
            {
                Board.Square currSquare = board.BoardArr[2, col];
                currSquare.Piece = new Pawn(2, col, Color.White);
                Board.Square expectedMoveSquare = board.BoardArr[3, col];
                Move expectedMove = new(currSquare, expectedMoveSquare);
                CollectionAssert.Contains(currSquare.Piece.GetValidMoves(board).ToList(), expectedMove);
            }
        }

        [TestMethod]
        public void TestCannotMoveForwardWhenOccupiedByPiece()
        {
            //Board.Square currSquare = board.BoardArr[2, 0];

            Board.Square whitePawnSquare = board.BoardArr[2, 1];
            Board.Square blackPawnSquare = board.BoardArr[3, 1];

            whitePawnSquare.Piece = new Pawn(2, 1, Color.White);
            blackPawnSquare.Piece = new Pawn(3, 1, Color.Black);


            Move invalidWhitePawnMove = new(whitePawnSquare, blackPawnSquare);
            Move invalidBlackPawnMove = new(blackPawnSquare, whitePawnSquare);

            CollectionAssert.DoesNotContain(whitePawnSquare.Piece.GetValidMoves(board).ToList(), invalidWhitePawnMove);
            CollectionAssert.DoesNotContain(blackPawnSquare.Piece.GetValidMoves(board).ToList(), invalidBlackPawnMove);
        }

        [TestMethod]
        public void TestCanMoveDiagonalIfOccupiedByPiece()
        {
            Board.Square whitePawnSquare = board.BoardArr[2, 1];
            Board.Square blackPawnSquare1 = board.BoardArr[3, 2];
            Board.Square blackPawnSquare2 = board.BoardArr[3, 0];

            whitePawnSquare.Piece = new Pawn(2, 1, Color.White);
            blackPawnSquare1.Piece = new Pawn(3, 2, Color.Black);
            blackPawnSquare2.Piece = new Pawn(3, 0, Color.Black);


            Move whitePawnDiagonalMove1 = new(whitePawnSquare, blackPawnSquare1);
            Move whitePawnDiagonalMove2 = new(whitePawnSquare, blackPawnSquare2);
            Move blackPawn1DiagonalMove = new(blackPawnSquare1, whitePawnSquare);
            Move blackPawn2DiagonalMove = new(blackPawnSquare2, whitePawnSquare);

            CollectionAssert.Contains(whitePawnSquare.Piece.GetValidMoves(board).ToList(), whitePawnDiagonalMove1, message: $"Moves present: {string.Join(',', whitePawnSquare.Piece.GetValidMoves(board).ToList())}");
            CollectionAssert.Contains(whitePawnSquare.Piece.GetValidMoves(board).ToList(), whitePawnDiagonalMove2, message: $"Moves present: {string.Join(',', whitePawnSquare.Piece.GetValidMoves(board).ToList())}");

            CollectionAssert.Contains(blackPawnSquare1.Piece.GetValidMoves(board).ToList(), blackPawn1DiagonalMove, message: $"Moves present: {string.Join(',', blackPawnSquare1.Piece.GetValidMoves(board).ToList())}");
            CollectionAssert.Contains(blackPawnSquare2.Piece.GetValidMoves(board).ToList(), blackPawn2DiagonalMove, message: $"Moves present: {string.Join(',', blackPawnSquare2.Piece.GetValidMoves(board).ToList())}");
        }
    }

    [TestClass]
    public class TestRookMoves
    {
        Board board = new(true);

        [TestInitialize]
        public void setupBoard()
        {
            board = new(true);
        }

        [TestMethod]
        public void TestCanMoveForwardUntilOccupiedByFriendlyPiece()
        {
            int lowerRow = 2;
            int upperRow = 5;
            Board.Square lowerRookSquare = board.BoardArr[lowerRow, 0];
            Board.Square upperRookSquare = board.BoardArr[upperRow, 0];

            lowerRookSquare.Piece = new Rook(lowerRow, 0, Color.White);
            upperRookSquare.Piece = new Rook(upperRow, 0, Color.White);

            // verify that both rooks can move to any empty square between them
            for (int row = lowerRow + 1; row < upperRow; ++row)
            {
                Move lowerRookMove = new(lowerRookSquare, board.BoardArr[row, 0]);

                CollectionAssert.Contains(lowerRookSquare.Piece.GetValidMoves(board).ToList(), lowerRookMove, message: $"Moves present: {string.Join(',', lowerRookSquare.Piece.GetValidMoves(board).ToList())}");

                Move upperRookMove = new(upperRookSquare, board.BoardArr[row, 0]);
                CollectionAssert.Contains(upperRookSquare.Piece.GetValidMoves(board).ToList(), upperRookMove, message: $"Moves present: {string.Join(',', upperRookSquare.Piece.GetValidMoves(board).ToList())}");
            }

            // verify that rooks can't move on top of or past each other
            for (int row = upperRow; row < 8; ++row)
            {
                Move invalidLowerRookMove = new(lowerRookSquare, board.BoardArr[row, 0]);
                CollectionAssert.DoesNotContain(lowerRookSquare.Piece.GetValidMoves(board).ToList(), invalidLowerRookMove);
            }

            for (int row = lowerRow; lowerRow >= 0; --row)
            {
                Move invalidUpperRookMove = new(upperRookSquare, board.BoardArr[row, 0]);
                CollectionAssert.DoesNotContain(upperRookSquare.Piece.GetValidMoves(board).ToList(), invalidUpperRookMove);
            }
        }
    }
}