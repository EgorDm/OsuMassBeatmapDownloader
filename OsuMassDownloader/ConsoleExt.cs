using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuMassDownloader
{
    public enum LogLevel
    {
        Error,
        Warning,
        Instruction,
        Info
    }

    public static class ConsoleExt
    {
        public static void Die() {
            Log("Press any key to continue.");
            Console.ReadKey();
        }

        public static void Log(string message, LogLevel level = LogLevel.Info) {
            switch (level) {
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case LogLevel.Instruction:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
            var date = DateTime.Now;
            var line = $"[{date.ToShortDateString()} {date.ToShortTimeString()}] {message}";
            Console.WriteLine(line);
            Console.ResetColor();
        }

        public static bool Confirm(string msg = "") {
            Log($"{msg}[y/n]", LogLevel.Instruction);
            while (true) {
                var response = Console.ReadLine();
                if (response.Equals("y")) return true;
                if (response.Equals("n")) return false;
            }
        }
    }
}
