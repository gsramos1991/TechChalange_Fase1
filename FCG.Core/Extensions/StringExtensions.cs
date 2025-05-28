using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCG.Core.Extensions;
public static class StringExtensions
{
    public static bool IsValidGuid(this string input)
    {
        return IsValidGuid(input, "D");
    }

    public static bool IsValidGuid(this string input, string format)
    {
        return Guid.TryParseExact(input, format, out _);
    }
}

