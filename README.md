# Lin.ScreenCapture

Cross-platform screen capture library for .NET using SkiaSharp.

## Features
- Capture full desktop into SKBitmap
- Optional scaling
- Frame streaming via DesktopVideo at a target FPS
- Windows and Linux implementations

## Projects
- Lin.ScreenCapture: core abstractions (IDesktop, DesktopVideo)
- Lin.ScreenCapture.Windows: Windows implementation (GDI/Vanara)
- Lin.ScreenCapture.Linux: Linux implementation (X11)
- Lin.ScreenCaptrue.Test: MSTest project

## Target frameworks
- net6.0, net8.0, net9.0, net10.0

## Dependencies
- Core: SkiaSharp
- Windows: SkiaSharp.NativeAssets.Win32, Vanara.PInvoke.User32, System.Drawing.Common
- Linux: SkiaSharp.NativeAssets.Linux, X11

## Usage

### Windows
```csharp
using Lin.ScreenCapture;

using var desktop = new Desktop();
desktop.ScaleSize(0.5);

using var bitmap = desktop.GetSKBitmap();
// TODO: encode or save bitmap
```

### Linux
```csharp
using Lin.ScreenCapture;

using var desktop = new Desktop();
using var bitmap = desktop.GetSKBitmap();
```

### Video stream
```csharp
using Lin.ScreenCapture;

using var desktop = new Desktop();
using var video = new DesktopVideo(desktop);

using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
foreach (var frame in video.GetBitmaps(5, cts.Token))
{
    // process frame
    frame.Dispose();
}
```

## Build
```bash
dotnet build Lin.ScreenCapture.sln
```

## Test
```bash
dotnet test Lin.ScreenCaptrue.Test/Lin.ScreenCaptrue.Test.csproj
```

## Notes
- GetSKBitmap returns a new SKBitmap instance; the caller must dispose it.
- Linux implementation uses X11; ensure an X server is available.

## License
See LICENSE.txt
