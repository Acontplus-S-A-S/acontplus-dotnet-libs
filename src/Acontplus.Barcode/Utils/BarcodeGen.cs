using Acontplus.Barcode.Models;
using SkiaSharp;
using ZXing;
using ZXing.Common;
using ZXing.SkiaSharp.Rendering;

namespace Acontplus.Barcode.Utils;

public static class BarcodeGen
{
    public static byte[] Create(BarcodeConfig config)
    {
        if (string.IsNullOrEmpty(config.Text))
        {
            throw new ArgumentException("Text cannot be null or empty", nameof(config.Text));
        }

        // Merge default options with additional options
        var options = config.AdditionalOptions ?? new EncodingOptions();
        options.Width = config.Width;
        options.Height = config.Height;
        options.Margin = config.Margin;
        options.PureBarcode = !config.IncludeLabel;

        // Create a barcode writer
        var writer = new BarcodeWriter<SKBitmap>
        {
            Format = config.Format,
            Options = options,
            Renderer = new SKBitmapRenderer()
        };

        // Generate the barcode
        var barcodeBitmap = writer.Write(config.Text);

        // Encode the barcode image
        using var image = SKImage.FromBitmap(barcodeBitmap);
        using var data = image.Encode(config.OutputFormat, config.Quality);
        using var memoryStream = new MemoryStream();
        data.SaveTo(memoryStream);

        return memoryStream.ToArray();
    }
}
