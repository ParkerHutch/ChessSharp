
using ChessSharp;

public static class CustomCollectionAsserter
{
    public static void Contains(IEnumerable<Move> actualMoves, Move moveInQuestion)
    {

        CollectionAssert.Contains(actualMoves.ToList(), moveInQuestion, message: $"{moveInQuestion} not in [{string.Join(", ", actualMoves.ToList())}]");
    }
    public static void DoesNotContain(IEnumerable<Move> actualMoves, Move moveInQuestion)
    {

        CollectionAssert.DoesNotContain(actualMoves.ToList(), moveInQuestion, message: $"{moveInQuestion} should not be a valid move. Moves: [{string.Join(", ", actualMoves.ToList())}]");
    }
}

namespace ChessUtilsTests
{
    [TestClass]
    public class TestNoMovementMoves
    {
        [TestMethod]
        public void TestNoMovementMovesForAllPieces()
        {
            Board board = new(true);
            Board.Square pieceSquare = board.BoardArr[1, 1];
            
            pieceSquare.Piece = new Queen(1, 1, Color.White);
            CustomCollectionAsserter.DoesNotContain(pieceSquare.Piece.GetValidMoves(board), new Move(pieceSquare, pieceSquare));
            pieceSquare.Piece = new Rook(1, 1, Color.White);
            CustomCollectionAsserter.DoesNotContain(pieceSquare.Piece.GetValidMoves(board), new Move(pieceSquare, pieceSquare));
            pieceSquare.Piece = new Bishop(1, 1, Color.White);
            CustomCollectionAsserter.DoesNotContain(pieceSquare.Piece.GetValidMoves(board), new Move(pieceSquare, pieceSquare));
            pieceSquare.Piece = new Knight(1, 1, Color.White);
            CustomCollectionAsserter.DoesNotContain(pieceSquare.Piece.GetValidMoves(board), new Move(pieceSquare, pieceSquare));
            pieceSquare.Piece = new Pawn(1, 1, Color.White);
            CustomCollectionAsserter.DoesNotContain(pieceSquare.Piece.GetValidMoves(board), new Move(pieceSquare, pieceSquare));
            pieceSquare.Piece = new King(1, 1, Color.White);
            CustomCollectionAsserter.DoesNotContain(pieceSquare.Piece.GetValidMoves(board), new Move(pieceSquare, pieceSquare));
        }
    }

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
            CustomCollectionAsserter.Contains(blackPawnSquare1.Piece.GetValidMoves(board), blackPawn1DiagonalMove);
            CustomCollectionAsserter.Contains(blackPawnSquare2.Piece.GetValidMoves(board), blackPawn2DiagonalMove);
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

