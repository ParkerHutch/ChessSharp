using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChessSharpGUI
{
    public static class Images
    {
        public readonly static ImageSource Empty = LoadImage("king_wikimedia_commons.png");
        public readonly static ImageSource King = LoadImage("king_wikimedia_commons.png");
        private static ImageSource LoadImage(string filename)
        {
            return new BitmapImage(new Uri($"Assets/{filename}", UriKind.Relative));
        }
    }
}
