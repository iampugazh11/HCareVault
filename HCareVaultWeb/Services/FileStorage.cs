using Microsoft.AspNetCore.Components.Forms;

namespace HCareVault.Services;

/// <summary>A file picked in the browser and held in memory until the form is saved.</summary>
public sealed record PendingUpload(string FileName, string ContentType, byte[] Data)
{
    public bool IsImage => ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
    public string DataUrl => $"data:{ContentType};base64,{Convert.ToBase64String(Data)}";
}

/// <summary>Stores uploaded images / reports in the "uploads" folder next to the database.</summary>
public class FileStorage
{
    public const long MaxImageBytes = 5 * 1024 * 1024;
    public const long MaxAttachmentBytes = 15 * 1024 * 1024;

    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp", ".pdf"
    };

    private readonly string _root;

    public FileStorage(IWebHostEnvironment env)
    {
        _root = Path.Combine(env.ContentRootPath, "uploads");
        Directory.CreateDirectory(_root);
    }

    public static string Url(string storedName) => $"/files/{storedName}";

    public static bool IsAllowed(string fileName) => AllowedExtensions.Contains(Path.GetExtension(fileName));

    public static async Task<PendingUpload> ReadAsync(IBrowserFile file, long maxBytes)
    {
        await using var stream = file.OpenReadStream(maxBytes);
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        return new PendingUpload(file.Name, file.ContentType, ms.ToArray());
    }

    public async Task<string> SaveAsync(PendingUpload upload)
    {
        var ext = Path.GetExtension(upload.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext)) ext = upload.IsImage ? ".jpg" : ".bin";
        var storedName = $"{Guid.NewGuid():N}{ext}";
        await File.WriteAllBytesAsync(Path.Combine(_root, storedName), upload.Data);
        return storedName;
    }

    /// <summary>Returns the full path of a stored file, or null if the name is invalid or missing.</summary>
    public string? GetPath(string storedName)
    {
        if (string.IsNullOrWhiteSpace(storedName) || storedName != Path.GetFileName(storedName)) return null;
        var path = Path.Combine(_root, storedName);
        return File.Exists(path) ? path : null;
    }

    public void Delete(string? storedName)
    {
        if (storedName is null) return;
        var path = GetPath(storedName);
        if (path is not null)
        {
            try { File.Delete(path); } catch (IOException) { /* best effort */ }
        }
    }

    public static string FormatSize(long bytes) =>
        bytes >= 1024 * 1024 ? $"{bytes / 1024d / 1024d:0.#} MB" : $"{Math.Max(1, bytes / 1024)} KB";
}
