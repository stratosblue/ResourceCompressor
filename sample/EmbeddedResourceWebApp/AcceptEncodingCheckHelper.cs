using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Primitives;

public static partial class AcceptEncodingCheckHelper
{
    private static readonly Regex s_gzipAcceptedCheckRegex = GetGZipAcceptedCheckRegex();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGZipAccepted(string? acceptEncoding) => !string.IsNullOrWhiteSpace(acceptEncoding) && s_gzipAcceptedCheckRegex.IsMatch(acceptEncoding);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGZipAccepted(in StringValues acceptEncodingValues)
    {
        return acceptEncodingValues.Count switch
        {
            0 => false,
            1 => IsGZipAccepted(acceptEncodingValues[0]),
            _ => SlowCheckIsGZipAccepted(in acceptEncodingValues),
        };
    }

    [GeneratedRegex(@"(^|,)\s*gzip\s*(;|,|$)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex GetGZipAcceptedCheckRegex();

    private static bool SlowCheckIsGZipAccepted(in StringValues values)
    {
        var valuesCount = values.Count;
        for (var i = 0; i < valuesCount; i++)
        {
            if (IsGZipAccepted(values[i]))
            {
                return true;
            }
        }
        return false;
    }
}
