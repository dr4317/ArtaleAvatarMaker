using System.Drawing.Drawing2D;
using Manina.Windows.Forms;

namespace MapleNecrocer;

internal static class DpiScalingHelper
{
    public static void Apply(Control root)
    {
        var scale = GetScaleFactor(root);
        Apply(root, scale);
    }

    public static void Apply(Control root, float scale)
    {
        ApplyRecursive(root, scale);
    }

    public static int Scale(int value, float scale)
    {
        return Math.Max(1, (int)Math.Round(value * scale));
    }

    public static Size Scale(Size size, float scale)
    {
        return new Size(Scale(size.Width, scale), Scale(size.Height, scale));
    }

    private static void ApplyRecursive(Control control, float scale)
    {
        if (control is Button button && button.Image != null)
        {
            button.Image = ScaleImage(button.Image, scale);
        }
        else if (control is PictureBox pictureBox && pictureBox.SizeMode == PictureBoxSizeMode.CenterImage)
        {
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
        }
        else if (control is ImageListView imageListView)
        {
            imageListView.ThumbnailSize = Scale(imageListView.ThumbnailSize, scale);
        }

        foreach (Control child in control.Controls)
        {
            ApplyRecursive(child, scale);
        }
    }

    private static float GetScaleFactor(Control control)
    {
        if (control.IsHandleCreated)
        {
            return control.DeviceDpi / 96f;
        }

        using var graphics = control.CreateGraphics();
        return graphics.DpiX / 96f;
    }

    private static Image ScaleImage(Image image, float scale)
    {
        var scaledWidth = Scale(image.Width, scale);
        var scaledHeight = Scale(image.Height, scale);
        var scaledImage = new Bitmap(scaledWidth, scaledHeight);
        scaledImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

        using var graphics = Graphics.FromImage(scaledImage);
        graphics.CompositingMode = CompositingMode.SourceCopy;
        graphics.CompositingQuality = CompositingQuality.HighQuality;
        graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        graphics.SmoothingMode = SmoothingMode.None;
        graphics.DrawImage(image, 0, 0, scaledWidth, scaledHeight);

        return scaledImage;
    }
}