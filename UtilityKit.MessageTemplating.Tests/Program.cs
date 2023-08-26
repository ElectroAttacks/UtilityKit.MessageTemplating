using System;
using System.Diagnostics;
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

        //string template = MessageTemplateCache.Get().WithIdentifier("0xEE832E7E").Template();

        stopwatch.Stop();

        //Console.WriteLine(template);
        Console.WriteLine($"{stopwatch.ElapsedMilliseconds} ms");
    }
}