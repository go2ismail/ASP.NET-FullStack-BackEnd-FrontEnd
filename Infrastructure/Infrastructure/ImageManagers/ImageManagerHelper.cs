// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

namespace Infrastructure.ImageManagers;

public static class ImageManagerHelper
{
    private static readonly Dictionary<string, string> MimeTypes = new()
    {
        { ".jpg", "image/jpeg" },
        { ".jpeg", "image/jpeg" },
        { ".png", "image/png" },
        { ".gif", "image/gif" },
        { ".bmp", "image/bmp" },
    };

    public static string GetMimeType(string extension)
    {
        if (string.IsNullOrEmpty(extension))
            throw new ImageManagerException($"Extension cannot be null or empty: {nameof(extension)}");

        extension = extension.ToLowerInvariant();

        return MimeTypes.TryGetValue(extension, out string? value)
            ? value : "application/octet-stream";
    }
}
