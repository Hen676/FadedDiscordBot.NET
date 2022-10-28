using FadedBot;
using System;
using System.Threading.Tasks;

namespace FadedVanguardBot
{
    class Program
    {
        public static string version = "1.0.3";
        static Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                return new Bot().MainAsync();
            }
            else
            {
                foreach (string arg in args)
                {
                    switch (arg)
                    {
                        case "-v":
                            Console.WriteLine(version);
                            return Task.CompletedTask;
                        case "-h":
                            Console.WriteLine("Implemnt :(");
                            break;
                        default:
                            break;
                    }
                }
                return new Bot().MainAsync();
            }
        }

        public static bool IsDebug()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }
}
