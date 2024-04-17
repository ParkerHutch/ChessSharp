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
        public void TestFreeSquareAheadIsValidMove()
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
    }
}