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

namespace ChessSharpGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly int rows = 8, cols = 8;
        private readonly Image[,] gridImages;

        public MainWindow()
        {
            InitializeComponent();
            gridImages = SetupGrid();
        }

        private Image[,] SetupGrid()
        {
            Image[,] images = new Image[rows, cols];
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;

            //using (Graphics g = Graphics.FromImage(bitmap))
            //{
            //    for (int r = 0; r < rows; r++)
            //    {
            //        for (int c = 0; c < cols; c++)
            //        {
            //            Color color = ((r + c) % 2 == 0) ? Color.Black : Color.White;
            //            Brush brush = new SolidBrush(color);
            //            g.FillRectangle(brush, c * squareSize, r * squareSize, squareSize, squareSize);
            //        }
            //    }
            //}

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {

                    Image image = new Image
                    {
                        Source = (r + c) % 2 == 0 ? Images.Black : Images.White
                    };


                    //System.Drawing.Size squareSize = new System.Drawing.Size(100, 100);
                    //Bitmap squareImage = new Bitmap(squareSize.Width, squareSize.Height);
                    //using (Graphics graphics = Graphics.FromImage(squareImage))
                    //{
                    //    graphics.FillRectangle(System.Drawing.Brushes.White, 0, 0, squareSize.Width, squareSize.Height);
                    //    graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    //    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    //    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                    //    graphics.DrawImage(Images.Empty, (squareSize.Width / 2) - (Images.Empty.Width / 2), (squareSize.Height / 2) - (Images.Empty.Height / 2), Images.Empty.Width, Images.Empty.Height);
                    //}

                    images[r, c] = image;
                    GameGrid.Children.Add(image);

                }
            }
            return images;
        }

    }
}