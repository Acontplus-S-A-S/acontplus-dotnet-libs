namespace Acontplus.Utilities.Text;

/// <summary>
/// Provides text manipulation utilities for string formatting and splitting.
/// </summary>
public static class TextHandlers
{
    /// <summary>
    /// Splits a string into lines of a specified length, inserting a newline (\\n) after each segment.
    /// </summary>
    /// <param name="input">The input string to split.</param>
    /// <param name="length">The maximum length of each line segment.</param>
    /// <returns>The formatted string with line breaks.</returns>
    public static string StringSplit(string input, int length)
    {
        var sb = new StringBuilder();

        for (var i = 0; i < input.Length; i++)
        {
            sb.Append(input[i]);
            if ((i + 1) % length == 0)
            {
                sb.Append("\\n");
            }
        }

        var output = sb.ToString();
        return output;
    }
}
