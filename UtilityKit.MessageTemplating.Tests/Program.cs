using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using UtilityKit.MessageTemplating;
using UtilityKit.MessageTemplating.Attributes;

internal class Program
{

    [MessageTemplate("Hello, World!")]
    [MessageTemplate("Hello, World!", "0xEE832E7E")]
    private static void Main(string[] args)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        MessageTemplateCache.InitializeAsync(Assembly.GetExecutingAssembly()).GetAwaiter().GetResult();

        stopwatch.Stop();

        Console.WriteLine($"Initialization took: {stopwatch.ElapsedMilliseconds} ms");

        stopwatch.Restart();

        string template = MessageTemplateCache.Get().WithIdentifier("0xEE832E7E").Template();

        stopwatch.Stop();

        Console.WriteLine($"Template {template} retrieved within {stopwatch.Elapsed} ms");

        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("en-US");

        MessageTemplateCache.Get().Message(null, "");
    }
}