using FadedBot;
using System;
using System.Threading.Tasks;

namespace FadedVanguardBot0._1
{
    class Program
    {
        public static string version = "1.0.0";
        static Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                return new Bot().MainAsync();
            }
            else
            {
                foreach (String arg in args)
                {
                    switch (arg)
                    {
                        case "-v":
                            Console.WriteLine(version);
                            return Task.CompletedTask;
                        case "-i":
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
