using SkiaSharp;
using ZXing;
using ZXing.Common;

namespace Acontplus.Barcode.Models;

public class BarcodeConfig
{
    public string Text { get; set; } = string.Empty; // The text to encode
    public BarcodeFormat Format { get; set; } = BarcodeFormat.CODE_128; // Barcode format
    public int Width { get; set; } = 300; // Barcode width
    public int Height { get; set; } = 100; // Barcode height
    public bool IncludeLabel { get; set; } = false; // Include label below the barcode
    public int Margin { get; set; } = 10; // Margin around the barcode
    public SKEncodedImageFormat OutputFormat { get; set; } = SKEncodedImageFormat.Png; // Output image format
    public int Quality { get; set; } = 100; // Image quality (for formats like JPEG)
    public EncodingOptions? AdditionalOptions { get; set; } // Custom encoding options
}
