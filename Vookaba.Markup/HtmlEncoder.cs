using System;
using System.Text;

namespace Vookaba.Markup
{
    //based on WebUtility.HtmlEncode
    //https://github.com/dotnet/runtime/blob/c5c7967ddccc46c84c98c0e8c7e00c3009c65894/src/libraries/System.Private.CoreLib/src/System/Net/WebUtility.cs
    internal static class HtmlEncoder
    {

        private const int MaxInt32Digits = 10;
        private const int UNICODE_PLANE01_START = 0x10000;

        private static int GetNextUnicodeScalarValueFromUtf16Surrogate(ReadOnlySpan<char> input, ref int index)
        {
            if (input.Length - index <= 1) { return 65533; }
            char c = input[index];
            char c2 = input[index + 1];
            if (!char.IsSurrogatePair(c, c2)) { return 65533; }
            index++;
            return (c - 55296) * 1024 + (c2 - 56320) + 65536;
        }

        internal static void AppendEncoded(ReadOnlySpan<char> input, StringBuilder output)
        {
            for (int i = 0; i < input.Length; i++)
            {
                char ch = input[i];
                if (ch <= '>')
                {
                    switch (ch)
                    {
                        case '<':
                            output.Append("&lt;");
                            break;
                        case '>':
                            output.Append("&gt;");
                            break;
                        case '"':
                            output.Append("&quot;");
                            break;
                        case '\'':
                            output.Append("&#39;");
                            break;
                        case '&':
                            output.Append("&amp;");
                            break;
                        case '\n':
                            output.Append("<br>");
                            break;
                        case '\r':
                            break;
                        default:
                            output.Append(ch);
                            break;
                    }
                }
                else
                {
                    int valueToEncode = -1; // set to >= 0 if needs to be encoded

                    if (ch >= 160 && ch < 256)
                    {
                        // The seemingly arbitrary 160 comes from RFC
                        valueToEncode = ch;
                    }
                    else
                        if (char.IsSurrogate(ch))
                    {
                        int scalarValue = GetNextUnicodeScalarValueFromUtf16Surrogate(input, ref i);
                        if (scalarValue >= UNICODE_PLANE01_START)
                        {
                            valueToEncode = scalarValue;
                        }
                        else
                        {
                            // Don't encode BMP characters (like U+FFFD) since they wouldn't have
                            // been encoded if explicitly present in the string anyway.
                            ch = (char)scalarValue;
                        }
                    }

                    if (valueToEncode >= 0)
                    {
                        // value needs to be encoded
                        output.Append("&#");

                        // Use the buffer directly and reserve a conservative estimate of 10 chars.
#pragma warning disable CA2014 // Do not use stackalloc in loops
                        Span<char> encodingBuffer = stackalloc char[MaxInt32Digits];
#pragma warning restore CA2014 // Do not use stackalloc in loops
                        valueToEncode.TryFormat(encodingBuffer, out int charsWritten); // Invariant
                        output.Append(encodingBuffer[..charsWritten]);
                        output.Append(';');
                    }
                    else
                    {
                        // write out the character directly
                        output.Append(ch);
                    }
                }
            }
        }
    }
}
