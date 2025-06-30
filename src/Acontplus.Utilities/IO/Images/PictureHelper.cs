namespace Acontplus.Utilities.IO.Images;

public static class PictureHelper
{
    // some magic bytes for the most important image formats, see Wikipedia for more
    private static readonly List<byte> jpg = new() { 0xFF, 0xD8 };
    private static readonly List<byte> bmp = new() { 0x42, 0x4D };
    private static readonly List<byte> gif = new() { 0x47, 0x49, 0x46 };

    private static readonly List<byte> png = new()
    {
        0x89,
        0x50,
        0x4E,
        0x47,
        0x0D,
        0x0A,
        0x1A,
        0x0A
    };

    private static readonly List<byte> svg_xml_small = new()
    {
        0x3C,
        0x3F,
        0x78,
        0x6D,
        0x6C
    }; // "<?xml"

    private static readonly List<byte> svg_xml_capital = new()
    {
        0x3C,
        0x3F,
        0x58,
        0x4D,
        0x4C
    }; // "<?XML"

    private static readonly List<byte> svg_small = new() { 0x3C, 0x73, 0x76, 0x67 }; // "<svg"
    private static readonly List<byte> svg_capital = new() { 0x3C, 0x53, 0x56, 0x47 }; // "<SVG"
    private static readonly List<byte> intel_tiff = new() { 0x49, 0x49, 0x2A, 0x00 };
    private static readonly List<byte> motorola_tiff = new() { 0x4D, 0x4D, 0x00, 0x2A };

    private static readonly List<(List<byte> magic, string extension)> imageFormats = new()
    {
        (jpg, "jpg"),
        (bmp, "bmp"),
        (gif, "gif"),
        (png, "png"),
        (svg_small, "svg"),
        (svg_capital, "svg"),
        (intel_tiff, "tif"),
        (motorola_tiff, "tif"),
        (svg_xml_small, "svg"),
        (svg_xml_capital, "svg")
    };

    public static string TryGetExtension(byte[] array)
    {
        // check for simple formats first
        foreach (var imageFormat in imageFormats)
        {
            if (array.IsImage(imageFormat.magic))
            {
                if (imageFormat.magic != svg_xml_small && imageFormat.magic != svg_xml_capital)
                {
                    return imageFormat.extension;
                }

                // special handling for SVGs starting with XML tag
                var readCount = imageFormat.magic.Count; // skip XML tag
                var maxReadCount = 1024;

                do
                {
                    if (array.IsImage(svg_small, readCount) || array.IsImage(svg_capital, readCount))
                    {
                        return imageFormat.extension;
                    }

                    readCount++;
                } while (readCount < maxReadCount && readCount < array.Length - 1);

                return null;
            }
        }

        return null;
    }

    private static bool IsImage(this byte[] array, List<byte> comparer, int offset = 0)
    {
        var arrayIndex = offset;
        foreach (var c in comparer)
        {
            if (arrayIndex > array.Length - 1 || array[arrayIndex] != c)
            {
                return false;
            }

            ++arrayIndex;
        }

        return true;
    }
}
