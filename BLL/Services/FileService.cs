using Common.Models;
using Common.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BLL.Services;

public interface IFileService
{
    Task Save(string fileName, Stream stream, CancellationToken cancellationToken = default);
    FileStream Read(string fileName, CancellationToken cancellationToken = default);
    void Delete(string fileName);
}

public class FileService : IFileService
{
    #region Ctor

    public FileService(ILogger<FileService> logger, IOptions<MiscOptions> miscOptions)
    {
        _fullName = GetType().FullName;
        _logger = logger;
        _miscOptions = miscOptions.Value;
    }

    #endregion

    #region Fields

    private readonly string _fullName;
    private readonly ILogger<FileService> _logger;
    private readonly MiscOptions _miscOptions;

    #endregion

    #region Methods

    public async Task Save(string fileName, Stream stream, CancellationToken cancellationToken = default)
    {
        var dirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _miscOptions.ContentPath);
        var filePath = Path.Combine(dirPath, fileName);
        Directory.CreateDirectory(dirPath);
        var fileStream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write);
        await stream.CopyToAsync(fileStream, cancellationToken);
        await fileStream.FlushAsync(cancellationToken);
        fileStream.Close();
    }

    public FileStream Read(string fileName, CancellationToken cancellationToken)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(_fullName, nameof(Read)));

        var dirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _miscOptions.ContentPath);
        var filePath = Path.Combine(dirPath, fileName);
        Directory.CreateDirectory(dirPath);
        var fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(_fullName, nameof(Read)));

        return fileStream;
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