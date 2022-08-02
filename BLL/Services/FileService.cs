using System.Diagnostics;
using Common.Exceptions;
using Common.Models;
using Common.Options;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SkiaSharp;

namespace BLL.Services;

public interface IFileService
{
    Task<(long size, string fileName, string fileNamePreview)> Save(
        string fileName,
        Stream stream,
        CancellationToken cancellationToken = default
    );

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

    public async Task<(long size, string fileName, string fileNamePreview)> Save(
        string fileName,
        Stream stream,
        CancellationToken cancellationToken = default
    )
    {
        var dirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _miscOptions.ContentPath);
        var filePath = Path.Combine(dirPath, fileName);
        Directory.CreateDirectory(dirPath);
        var fileStream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write);
        await stream.CopyToAsync(fileStream, cancellationToken);
        await fileStream.FlushAsync(cancellationToken);
        fileStream.Close();

        var fileInfo = new FileInfo(filePath);

        var fectp = new FileExtensionContentTypeProvider();

        try
        {
            if (fectp.Mappings[fileInfo.Extension].StartsWith("video/"))
            {
                var filePathPreview = Path.Combine(dirPath, Guid.NewGuid() + ".jpg");

                var ffmpeg = new Process()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "ffmpeg",
                        Arguments =
                            $"-ss 00:00:01.00 -i \"" + filePath + "\" -vf \"scale=320:320:force_original_aspect_ratio=decrease\" -vframes 1 \"" + filePathPreview + "\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = false,
                        RedirectStandardInput = false,
                        RedirectStandardError = false,
                        CreateNoWindow = true
                    }
                };

                if (!ffmpeg.Start())
                    throw new CustomException($"Failed to start ffmpeg for file \"{filePath}\"!");

                await ffmpeg.WaitForExitAsync(cancellationToken);
                
                var fileInfoPreview = new FileInfo(filePathPreview);
                
                return (fileInfo.Length, fileInfo.Name, fileInfoPreview.Name);
            }

            if (fectp.Mappings[fileInfo.Extension].StartsWith("image/"))
            {
                var bitmap = SKBitmap.Decode(filePath);

                var maxWidthHeight = Math.Max(bitmap.Width, bitmap.Height);
                var requiredMaxWidthHeight = 256;

                var scaleFactor = requiredMaxWidthHeight / (float)maxWidthHeight;

                var newWidth = (int)Math.Round(bitmap.Width * scaleFactor);
                var newHeight = (int)Math.Round(bitmap.Height * scaleFactor);

                var toBitmap = new SKBitmap(newWidth, newHeight, bitmap.ColorType, bitmap.AlphaType);
                
                var canvas = new SKCanvas(toBitmap);
                canvas.SetMatrix(SKMatrix.CreateScale(scaleFactor, scaleFactor));

                canvas.DrawBitmap(bitmap, 0, 0);
                canvas.ResetMatrix();
                canvas.Flush();

                var image = SKImage.FromBitmap(toBitmap);
                var data = image.Encode(SKEncodedImageFormat.Jpeg, 90);

                var filePathPreview = Path.Combine(dirPath, Guid.NewGuid() + ".jpg");
                var fileStreamPreview = new FileStream(filePathPreview, FileMode.CreateNew, FileAccess.Write);
                data.SaveTo(fileStreamPreview);
                await fileStreamPreview.FlushAsync(cancellationToken);
                fileStreamPreview.Close();

                var fileInfoPreview = new FileInfo(filePathPreview);

                return (fileInfo.Length, fileInfo.Name, fileInfoPreview.Name);
            }
            
            _logger.LogInformation(Localize.Log.Method(_fullName, nameof(Save),
                $"Skipping to generate preview for file \"{filePath}\": File type not supported!"));
        }
        catch (Exception e)
        {
            throw new CustomException($"Failed to generate preview for file \"{filePath}\": Exception: {e}");
        }

        return (fileInfo.Length, fileInfo.Name, null);
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