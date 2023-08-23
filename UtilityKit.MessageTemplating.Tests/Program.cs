using System;
using UtilityKit.MessageTemplating;
using UtilityKit.MessageTemplating.Attributes;

internal class Program
{

    [MessageTemplate("Hello, World!")]
    [MessageTemplate("Hello, World!", "0xEE832E7E")]
    private static void Main(string[] args)
    {
        int test = 10;

        MessageTemplateCache.CreateRequest("D:\\UtilityKitDotNet\\UtilityKit.MessageTemplating\\UtilityKit.MessageTemplating.Tests\\Program.cs", "Main", 11);

        Console.WriteLine(test);

        switch (test)
        {

            case > 3:
                Console.WriteLine("Greater than 3");
                break;
            case > 2:
                Console.WriteLine("Greater than 2");
                break;

            case > 1:

                Console.WriteLine("Greater than 1");
                break;
        }
    }
}