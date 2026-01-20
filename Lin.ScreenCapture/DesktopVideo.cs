using System.Diagnostics;
using System.Drawing;
using SkiaSharp;

namespace Lin.ScreenCapture
{
    public class DesktopVideo : IDisposable
    {
        public int Width => Desktop.Width;
        public int Height => Desktop.Height;
        public Size Size => Desktop.Size;
        public double Scale => Desktop.Scale;
        public IDesktop Desktop { get; init; }

        public DesktopVideo(IDesktop desktop)
        {
            Desktop = desktop ?? throw new ArgumentNullException(nameof(desktop));
        }

        public void ScaleSize(double scale) => Desktop.ScaleSize(scale);

        public IEnumerable<SKBitmap> GetBitmaps(int fps, CancellationToken cancellationToken = default)
        {
            if (fps <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(fps), "fps 必须大于 0。");
            }

            double frameIntervalMs = 1000d / fps;
            var stopwatch = new Stopwatch();

            while (!cancellationToken.IsCancellationRequested)
            {
                stopwatch.Restart();
                // 获取当前屏幕截图（由调用方负责释放）
                yield return Desktop.GetSKBitmap();

                double elapsedMs = stopwatch.Elapsed.TotalMilliseconds;
                int delayMs = (int)Math.Max(0, frameIntervalMs - elapsedMs);
                if (delayMs > 0 && cancellationToken.WaitHandle.WaitOne(delayMs))
                {
                    yield break;
                }
            }
        }

        public void Dispose()
        {
            Desktop.Dispose();
        }
    }
}
