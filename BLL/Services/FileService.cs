using Common.Models;
using Common.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BLL.Services;

public interface IFileService
{
    /// <summary>
    /// Saves file data with given filename
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="data"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task Save(string fileName, byte[] data, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets file data with given filename
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<byte[]> Read(string fileName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes file with given filename
    /// </summary>
    /// <param name="fileName"></param>
    void Delete(string fileName);
}

public class FileService : IFileService
{
    #region Fields

    private readonly string _fullName;
    private readonly ILogger<FileService> _logger;
    private readonly MiscOptions _miscOptions;

    #endregion

    #region Ctor

    public FileService(ILogger<FileService> logger, IOptions<MiscOptions> miscOptions)
    {
        _fullName = GetType().FullName;
        _logger = logger;
        _miscOptions = miscOptions.Value;
    }

    #endregion

    #region Methods

    public async Task Save(string fileName, byte[] data, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(_fullName, nameof(Save)));

        var dirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _miscOptions.ContentPath);
        var filePath = Path.Combine(dirPath, fileName);
        Directory.CreateDirectory(dirPath);
        var fs = new FileStream(filePath, FileMode.CreateNew);
        await fs.WriteAsync(data, cancellationToken);
        fs.Close();

        _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(_fullName, nameof(Save)));
    }

    public async Task<byte[]> Read(string fileName, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(_fullName, nameof(Read)));

        var dirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _miscOptions.ContentPath);
        var filePath = Path.Combine(dirPath, fileName);
        Directory.CreateDirectory(dirPath);
        var ms = new MemoryStream();
        var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        await fs.CopyToAsync(ms, cancellationToken);
        fs.Close();

        _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(_fullName, nameof(Read)));

        return ms.ToArray();
    }

    public void Delete(string fileName)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(_fullName, nameof(Delete)));

        var dirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _miscOptions.ContentPath);
        var filePath = Path.Combine(dirPath, fileName);
        Directory.CreateDirectory(dirPath);
        File.Delete(filePath);

        _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(_fullName, nameof(Delete)));
    }

    #endregion
}