using Microsoft.Build.Framework;

using ResourceCompressor.Util;

namespace ResourceCompressor.Extensions;

public static class ITaskItemExtensions
{
    #region Public 方法

    public static T GetTypedMetadata<T>(this ITaskItem taskItem, string metadataName, T defaultValue = default) where T : struct
    {
        var metadataValue = taskItem.GetMetadata(metadataName);

        if (typeof(T).IsEnum)
        {
            return ParseUtil.ParseEnumValue<T>(metadataValue, defaultValue);
        }

        if (string.IsNullOrWhiteSpace(metadataValue))
        {
            return defaultValue;
        }
        return (T)Convert.ChangeType(metadataValue, typeof(T));
    }

    #endregion Public 方法
}
