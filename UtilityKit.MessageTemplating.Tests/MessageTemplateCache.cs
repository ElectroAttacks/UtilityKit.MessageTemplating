using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UtilityKit.MessageTemplating;

public static class MessageTemplateCache
{

    private static readonly Dictionary<AttributeLocation, List<Template>> _items;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CacheRequest CreateRequest([CallerFilePath] in string filePath = "", [CallerMemberName] in string methodName = "", [CallerLineNumber] in int lineNumber = 0)
        => new(filePath, methodName, lineNumber);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CacheRequest WithIdentifier(in this CacheRequest cacheRequest, in string identifier)
        => new(cacheRequest, identifier);

}
