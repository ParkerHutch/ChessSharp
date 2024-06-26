﻿using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//using System.Drawing;
//using System.Windows.Forms;

using ChessSharp;
using static ChessSharp.Board;

namespace ChessSharpGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly Dictionary<PieceType, ImageSource> capturablePieceTypeToImage = new()
        {
            { PieceType.King, Images.King_Red },
            { PieceType.Queen, Images.Queen_Red },
            { PieceType.Rook, Images.Rook_Red },
            { PieceType.Bishop, Images.Bishop_Red },
            { PieceType.Knight, Images.Knight_Red },
            { PieceType.Pawn, Images.Pawn_Red }
        };

        private readonly Dictionary<PieceType, Dictionary<ChessSharp.Color, ImageSource>> pieceTypeToImage = new()
        {
            {PieceType.King, new Dictionary<ChessSharp.Color, ImageSource>()
                {
                    { ChessSharp.Color.White, Images.King_Light },
                    { ChessSharp.Color.Black, Images.King_Dark }
                } 
            },
            {PieceType.Queen, new Dictionary<ChessSharp.Color, ImageSource>()
                {
                    { ChessSharp.Color.White, Images.Queen_Light },
                    { ChessSharp.Color.Black, Images.Queen_Dark }
                }
            },
            {PieceType.Rook, new Dictionary<ChessSharp.Color, ImageSource>()
                {
                    { ChessSharp.Color.White, Images.Rook_Light },
                    { ChessSharp.Color.Black, Images.Rook_Dark }
                }
            },
            {PieceType.Bishop, new Dictionary<ChessSharp.Color, ImageSource>()
                {
                    { ChessSharp.Color.White, Images.Bishop_Light },
                    { ChessSharp.Color.Black, Images.Bishop_Dark }
                }
            },
            {PieceType.Knight, new Dictionary<ChessSharp.Color, ImageSource>()
                {
                    { ChessSharp.Color.White, Images.Knight_Light },
                    { ChessSharp.Color.Black, Images.Knight_Dark }
                }
            },
            {PieceType.Pawn, new Dictionary<ChessSharp.Color, ImageSource>()
                {
                    { ChessSharp.Color.White, Images.Pawn_Light },
                    { ChessSharp.Color.Black, Images.Pawn_Dark }
                }
            }
        };

        private readonly int rows = 8, cols = 8;
        private readonly Image[,] pieceImages;
        private readonly Image[,] gridImages;
        private Board board;
        private IPiece? selectedPiece = null;

        public MainWindow()
        {
            InitializeComponent();
            pieceImages = SetupOverlayGrid();
            gridImages = SetupGrid();
            
            board = new(false);
        }

        private Image[,] SetupGrid()
        {
            Image[,] images = new Image[rows, cols];
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {

                    Image image = new Image
                    {
                        Source = (r + c) % 2 == 0 ? Images.Green : Images.White
                    };

                    images[r, c] = image;
                    GameGrid.Children.Add(image);

                }
            }
            return images;
        }

        private Image[,] SetupOverlayGrid()
        {
            Image[,] images = new Image[rows, cols];
            OverlayGrid.Rows = rows;
            OverlayGrid.Columns = cols;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Image image = new Image
                    {
                        Source = (r + c) % 2 == 0 ? Images.Green : Images.White
                    };

                    images[r, c] = image;
                    OverlayGrid.Children.Add(image);
                }
            }
            return images;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Draw();
            await GameLoop();
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                Move? randomMove = board.GetRandomMoveForColor(ChessSharp.Color.Black);
                if (randomMove != null) {
                    board.ExecuteMove(randomMove);
                    Draw();
                }
            }
        }

        private Board.Square? getBoardSquareFromMouseClickPosition(Point mouseClickPoint)
        {
            double squareWidth = GameGrid.ActualWidth / 8;
            double squareHeight = GameGrid.ActualHeight / 8;
            int rowIndex = (int)(mouseClickPoint.Y / squareWidth);
            int colIndex = (int)(mouseClickPoint.X / squareHeight);

            if (board.GetSquareAt(rowIndex, colIndex, out Board.Square? square))
            {
                return square;
            }
            return null;
        }

        private void ClearAllMoveOverlays()
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (board.BoardArr[r, c].Piece != null)
                    {
                        IPiece piece = board.BoardArr[r, c].Piece!;
                        pieceImages[r, c].Source = pieceTypeToImage[piece.Type][piece.Color];
                    }
                    else
                    {
                        pieceImages[r, c].Source = gridImages[r, c].Source;
                    }
                }
            }
        }
        private void HighlightValidMovesForPiece(IPiece piece)
        {
            ClearAllMoveOverlays();
            foreach (Move move in piece.GetValidMoves(board, true))
            {
                IPiece? pieceAtSquare = move.NextSquare.Piece;
                if (pieceAtSquare != null)
                {
                    pieceImages[move.NextSquare.Location.Row, move.NextSquare.Location.Col].Source = capturablePieceTypeToImage[pieceAtSquare.Type];
                } else
                {
                    pieceImages[move.NextSquare.Location.Row, move.NextSquare.Location.Col].Source = Images.MoveOverlay;
                }
            }
        }
        private void Window_MouseDown(object sender, MouseEventArgs e)
        {
            Point mouseClickPoint = e.GetPosition(GameGrid);
            Board.Square? square = getBoardSquareFromMouseClickPosition(mouseClickPoint);
            if (square != null && selectedPiece != null)
            {
                Move? move = selectedPiece.GetValidMoves(board, true).FirstOrDefault(x => x.NextSquare == square);
                if (move != null)
                {
                    // move the piece
                    board.ExecuteMove(move);
                    selectedPiece = null;
                    DrawGrid();
                } else
                {
                    if (square.Piece != null && square.Piece.Color == board.CurrentTurn)
                    {
                        // same logic as the outside else for the grandparent if here
                        selectedPiece = square.Piece;
                        HighlightValidMovesForPiece(square.Piece);
                    }
                }
            }
            else if (square != null && square.Piece != null && square.Piece.Color == board.CurrentTurn)
            {
                selectedPiece = square.Piece;
                HighlightValidMovesForPiece(square.Piece);
            }
        }

        private void Draw()
        {
            DrawGrid();
        }
        private void DrawGrid()
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (board.BoardArr[r, c].Piece != null)
                    {
                        IPiece piece = board.BoardArr[r, c].Piece!;
                        if (piece.Type == PieceType.King && board.IsKingInCheck(piece.Color))
                        {
                            pieceImages[r, c].Source = capturablePieceTypeToImage[PieceType.King];
                        } else
                        {
                            pieceImages[r, c].Source = pieceTypeToImage[piece.Type][piece.Color];
                        }
                    } else
                    {
                        pieceImages[r, c].Source = gridImages[r, c].Source;
                    }
                }
            }
        }

        private async Task GameLoop()
        {
            while (board.GetGameState() == GameState.Ongoing) // TODO should be not board.gameOver or something
            {
                await Task.Delay(100);
                if (board.GetGameState() == GameState.Ongoing && board.CurrentTurn == ChessSharp.Color.Black)
                {
                    Move? randomMove = board.GetRandomMoveForColor(ChessSharp.Color.Black);
                    if (randomMove != null)
                    {
                        board.ExecuteMove(randomMove);
                        Draw();
                    }
                    else
                    {
                        DebugTextBox.Text = "Could not find a move for black...";
                    }
                } else if (board.GetGameState() != GameState.Ongoing)
                {
                    string text = board.GetGameState() switch
                    {
                        GameState.Stalemate => "Stalemate",
                        GameState.Checkmate => $"{(board.CurrentTurn == ChessSharp.Color.White ? "Black" : "White")} wins by checkmate",
                        _ => "Invalid game state"
                    };
                    DebugTextBox.Text = text;
                    //Draw();
                }
                //Draw();
            }
            
        }



    }

    
}