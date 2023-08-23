namespace UtilityKit.MessageTemplating;

public readonly struct CacheRequest
{
    public CacheRequest(CacheRequest cacheRequest, string identifier)
    {
    }

    public CacheRequest(string filePath, string methodName, int lineNumber)
    {
        FilePath = filePath;
        MethodName = methodName;
        LineNumber = lineNumber;
    }

    public string FilePath { get; }
    public string MethodName { get; }
    public int LineNumber { get; }
}