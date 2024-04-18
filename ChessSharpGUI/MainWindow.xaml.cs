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

namespace ChessSharpGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Dictionary<PieceType, ImageSource> pieceTypeToImage = new()
        {
            { PieceType.King, Images.King },
            { PieceType.Queen, Images.Queen },
            { PieceType.Rook, Images.Rook },
            { PieceType.Bishop, Images.Bishop },
            { PieceType.Pawn, Images.Pawn }
        };

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

        private void Window_MouseDown(object sender, MouseEventArgs e)
        {
            DebugTextBox.Text = $"Mouse clicked: {(e.GetPosition(GameGrid))}";
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
                        PieceType pieceType = board.BoardArr[r, c].Piece!.Type;
                        pieceImages[r, c].Source = pieceTypeToImage[pieceType];
                    } else
                    {
                        pieceImages[r, c].Source = gridImages[r, c].Source;
                    }
                }
            }
        }



    }

    
}