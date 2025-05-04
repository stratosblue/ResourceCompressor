namespace ResourceCompressor.Util;

public static class DirectoryUtil
{
    #region Public 方法

    public static void EnsureDirectory(string directory)
    {
        if (!Directory.Exists(directory))
        {
            try
            {
                Directory.CreateDirectory(directory);
            }
            catch
            {
                if (!Directory.Exists(directory))
                {
                    throw;
                }
            }
        }
    }

    #endregion Public 方法
}
