using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace BetaSharp;

public class ScreenShotHelper
{
    public static string saveScreenshot(string gameDir, int width, int height)
    {
        return "Screenshots are not supported";
    }

    public static string saveScreenshot(string gameDir, int width, int height, byte[] rgbPixels)
    {
        if (rgbPixels == null || rgbPixels.Length < width * height * 3)
            return "Failed to save: invalid pixel data";
        if (string.IsNullOrEmpty(gameDir))
            return "Failed to save: invalid game directory";

        try
        {
            string screenshotsPath = System.IO.Path.Combine(gameDir, "screenshots");
            System.IO.Directory.CreateDirectory(screenshotsPath);

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss");
            int suffix = 1;
            string fileName;
            string fullPath;
            do
            {
                fileName = suffix == 1 ? timestamp + ".png" : timestamp + "_" + suffix + ".png";
                fullPath = System.IO.Path.Combine(screenshotsPath, fileName);
                suffix++;
            } while (System.IO.File.Exists(fullPath));

            // OpenGL returns rows bottom-to-top; flip to top-to-bottom for the image file.
            int rowStride = width * 3;
            byte[] flipped = new byte[rgbPixels.Length];
            for (int y = 0; y < height; y++)
            {
                int srcRow = height - 1 - y;
                int srcOffset = srcRow * rowStride;
                int dstOffset = y * rowStride;
                Buffer.BlockCopy(rgbPixels, srcOffset, flipped, dstOffset, rowStride);
            }

            using (Image<Rgb24> image = Image.LoadPixelData<Rgb24>(flipped, width, height))
            {
                image.SaveAsPng(fullPath);
            }

            return "Saved screenshot as " + fileName;
        }
        catch (Exception ex)
        {
            return "Failed to save: " + ex.Message;
        }
    }
}