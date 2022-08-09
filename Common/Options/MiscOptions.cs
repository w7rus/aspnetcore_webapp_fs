namespace Common.Options;

public class MiscOptions
{
    public string ContentPath { get; set; } = "content";
    public string[] CorsAllowedOrigins { get; set; }
    public bool IsFilePreviewsEnabled { get; set; }
    public int FilePreviewMaxLongEdgeLength { get; set; }

    public string AuthenticationAccessToken { get; set; }
}