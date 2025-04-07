namespace FileService.Options;

public class MinioOptions
{
    public const string MINIO = "Minio";
    public string Endpoint { get; init; } = string.Empty;
    public string AccessKey { get; init; } = string.Empty;
    public string SecretKey { get; init; } = string.Empty;
    
    public bool WithSsL { get; init; } = false;
    
}