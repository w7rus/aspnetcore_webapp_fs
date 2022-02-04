using Common.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BLL.Services;

public interface IFileService
{
    Task Save(string fileName, byte[] data, CancellationToken cancellationToken = new());
    Task<byte[]> Read(string fileName, CancellationToken cancellationToken = new());
    void Delete(string fileName);
}

public class FileService : IFileService
{
    #region Fields

    private readonly ILogger<FileService> _logger;
    private readonly MiscOptions _miscOptions;

    #endregion

    #region Ctor

    public FileService(ILogger<FileService> logger, IOptions<MiscOptions> miscOptions)
    {
        _logger = logger;
        _miscOptions = miscOptions.Value;
    }

    #endregion

    #region Methods

    public async Task Save(string fileName, byte[] data, CancellationToken cancellationToken = new())
    {
        var dirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _miscOptions.ContentPath);
        var filePath = Path.Combine(dirPath, fileName);
        Directory.CreateDirectory(dirPath);
        var fs = new FileStream(filePath, FileMode.CreateNew);
        await fs.WriteAsync(data, cancellationToken);
        fs.Close();
    }

    public async Task<byte[]> Read(string fileName, CancellationToken cancellationToken = new())
    {
        var dirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _miscOptions.ContentPath);
        var filePath = Path.Combine(dirPath, fileName);
        Directory.CreateDirectory(dirPath);
        var ms = new MemoryStream();
        var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        await fs.CopyToAsync(ms, cancellationToken);
        fs.Close();
        return ms.ToArray();
    }

    public void Delete(string fileName)
    {
        var dirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _miscOptions.ContentPath);
        var filePath = Path.Combine(dirPath, fileName);
        Directory.CreateDirectory(dirPath);
        File.Delete(filePath);
    }

    #endregion
}