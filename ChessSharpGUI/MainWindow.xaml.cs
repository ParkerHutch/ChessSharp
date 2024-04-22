using System.Text;
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
        private class PieceKey : IEquatable<PieceKey>
        {
            public PieceType Type { get; }

            public ChessSharp.Color Color { get; }

            public PieceKey(PieceType type, ChessSharp.Color color)
            {
                Type = type;
                Color = color;
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as PieceKey);
            }

            public bool Equals(PieceKey other)
            {
                return other != null && Type == other.Type && Color == other.Color;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 23 + Type.GetHashCode();
                    hash = hash * 23 + Color.GetHashCode();
                    return hash;
                }
            }
        }

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

        //private readonly Dictionary<PieceKey, ImageSource> pieceTypeToImage = new()
        //{
        //    { new PieceKey(PieceType.King, ChessSharp.Color.White), Images.King },
        //    { new PieceKey(PieceType.King, ChessSharp.Color.Black), Images.King },
        //    { new PieceKey(PieceType.Pawn, ChessSharp.Color.White), Images.Pawn },
        //    { new PieceKey(PieceType.Pawn, ChessSharp.Color.Black), Images.King },
        //    { new PieceKey(PieceType.Queen, ChessSharp.Color.White), Images.Queen },
        //    { new PieceKey(PieceType.Queen, ChessSharp.Color.Black), Images.Queen },
        //    { new PieceKey(PieceType.Rook, ChessSharp.Color.White), Images.Rook },
        //    { new PieceKey(PieceType.Rook, ChessSharp.Color.Black), Images.Rook },
        //    { new PieceKey(PieceType.Bishop, ChessSharp.Color.White), Images.Bishop },
        //    { new PieceKey(PieceType.Bishop, ChessSharp.Color.Black), Images.Bishop },
        //    { new PieceKey(PieceType.Knight, ChessSharp.Color.White), Images.Knight },
        //    { new PieceKey(PieceType.Knight, ChessSharp.Color.Black), Images.Knight },
        //    { PieceType.Queen, Images.Queen },
        //    { PieceType.Rook, Images.Rook },
        //    { PieceType.Bishop, Images.Bishop },
        //    { PieceType.Pawn, Images.Pawn }
        //};

        private readonly int rows = 8, cols = 8;
        private readonly Image[,] pieceImages;
        private readonly Image[,] gridImages;
        private Board board;

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
                        Source = (r + c) % 2 == 0 ? Images.Black : Images.White
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
                        Source = (r + c) % 2 == 0 ? Images.Black : Images.White
                    };

                    images[r, c] = image;
                    OverlayGrid.Children.Add(image);
                }
            }
            return images;
        }

        //private Image[,] SetupPieceImages()
        //{
        //    Image[,] images = new Image[rows, cols];
        //    PieceGrid.Rows = rows;
        //    PieceGrid.Columns = cols;

        //    //for (int r = 0; r < rows; r++)
        //    //{
        //    //    for (int c = 0; c < cols; c++)
        //    //    {

        //    //        Image image = new Image
        //    //        {
        //    //            Source = (r + c) % 2 == 0 ? Images.Black : Images.White
        //    //        };

        //    //        images[r, c] = image;
        //    //        GameGrid.Children.Add(image);

        //    //    }
        //    //}
        //    return images;
        //}

        // see https://www.nbdtech.com/Blog/archive/2010/06/21/wpf-adorners-part-1-ndash-what-are-adorners.aspx
        //class FourBoxes : Adorner
        //{
        //    public FourBoxes(UIElement adornedElement) :
        //        base(adornedElement)
        //    {
        //    }

        //    protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        //    {
        //        drawingContext.DrawRectangle(Brushes.Red, null, new System.Windows.Rect(0, 0, 5, 5));
        //        drawingContext.DrawRectangle(Brushes.Red, null, new System.Windows.Rect(0, ActualHeight - 5, 5, 5));
        //        drawingContext.DrawRectangle(Brushes.Red, null, new System.Windows.Rect(ActualWidth - 5, 0, 5, 5));
        //        drawingContext.DrawRectangle(Brushes.Red, null, new System.Windows.Rect(ActualWidth - 5, ActualHeight - 5, 5, 5));
        //    }
        //}

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Draw();
            //AdornerLayer.GetAdornerLayer(GameGrid).Add(new FourBoxes(GameGrid));
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            DebugTextBox.Text = $"Key clicked: {e.Key}";
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
            foreach (Move move in piece.GetValidMoves(board))
            {
                pieceImages[move.NextSquare.Location.Row, move.NextSquare.Location.Col].Source = Images.MoveOverlay;
            }
        }
        private void Window_MouseDown(object sender, MouseEventArgs e)
        {
            Point mouseClickPoint = e.GetPosition(GameGrid);
            Board.Square? square = getBoardSquareFromMouseClickPosition(mouseClickPoint);
            if (square != null && square.Piece != null)
            {
                DebugTextBox.Text = $"Square clicked: {(square.Location.Row, square.Location.Col)}";
                HighlightValidMovesForPiece(square.Piece);
                //foreach (Move move in square.Piece.GetValidMoves(board))
                //{
                //    pieceImages[move.NextSquare.Location.Row, move.NextSquare.Location.Col].Source = Images.MoveOverlay;
                //}
                //pieceImages[square.Location.Row, square.Location.Col].Source = Images.MoveOverlay;
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
                        pieceImages[r, c].Source = pieceTypeToImage[piece.Type][piece.Color];
                    } else
                    {
                        pieceImages[r, c].Source = gridImages[r, c].Source;
                    }
                }
            }
        }



    }

    
}