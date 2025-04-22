using System;
using Tofu.Webserver;

namespace ConsoleWebserver;

class Program
{
    static void Main(string[] args)
    {
        Server.Start();
        Console.ReadLine();
    }
}
