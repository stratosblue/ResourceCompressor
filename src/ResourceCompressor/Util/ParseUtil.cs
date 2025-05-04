namespace ResourceCompressor.Util;

public static class ParseUtil
{
    #region Public 方法

    public static T ParseEnumValue<T>(string value, T defaultValue = default) where T : struct
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return defaultValue;
        }
        if (!Enum.TryParse<T>(value, true, out var enumValue))
        {
            throw new InvalidOperationException($"Unsupported {typeof(T).Name} value - \"{value}\"");
        }

        return enumValue;
    }

    #endregion Public 方法
}
