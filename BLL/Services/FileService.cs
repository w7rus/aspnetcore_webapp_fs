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

        if (_miscOptions.IsFilePreviewsEnabled)
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
                                $"-ss 00:00:01.00 -i \"{filePath}\" -vf \"scale={_miscOptions.FilePreviewMaxLongEdgeLength}:{_miscOptions.FilePreviewMaxLongEdgeLength}:force_original_aspect_ratio=decrease\" -vframes 1 \"{filePathPreview}\"",
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
                    var rwfileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
                    
                    var filePathPreview = Path.Combine(dirPath, Guid.NewGuid() + ".jpg");

                    {
                        using var skManagedStream = new SKManagedStream(rwfileStream);
                        using var skCodec = SKCodec.Create(skManagedStream);
                        using var skBitmap = SKBitmap.Decode(skCodec);
                        var skBitmapWidth = (float)skBitmap.Width;
                        var skBitmapHeight = (float)skBitmap.Height;
                        Action<SKCanvas> transform = _ => { };
                        switch (skCodec.EncodedOrigin)
                        {
                            case SKEncodedOrigin.TopLeft:
                                break;
                            case SKEncodedOrigin.TopRight:
                                // flip along the x-axis
                                transform = canvas => canvas.Scale(-1, 1, skBitmapWidth / 2, skBitmapHeight / 2);
                                break;
                            case SKEncodedOrigin.BottomRight:
                                transform = canvas => canvas.RotateDegrees(180, skBitmapWidth / 2, skBitmapHeight / 2);
                                break;
                            case SKEncodedOrigin.BottomLeft:
                                // flip along the y-axis
                                transform = canvas => canvas.Scale(1, -1, skBitmapWidth / 2, skBitmapHeight / 2);
                                break;
                            case SKEncodedOrigin.LeftTop:
                                skBitmapWidth = skBitmap.Height;
                                skBitmapHeight = skBitmap.Width;
                                transform = canvas =>
                                {
                                    // Rotate 90
                                    canvas.RotateDegrees(90, skBitmapWidth / 2, skBitmapHeight / 2);
                                    canvas.Scale(skBitmapHeight * 1.0f / skBitmapWidth, -skBitmapWidth * 1.0f / skBitmapHeight, skBitmapWidth / 2, skBitmapHeight / 2);
                                };
                                break;
                            case SKEncodedOrigin.RightTop:
                                skBitmapWidth = skBitmap.Height;
                                skBitmapHeight = skBitmap.Width;
                                transform = canvas =>
                                {
                                    // Rotate 90
                                    canvas.RotateDegrees(90, skBitmapWidth / 2, skBitmapHeight / 2);
                                    canvas.Scale(skBitmapHeight * 1.0f / skBitmapWidth, skBitmapWidth * 1.0f / skBitmapHeight, skBitmapWidth / 2, skBitmapHeight / 2);
                                };
                                break;
                            case SKEncodedOrigin.RightBottom:
                                skBitmapWidth = skBitmap.Height;
                                skBitmapHeight = skBitmap.Width;
                                transform = canvas =>
                                {
                                    // Rotate 90
                                    canvas.RotateDegrees(90, skBitmapWidth / 2, skBitmapHeight / 2);
                                    canvas.Scale(-skBitmapHeight * 1.0f / skBitmapWidth, skBitmapWidth * 1.0f / skBitmapHeight, skBitmapWidth / 2, skBitmapHeight / 2);
                                };
                                break;
                            case SKEncodedOrigin.LeftBottom:
                                skBitmapWidth = skBitmap.Height;
                                skBitmapHeight = skBitmap.Width;
                                transform = canvas =>
                                {
                                    // Rotate 90
                                    canvas.RotateDegrees(90, skBitmapWidth / 2, skBitmapHeight / 2);
                                    canvas.Scale(-skBitmapHeight * 1.0f / skBitmapWidth, -skBitmapWidth * 1.0f / skBitmapHeight, skBitmapWidth / 2, skBitmapHeight / 2);
                                };
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        var info = new SKImageInfo((int)Math.Round(skBitmapWidth), (int)Math.Round(skBitmapHeight));
                        using var skSurface = SKSurface.Create(info);
                        using var skPaint = new SKPaint();
                        skPaint.IsAntialias = true;
                        skPaint.FilterQuality = SKFilterQuality.High;
                        transform.Invoke(skSurface.Canvas);
                        skSurface.Canvas.DrawBitmap(skBitmap, info.Rect, skPaint);
                        skSurface.Canvas.Flush();
                            
                        using var skImage = skSurface.Snapshot();
                        var skData = skImage.Encode(SKEncodedImageFormat.Jpeg, 100);
                        rwfileStream.SetLength(0);
                        await rwfileStream.FlushAsync(cancellationToken);
                        skData.SaveTo(rwfileStream);
                        await rwfileStream.FlushAsync(cancellationToken);
                        rwfileStream.Seek(0, SeekOrigin.Begin);
                    }

                    {
                        using var skManagedStream = new SKManagedStream(rwfileStream);
                        using var skCodec = SKCodec.Create(skManagedStream);
                        using var skBitmap = SKBitmap.Decode(skCodec);
                        var skBitmapWidth = (float)skBitmap.Width;
                        var skBitmapHeight = (float)skBitmap.Height;
                        
                        var maxWidthHeight = Math.Max(skBitmapWidth, skBitmapHeight);
                        var scaleFactor = _miscOptions.FilePreviewMaxLongEdgeLength / maxWidthHeight;
                        var skBitmapWidthPreview = Math.Round(skBitmapWidth * scaleFactor);
                        var skBitmapHeightPreview = Math.Round(skBitmapHeight * scaleFactor);
                            
                        var skBitmapPreview = new SKBitmap((int)skBitmapWidthPreview, (int)skBitmapHeightPreview);
                        skBitmapPreview.Erase(SKColors.Transparent);
                        skBitmap.ScalePixels(skBitmapPreview, SKFilterQuality.High);

                        var infoPreview = new SKImageInfo((int)skBitmapWidthPreview, (int)skBitmapHeightPreview);
                        using var skSurfacePreview = SKSurface.Create(infoPreview);
                        using var skPaintPreview = new SKPaint();
                        skPaintPreview.IsAntialias = true;
                        skPaintPreview.FilterQuality = SKFilterQuality.High;
                        skSurfacePreview.Canvas.DrawBitmap(skBitmapPreview, infoPreview.Rect, skPaintPreview);
                        skSurfacePreview.Canvas.Flush();
                            
                        using var skImagePreview = skSurfacePreview.Snapshot();
                        var skDataPreview = skImagePreview.Encode(SKEncodedImageFormat.Jpeg, 100);
                        var fileStreamPreview = new FileStream(filePathPreview, FileMode.CreateNew, FileAccess.Write);
                        skDataPreview.SaveTo(fileStreamPreview);
                        await fileStreamPreview.FlushAsync(cancellationToken);
                        fileStreamPreview.Close();
                    }
                    
                    rwfileStream.Close();

                    // var bitmap = SKBitmap.Decode(filePath);
                    // var maxWidthHeight = Math.Max(bitmap.Width, bitmap.Height);
                    //
                    // var scaleFactor = _miscOptions.FilePreviewMaxLongEdgeLength / (float)maxWidthHeight;
                    //
                    // var newWidth = (int)Math.Round(bitmap.Width * scaleFactor);
                    // var newHeight = (int)Math.Round(bitmap.Height * scaleFactor);
                    //
                    // var toBitmap = new SKBitmap(newWidth, newHeight, bitmap.ColorType, bitmap.AlphaType);
                    // toBitmap.Erase(SKColors.Transparent);
                    //
                    // var canvas = new SKCanvas(toBitmap);
                    // canvas.Clear(SKColors.Transparent);
                    // canvas.SetMatrix(SKMatrix.CreateScale(scaleFactor, scaleFactor));
                    //
                    // canvas.DrawBitmap(bitmap, 0, 0);
                    // canvas.ResetMatrix();
                    // canvas.Flush();
                    //
                    // var image = SKImage.FromBitmap(toBitmap);
                    // var data = image.Encode(SKEncodedImageFormat.Jpeg, 100);
                    //
                    // var filePathPreview = Path.Combine(dirPath, Guid.NewGuid() + ".jpg");
                    // var fileStreamPreview = new FileStream(filePathPreview, FileMode.CreateNew, FileAccess.Write);
                    // data.SaveTo(fileStreamPreview);
                    // await fileStreamPreview.FlushAsync(cancellationToken);
                    // fileStreamPreview.Close();

                    fileInfo = new FileInfo(filePath);
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
        var fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

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