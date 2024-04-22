using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChessSharpGUI
{
    public static class Images
    {
        public readonly static ImageSource Empty = LoadImage("Empty.png");
        public readonly static ImageSource King_Light = LoadImage("king_light.png");
        public readonly static ImageSource King_Dark = LoadImage("king_dark.png");
        public readonly static ImageSource Queen_Light = LoadImage("queen_light.png");
        public readonly static ImageSource Queen_Dark = LoadImage("queen_dark.png");
        public readonly static ImageSource Rook_Light = LoadImage("rook_light.png");
        public readonly static ImageSource Rook_Dark = LoadImage("rook_dark.png");
        public readonly static ImageSource Knight_Light = LoadImage("knight_light.png");
        public readonly static ImageSource Knight_Dark = LoadImage("knight_dark.png");
        public readonly static ImageSource Bishop_Light = LoadImage("bishop_light.png");
        public readonly static ImageSource Bishop_Dark = LoadImage("bishop_dark.png");
        public readonly static ImageSource Pawn_Light = LoadImage("pawn_light.png");
        public readonly static ImageSource Pawn_Dark = LoadImage("pawn_dark.png");
        public readonly static ImageSource Black = LoadImage("solid_black_wikimedia_commons.png");
        public readonly static ImageSource White = LoadImage("solid_white_wikimedia_commons.png");
        public readonly static ImageSource MoveOverlay = LoadImage("pink_circle.png");
        private static ImageSource LoadImage(string filename)
        {
            return new BitmapImage(new Uri($"Assets/{filename}", UriKind.Relative));
        }
    }
}
