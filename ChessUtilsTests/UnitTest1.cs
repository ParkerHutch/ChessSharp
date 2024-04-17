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
}