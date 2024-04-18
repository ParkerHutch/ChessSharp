
using ChessSharp;

public static class CustomCollectionAsserter
{
    public static void Contains(IEnumerable<Move> actualMoves, Move moveInQuestion)
    {

        CollectionAssert.Contains(actualMoves.ToList(), moveInQuestion, message: $"{moveInQuestion} not in [{string.Join(", ", actualMoves.ToList())}]");
    }
    public static void DoesNotContain(IEnumerable<Move> actualMoves, Move moveInQuestion)
    {

        CollectionAssert.Contains(actualMoves.ToList(), moveInQuestion, message: $"{moveInQuestion} should not be a valid move. Moves: [{string.Join(", ", actualMoves.ToList())}]");
    }
}

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

            CustomCollectionAsserter.Contains(whitePawnSquare.Piece.GetValidMoves(board), whitePawnDiagonalMove1);
            CustomCollectionAsserter.Contains(whitePawnSquare.Piece.GetValidMoves(board), whitePawnDiagonalMove2);
            //CollectionAssert.Contains(whitePawnSquare.Piece.GetValidMoves(board).ToList(), whitePawnDiagonalMove1, message: $"Moves present: {string.Join(',', whitePawnSquare.Piece.GetValidMoves(board).ToList())}");
            //CollectionAssert.Contains(whitePawnSquare.Piece.GetValidMoves(board).ToList(), whitePawnDiagonalMove2, message: $"Moves present: {string.Join(',', whitePawnSquare.Piece.GetValidMoves(board).ToList())}");
            CustomCollectionAsserter.Contains(blackPawnSquare1.Piece.GetValidMoves(board), blackPawn1DiagonalMove);
            CustomCollectionAsserter.Contains(blackPawnSquare2.Piece.GetValidMoves(board), blackPawn2DiagonalMove);
            //CollectionAssert.Contains(blackPawnSquare1.Piece.GetValidMoves(board).ToList(), blackPawn1DiagonalMove, message: $"Moves present: {string.Join(',', blackPawnSquare1.Piece.GetValidMoves(board).ToList())}");
            //CollectionAssert.Contains(blackPawnSquare2.Piece.GetValidMoves(board).ToList(), blackPawn2DiagonalMove, message: $"Moves present: {string.Join(',', blackPawnSquare2.Piece.GetValidMoves(board).ToList())}");
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
        public void TestNoMoveIsNotValidMove()
        {
            Board.Square rookSquare = board.BoardArr[4, 4];
            rookSquare.Piece = new Rook(4, 4, Color.White);
            Move invalidMove = new(rookSquare, rookSquare);
            CustomCollectionAsserter.DoesNotContain(rookSquare.Piece.GetValidMoves(board), invalidMove);

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

                CustomCollectionAsserter.Contains(lowerRookSquare.Piece.GetValidMoves(board), lowerRookMove);

                Move upperRookMove = new(upperRookSquare, board.BoardArr[row, 0]);
                CustomCollectionAsserter.Contains(upperRookSquare.Piece.GetValidMoves(board), upperRookMove);
            }

            // verify that rooks can't move on top of or past each other
            for (int row = upperRow; row < 8; ++row)
            {
                Move invalidLowerRookMove = new(lowerRookSquare, board.BoardArr[row, 0]);
                CustomCollectionAsserter.DoesNotContain(lowerRookSquare.Piece.GetValidMoves(board), invalidLowerRookMove);
            }

            for (int row = lowerRow; lowerRow >= 0; --row)
            {
                Move invalidUpperRookMove = new(upperRookSquare, board.BoardArr[row, 0]);
                CustomCollectionAsserter.DoesNotContain(upperRookSquare.Piece.GetValidMoves(board), invalidUpperRookMove);
            }
        }
    }

    [TestClass]
    public class TestBishopMoves
    {
        Board board = new(true);

        [TestInitialize]
        public void setupBoard()
        {
            board = new(true);
        }

        [TestMethod]
        public void TestCanMoveDiagonalUntilSquareOccupiedByFriendlyPiece()
        {
            // bishop sandwiched by other friendly pieces
            Board.Square bishopSquare = board.BoardArr[3, 4]; // equivalent to E4

            bishopSquare.Piece = new Bishop(3, 4, Color.White);

            board.BoardArr[6, 2].Piece = new Pawn(6, 2, Color.White);
            board.BoardArr[6, 5].Piece = new Pawn(6, 5, Color.White);

            // check the bottom-left to upper-right diagonal moves are valid
            for (int row = 0; row < 3; ++row)
            {
                Move lowerDiagonalLeftMove = new(bishopSquare, board.BoardArr[row, row + 1]);
                Move lowerDiagonalRightMove = new(bishopSquare, board.BoardArr[row, 7 - row]);
                CustomCollectionAsserter.Contains(bishopSquare.Piece.GetValidMoves(board), lowerDiagonalLeftMove);
                CustomCollectionAsserter.Contains(bishopSquare.Piece.GetValidMoves(board), lowerDiagonalRightMove);
            }

            for (int row = 4; row < 6; ++ row)
            {
                int squaresAboveBishopCount = 4 - row;
                Move upperDiagonalLeftMove = new(bishopSquare, board.BoardArr[row, row - squaresAboveBishopCount]);
                Move upperDiagonalRightMove = new(bishopSquare, board.BoardArr[row, row]);
                CustomCollectionAsserter.Contains(bishopSquare.Piece.GetValidMoves(board), upperDiagonalLeftMove);
                CustomCollectionAsserter.Contains(bishopSquare.Piece.GetValidMoves(board), upperDiagonalRightMove);
            }

            // check that moves on or after the pawns are not valid
            for (int row = 6; row < 8; ++row)
            {
                int squaresAboveBishopCount = 4 - row;
                Move upperDiagonalLeftMove = new(bishopSquare, board.BoardArr[row, row - squaresAboveBishopCount]);
                Move upperDiagonalRightMove = new(bishopSquare, board.BoardArr[row, row]);
                CustomCollectionAsserter.DoesNotContain(bishopSquare.Piece.GetValidMoves(board), upperDiagonalLeftMove);
                CustomCollectionAsserter.DoesNotContain(bishopSquare.Piece.GetValidMoves(board), upperDiagonalRightMove);
            }
        }
    }

    [TestClass]
    public class TestKnightMoves
    {
        Board board = new(true);

        [TestInitialize]
        public void setupBoard()
        {
            board = new(true);
        }

        [TestMethod]
        public void TestValidMoves()
        {
            Board.Square knightSquare = board.BoardArr[3, 4];
            knightSquare.Piece = new Knight(3, 4, Color.White);
            // (col, row) (flip these for checks below): (4, 3) -> (3, 1), (3, 5), (2, 2), (2, 4), (5, 1), (6, 2), (6, 4), (5, 3)
            CustomCollectionAsserter.Contains(knightSquare.Piece.GetValidMoves(board), new Move(knightSquare, board.BoardArr[1, 3]));
            CustomCollectionAsserter.Contains(knightSquare.Piece.GetValidMoves(board), new Move(knightSquare, board.BoardArr[3, 1]));
            CustomCollectionAsserter.Contains(knightSquare.Piece.GetValidMoves(board), new Move(knightSquare, board.BoardArr[5, 3]));
            CustomCollectionAsserter.Contains(knightSquare.Piece.GetValidMoves(board), new Move(knightSquare, board.BoardArr[3, 5]));
            CustomCollectionAsserter.Contains(knightSquare.Piece.GetValidMoves(board), new Move(knightSquare, board.BoardArr[2, 2]));
            CustomCollectionAsserter.Contains(knightSquare.Piece.GetValidMoves(board), new Move(knightSquare, board.BoardArr[2, 4]));
            CustomCollectionAsserter.Contains(knightSquare.Piece.GetValidMoves(board), new Move(knightSquare, board.BoardArr[6, 2]));
            CustomCollectionAsserter.Contains(knightSquare.Piece.GetValidMoves(board), new Move(knightSquare, board.BoardArr[6, 4]));
        }
    }

    [TestClass]
    public class TestQueenMoves
    {
        Board board = new(true);

        [TestInitialize]
        public void setupBoard()
        {
            board = new(true);
        }

        [TestMethod]
        public void TestValidMoves()
        {
            // TODO use bishop and rook tests here first then add king tests
            Assert.IsTrue(false);
        }
    }

    [TestClass]
    public class TestKingMoves
    {
        Board board = new(true);

        [TestInitialize]
        public void setupBoard()
        {
            board = new(true);
        }

        [TestMethod]
        public void TestValidMoves()
        {
            Assert.IsTrue(false);
        }
    }
}