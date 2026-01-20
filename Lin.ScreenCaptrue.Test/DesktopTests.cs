using System.Runtime.Versioning;
using Lin.ScreenCapture;

namespace Lin.ScreenCaptrue.Test
{
    [TestClass]
    public sealed class DesktopTests
    {
        [TestMethod]
        [SupportedOSPlatform("windows")]
        public void Constructor_ShouldInitializeCorrectly()
        {
            using var desktop = new Desktop();
            Assert.IsTrue(desktop.Width > 0);
            Assert.IsTrue(desktop.Height > 0);
            Assert.AreEqual(1, desktop.Scale);
        }

        [TestMethod]
        [SupportedOSPlatform("windows")]
        public void Dispose_ShouldReleaseResourcesCorrectly()
        {
            using var desktop = new Desktop();
            desktop.Dispose();
            // No exception should be thrown during disposal
        }

        [TestMethod]
        [SupportedOSPlatform("windows")]
        public void GetSKBitmap_ShouldReturnResizedBitmap()
        {
            using var desktop = new Desktop();
            desktop.ScaleSize(0.5);
            using var bitmap = desktop.GetSKBitmap();

            Assert.IsNotNull(bitmap);
            Assert.AreEqual((int)(desktop.Width * 0.5), bitmap.Width);
            Assert.AreEqual((int)(desktop.Height * 0.5), bitmap.Height);
        }

        [TestMethod]
        [SupportedOSPlatform("windows")]
        public void ScaleSize_ShouldUpdateScaleCorrectly()
        {
            using var desktop = new Desktop();
            desktop.ScaleSize(2);
            Assert.AreEqual(2, desktop.Scale);
        }
    }
}
