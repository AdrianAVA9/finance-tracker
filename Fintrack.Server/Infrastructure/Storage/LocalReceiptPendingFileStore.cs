using Fintrack.Server.Application.Abstractions.Storage;
using Microsoft.AspNetCore.Hosting;

namespace Fintrack.Server.Infrastructure.Storage;

internal sealed class LocalReceiptPendingFileStore : IReceiptPendingFileStore
{
    private readonly string _root;

    public LocalReceiptPendingFileStore(IWebHostEnvironment environment)
    {
        _root = Path.Combine(environment.ContentRootPath, "App_Data", "pending-receipts");
        Directory.CreateDirectory(_root);
    }

    public async Task<string> SaveAsync(
        Stream content,
        string originalExtension,
        CancellationToken cancellationToken = default)
    {
        var ext = string.IsNullOrWhiteSpace(originalExtension)
            ? ".bin"
            : originalExtension.StartsWith('.')
                ? originalExtension
                : "." + originalExtension;

        var name = $"{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(_root, name);
        await using (var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await content.CopyToAsync(fs, cancellationToken);
        }

        return name;
    }

    public Stream OpenRead(string storageRelativePath)
    {
        var path = Path.Combine(_root, storageRelativePath);
        return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    public void TryDelete(string storageRelativePath)
    {
        try
        {
            var path = Path.Combine(_root, storageRelativePath);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch
        {
            // best-effort cleanup
        }
    }
}
