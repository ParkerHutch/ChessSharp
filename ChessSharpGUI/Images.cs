using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChessSharpGUI
{
    public static class Images
    {
        public readonly static ImageSource Empty = LoadImage("Empty.png");
        public readonly static ImageSource King = LoadImage("king_light.png");
        public readonly static ImageSource Queen = LoadImage("queen_light.png");
        public readonly static ImageSource Rook = LoadImage("rook_light.png");
        public readonly static ImageSource Knight = LoadImage("knight_light.png");
        public readonly static ImageSource Bishop = LoadImage("bishop_light.png");
        public readonly static ImageSource Pawn = LoadImage("pawn_light.png");
        public readonly static ImageSource Black = LoadImage("solid_black_wikimedia_commons.png");
        public readonly static ImageSource White = LoadImage("solid_white_wikimedia_commons.png");
        private static ImageSource LoadImage(string filename)
        {
            return new BitmapImage(new Uri($"Assets/{filename}", UriKind.Relative));
        }
    }
}