            for (int row = lowerRow; row >= 0; --row)
            {
                Move invalidUpperRookMove = new(upperRookSquare, board.BoardArr[row, 0]);
                CustomCollectionAsserter.DoesNotContain(upperRookSquare.Piece.GetValidMoves(board), invalidUpperRookMove);
            }
        }

        [TestMethod]
        public void TestHorizontalMovesToUnoccupiedSquares()
        {
            Board.Square rookSquare = board.BoardArr[0, 4];

            rookSquare.Piece = new Rook(0, 4, Color.White);
            for (int col = 0; col < 8; ++col)
            {
                if (col == rookSquare.Location.Col)
                {
                    continue;
                }
                Move horizontalRookMove = new Move(rookSquare, board.BoardArr[rookSquare.Location.Row, col]);
                CustomCollectionAsserter.Contains(rookSquare.Piece.GetValidMoves(board), horizontalRookMove);
            }

        }
        [TestMethod]
        public void TestVerticalMovesToUnoccupiedSquares()
        {
            Board.Square rookSquare = board.BoardArr[4, 0];

            rookSquare.Piece = new Rook(4, 0, Color.White);
            for (int row = 0; row < 8; ++row)
            {
                if (row == rookSquare.Location.Row)
                {
                    continue;
                }
                Move horizontalRookMove = new Move(rookSquare, board.BoardArr[row, rookSquare.Location.Col]);
                CustomCollectionAsserter.Contains(rookSquare.Piece.GetValidMoves(board), horizontalRookMove);
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

            bishopSquare.Piece = new Bishop(bishopSquare.Location.Row, bishopSquare.Location.Col, Color.White);

            int pawnRow = 5;
            board.BoardArr[pawnRow, 2].Piece = new Pawn(pawnRow, 2, Color.White);
            board.BoardArr[pawnRow, 6].Piece = new Pawn(pawnRow, 6, Color.White);

            // check the bottom-left to upper-right diagonal moves are valid
            for (int row = 0; row < bishopSquare.Location.Row; ++row)
            {
                Move lowerDiagonalLeftMove = new(bishopSquare, board.BoardArr[row, row + 1]);
                Move lowerDiagonalRightMove = new(bishopSquare, board.BoardArr[row, 7 - row]);
                CustomCollectionAsserter.Contains(bishopSquare.Piece.GetValidMoves(board), lowerDiagonalLeftMove);
                CustomCollectionAsserter.Contains(bishopSquare.Piece.GetValidMoves(board), lowerDiagonalRightMove);
            }

            for (int row = bishopSquare.Location.Row + 1; row < pawnRow; ++row)
            {
                int squaresAboveBishopCount = row - bishopSquare.Location.Row;
                Move upperDiagonalLeftMove = new(bishopSquare, board.BoardArr[row, bishopSquare.Location.Col - squaresAboveBishopCount]);
                Move upperDiagonalRightMove = new(bishopSquare, board.BoardArr[row, bishopSquare.Location.Col + squaresAboveBishopCount]);
                CustomCollectionAsserter.Contains(bishopSquare.Piece.GetValidMoves(board), upperDiagonalLeftMove);
                CustomCollectionAsserter.Contains(bishopSquare.Piece.GetValidMoves(board), upperDiagonalRightMove);
            }

            // check that moves on or after the pawns are not valid
            for (int row = pawnRow; row < 8; ++row)
            {
                int squaresAboveBishopCount = row - bishopSquare.Location.Row;
                if (board.getSquareAt(row, bishopSquare.Location.Col - squaresAboveBishopCount, out Board.Square? northwestDiagonalSquare))
                {
                    Move invalidNorthwestMove = new(bishopSquare, northwestDiagonalSquare!);
                    CustomCollectionAsserter.DoesNotContain(bishopSquare.Piece.GetValidMoves(board), invalidNorthwestMove);
                }
                if (board.getSquareAt(row, bishopSquare.Location.Col + squaresAboveBishopCount, out Board.Square? northeastDiagonalSquare))
                {
                    Move invalidNortheastMove = new(bishopSquare, northeastDiagonalSquare!);
                    CustomCollectionAsserter.DoesNotContain(bishopSquare.Piece.GetValidMoves(board), invalidNortheastMove);
                }
            }
        }

        [TestMethod]
        public void TestCanMoveToDiagonalSquareIfOccupiedByEnemy()
        {
            // bishop sandwiched by other friendly pieces
            Board.Square bishopSquare = board.BoardArr[3, 4]; // equivalent to E4

            bishopSquare.Piece = new Bishop(bishopSquare.Location.Row, bishopSquare.Location.Col, Color.White);

            int offset = 1;

            List<Board.Square> enemyPawnSquares = new List<Board.Square>
            {
                board.BoardArr[bishopSquare.Location.Row - offset, 
                    bishopSquare.Location.Col - offset],
                board.BoardArr[bishopSquare.Location.Row - offset,
                    bishopSquare.Location.Col + offset],
                board.BoardArr[bishopSquare.Location.Row + offset,
                    bishopSquare.Location.Col - offset],
                board.BoardArr[bishopSquare.Location.Row + offset,
                    bishopSquare.Location.Col + offset],

            };
            foreach(Board.Square enemyPawnSquare in enemyPawnSquares)
            {
                enemyPawnSquare.Piece = new Pawn(enemyPawnSquare.Location.Row, 
                    enemyPawnSquare.Location.Col, Color.Black);
                CustomCollectionAsserter.Contains(bishopSquare.Piece.GetValidMoves(board),
                    new Move(bishopSquare, enemyPawnSquare));
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
        public void TestCanMoveToEmptySquares()
        {
            Board.Square knightSquare = board.BoardArr[3, 4];
            knightSquare.Piece = new Knight(3, 4, Color.White);
            // (col, row) (flip these for checks below): (4, 3) -> (3, 1), (3, 5), (2, 2), (2, 4), (5, 1), (6, 2), (6, 4), (5, 3)
            CustomCollectionAsserter.Contains(knightSquare.Piece.GetValidMoves(board), new Move(knightSquare, board.BoardArr[2, 6]));
            CustomCollectionAsserter.Contains(knightSquare.Piece.GetValidMoves(board), new Move(knightSquare, board.BoardArr[2, 2]));
            CustomCollectionAsserter.Contains(knightSquare.Piece.GetValidMoves(board), new Move(knightSquare, board.BoardArr[4, 6]));
            CustomCollectionAsserter.Contains(knightSquare.Piece.GetValidMoves(board), new Move(knightSquare, board.BoardArr[4, 2]));
            CustomCollectionAsserter.Contains(knightSquare.Piece.GetValidMoves(board), new Move(knightSquare, board.BoardArr[1, 5]));
            CustomCollectionAsserter.Contains(knightSquare.Piece.GetValidMoves(board), new Move(knightSquare, board.BoardArr[1, 3]));
            CustomCollectionAsserter.Contains(knightSquare.Piece.GetValidMoves(board), new Move(knightSquare, board.BoardArr[5, 3]));
            CustomCollectionAsserter.Contains(knightSquare.Piece.GetValidMoves(board), new Move(knightSquare, board.BoardArr[5, 5]));
        }

        [TestMethod]
        public void TestCannotMoveToSquaresOccupiedByFriendlyPieces()
        {
            Board.Square knightSquare = board.BoardArr[3, 4];
            knightSquare.Piece = new Knight(3, 4, Color.White);
            List<(int friendlyPawnRow, int friendlyPawnCol)> friendlyPawnPositions = [
                (2, 6),
                (2, 2),
                (4, 6),
                (4, 2),
                (1, 5),
                (1, 3),
                (5, 3),
                (5, 5)
            ];
            foreach((int friendlyPawnRow, int friendlyPawnCol) in friendlyPawnPositions)
            {
                board.BoardArr[friendlyPawnRow, friendlyPawnCol].Piece = 
                    new Pawn(friendlyPawnRow, friendlyPawnCol, Color.White);
                CustomCollectionAsserter.DoesNotContain(knightSquare.Piece.GetValidMoves(board), 
                    new Move(knightSquare, board.BoardArr[friendlyPawnRow, friendlyPawnCol]));
            }
        }

        [TestMethod]
        public void TestCanMoveToSquaresOccupiedByEnemyPieces()
        {
            Board.Square knightSquare = board.BoardArr[3, 4];
            knightSquare.Piece = new Knight(3, 4, Color.White);
            List<(int enemyPawnRow, int enemyPawnCol)> enemyPawnPositions = [
                (2, 6),
                (2, 2),
                (4, 6),
                (4, 2),
                (1, 5),
                (1, 3),
                (5, 3),
                (5, 5)
            ];
            foreach ((int enemyPawnRow, int enemyPawnCol) in enemyPawnPositions)
            {
                board.BoardArr[enemyPawnRow, enemyPawnCol].Piece =
                    new Pawn(enemyPawnRow, enemyPawnCol, Color.Black);
                CustomCollectionAsserter.Contains(knightSquare.Piece.GetValidMoves(board),
                    new Move(knightSquare, board.BoardArr[enemyPawnRow, enemyPawnCol]));
            }
        }
    }

    [TestClass]
    public class TestQueenMoves
    {
        public Board board = new(true);

        [TestInitialize]
        public void setupBoard()
        {
            board = new(true);
        }

        [TestClass]
        public class TestQueenHasKingMoves()
        {
            public Board board = new(true);

            [TestInitialize]
            public void setupBoard()
            {
                board = new(true);
            }

            [TestMethod]
            public void TestCanMoveToEmptySquares()
            {
                Board.Square queenSquare = board.BoardArr[3, 4];
                queenSquare.Piece = new Queen(queenSquare.Location.Row, queenSquare.Location.Col, Color.White);

                for (int row = queenSquare.Location.Row - 1; row <= queenSquare.Location.Row + 1; row++)
                {
                    for (int col = queenSquare.Location.Col - 1; col <= queenSquare.Location.Col + 1; col++)
                    {
                        if (row == queenSquare.Location.Row && col == queenSquare.Location.Col)
                        {
                            continue;
                        }
                        if (board.getSquareAt(row, col, out Board.Square? neighborSquare))
                        {
                            CustomCollectionAsserter.Contains
                                (queenSquare.Piece.GetValidMoves(board),
                                new Move(queenSquare, neighborSquare!));
                        }
                    }
                }
            }
            [TestMethod]
            public void TestCanMoveToEnemyOccupiedSquares()
            {
                Board.Square queenSquare = board.BoardArr[3, 4];
                queenSquare.Piece = new Queen(queenSquare.Location.Row, queenSquare.Location.Col, Color.White);

                for (int row = queenSquare.Location.Row - 1; row <= queenSquare.Location.Row + 1; row++)
                {
                    for (int col = queenSquare.Location.Col - 1; col <= queenSquare.Location.Col + 1; col++)
                    {
                        if (row == queenSquare.Location.Row && col == queenSquare.Location.Col)
                        {
                            continue;
                        }
                        if (board.getSquareAt(row, col, out Board.Square? neighborSquare))
                        {
                            neighborSquare!.Piece = new Pawn(row, col, Color.Black);
                            CustomCollectionAsserter.Contains
                                (queenSquare.Piece.GetValidMoves(board),
                                new Move(queenSquare, neighborSquare!));
                        }
                    }
                }
            }

            [TestMethod]
            public void TestCannotMoveToFriendlyOccupiedSquares()
            {
                Board.Square queenSquare = board.BoardArr[3, 4];
                queenSquare.Piece = new Queen(queenSquare.Location.Row, queenSquare.Location.Col, Color.White);

                for (int row = queenSquare.Location.Row - 1; row <= queenSquare.Location.Row + 1; row++)
                {
                    for (int col = queenSquare.Location.Col - 1; col <= queenSquare.Location.Col + 1; col++)
                    {
                        if (row == queenSquare.Location.Row && col == queenSquare.Location.Col)
                        {
                            continue;
                        }
                        if (board.getSquareAt(row, col, out Board.Square? neighborSquare))
                        {
                            neighborSquare!.Piece = new Pawn(row, col, Color.White);
                            CustomCollectionAsserter.DoesNotContain
                                (queenSquare.Piece.GetValidMoves(board),
                                new Move(queenSquare, neighborSquare!));
                        }
                    }
                }
            }
        }

        [TestClass]
        public class TestQueenHasRookMoves
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
                Board.Square lowerQueenSquare = board.BoardArr[lowerRow, 0];
                Board.Square upperQueenSquare = board.BoardArr[upperRow, 0];

                lowerQueenSquare.Piece = new Queen(lowerRow, 0, Color.White);
                upperQueenSquare.Piece = new Queen(upperRow, 0, Color.White);

                // verify that both Queens can move to any empty square between them
                for (int row = lowerRow + 1; row < upperRow; ++row)
                {
                    Move lowerQueenMove = new(lowerQueenSquare, board.BoardArr[row, 0]);

                    CustomCollectionAsserter.Contains(lowerQueenSquare.Piece.GetValidMoves(board), lowerQueenMove);

                    Move upperQueenMove = new(upperQueenSquare, board.BoardArr[row, 0]);
                    CustomCollectionAsserter.Contains(upperQueenSquare.Piece.GetValidMoves(board), upperQueenMove);
                }

                // verify that Queens can't move on top of or past each other
                for (int row = upperRow; row < 8; ++row)
                {
                    Move invalidLowerQueenMove = new(lowerQueenSquare, board.BoardArr[row, 0]);
                    CustomCollectionAsserter.DoesNotContain(lowerQueenSquare.Piece.GetValidMoves(board), invalidLowerQueenMove);
                }

                for (int row = lowerRow; row >= 0; --row)
                {
                    Move invalidUpperQueenMove = new(upperQueenSquare, board.BoardArr[row, 0]);
                    CustomCollectionAsserter.DoesNotContain(upperQueenSquare.Piece.GetValidMoves(board), invalidUpperQueenMove);
                }
            }

            [TestMethod]
            public void TestHorizontalMovesToUnoccupiedSquares()
            {
                Board.Square queenSquare = board.BoardArr[0, 4];

                queenSquare.Piece = new Queen(0, 4, Color.White);
                for (int col = 0; col < 8; ++col)
                {
                    if (col == queenSquare.Location.Col)
                    {
                        continue;
                    }
                    Move horizontalQueenMove = new Move(queenSquare, board.BoardArr[queenSquare.Location.Row, col]);
                    CustomCollectionAsserter.Contains(queenSquare.Piece.GetValidMoves(board), horizontalQueenMove);
                }

            }
            [TestMethod]
            public void TestVerticalMovesToUnoccupiedSquares()
            {
                Board.Square queenSquare = board.BoardArr[4, 0];

                queenSquare.Piece = new Queen(4, 0, Color.White);
                for (int row = 0; row < 8; ++row)
                {
                    if (row == queenSquare.Location.Row)
                    {
                        continue;
                    }
                    Move horizontalQueenMove = new Move(queenSquare, board.BoardArr[row, queenSquare.Location.Col]);
                    CustomCollectionAsserter.Contains(queenSquare.Piece.GetValidMoves(board), horizontalQueenMove);
                }

            }
        }

        [TestClass]
        public class TestQueenHasBishopMoves
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
                Board.Square queenSquare = board.BoardArr[3, 4]; // equivalent to E4

                queenSquare.Piece = new Bishop(queenSquare.Location.Row, queenSquare.Location.Col, Color.White);

                int pawnRow = 5;
                board.BoardArr[pawnRow, 2].Piece = new Pawn(pawnRow, 2, Color.White);
                board.BoardArr[pawnRow, 6].Piece = new Pawn(pawnRow, 6, Color.White);

                // check the bottom-left to upper-right diagonal moves are valid
                for (int row = 0; row < queenSquare.Location.Row; ++row)
                {
                    Move lowerDiagonalLeftMove = new(queenSquare, board.BoardArr[row, row + 1]);
                    Move lowerDiagonalRightMove = new(queenSquare, board.BoardArr[row, 7 - row]);
                    CustomCollectionAsserter.Contains(queenSquare.Piece.GetValidMoves(board), lowerDiagonalLeftMove);
                    CustomCollectionAsserter.Contains(queenSquare.Piece.GetValidMoves(board), lowerDiagonalRightMove);
                }

                for (int row = queenSquare.Location.Row + 1; row < pawnRow; ++row)
                {
                    int squaresAboveBishopCount = row - queenSquare.Location.Row;
                    Move upperDiagonalLeftMove = new(queenSquare, board.BoardArr[row, queenSquare.Location.Col - squaresAboveBishopCount]);
                    Move upperDiagonalRightMove = new(queenSquare, board.BoardArr[row, queenSquare.Location.Col + squaresAboveBishopCount]);
                    CustomCollectionAsserter.Contains(queenSquare.Piece.GetValidMoves(board), upperDiagonalLeftMove);
                    CustomCollectionAsserter.Contains(queenSquare.Piece.GetValidMoves(board), upperDiagonalRightMove);
                }

                // check that moves on or after the pawns are not valid
                for (int row = pawnRow; row < 8; ++row)
                {
                    int squaresAboveBishopCount = row - queenSquare.Location.Row;
                    if (board.getSquareAt(row, queenSquare.Location.Col - squaresAboveBishopCount, out Board.Square? northwestDiagonalSquare))
                    {
                        Move invalidNorthwestMove = new(queenSquare, northwestDiagonalSquare!);
                        CustomCollectionAsserter.DoesNotContain(queenSquare.Piece.GetValidMoves(board), invalidNorthwestMove);
                    }
                    if (board.getSquareAt(row, queenSquare.Location.Col + squaresAboveBishopCount, out Board.Square? northeastDiagonalSquare))
                    {
                        Move invalidNortheastMove = new(queenSquare, northeastDiagonalSquare!);
                        CustomCollectionAsserter.DoesNotContain(queenSquare.Piece.GetValidMoves(board), invalidNortheastMove);
                    }
                }
            }

            [TestMethod]
            public void TestCanMoveToDiagonalSquareIfOccupiedByEnemy()
            {
                // bishop sandwiched by other friendly pieces
                Board.Square queenSquare = board.BoardArr[3, 4]; // equivalent to E4

                queenSquare.Piece = new Queen(queenSquare.Location.Row, queenSquare.Location.Col, Color.White);

                int offset = 1;

                List<Board.Square> enemyPawnSquares = new List<Board.Square>
            {
                board.BoardArr[queenSquare.Location.Row - offset,
                    queenSquare.Location.Col - offset],
                board.BoardArr[queenSquare.Location.Row - offset,
                    queenSquare.Location.Col + offset],
                board.BoardArr[queenSquare.Location.Row + offset,
                    queenSquare.Location.Col - offset],
                board.BoardArr[queenSquare.Location.Row + offset,
                    queenSquare.Location.Col + offset],

            };
                foreach (Board.Square enemyPawnSquare in enemyPawnSquares)
                {
                    enemyPawnSquare.Piece = new Pawn(enemyPawnSquare.Location.Row,
                        enemyPawnSquare.Location.Col, Color.Black);
                    CustomCollectionAsserter.Contains(queenSquare.Piece.GetValidMoves(board),
                        new Move(queenSquare, enemyPawnSquare));
                }
            }
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
        public void TestCanMoveToEmptySquares()
        {
            Board.Square kingSquare = board.BoardArr[3, 4];
            kingSquare.Piece = new King(kingSquare.Location.Row, kingSquare.Location.Col, Color.White);

            for (int row = kingSquare.Location.Row - 1; row <= kingSquare.Location.Row + 1; row++)
            {
                for (int col = kingSquare.Location.Col - 1; col <= kingSquare.Location.Col + 1; col++)
                {
                    if (row == kingSquare.Location.Row && col == kingSquare.Location.Col)
                    {
                        continue;
                    }
                    if (board.getSquareAt(row, col, out Board.Square? neighborSquare))
                    {
                        CustomCollectionAsserter.Contains
                            (kingSquare.Piece.GetValidMoves(board),
                            new Move(kingSquare, neighborSquare!));
                    }
                }
            }
        }
        [TestMethod]
        public void TestCanMoveToEnemyOccupiedSquares()
        {
            Board.Square kingSquare = board.BoardArr[3, 4];
            kingSquare.Piece = new King(kingSquare.Location.Row, kingSquare.Location.Col, Color.White);

            for (int row = kingSquare.Location.Row - 1; row <= kingSquare.Location.Row + 1; row++)
            {
                for (int col = kingSquare.Location.Col - 1; col <= kingSquare.Location.Col + 1; col++)
                {
                    if (row == kingSquare.Location.Row && col == kingSquare.Location.Col)
                    {
                        continue;
                    }
                    if (board.getSquareAt(row, col, out Board.Square? neighborSquare))
                    {
                        neighborSquare!.Piece = new Pawn(row, col, Color.Black);
                        CustomCollectionAsserter.Contains
                            (kingSquare.Piece.GetValidMoves(board),
                            new Move(kingSquare, neighborSquare!));
                    }
                }
            }
        }

        [TestMethod]
        public void TestCannotMoveToFriendlyOccupiedSquares()
        {
            Board.Square kingSquare = board.BoardArr[3, 4];
            kingSquare.Piece = new King(kingSquare.Location.Row, kingSquare.Location.Col, Color.White);

            for (int row = kingSquare.Location.Row - 1; row <= kingSquare.Location.Row + 1; row++)
            {
                for (int col = kingSquare.Location.Col - 1; col <= kingSquare.Location.Col + 1; col++)
                {
                    if (row == kingSquare.Location.Row && col == kingSquare.Location.Col)
                    {
                        continue;
                    }
                    if (board.getSquareAt(row, col, out Board.Square? neighborSquare))
                    {
                        neighborSquare!.Piece = new Pawn(row, col, Color.White);
                        CustomCollectionAsserter.DoesNotContain
                            (kingSquare.Piece.GetValidMoves(board),
                            new Move(kingSquare, neighborSquare!));
                    }
                }
            }
        }
    }
}