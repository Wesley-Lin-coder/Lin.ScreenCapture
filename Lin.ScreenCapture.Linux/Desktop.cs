using System.Drawing;
using System.Runtime.InteropServices;
using SkiaSharp;
using X11;

namespace Lin.ScreenCapture
{
    public class Desktop : IDesktop
    {
        private SafeDisplay Display = SafeDisplay.Null;
        private Window RootWindow;

        public int Width { get; }
        public int Height { get; }
        public double Scale { get; private set; } = IDesktop.DefaultScale;
        public Size Size => new Size(Width, Height);

        public Desktop()
        {
            Display = new SafeDisplay();
            if (Display.IsInvalid)
            {
                throw new X11Exception("无法打开显示器");
            }
            int screen = Xlib.XDefaultScreen(Display.DangerousGetHandle());
            RootWindow = Xlib.XRootWindow(Display.DangerousGetHandle(), screen);
            XWindowAttributes windowAttributes;
            Xlib.XGetWindowAttributes(Display.DangerousGetHandle(), RootWindow, out windowAttributes);
            Width = (int)windowAttributes.width;
            Height = (int)windowAttributes.height;
        }

        public SKBitmap GetSKBitmap()
        {
            // 获取屏幕图像
            XImage image = Xlib.XGetImage(
                Display.DangerousGetHandle(),
                RootWindow,
                0,
                0,
                (uint)Width,
                (uint)Height,
                0,
                PixmapFormat.ZPixmap
            );
            if (image.data == IntPtr.Zero)
            {
                throw new InvalidOperationException("获取屏幕图像失败。");
            }

            var bitmap = new SKBitmap(Width, Height, SKColorType.Bgra8888, SKAlphaType.Premul);
            try
            {
                IntPtr pixels = bitmap.GetPixels();
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        int pixel = Marshal.ReadInt32(image.data, y * image.bytes_per_line + x * 4);
                        Marshal.WriteInt32(pixels, (y * Width + x) * 4, pixel);
                    }
                }

                var (scaleWidth, scaleHeight) = GetScaledSize();
                if (scaleWidth == Width && scaleHeight == Height)
                {
                    return bitmap;
                }

                var resizedBitmap = bitmap.Resize(new SKSizeI(scaleWidth, scaleHeight), SKSamplingOptions.Default);
                if (resizedBitmap is null)
                {
                    throw new InvalidOperationException("缩放失败。");
                }

                bitmap.Dispose();
                return resizedBitmap;
            }
            catch
            {
                bitmap.Dispose();
                throw;
            }
        }

        public void ScaleSize(double scale)
        {
            if (double.IsNaN(scale) || double.IsInfinity(scale) || scale <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(scale), "scale 必须为大于 0 的有限数字。");
            }

            Scale = scale;
        }

        public void Dispose()
        {
            Display.Dispose();
        }

        private (int width, int height) GetScaledSize()
        {
            int scaleWidth = Math.Max(1, (int)(Width * Scale));
            int scaleHeight = Math.Max(1, (int)(Height * Scale));
            return (scaleWidth, scaleHeight);
        }

        private class X11Exception : Exception
        {
            public X11Exception(string? message)
                : base(message) { }
        }

        private class SafeDisplay : SafeHandle
        {
            private SafeDisplay(nint invalidHandleValue, bool ownsHandle)
                : base(invalidHandleValue, ownsHandle) { }

            public SafeDisplay()
                : this(default, true)
            {
                handle = Xlib.XOpenDisplay(null);
            }

            public static readonly SafeDisplay Null = new(default, false) { handle = IntPtr.Zero };
            public override bool IsInvalid => handle == IntPtr.Zero;

            protected override bool ReleaseHandle()
            {
                try
                {
                    Xlib.XCloseDisplay(handle);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
