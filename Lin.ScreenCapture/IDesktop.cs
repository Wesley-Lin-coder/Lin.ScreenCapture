using System.Drawing;
using SkiaSharp;

namespace Lin.ScreenCapture
{
    public interface IDesktop : IDisposable
    {
        public const double DefaultScale = 1d;
        int Width { get; }
        int Height { get; }
        double Scale { get; }
        Size Size { get; }
        SKBitmap GetSKBitmap();
        void ScaleSize(double scale);
    }
}
